using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using ShakeMe.Core.Services;
using ShakeMe.Services;

namespace ShakeMe.ViewModels;

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

    private async Task InitializeConnectionAsync()
    {
        await _webSocketService.ConnectAsync();
        _webSocketService.OnMessageReceived += HandleWebSocketMessage;
        
        // Envoyer les infos utilisateur dès la connexion
        await SendUserInfoAsync();
    }

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

    public void StartShakeDetection()
    {
        if (!_shakeService.IsRunning)
        {
            _shakeService.Start();
        }
    }

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

                // Récupérer les infos du partenaire
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

    public void Activate()
    {
        Console.WriteLine("📡 Activation MatchPageViewModel");
        _webSocketService.OnMessageReceived -= HandleWebSocketMessage;
        _webSocketService.OnMessageReceived += HandleWebSocketMessage;
    }

    public void Deactivate()
    {
        Console.WriteLine("🛑 Désactivation MatchPageViewModel");
        _webSocketService.OnMessageReceived -= HandleWebSocketMessage;
    }
}