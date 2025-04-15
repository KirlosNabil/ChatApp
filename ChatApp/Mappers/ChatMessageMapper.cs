using System.Reflection;
using ChatApp.DTOs;
using ChatApp.Models;

namespace ChatApp.Mappers
{
    public static class ChatMessageMapper
    {
        public static ChatMessageDTO ToDTO(ChatMessage chatMessage, string senderFullName)
        {
            return new ChatMessageDTO()
            {
                MessageId = chatMessage.Id,
                MessageContent = chatMessage.Message,
                SenderId = chatMessage.SenderId,
                SenderFullName = senderFullName,
                ReceiverId = chatMessage.ReceiverId,
                IsDelivered = false,
                IsRead = false
            };
        }
    }
}
