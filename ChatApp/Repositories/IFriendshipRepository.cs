using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IFriendshipRepository
    {
        public Task AddFriendship(Friendship friendship);
        public Task DeleteFriendship(int Id);
        public Task<Friendship> GetFriendship(int Id);
        public Task<List<Friendship>> GetUserFriendships(string userId);
        public Task<Friendship> GetFriendshipBetweenTwoUsers(string firstUserId, string secondUserId);
    }
}
