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
        public async Task<List<SearchUserViewModel>> SearchUser(string userId, string searchedName) 
        {
            if (string.IsNullOrEmpty(searchedName))
            {
                return new List<SearchUserViewModel>();
            }

            var name = searchedName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<FriendRequest> userFriendRequests = await _friendRequestRepository.GetUserFriendRequests(userId);
            List<string> requestsUsersIds = new List<string>();
            foreach(FriendRequest userFriendRequest in userFriendRequests) 
            {
                string friendId = (userFriendRequest.SenderId == userId ? userFriendRequest.ReceiverId : userFriendRequest.SenderId);
                if(userFriendRequest.Status != FriendRequestStatus.Rejected)
                {
                    requestsUsersIds.Add(friendId);
                }
            }

            var users = new List<SearchUserViewModel>();
            List<User> usersWithThisName = new List<User>();
            if (name.Length == 1)
            {
                usersWithThisName = await _userRepository.GetUsersWithName(name[0]);
            }
            else if (name.Length >= 2)
            {
                usersWithThisName = await _userRepository.GetUsersWithName(name[0], name[1]);
            }

            List<User> usersWithRelation = new List<User>();
            List<User> usersWithNoRelation = new List<User>();
            foreach(User user in usersWithThisName)
            {
                if (requestsUsersIds.Contains(user.Id))
                {
                    usersWithRelation.Add(user);
                }
                else
                {
                    usersWithNoRelation.Add(user);
                }
            }
            foreach (User user in usersWithRelation)
            {
                SearchUserViewModel retUser = new SearchUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                };
                FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(userId, user.Id);
                if (friendRequest.Status == FriendRequestStatus.Accepted)
                {
                    retUser.Relation = UserRelation.Friend;
                }
                else
                {
                    if (friendRequest.SenderId == userId)
                    {
                        retUser.Relation = UserRelation.SentFriendRequest;
                    }
                    else
                    {
                        retUser.Relation = UserRelation.ReceivedFriendRequest;
                    }
                }
                users.Add(retUser);
            }
            foreach (var user in usersWithNoRelation)
            {
                SearchUserViewModel retUser = new SearchUserViewModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Relation = UserRelation.NoRelation
                };
                users.Add(retUser);
            }
            return users;
        }
    }
}
