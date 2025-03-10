using System.Collections.Concurrent;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class FriendRequestHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();
        public FriendRequestHub(ApplicationDbContext dbContext)
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
                // Log the exception here
                Console.Error.WriteLine($"OnConnectedAsync error: {ex.Message}");
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
        public async Task SendFriendRequest(string receiverId)
        {
            string senderId = Context.UserIdentifier;
            User receiver = await _dbContext.Users.FindAsync(receiverId);

            if (receiver != null)
            {
                receiver.FriendList.Add(senderId);
                _dbContext.Users.Update(receiver);
                await _dbContext.SaveChangesAsync();

                if (_connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("NewFriendRequest", senderId);
                }
            }
        }
    }
}
