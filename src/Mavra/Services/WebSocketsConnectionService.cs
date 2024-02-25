using Mavra.Services.Interfaces;
using System.Collections.Concurrent;
using System.Text;

namespace Mavra.Services
{
    public class WebSocketsConnectionService : IWebSocketsConnectionService
    {
        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections = new ConcurrentDictionary<Guid, WebSocketConnection>();

        public void AddConnection(WebSocketConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
            Task.Run(() => { SendToAllAsync($"{connection.Id} - Connected!", connection.Id); });
        }

        public void RemoveConnection(Guid connectionId)
        {
            _ = _connections.TryRemove(connectionId, out WebSocketConnection? connection);
            Task.Run(() => { SendToAllAsync($"{connectionId} - Disconnected!"); });
        }

        public Task SendToAllAsync(string message)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                connectionsTasks.Add(connection.SendAsync(Encoding.UTF8.GetBytes(message)));
            }

            return Task.WhenAll(connectionsTasks);
        }

        public Task SendToAllAsync(string message, Guid connectionId)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                if (connection.Id != connectionId)
                    connectionsTasks.Add(connection.SendAsync(Encoding.UTF8.GetBytes(message)));
            }

            return Task.WhenAll(connectionsTasks);
        }

        public Task SendToAllAsync(byte[] message)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                connectionsTasks.Add(connection.SendAsync(message));
            }

            return Task.WhenAll(connectionsTasks);
        }

        public Task SendToAllAsync(byte[] message, Guid connectionId)
        {
            List<Task> connectionsTasks = new List<Task>();
            foreach (WebSocketConnection connection in _connections.Values)
            {
                if (connection.Id != connectionId)
                    connectionsTasks.Add(connection.SendAsync(message));
            }

            return Task.WhenAll(connectionsTasks);
        }
    }
}
