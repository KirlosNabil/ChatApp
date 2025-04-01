using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface INotificationRepository
    {
        public Task AddNotification(Notification notification);
        public Task UpdateNotification(Notification notification);
        public Task DeleteNotification(int Id);
        public Task<Notification> GetNotificatione(int Id);
        public Task<List<Notification>> GetUserNotifications(string userId);
        public Task<int> GetUserUnreadNotificationsCount(string userId);
    }
}
