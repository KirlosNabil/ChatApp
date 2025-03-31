using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IChatMessageRepository
    {
        public Task AddChatMessage(ChatMessage chatMessage);
        public Task DeleteChatMessage(int Id);
        public Task<ChatMessage> GetChatMessage(int Id);
        public Task<List<ChatMessage>> GetChatBetweenUsers(string firstUserdId, string secondUserId);
        public Task UpdateChatMessage(ChatMessage chatMessage);
        public Task<List<ChatMessage>> GetLastMessages(string userId);
        public Task<int> GetUnreadMessagesCount(string userId);
    }
}
