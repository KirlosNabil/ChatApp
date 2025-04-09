using ChatApp.Models;
using System.Security.Claims;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;
        public HomeService(IUserRepository userRepository, IFriendRequestRepository friendRequestRepository) 
        {
            _userRepository = userRepository;
            _friendRequestRepository = friendRequestRepository;
        }
        public async Task<List<UserViewModel>> SearchUser(string userId, string searchedName) 
        {
            if (string.IsNullOrEmpty(searchedName))
            {
                return new List<UserViewModel>();
            }
            var name = searchedName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<User> usersWithThisName = new List<User>();
            List<UserViewModel> users = new List<UserViewModel>();
            if (name.Length == 1)
            {
                usersWithThisName = await _userRepository.GetUsersWithName(name[0]);
            }
            else if (name.Length >= 2)
            {
                usersWithThisName = await _userRepository.GetUsersWithName(name[0], name[1]);
            }
            foreach (User user in usersWithThisName)
            {
                if(user.Id == userId)
                {
                    continue;
                }
                UserViewModel retUser = new UserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePicturePath = user.ProfilePicturePath
                };
                users.Add(retUser);
            }
            return users;
        }
    }
}
