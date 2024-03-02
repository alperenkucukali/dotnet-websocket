using Mavra.Models;
using Mavra.Services.Interfaces;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace Mavra.Services
{
    public class WebSocketsConnectionService : IWebSocketsConnectionService
    {
        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections = new ConcurrentDictionary<Guid, WebSocketConnection>();

        public void AddConnection(WebSocketConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
            Task.Run(() => { SendToAllAsync(new ChatMessage("System", $"{connection.Username} connected!", true), connection.Id); });
        }

        public void RemoveConnection(Guid connectionId)
        {
            _ = _connections.TryRemove(connectionId, out WebSocketConnection? connection);
            Task.Run(() => { SendToAllAsync(new ChatMessage("System", $"{connection?.Username} disconnected!", true)); });
        }

        public Task SendToAllAsync(ChatMessage message)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                connectionsTasks.Add(connection.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))));
            }

            return Task.WhenAll(connectionsTasks);
        }

        public Task SendToAllAsync(ChatMessage message, Guid connectionId)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                if (connection.Id != connectionId)
                    connectionsTasks.Add(connection.SendAsync(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message))));
            }

            return Task.WhenAll(connectionsTasks);
        }
    }
}
