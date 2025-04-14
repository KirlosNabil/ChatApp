using ChatApp.DTOs;
using ChatApp.Mappers;
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
        public async Task<UserDTO> GetUser(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            UserDTO userDTO = UserMapper.ToDTO(user);
            return userDTO;
        }
        public async Task<UserViewModel> GetUserViewModel(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            UserViewModel userViewModel = UserMapper.ToViewModel(user);
            return userViewModel;
        }
        public async Task<ViewUserViewModel> GetViewUserViewModel(string myId, string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            List<UserViewModel> mutualFriends = await _friendService.GetMutualFriends(myId, userId);
            UserRelation relation = await _friendService.GetUserRelation(myId, userId);
            ViewUserViewModel viewUserViewModel = UserMapper.ToViewUserViewModel(user, mutualFriends, relation);
            return viewUserViewModel;
        }
        public async Task<string> GetUserFullName(string userId)
        {
            string userFullName = await _userRepository.GetUserFullNameById(userId);
            return userFullName;
        }
    }
}
