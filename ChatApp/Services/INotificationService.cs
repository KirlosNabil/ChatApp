using ChatApp.Models;

namespace ChatApp.Services
{
    public interface INotificationService
    {
        public Task<List<Notification>> GetUserNotifications(string userId);
    }
}
