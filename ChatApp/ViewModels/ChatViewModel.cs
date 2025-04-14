using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class ChatViewModel
    {
        public List<Tuple<ChatMessage,string>> ChatMessages { get; set; } = new List<Tuple<ChatMessage,string>>(); // Message, Sender Name
        public string friendId { get; set; }
        public string friendName { get; set; }
    }
}
