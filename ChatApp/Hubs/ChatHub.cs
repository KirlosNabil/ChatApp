using System.Collections.Concurrent;
using System.Reflection;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private static ConcurrentDictionary<string, List<string>> _connectedUsers = new ConcurrentDictionary<string, List<string>>();
        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                string userId = Context.UserIdentifier;
                if (userId != null)
                {
                    _connectedUsers.GetOrAdd(userId, _ => new List<string>()).Add(Context.ConnectionId);
                    await ReceiveSentMessages(userId);
                }
            }
            catch (Exception ex) { }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                string userId = Context.UserIdentifier;
                if (userId != null && _connectedUsers.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _connectedUsers.TryRemove(userId, out _);
                    }
                }
            }
            catch (Exception ex){}
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string receiverId, string message)
        {
            try
            {
                string userId = Context.UserIdentifier;

                ChatMessageDTO chatMessageDTO = await _chatService.SendMessage(userId, receiverId, message);

                if (_connectedUsers.ContainsKey(receiverId))
                {
                    await _chatService.MarkMessageAsDelivered(chatMessageDTO.MessageId);

                    await Clients.User(receiverId).SendAsync("ReceiveMessage", chatMessageDTO);
                }
                if (_connectedUsers.ContainsKey(userId))
                {
                    await Clients.User(userId).SendAsync("ReceiveMessage", chatMessageDTO);
                }
            }
            catch (Exception ex){}
        }
        public async Task MarkMessagesAsRead(string receiverId)
        {
            try
            {
                string userId = Context.UserIdentifier;
                List<int> unreadMessagesIds = await _chatService.GetUnreadChatMessagesIds(userId, receiverId);
                foreach (int messageId in unreadMessagesIds)
                {
                    await _chatService.MarkMessageAsRead(messageId);
                    await Clients.User(receiverId).SendAsync("MessageRead", messageId);
                }
            }
            catch (Exception ex) { }
        }
        public async Task ReceiveSentMessages(string userId)
        {
            List<int> sentMessagesIds = await _chatService.GetSentChatMessagesIds(userId);
            foreach (int messageId in sentMessagesIds)
            {
                await _chatService.MarkMessageAsDelivered(messageId);
                ChatMessage message = await _chatService.GetMessageById(messageId);
                ChatMessageDTO messageDTO = await _chatService.ChatMessageToDTO(message);

                await Clients.User(userId).SendAsync("ReceiveMessage", messageDTO);

                await Clients.User(message.SenderId).SendAsync("MessageDelivered", message.Id);
            }
        }
    }
}
