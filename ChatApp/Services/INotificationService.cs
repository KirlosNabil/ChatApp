using ChatApp.Models;

namespace ChatApp.Services
{
    public interface INotificationService
    {
        public Task<Notification> CreateSentFriendRequestNotification(string senderId, string receiverId);
        public Task<Notification> CreateAcceptedFriendRequestNotification(string senderId, string receiverId);
        public Task<List<Notification>> GetUserNotifications(string userId);
        public Task<int> GetUserUnreadNotificationsCount(string userId);
    }
}
