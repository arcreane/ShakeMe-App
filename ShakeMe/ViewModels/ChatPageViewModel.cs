using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Models;

namespace ShakeMe.ViewModels;

public partial class ChatPageViewModel : ObservableObject
{
    private ClientWebSocket _webSocket;
    private CancellationTokenSource _cts;

    [ObservableProperty]
    private string newMessage;

    public ObservableCollection<MessageModel> Messages { get; } = new();

    public ChatPageViewModel()
    {
        ConnectToWebSocket();
    }

    private async void ConnectToWebSocket()
    {
        try
        {
            _webSocket = new ClientWebSocket();
            _cts = new CancellationTokenSource();

            await _webSocket.ConnectAsync(new Uri("ws://10.0.2.2:8080"), _cts.Token);
            Console.WriteLine("🔌 Connecté au WebSocket !");

            // Démarre la réception en boucle
            _ = Task.Run(ReceiveMessages);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur de connexion WebSocket : {ex.Message}");
        }
    }

    private async Task ReceiveMessages()
    {
        var buffer = new byte[1024];

        while (_webSocket.State == WebSocketState.Open)
        {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fermé", _cts.Token);
                Console.WriteLine("❌ Déconnecté du WebSocket.");
            }
            else
            {
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"📩 Message reçu : {msg}");

                var messageModel = JsonSerializer.Deserialize<MessageModel>(msg);
                if (messageModel != null)
                {
                    // Récupère le pseudo local
                    var localPseudo = await SecureStorage.GetAsync("user_pseudo");

                    // Si ce message vient de moi, on ne l’ajoute pas
                    if (messageModel.Sender == localPseudo)
                    {
                        Console.WriteLine("📤 Message envoyé par moi, ignoré à la réception.");
                        continue;
                    }

                    messageModel.IsMine = false;
                    MainThread.BeginInvokeOnMainThread(() => Messages.Add(messageModel));
                }
            }
        }
    }


    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(NewMessage) || _webSocket == null || _webSocket.State != WebSocketState.Open)
            return;
        var pseudo = await SecureStorage.GetAsync("user_pseudo");
        var message = new MessageModel
        {
            Sender = pseudo ?? "Moi",
            Content = NewMessage,
            SentAt = DateTime.UtcNow,
            IsMine = true
        };

        var json = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);

        Messages.Add(message);
        NewMessage = string.Empty;
    }
}
