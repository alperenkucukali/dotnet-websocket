namespace Mavra.Models
{
    public class ChatMessage
    {
        public ChatMessage(string sender, string message, bool isSystemMessage)
        {
            Sender = sender;
            Message = message;
            IsSystemMessage = isSystemMessage;
        }
        public string Sender { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public bool IsSystemMessage { get; private set; }
    }
}
