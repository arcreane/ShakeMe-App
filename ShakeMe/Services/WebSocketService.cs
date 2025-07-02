using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ShakeMe.Services;

public class WebSocketService
{
    private ClientWebSocket _socket;
    private readonly Uri _uri = new("ws://10.0.2.2:8080");

    public bool IsConnected => _socket?.State == WebSocketState.Open;

    public async Task ConnectAsync()
    {
        if (_socket != null && _socket.State == WebSocketState.Open)
            return;

        _socket = new ClientWebSocket();
        await _socket.ConnectAsync(_uri, CancellationToken.None);
        _ = ListenAsync(); // start listener
        
    }

    public async Task SendAsync(object message)
    {
        var json = JsonSerializer.Serialize(message);
        var buffer = Encoding.UTF8.GetBytes(json);
        await _socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public event Action<string>? OnMessageReceived;

    private async Task ListenAsync()
    {
        var buffer = new byte[1024];
        while (_socket.State == WebSocketState.Open)
        {
            var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            OnMessageReceived?.Invoke(message);
        }
    }
}