using Mavra.Models;

namespace Mavra.Services.Interfaces
{
    public interface IWebSocketsConnectionService
    {
        void AddConnection(WebSocketConnection connection);
        void RemoveConnection(Guid connectionId);
        Task SendToAllAsync(ChatMessage message);
        Task SendToAllAsync(ChatMessage message, Guid connectionId);
    }
}
