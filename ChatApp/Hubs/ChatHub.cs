using System.Collections.Concurrent;
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
        private readonly ILogger<ChatHub> _logger;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();

        public ChatHub(ApplicationDbContext dbContext, ILogger<ChatHub> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message.SenderId, sender.FirstName + " " + sender.LastName, message.Message);
                        message.Delivered = true;
                    }

                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnConnectedAsync for user {UserId}", Context.UserIdentifier);
            }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OnDisconnectedAsync for user {UserId}", Context.UserIdentifier);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string message)
        {
            try
            {
                string senderId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(senderId))
                {
                    _logger.LogWarning("SendMessage called with null or empty senderId.");
                    return;
                }

                ChatMessage newMessage = new ChatMessage
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    Date = DateTime.UtcNow
                };

                await _dbContext.ChatMessages.AddAsync(newMessage);
                await _dbContext.SaveChangesAsync();
                User sender = _dbContext.Users.FirstOrDefault(u => u.Id == senderId);
                if (_connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, sender.FirstName + " " + sender.LastName, message);
                    newMessage.Delivered = true;
                    _dbContext.ChatMessages.Update(newMessage);
                    await _dbContext.SaveChangesAsync();
                }

                if (_connectedUsers.TryGetValue(senderId, out var senderConnectionId))
                {
                    await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", senderId, "You", message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMessage from {SenderId} to {ReceiverId}", Context.UserIdentifier, receiverId);
            }
        }
    }
}
