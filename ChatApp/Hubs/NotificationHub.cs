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
                    string notification = $"{sender.FirstName} {sender.LastName} sent you a friend request!";
                    Notification n = new Notification()
                    {
                        UserId = receiverId,
                        Content = notification,
                        Date = DateTime.Now,
                        isRead = false
                    };
                    _dbContext.Notifications.Add(n);
                    _dbContext.SaveChanges();
                    int notificationCount = _dbContext.Notifications
                    .Count(u => u.UserId == receiverId
                                 && u.isRead == false);
                    await Clients.User(receiverId).SendAsync("NotifyFriendRequest", pendingRequestsCount, n, notificationCount);
                }
            }
            catch (Exception ex) { }
        }
        public async Task AcceptFriendRequestNotification(string receiverId)
        {
            try
            {
                string senderId = Context.UserIdentifier;
                User sender = await _dbContext.Users.FindAsync(senderId);
                User receiver = await _dbContext.Users.FindAsync(receiverId);
                if (receiver != null)
                {
                    string notification = $"{sender.FirstName} {sender.LastName} accepted your friend request!";
                    Notification n = new Notification()
                    {
                        UserId = receiverId,
                        Content = notification,
                        Date = DateTime.Now,
                        isRead = false
                    };
                    _dbContext.Notifications.Add(n);
                    _dbContext.SaveChanges();
                    int notificationCount = _dbContext.Notifications
                    .Count(u => u.UserId == receiverId
                                 && u.isRead == false);
                    await Clients.User(receiverId).SendAsync("NotifyAcceptedFriendRequest", n, notificationCount);
                }
            }
            catch (Exception ex) { }
        }
    }
}
