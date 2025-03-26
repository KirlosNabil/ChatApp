using System.Collections.Concurrent;
using System.Linq;
using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class FriendHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();

        public FriendHub(ApplicationDbContext dbContext)
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
                    List<FriendRequest> friendRequests = _dbContext.FriendRequests
                        .Where(u => u.ReceiverId == userId && u.Status == FriendRequestStatus.Sent).ToList();
                    foreach (var pendingRequest in friendRequests)
                    {
                        User user = await _dbContext.Users.FindAsync(pendingRequest.SenderId);
                        string senderFirstName = user.FirstName;
                        string senderLastName = user.LastName;
                        await Clients.Client(Context.ConnectionId).SendAsync("NewFriendRequest", pendingRequest.SenderId, senderFirstName, senderLastName);
                        pendingRequest.Status = FriendRequestStatus.Pending;
                        _dbContext.Update(pendingRequest);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex){}

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

        public async Task SendFriendRequest(string receiverId)
        {
            try
            {
                string senderId = Context.UserIdentifier;
                User sender = await _dbContext.Users.FindAsync(senderId); 
                User receiver = await _dbContext.Users.FindAsync(receiverId);
                if (receiver != null)
                {
                    FriendRequest friendRequest = new FriendRequest()
                    {
                        ReceiverId = receiverId,
                        SenderId = senderId,
                        Status = FriendRequestStatus.Sent
                    };
                    _dbContext.FriendRequests.Add(friendRequest);
                    await _dbContext.SaveChangesAsync();

                    if (_connectedUsers.TryGetValue(receiverId, out var receiverConnectionId))
                    {
                        await Clients.Client(receiverConnectionId).SendAsync("NewFriendRequest", senderId, sender.FirstName, sender.LastName);
                        friendRequest.Status = FriendRequestStatus.Pending;
                        _dbContext.FriendRequests.Update(friendRequest);
                        await _dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex){}
        }

    }
}
