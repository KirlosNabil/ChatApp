using System.Security.Claims;
using ChatApp.DTOs;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatMessageRepository;
        private readonly IUserRepository _userRepository;
        public ChatService(IChatMessageRepository chatMessageRepository, IUserRepository userRepository) 
        {
            _chatMessageRepository = chatMessageRepository;
            _userRepository = userRepository;
        }
        public async Task<ChatViewModel> GetChat(string userId, string friendId)
        {
            User friend = await _userRepository.GetUserById(friendId);

            ChatViewModel chat = new ChatViewModel();
            chat.friendId = friendId;
            chat.friendName = friend.FirstName + " " + friend.LastName;

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
                    User sender = await _userRepository.GetUserById(message.SenderId);
                    senderName = sender.FirstName + " " + sender.LastName;
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
                User friend = await _userRepository.GetUserById(friendId);

                ChatsViewModel chatsViewModel = new ChatsViewModel();
                if (chat.SenderId == userId)
                {
                    chatsViewModel.lastMesasageSenderName = "You";
                }
                else
                {
                    chatsViewModel.lastMesasageSenderName = friend.FirstName + " " + friend.LastName;
                }
                chatsViewModel.lastMessage = chat.Message;
                chatsViewModel.friendName = friend.FirstName + " " + friend.LastName;
                chatsViewModel.friendId = friend.Id;
                chatsViewModel.countUnreadMessages = await _chatMessageRepository.GetUnreadMessagesCount(userId);
                chats.Add(chatsViewModel);
            }
            return chats;
        }
        public async Task<ChatMessageDTO> ChatMessageToDTO(ChatMessage chatMessage)
        {
            User sender = await _userRepository.GetUserById(chatMessage.SenderId);
            ChatMessageDTO chatMessageDTO = new ChatMessageDTO()
            {
                MessageId = chatMessage.Id,
                MessageContent = chatMessage.Message,
                SenderId = chatMessage.SenderId,
                SenderFullName = sender.FirstName + " " + sender.LastName,
                ReceiverId = chatMessage.ReceiverId,
                IsDelivered = false,
                IsRead = false
            };
            return chatMessageDTO;
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
            ChatMessageDTO chatMessageDTO = await ChatMessageToDTO(newMessage);
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
