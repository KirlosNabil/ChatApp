﻿using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
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
        public async Task UpdateChatMessage(ChatMessage chatMessage)
        {
            _dbContext.ChatMessages.Update(chatMessage);
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
        public async Task<List<ChatMessage>> GetLastMessages(string userId)
        {
            List<ChatMessage> lastMessagesOfEveryChat = await _dbContext.ChatMessages
           .Where(m => m.SenderId == userId || m.ReceiverId == userId)
           .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
           .Select(g => g.OrderByDescending(m => m.Date).FirstOrDefault())
           .ToListAsync();
            return lastMessagesOfEveryChat;
        }
        public async Task<int> GetUnreadMessagesCount(string userId)
        {
            int count = await _dbContext.ChatMessages.CountAsync(m => m.IsRead == false && (m.ReceiverId == userId));
            return count;
        }
        public async Task<List<ChatMessage>> GetSentMessages(string userId)
        {
            List<ChatMessage> sentMessages = await _dbContext.ChatMessages.Where(m => m.ReceiverId == userId && !m.Delivered).ToListAsync();
            return sentMessages;
        }
        public async Task<List<ChatMessage>> GetUnreadChatMessages(string userId, string senderId)
        {
            List<ChatMessage> messages = await _dbContext.ChatMessages.Where(m => m.SenderId == senderId && m.ReceiverId == userId && !m.IsRead).ToListAsync();
            return messages;
        }
    }
}
