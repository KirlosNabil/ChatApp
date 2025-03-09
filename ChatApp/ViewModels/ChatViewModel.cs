using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class ChatViewModel
    {
        public List<Tuple<ChatMessage,string>> ChatMessages { get; set; }
    }
}
