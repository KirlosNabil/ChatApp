using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IChatService
    {
        public Task<ChatViewModel> GetChat(string userId, string friendId);
        public Task<List<ChatsViewModel>> GetChats(string userId);
    }
}
