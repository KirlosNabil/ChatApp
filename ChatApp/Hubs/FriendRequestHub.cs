using System.Collections.Concurrent;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class FriendRequestHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<FriendRequestHub> _logger;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();

        public FriendRequestHub(ApplicationDbContext dbContext, ILogger<FriendRequestHub> logger)
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
                    User user = await _dbContext.Users
                                        .Include(u => u.FriendList)
                                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user?.FriendList != null && user.FriendList.Any())
                    {
                        foreach (var pendingRequest in user.FriendList)
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("NewFriendRequest", pendingRequest);
                        }
                    }
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

        public async Task SendFriendRequest(string receiverId)
        {
            try
            {
                string senderId = Context.UserIdentifier;
                if (string.IsNullOrEmpty(senderId))
                {
                    _logger.LogWarning("SendFriendRequest called with null or empty senderId.");
                    return;
                }

                User receiver = await _dbContext.Users.FindAsync(receiverId);
                if (receiver != null)
                {
                    receiver.FriendList.Add(senderId);
                    _dbContext.Users.Update(receiver);
                    await _dbContext.SaveChangesAsync();

                    // Notify the receiver that a new friend request has been received
                    if (_connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
                    {
                        await Clients.Client(receiverConnectionId).SendAsync("NewFriendRequest", senderId);
                    }

                    // Notify the sender that the request was sent
                    if (_connectedUsers.TryGetValue(senderId, out var senderConnectionId))
                    {
                        await Clients.Client(senderConnectionId).SendAsync("FriendRequestSent", receiverId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendFriendRequest from {SenderId} to {ReceiverId}", Context.UserIdentifier, receiverId);
            }
        }

    }
}
