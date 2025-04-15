using ChatApp.DTOs;
using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IGroupChatService
    {
        public Task<GroupChatViewModel> GetGroupChat(string userId, int groupId);
        public Task<List<GroupChatsViewModel>> GetGroupChats(string userId);
        public Task<GroupChatMessageDTO> SendGroupMessage(string userId, int groupId, string message);
        public Task<List<int>> GetUserGroupsIds(string userId);
        public Task CreateGroupChat(string userId, CreateGroupChatViewModel model);
    }
}
