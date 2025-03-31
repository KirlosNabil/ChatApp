using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IFriendRequestRepository
    {
        public Task AddFriendRequest(FriendRequest friendRequest);
        public Task DeleteFriendRequest(int Id);
        public Task<FriendRequest> GetFriendRequest(int Id);
        public Task<List<FriendRequest>> GetUserFriendRequests(string userId);
        public Task<FriendRequest> GetFriendRequestBetweenTwoUsers(string firstUserId, string secondUserId);
    }
}
