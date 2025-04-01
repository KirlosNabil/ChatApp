using System.Reflection;
using ChatApp.Models;
using ChatApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserService _userService;
        public NotificationService(INotificationRepository notificationRepository, IUserService userService) 
        {
            _notificationRepository = notificationRepository;
            _userService = userService;
        }
        public async Task<Notification> CreateSentFriendRequestNotification(string senderId, string receiverId)
        {
            Tuple<string, string> senderName = await _userService.GetUserName(senderId);
            string notificationContent = $"{senderName.Item1} {senderName.Item2} sent you a friend request!";
            Notification notification = new Notification()
            {
                UserId = receiverId,
                Content = notificationContent,
                Date = DateTime.Now,
                isRead = false
            };
            await _notificationRepository.AddNotification(notification);
            return notification;
        }
        public async Task<Notification> CreateAcceptedFriendRequestNotification(string senderId, string receiverId)
        {
            Tuple<string, string> senderName = await _userService.GetUserName(senderId);
            string notificationContent = $"{senderName.Item1} {senderName.Item2} accepted your friend request!";
            Notification notification = new Notification()
            {
                UserId = receiverId,
                Content = notificationContent,
                Date = DateTime.Now,
                isRead = false
            };
            await _notificationRepository.AddNotification(notification);
            return notification;
        }
        public async Task<List<Notification>> GetUserNotifications(string userId)
        {
            List<Notification> notifications = await _notificationRepository.GetUserNotifications(userId);
            foreach (Notification notification in notifications)
            {
                notification.isRead = true;
                await _notificationRepository.UpdateNotification(notification);
            }
            return notifications;
        }
        public async Task<int> GetUserUnreadNotificationsCount(string userId)
        {
            int unreadNotificationsCount = await _notificationRepository.GetUserUnreadNotificationsCount(userId);
            return unreadNotificationsCount;
        }
    }
}
