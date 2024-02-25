using System.Net.WebSockets;

namespace Mavra.Services
{
    public class WebSocketConnection
    {
        private WebSocket _webSocket;

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; private set; } = null!;

        public WebSocketConnection(WebSocket webSocket, string username)
        {
            _webSocket = webSocket;
            Username = username;
        }

        public Task SendAsync(byte[] message)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                ArraySegment<byte> messageSegment = new ArraySegment<byte>(message, 0, message.Length);

                return _webSocket.SendAsync(messageSegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
