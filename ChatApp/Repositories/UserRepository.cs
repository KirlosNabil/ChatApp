using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Repositories
{
    public class UserRepository : IUserRepository
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
        public async Task UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteUser(string Id)
        {
            User user = await _dbContext.Users.FindAsync(Id);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<User> GetUserById(string Id)
        {
            User user = await _dbContext.Users.FindAsync(Id);
            return user;
        }
        public async Task<List<User>> GetUsersWithName(string firstName, string? lastName = null)
        {
            IQueryable<User> query = _dbContext.Users.Where(u => u.FirstName.Contains(firstName));
            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(u => u.LastName.Contains(lastName));
            }
            List<User> users = await query.ToListAsync();
            return users;
        }
        public async Task<string> GetUserFullNameById(string userId)
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId).Select(u => new { u.FirstName, u.LastName }).FirstOrDefaultAsync();
            return $"{user.FirstName} {user.LastName}";
        }
    }
}
