﻿using System.Collections.Concurrent;
using System.Reflection;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();

        public ChatHub(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId != null)
                {
                    _connectedUsers[userId] = Context.ConnectionId;

                    var pendingMessages = _dbContext.ChatMessages
                        .Where(m => m.ReceiverId == userId && !m.Delivered)
                        .ToList();

                    foreach (var message in pendingMessages)
                    {
                        User sender = _dbContext.Users.FirstOrDefault(u => u.Id == message.SenderId);
                        message.Delivered = true;
                        await Clients.User(userId).SendAsync("ReceiveMessage", message.SenderId, sender.FirstName + " " + sender.LastName,
                            message.Message, message.Id, message.Delivered, message.IsRead);
                        await Clients.User(sender.Id).SendAsync("MessageDelivered", message.Id);
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex) { }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId != null)
                {
                    _connectedUsers.TryRemove(userId, out _);
                }
            }
            catch (Exception ex){}

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            try
            {
                string senderId = Context.UserIdentifier;

                ChatMessage newMessage = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    Date = DateTime.UtcNow,
                    Delivered = false,
                    IsRead = false
                };

                await _dbContext.ChatMessages.AddAsync(newMessage);
                await _dbContext.SaveChangesAsync();
                User sender = _dbContext.Users.FirstOrDefault(u => u.Id == senderId);
                if (_connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
                {
                    newMessage.Delivered = true;
                    _dbContext.ChatMessages.Update(newMessage);
                    await _dbContext.SaveChangesAsync();
                    await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, sender.FirstName + " " + sender.LastName, message,
                        newMessage.Id, newMessage.Delivered, newMessage.IsRead);

                }

                if (_connectedUsers.TryGetValue(senderId, out var senderConnectionId))
                {
                    await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, "You", message, 
                        newMessage.Id, newMessage.Delivered, newMessage.IsRead);
                }
            }
            catch (Exception ex){}
        }

        public async Task MarkMessagesAsRead(string receiverId)
        {
            try
            {
                string userId = Context.UserIdentifier;
                var pendingMessages = _dbContext.ChatMessages
                       .Where(m => m.SenderId == receiverId && m.ReceiverId == userId)
                       .ToList();
                if (pendingMessages.Count == 0)
                {
                    Console.WriteLine("No unread messages found.");
                    return;
                }
                foreach (var pendingMessage in pendingMessages)
                {
                    pendingMessage.IsRead = true;
                    _dbContext.ChatMessages.Update(pendingMessage);
                    await Clients.User(receiverId).SendAsync("MessageRead", pendingMessage.Id);
                }
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex) { }
        }
    }
}
