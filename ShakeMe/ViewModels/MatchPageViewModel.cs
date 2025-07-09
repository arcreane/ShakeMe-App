using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using ShakeMe.Core.Services;
using ShakeMe.Services;

namespace ShakeMe.ViewModels;

/// <summary>
/// ViewModel de la page de matchmaking : gère la détection de shake,
/// la communication via WebSocket et l'état de la conversation.
/// </summary>
public partial class MatchPageViewModel : ObservableObject
{
    private readonly ShakeDetectorService _shakeService;
    private readonly MatchmakingService _matchmakingService;
    private readonly IUserService _userService;
    private readonly WebSocketService _webSocketService;
    private readonly ConversationStateService _conversationState;

    private bool _inConversation = false;
    public bool InConversation
    {
        get => _inConversation;
        set
        {
            if (SetProperty(ref _inConversation, value))
                OnPropertyChanged(nameof(ConversationStatusMessage));
        }
    }

    public string ConversationStatusMessage => InConversation
        ? "Shake pour trouver un nouveau match !"
        : "Shake pour démarrer un match";

    private bool _isMatching = false;

    public MatchPageViewModel(
        ShakeDetectorService shakeService,
        MatchmakingService matchmakingService,
        IUserService userService,
        WebSocketService webSocketService,
        ConversationStateService conversationState)
    {
        Console.WriteLine("✅ MatchPageViewModel instancié");

        _shakeService = shakeService;
        _matchmakingService = matchmakingService;
        _userService = userService;
        _webSocketService = webSocketService;
        _conversationState = conversationState;

        _ = InitializeConnectionAsync();
    }
    /// <summary>
    /// Initialise la connexion WebSocket et envoie les infos utilisateur.
    /// </summary>
    private async Task InitializeConnectionAsync()
    {
        await _webSocketService.ConnectAsync();
        _webSocketService.OnMessageReceived += HandleWebSocketMessage;
        
        // Envoyer les infos utilisateur dès la connexion
        await SendUserInfoAsync();
    }
    /// <summary>
    /// Envoie le pseudo et l'ID utilisateur via WebSocket.
    /// Crée un pseudo aléatoire si nécessaire.
    /// </summary>
    private async Task SendUserInfoAsync()
    {
        try
        {
            var userPseudo = await SecureStorage.GetAsync("user_pseudo");
            var userId = await SecureStorage.GetAsync("user_id");

            if (string.IsNullOrEmpty(userPseudo))
            {
                // Générer un pseudo si pas encore fait
                var random = new Random();
                userPseudo = $"User_{DateTime.Now.Ticks % 10000}_{random.Next(100, 999)}";
                await SecureStorage.SetAsync("user_pseudo", userPseudo);
            }

            var userInfo = new
            {
                type = "user_info",
                pseudo = userPseudo,
                userId = userId
            };

            await _webSocketService.SendAsync(userInfo);
            Console.WriteLine($"📤 Infos utilisateur envoyées : {userPseudo}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur envoi infos utilisateur : {ex.Message}");
        }
    }
    /// <summary>
    /// Démarre la détection de shake.
    /// </summary>
    public void StartShakeDetection()
    {
        if (!_shakeService.IsRunning)
        {
            _shakeService.Start();
        }
    }
    /// <summary>
    /// Traite un shake : envoie un événement de matchmaking ou simule un match en mode invité.
    /// </summary>
    public async Task HandleMatchAsync()
    {
        if (_isMatching)
        {
            Console.WriteLine("⏳ Matchmaking en cours, shake ignoré");
            return;
        }

        _isMatching = true;
        Console.WriteLine("📳 Shake détecté → envoi au WebSocket...");

        try
        {
            var guestMode = await SecureStorage.GetAsync("guest_mode");

            if (guestMode == "true")
            {
                Console.WriteLine("👤 Mode invité détecté : simulation d'un match");
                await SimulateMatchAsync();
            }
            else
            {
                Console.WriteLine($"📡 Envoi shake via WebSocket");
                await _webSocketService.SendAsync(new { type = "shake" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur WebSocket: {ex.Message}");
        }
        finally
        {
            await Task.Delay(3000); // Anti-spam
            _isMatching = false;
        }
    }
    /// <summary>
    /// Gestion des messages WebSocket, traite notamment les notifications de match.
    /// </summary>
    private async void HandleWebSocketMessage(string json)
    {
        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            if (data is null) return;

            if (data.TryGetValue("type", out var typeObj) && typeObj?.ToString() == "match")
            {
                InConversation = true;

                var msg = data.TryGetValue("message", out var messageObj)
                    ? messageObj?.ToString()
                    : "Match réussi ! 🎉";

                var iceBreaker = data.TryGetValue("iceBreaker", out var iceBreakerObj)
                    ? iceBreakerObj?.ToString()
                    : "Discutons ensemble !";

                // Récupération du pseudo du partenaire
                string partnerPseudo = "Anonyme";
                if (data.TryGetValue("partnerInfo", out var partnerInfoObj))
                {
                    var partnerInfoJson = partnerInfoObj?.ToString();
                    if (!string.IsNullOrEmpty(partnerInfoJson))
                    {
                        try
                        {
                            var partnerInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(partnerInfoJson);
                            if (partnerInfo?.TryGetValue("pseudo", out var pseudoObj) == true)
                            {
                                partnerPseudo = pseudoObj?.ToString() ?? "Anonyme";
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Erreur parsing partnerInfo : {ex.Message}");
                        }
                    }
                }

                _conversationState.IceBreaker = iceBreaker;
                _conversationState.PartnerPseudo = partnerPseudo; // Stocker le pseudo du partenaire

                try
                {
                    Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));
                    Console.WriteLine("📳 Vibration envoyée !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Impossible de vibrer : {ex.Message}");
                }

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Match trouvé 🎉", 
                        $"{msg}\nVous parlez avec : {partnerPseudo}", "OK");
                    await Shell.Current.GoToAsync("//chat");
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur réception WebSocket : {ex.Message}");
        }
    }
    /// <summary>
    /// Quitte la conversation en cours et notifie le serveur.
    /// </summary>
    public async Task HandleReentryAsync()
    {
        if (InConversation)
        {
            Console.WriteLine("👋 L'utilisateur quitte la conversation précédente");

            try
            {
                await _webSocketService.SendAsync(new { type = "leave_conversation" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur en envoyant leave_conversation : {ex.Message}");
            }

            InConversation = false;
        }
    }

    /// <summary>
    /// Simule un match pour le mode invité et affiche directement l'alerte.
    /// </summary>
    private async Task SimulateMatchAsync()
    {
        _conversationState.IceBreaker = "Bienvenue dans le mode invité !";
        _conversationState.PartnerPseudo = "Bot de test";

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromSeconds(1));
            Console.WriteLine("📳 Vibration simulée en mode invité !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Impossible de vibrer : {ex.Message}");
        }

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.DisplayAlert("Match trouvé 🎉", 
                $"{_conversationState.IceBreaker}\nVous parlez avec : {_conversationState.PartnerPseudo}", "OK");
            await Shell.Current.GoToAsync("//chat");
        });
    }

    /// <summary>
    /// Active la ViewModel (attache l'événement WebSocket).
    /// </summary>
    public void Activate()
    {
        Console.WriteLine("📡 Activation MatchPageViewModel");
        _webSocketService.OnMessageReceived -= HandleWebSocketMessage;
        _webSocketService.OnMessageReceived += HandleWebSocketMessage;
    }

    /// <summary>
    /// Désactive la ViewModel (détache l'événement WebSocket).
    /// </summary>
    public void Deactivate()
    {
        Console.WriteLine("🛑 Désactivation MatchPageViewModel");
        _webSocketService.OnMessageReceived -= HandleWebSocketMessage;
    }
}