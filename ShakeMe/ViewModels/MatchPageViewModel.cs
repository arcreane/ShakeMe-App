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
    private static readonly List<string> IceBreakers = new()
    {
        "Si tu pouvais voyager n'importe où, tu irais où ? 🌍",
        "Quel est ton dernier film préféré ? 🎬",
        "Plutôt chat ou chien ? 🐱🐶",
        "Ton plat préféré sans hésiter ? 🍝",
        "Si tu gagnes 1 million d'euros demain, tu fais quoi ? 💸"
    };
    private bool _inConversation = false;

    public bool InConversation
    {
        get => _inConversation;
        set
        {
            if (SetProperty(ref _inConversation, value))
            {
                OnPropertyChanged(nameof(ConversationStatusMessage));
            }
        }
    }
    
    public string ConversationStatusMessage => InConversation ? 
        "Shake pour trouver un nouveau match !" : 
        "Shake pour démarrer un match";


    private bool _isMatching = false;

    public MatchPageViewModel(
        ShakeDetectorService shakeService,
        MatchmakingService matchmakingService,
        IUserService userService,
        WebSocketService webSocketService)
    {
        Console.WriteLine("✅ MatchPageViewModel instancié");

        _shakeService = shakeService;
        _matchmakingService = matchmakingService;
        _userService = userService;
        _webSocketService = webSocketService;

        _ = _webSocketService.ConnectAsync(); // démarrage de la connexion WebSocket
        _webSocketService.OnMessageReceived += HandleWebSocketMessage; // écoute des messages
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

                // Simule immédiatement un match
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
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (data is null) return;

            if (data.TryGetValue("type", out var type))
            {
                if (type == "matched")
                {
                    InConversation = true;
                    Console.WriteLine("✅ Match confirmé, en conversation");

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.GoToAsync("//chat");
                    });
                }
                else if (type == "match")
                {
                    var msg = data.TryGetValue("message", out var message)
                        ? message
                        : "Match réussi ! 🎉";

                    var iceBreaker = data.TryGetValue("iceBreaker", out var iceBreakerMessage)
                        ? iceBreakerMessage
                        : "Discutons ensemble !";

                    App.PendingIceBreaker = iceBreaker;

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
                        await Shell.Current.DisplayAlert("Match trouvé 🎉", msg, "OK");
                    });
                }
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
            await Shell.Current.DisplayAlert("Match trouvé 🎉", "Bienvenue dans le mode invité !", "OK");
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
