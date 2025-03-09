using System.Collections.Concurrent;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();
        ChatHub(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public override async Task OnConnectedAsync()
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
                    await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", message.SenderId, message.Message);
                    message.Delivered = true;
                }

                await _dbContext.SaveChangesAsync();
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
            {
                _connectedUsers.TryRemove(userId, out _);
            }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string receiverId, string message)
        {
            string senderId = Context.UserIdentifier;
            ChatMessage newMessage = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Date = DateTime.UtcNow
            };

            await _dbContext.ChatMessages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();

            if (senderId != null && _connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message);
                newMessage.Delivered = true;
                _dbContext.ChatMessages.Update(newMessage);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
