using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IUserRepository
    {
        public Task AddUser(User user);
        public Task UpdateUser(User user);
        public Task DeleteUser(string Id);
        public Task<User> GetUserById(string Id);
        public Task<List<User>> GetUsersWithName(string firstName, string? lastName = null);
        public Task<string> GetUserFullNameById(string userId);
    }
}
