using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<FriendRequest>> GetUserFriendRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _dbContext.FriendRequests
            .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId))
            .ToListAsync();
            return friendRequests;
        }
        public async Task<FriendRequest> GetFriendRequestBetweenTwoUsers(string firstUserId, string secondUserId)
        {
            FriendRequest friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(u => (u.SenderId == firstUserId && u.ReceiverId == secondUserId)
                || (u.SenderId == secondUserId && u.ReceiverId == firstUserId));
            return friendRequest;
        }
    }
}
