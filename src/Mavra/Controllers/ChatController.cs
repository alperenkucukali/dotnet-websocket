using Mavra.Services;
using Mavra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace Mavra.Controllers
{
    public class ChatController : ControllerBase
    {
        private IWebSocketsConnectionService _connectionsService;

        public ChatController(IWebSocketsConnectionService connectionsService)
        {
            _connectionsService = connectionsService;
        }

        [Route("/chat/{username}")]
        public async Task Get([FromRoute]string username)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                WebSocketConnection webSocketConnection = new WebSocketConnection(webSocket, username);
                _connectionsService.AddConnection(webSocketConnection);
                var buffer = new byte[1024 * 4];
                var receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    await _connectionsService.SendToAllAsync(buffer, webSocketConnection.Id);
                    receiveResult = await webSocket.ReceiveAsync(
                        new ArraySegment<byte>(buffer), CancellationToken.None);
                }

                await webSocket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
                _connectionsService.RemoveConnection(webSocketConnection.Id);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
