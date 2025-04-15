using ChatApp.DTOs;
using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IChatService
    {
        public Task<ChatViewModel> GetChat(string userId, string friendId);
        public Task<List<ChatsViewModel>> GetChats(string userId);
        public Task<ChatMessageDTO> SendMessage(string userId, string receiverId, string message);
        public Task MarkMessageAsDelivered(int messageId);
        public Task MarkMessageAsRead(int messageId);
        public Task<List<int>> GetUnreadChatMessagesIds(string userId, string friendId);
        public Task<List<int>> GetSentChatMessagesIds(string userId);
        public Task<ChatMessage> GetMessageById(int messageId);
    }
}
