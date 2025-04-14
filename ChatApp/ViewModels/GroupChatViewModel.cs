using ChatApp.DTOs;
using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class GroupChatViewModel
    {
        public List<GroupChatMessageDTO> GroupChatMessages { get; set; }
        public List<UserDTO> GroupMembers { get; set; }
        public string GroupName { get; set; }
        public string GroupOwnerName { get; set; }
        public string GroupOwnerId { get; set; }
        public string UserName { get; set; }
    }
}
