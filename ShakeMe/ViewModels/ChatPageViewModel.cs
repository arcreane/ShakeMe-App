using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Models;
using ShakeMe.Services;

namespace ShakeMe.ViewModels;

public partial class ChatPageViewModel : ObservableObject
{
    private readonly WebSocketService _webSocketService;

    [ObservableProperty]
    private string newMessage;

    public ObservableCollection<MessageModel> Messages { get; } = new();

    private string _userPseudo = "";

    public ChatPageViewModel(WebSocketService webSocketService)
    {
        _webSocketService = webSocketService;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        _userPseudo = await SecureStorage.GetAsync("user_pseudo") ?? "moi";

        if (!_webSocketService.IsConnected)
        {
            Console.WriteLine("⚠️ WebSocket pas connecté, tentative de reconnexion...");
            await _webSocketService.ConnectAsync();
        }

        _webSocketService.OnMessageReceived -= HandleIncomingMessage;
        _webSocketService.OnMessageReceived += HandleIncomingMessage;

        // ✅ Toujours reset la conversation ici
        ResetConversation();

        Console.WriteLine("📡 Envoi readyForIceBreaker depuis ChatPageViewModel");
        try
        {
            await _webSocketService.SendAsync(new { type = "readyForIceBreaker" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur en envoyant readyForIceBreaker : {ex.Message}");
        }
    }


    private void HandleIncomingMessage(string json)
    {
        try
        {
            var document = JsonDocument.Parse(json);
            if (!document.RootElement.TryGetProperty("type", out var typeProp)) return;

            var messageType = typeProp.GetString();

            if (messageType == "message")
            {
                var sender = document.RootElement.GetProperty("sender").GetString();
                var content = document.RootElement.GetProperty("content").GetString();
                var sentAt = document.RootElement.GetProperty("sentAt").GetDateTime();

                if (sender == _userPseudo)
                {
                    Console.WriteLine("📤 Message reçu de moi-même, ignoré.");
                    return;
                }

                var msg = new MessageModel
                {
                    Sender = sender ?? "?",
                    Content = content ?? "",
                    SentAt = sentAt,
                    IsMine = false
                };

                MainThread.BeginInvokeOnMainThread(() => Messages.Add(msg));
            }
            else if (messageType == "match")
            {
                Console.WriteLine("🎯 Ice breaker reçu dans ChatPageViewModel");
                ResetConversation(); 

                var iceBreaker = document.RootElement.GetProperty("iceBreaker").GetString();

                var msg = new MessageModel
                {
                    Sender = "ShakeMeBot 🤖",
                    Content = iceBreaker ?? "Discutons ensemble !",
                    SentAt = DateTime.UtcNow,
                    IsMine = false
                };

                MainThread.BeginInvokeOnMainThread(() => Messages.Add(msg));
            }
            else if (messageType == "info")
            {
                Console.WriteLine("ℹ️ Message info reçu dans ChatPageViewModel");

                var infoMessage = document.RootElement.GetProperty("message").GetString();

                var msg = new MessageModel
                {
                    Sender = "ShakeMeBot 🤖",
                    Content = infoMessage ?? "Information.",
                    SentAt = DateTime.UtcNow,
                    IsMine = false
                };

                MainThread.BeginInvokeOnMainThread(() => Messages.Add(msg));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur parsing message WebSocket : {ex.Message}");
        }
    }


    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMessage) || !_webSocketService.IsConnected)
            return;

        var msg = new MessageModel
        {
            Sender = _userPseudo,
            Content = NewMessage,
            SentAt = DateTime.UtcNow,
            IsMine = true
        };

        Messages.Add(msg);
        NewMessage = string.Empty;

        await _webSocketService.SendAsync(new
        {
            type = "message",
            sender = msg.Sender,
            content = msg.Content
        });
    }
    
    public void ResetConversation()
    {
        MainThread.BeginInvokeOnMainThread(() => Messages.Clear());
        NewMessage = string.Empty;
    }

}
