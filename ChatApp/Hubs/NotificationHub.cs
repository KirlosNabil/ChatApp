using ChatApp.Data;
using System.Collections.Concurrent;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext _dbContext;
        public NotificationHub(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task FriendRequestNotification(string receiverId)
        {
            try
            {
                string senderId = Context.UserIdentifier;
                User sender = await _dbContext.Users.FindAsync(senderId);
                User receiver = await _dbContext.Users.FindAsync(receiverId);
                if (receiver != null)
                {
                    int pendingRequestsCount = _dbContext.FriendRequests
                        .Count(u => u.ReceiverId == receiverId 
                    && u.Status == FriendRequestStatus.Pending);
                    await Clients.User(receiverId).SendAsync("NotifyFriendRequest", pendingRequestsCount);
                }
            }
            catch (Exception ex) { }
        }
    }
}
