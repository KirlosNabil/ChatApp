using ChatApp.Models;
using ChatApp.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public NotificationService(INotificationRepository notificationRepository) 
        {
            _notificationRepository = notificationRepository;
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
    }
}
