using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IUserRepository
    {
        public Task AddUser(User user);
        public Task DeleteUser(string Id);
        public Task<User> GetUser(string Id);
    }
}
