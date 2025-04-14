using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IGroupChatService
    {
        public Task<GroupChatViewModel> GetGroupChat(string userId, int groupId);
        public Task<List<GroupChatsViewModel>> GetGroupChats(string userId);
    }
}
