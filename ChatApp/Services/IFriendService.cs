using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IFriendService
    {
        public Task SendFriendRequest(string senderId, string receiverId);
        public Task<List<FriendRequestViewModel>> GetUserFriendRequests(string userId);
        public Task<int> GetUserFriendRequestsCount(string userId);
        public Task<List<SentRequestViewModel>> GetUserSentRequests(string userId);
        public Task<List<FriendRequestViewModel>> GetUserUnreceivedRequests(string userId);
        public Task RemoveRequest(string userId, string receiverId);
        public Task UpdateFriendRequestStatus(string senderId, string receiverId, FriendRequestStatus status);
        public Task RemoveFriend(string userId, string friendId);
        public Task AcceptFriendRequest(string userId, string senderId);
        public Task RejectFriendRequest(string userId, string senderId);
        public Task<List<FriendViewModel>> GetUserFriends(string userId);
        public Task<List<string>> GetUserFriendsIds(string userId);
    }
}
