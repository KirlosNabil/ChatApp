using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IFriendRequestRepository
    {
        public Task AddFriendRequest(FriendRequest friendRequest);
        public Task DeleteFriendRequest(int Id);
        public Task<FriendRequest> GetFriendRequest(int Id);
    }
}
