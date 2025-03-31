using ChatApp.Data;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public FriendRequestRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddFriendRequest(FriendRequest friendRequest)
        {
            _dbContext.FriendRequests.Add(friendRequest);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteFriendRequest(int Id)
        {
            FriendRequest friendRequest = await _dbContext.FriendRequests.FindAsync(Id);
            _dbContext.FriendRequests.Remove(friendRequest);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<FriendRequest> GetFriendRequest(int Id)
        {
            FriendRequest friendRequest = await _dbContext.FriendRequests.FindAsync(Id);
            return friendRequest;
        }
    }
}
