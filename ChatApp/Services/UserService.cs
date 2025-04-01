using ChatApp.Models;
using ChatApp.Repositories;

namespace ChatApp.Services
{
    public class UserService : IUserService
    {
        private readonly IFriendRequestRepository _friendRepository;
        private readonly IUserRepository _userRepository;
        public UserService(IFriendRequestRepository friendRepository, IUserRepository userRepository)
        {
            _friendRepository = friendRepository;
            _userRepository = userRepository;
        }
        public async Task<Tuple<string, string>> GetUserName(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            Tuple<string, string> name = new Tuple<string, string>(user.FirstName, user.LastName);
            return name;
        }
    }
}
