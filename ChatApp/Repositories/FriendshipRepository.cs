using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public FriendshipRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddFriendship(Friendship friendship)
        {
            _dbContext.Friendships.Add(friendship);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteFriendship(int Id)
        {
            Friendship friendship = await _dbContext.Friendships.FindAsync(Id);
            _dbContext.Friendships.Remove(friendship);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<Friendship> GetFriendship(int Id)
        {
            Friendship friendship = await _dbContext.Friendships.FindAsync(Id);
            return friendship;
        }
        public async Task<List<Friendship>> GetUserFriendships(string userId)
        {
            List<Friendship> friendships = await _dbContext.Friendships
            .Where(fr => (fr.FirstUserId == userId || fr.SecondUserId == userId))
            .ToListAsync();
            return friendships;
        }
        public async Task<Friendship> GetFriendshipBetweenTwoUsers(string firstUserId, string secondUserId)
        {
            Friendship friendship = await _dbContext.Friendships.FirstOrDefaultAsync(u => ((u.FirstUserId == firstUserId && u.SecondUserId == secondUserId)
                || (u.FirstUserId == secondUserId && u.SecondUserId == firstUserId)));
            return friendship;
        }
    }
}
