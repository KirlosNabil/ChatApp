using System.Security.Claims;
using ChatApp.DTOs;
using ChatApp.Mappers;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IUserService _userService;
        public ChatService(IChatMessageRepository chatMessageRepository, IUserService userService) 
        {
            _chatMessageRepository = chatMessageRepository;
            _userService = userService;
        }
        public async Task<ChatViewModel> GetChat(string userId, string friendId)
        {
            string friendName = await _userService.GetUserFullName(friendId);

            ChatViewModel chat = new ChatViewModel();
            chat.friendId = friendId;
            chat.friendName = friendName;

            List<ChatMessage> chatMessages = await _chatMessageRepository.GetChatBetweenUsers(userId, friendId);
            foreach (ChatMessage message in chatMessages)
            {
                string senderName;
                if (message.SenderId == userId)
                {
                    senderName = "You";
                }
                else
                {
                    senderName = await _userService.GetUserFullName(message.SenderId);
                }

                chat.ChatMessages.Add(new Tuple<ChatMessage, string>(message, senderName));
            }

            return chat;
        }
        public async Task<List<ChatsViewModel>> GetChats(string userId)
        {
            List<ChatMessage> lastChatMessages = await _chatMessageRepository.GetLastMessages(userId);

            List<ChatsViewModel> chats = new List<ChatsViewModel>();
            foreach (ChatMessage chat in lastChatMessages)
            {
                string friendId = (chat.SenderId == userId ? chat.ReceiverId : chat.SenderId);
                string friendName = await _userService.GetUserFullName(friendId);

                ChatsViewModel chatsViewModel = new ChatsViewModel();
                if (chat.SenderId == userId)
                {
                    chatsViewModel.lastMesasageSenderName = "You";
                }
                else
                {
                    chatsViewModel.lastMesasageSenderName = friendName;
                }
                chatsViewModel.lastMessage = chat.Message;
                chatsViewModel.friendName = friendName;
                chatsViewModel.friendId = friendId;
                chatsViewModel.countUnreadMessages = await _chatMessageRepository.GetUnreadMessagesCount(userId);
                chats.Add(chatsViewModel);
            }
            return chats;
        }
        public async Task<ChatMessageDTO> SendMessage(string userId, string receiverId, string message)
        {
            ChatMessage newMessage = new ChatMessage
            {
                SenderId = userId,
                ReceiverId = receiverId,
                Message = message,
                Date = DateTime.UtcNow,
                Delivered = false,
                IsRead = false
            };
            await _chatMessageRepository.AddChatMessage(newMessage);
            string senderFullName = await _userService.GetUserFullName(userId);
            ChatMessageDTO chatMessageDTO = ChatMessageMapper.ToDTO(newMessage, senderFullName);
            return chatMessageDTO;
        }
        public async Task MarkMessageAsDelivered(int messageId)
        {
            ChatMessage chatMessage = await _chatMessageRepository.GetChatMessage(messageId);
            chatMessage.Delivered = true;
            await _chatMessageRepository.UpdateChatMessage(chatMessage);
        }
        public async Task MarkMessageAsRead(int messageId)
        {
            ChatMessage chatMessage = await _chatMessageRepository.GetChatMessage(messageId);
            chatMessage.IsRead = true;
            await _chatMessageRepository.UpdateChatMessage(chatMessage);
        }
        public async Task<List<int>> GetUnreadChatMessagesIds(string userId, string friendId)
        {
            List<ChatMessage> unreadMessages = await _chatMessageRepository.GetUnreadChatMessages(userId, friendId);
            List<int> messagesIds = new List<int>();
            foreach(ChatMessage unreadMessage in unreadMessages)
            {
                messagesIds.Add(unreadMessage.Id);
            }
            return messagesIds;
        }
        public async Task<List<int>> GetSentChatMessagesIds(string userId)
        {
            List<ChatMessage> sentMessages = await _chatMessageRepository.GetSentMessages(userId);
            List<int> messagesIds = new List<int>();
            foreach (ChatMessage unreadMessage in sentMessages)
            {
                messagesIds.Add(unreadMessage.Id);
            }
            return messagesIds;
        }
        public async Task<ChatMessage> GetMessageById(int messageId)
        {
            ChatMessage message = await _chatMessageRepository.GetChatMessage(messageId);
            return message;
        }
    }
}
