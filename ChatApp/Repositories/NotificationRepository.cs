using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public NotificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddNotification(Notification notification)
        {
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateNotification(Notification notification)
        {
            _dbContext.Notifications.Update(notification);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteNotification(int Id)
        {
            Notification notification = await _dbContext.Notifications.FindAsync(Id);
            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Notification> GetNotificatione(int Id)
        {
            Notification notification = await _dbContext.Notifications.FindAsync(Id);
            return notification;
        }
        public async Task<List<Notification>> GetUserNotifications(string userId)
        {
            List<Notification> notifications = await _dbContext.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderBy(m => m.Date)
                    .ToListAsync();
            return notifications;
        }
        public async Task<int> GetUserUnreadNotificationsCount(string userId)
        {
            int unreadNotificationsCount = await _dbContext.Notifications.CountAsync(u => u.UserId == userId && u.isRead == false);
            return unreadNotificationsCount;
        }
    }
}
