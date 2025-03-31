using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class ChatMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public ChatMessageRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddChatMessage(ChatMessage chatMessage)
        {
            _dbContext.ChatMessages.Add(chatMessage);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteChatMessage(int Id)
        {
            ChatMessage chatMessage = await _dbContext.ChatMessages.FindAsync(Id);
            _dbContext.ChatMessages.Remove(chatMessage);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<ChatMessage> GetChatMessage(int Id)
        {
            ChatMessage chatMessage = await _dbContext.ChatMessages.FindAsync(Id);
            return chatMessage;
        }
        public async Task<List<ChatMessage>> GetChatBetweenUsers(string firstUserdId, string secondUserId)
        {
            List<ChatMessage> chatMessages = await _dbContext.ChatMessages
                    .Where(m => ((m.SenderId == firstUserdId && m.ReceiverId == secondUserId) ||
                                (m.SenderId == secondUserId && m.ReceiverId == firstUserdId)))
                    .OrderBy(m => m.Date)
                    .ToListAsync();
            return chatMessages;
        }
    }
}
