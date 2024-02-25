namespace Mavra.Services.Interfaces
{
    public interface IWebSocketsConnectionService
    {
        void AddConnection(WebSocketConnection connection);
        void RemoveConnection(Guid connectionId);
        Task SendToAllAsync(string message);
        Task SendToAllAsync(byte[] message);
        Task SendToAllAsync(string message, Guid connectionId);
        Task SendToAllAsync(byte[] message, Guid connectionId);
    }
}
