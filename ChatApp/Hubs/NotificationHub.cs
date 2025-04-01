using ChatApp.Data;
using System.Collections.Concurrent;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ChatApp.Services;

namespace ChatApp.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IFriendService _friendService;
        public NotificationHub(INotificationService notificationService, IFriendService friendService)
        {
            _notificationService = notificationService;
            _friendService = friendService;
        }
        public async Task FriendRequestNotification(string receiverId)
        {
            try
            {
                string userId = Context.UserIdentifier;
                Notification notification = await _notificationService.CreateSentFriendRequestNotification(userId, receiverId);
                int friendRequestsCount = await _friendService.GetUserFriendRequestsCount(receiverId);
                int notificationsCount = await _notificationService.GetUserUnreadNotificationsCount(receiverId);

                await Clients.User(receiverId).SendAsync("NotifyFriendRequest", friendRequestsCount, notification, notificationsCount);
            }
            catch (Exception ex) { }
        }
        public async Task AcceptFriendRequestNotification(string receiverId)
        {
            try
            {
                string userId = Context.UserIdentifier;
                Notification notification = await _notificationService.CreateAcceptedFriendRequestNotification(userId, receiverId);
                int notificationsCount = await _notificationService.GetUserUnreadNotificationsCount(receiverId);

                await Clients.User(receiverId).SendAsync("NotifyAcceptedFriendRequest", notification, notificationsCount);
            }
            catch (Exception ex) { }
        }
    }
}
