using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class GroupChatViewModel
    {
        public List<Tuple<GroupChatMessage, string>> GroupChatMessages { get; set; } = new List<Tuple<GroupChatMessage, string>>(); // Message, Sender Name
        public List<Tuple<string, string>> GroupMembers { get; set; } = new List<Tuple<string, string>>(); // Ids, Names
        public string GroupName { get; set; }
        public string GroupOwnerName { get; set; }
        public string GroupOwnerId { get; set; }
    }
}
