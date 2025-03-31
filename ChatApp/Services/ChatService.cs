using System.Security.Claims;
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
                if (message.ReceiverId == userId)
                {
                    message.IsRead = true;
                }
                await _chatMessageRepository.UpdateChatMessage(message);
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
    }
}
