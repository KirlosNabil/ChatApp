using ChatApp.Data;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteUser(string Id)
        {
            User user = await _dbContext.Users.FindAsync(Id);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<User> GetUser(string Id)
        {
            User user = await _dbContext.Users.FindAsync(Id);
            return user;
        }
    }
}
