using ChatApp.DTOs;

namespace ChatApp.ViewModels
{
    public class CreateGroupChatViewModel
    {
        public string Name { get; set; }
        public List<string> Members { get; set; } = new List<string>();
        public List<FriendViewModel> Friends { get; set; } = new List<FriendViewModel>();
    }
}
