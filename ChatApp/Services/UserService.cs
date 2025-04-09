using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendService _friendService;
        public UserService(IUserRepository userRepository, IFriendService friendService)
        {
            _userRepository = userRepository;
            _friendService = friendService;
        }
        public async Task<Tuple<string, string>> GetUserName(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            Tuple<string, string> name = new Tuple<string, string>(user.FirstName, user.LastName);
            return name;
        }
        public async Task<UserViewModel> GetUser(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            UserViewModel userViewModel = new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath
            };
            return userViewModel;
        }
        public async Task<ViewUserViewModel> GetUserView(string myId, string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            List<UserViewModel> mutualFriends = await _friendService.GetMutualFriends(myId, userId);
            UserRelation relation = await _friendService.GetUserRelation(myId, userId);
            ViewUserViewModel viewUserViewModel = new ViewUserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath,
                Relation = relation,
                MutualFriends = mutualFriends
            };
            return viewUserViewModel;
        }
    }
}
