using System.Security.Claims;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRequestRepository _friendRepository;
        private readonly IUserRepository _userRepository;
        public FriendService(IFriendRequestRepository friendRepository, IUserRepository userRepository)
        {
            _friendRepository = friendRepository;
            _userRepository = userRepository;
        }
        public async Task SendFriendRequest(string senderId, string receiverId)
        {
            FriendRequest friendRequest = new FriendRequest()
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.Sent
            };
            await _friendRepository.AddFriendRequest(friendRequest);
        }
        public async Task<List<FriendRequestViewModel>> GetUserFriendRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _friendRepository.GetUserFriendRequests(userId);
            List<FriendRequestViewModel> friendRequestViewModels = new List<FriendRequestViewModel>();
            foreach (FriendRequest friendRequest in friendRequests)
            {
                if(friendRequest.ReceiverId == userId &&  friendRequest.Status == FriendRequestStatus.Pending)
                {
                    User sender = await _userRepository.GetUserById(friendRequest.SenderId);
                    FriendRequestViewModel friendRequestViewModel = new FriendRequestViewModel()
                    {
                        SenderFirstName = sender.FirstName,
                        SenderLastName = sender.LastName,
                        SenderId = sender.Id
                    };
                    friendRequestViewModels.Add(friendRequestViewModel);
                }
            }
            return friendRequestViewModels;
        }
        public async Task<List<SentRequestViewModel>> GetUserSentRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _friendRepository.GetUserFriendRequests(userId);
            List<SentRequestViewModel> sentRequestViewModels = new List<SentRequestViewModel>();
            foreach (FriendRequest friendRequest in friendRequests)
            {
                if(friendRequest.SenderId == userId && (friendRequest.Status == FriendRequestStatus.Pending || friendRequest.Status == FriendRequestStatus.Sent))
                {
                    User receiver = await _userRepository.GetUserById(friendRequest.ReceiverId);
                    SentRequestViewModel sentRequestViewModel = new SentRequestViewModel()
                    {
                        ReceiverFirstName = receiver.FirstName,
                        ReceiverLastName = receiver.LastName,
                        ReceiverId = receiver.Id
                    };
                    sentRequestViewModels.Add(sentRequestViewModel);
                }
            }
            return sentRequestViewModels;
        }
        public async Task<List<FriendRequestViewModel>> GetUserUnreceivedRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _friendRepository.GetUserFriendRequests(userId);
            List<FriendRequestViewModel> friendRequestViewModels = new List<FriendRequestViewModel>();
            foreach (FriendRequest friendRequest in friendRequests)
            {
                if (friendRequest.ReceiverId == userId && friendRequest.Status == FriendRequestStatus.Sent)
                {
                    User sender = await _userRepository.GetUserById(friendRequest.SenderId);
                    FriendRequestViewModel friendRequestViewModel = new FriendRequestViewModel()
                    {
                        SenderFirstName = sender.FirstName,
                        SenderLastName = sender.LastName,
                        SenderId = sender.Id
                    };
                    friendRequestViewModels.Add(friendRequestViewModel);
                }
            }
            return friendRequestViewModels;
        }
        public async Task UpdateFriendRequestStatus(string senderId, string receiverId, FriendRequestStatus status)
        {
            FriendRequest friendRequest = await _friendRepository.GetFriendRequestBetweenTwoUsers(senderId, receiverId);
            friendRequest.Status = status;
            await _friendRepository.UpdateFriendRequest(friendRequest);
        }
        public async Task RemoveRequest(string userId, string receiverId)
        {
            FriendRequest friendRequest = await _friendRepository.GetFriendRequestBetweenTwoUsers(userId, receiverId);
            await _friendRepository.DeleteFriendRequest(friendRequest.Id);
        }
        public async Task RemoveFriend(string userId, string friendId)
        {
            User user = await _userRepository.GetUserById(userId);
            User friend = await _userRepository.GetUserById(friendId);
            user.FriendList.Remove(friendId);
            friend.FriendList.Remove(userId);
            await _userRepository.UpdateUser(user);
            await _userRepository.UpdateUser(friend);
            FriendRequest friendRequest = await _friendRepository.GetFriendRequestBetweenTwoUsers(userId, friendId);
            await _friendRepository.DeleteFriendRequest(friendRequest.Id);
        }
        public async Task AcceptFriendRequest(string userId, string senderId)
        {
            User user = await _userRepository.GetUserById(userId);
            User sender = await _userRepository.GetUserById(senderId);
            user.FriendList.Add(sender.Id);
            sender.FriendList.Add(user.Id);
            await _userRepository.UpdateUser(user);
            await _userRepository.UpdateUser(sender);
            await UpdateFriendRequestStatus(userId, senderId, FriendRequestStatus.Accepted);
        }
        public async Task RejectFriendRequest(string userId, string senderId)
        {
            await UpdateFriendRequestStatus(userId, senderId, FriendRequestStatus.Rejected);
        }
        public async Task<List<FriendViewModel>> GetUserFriends(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            List<FriendViewModel> friends = new List<FriendViewModel>();
            foreach (string friend in user.FriendList)
            {
                User friendUser = await _userRepository.GetUserById(friend);
                FriendViewModel friendViewModel = new FriendViewModel()
                {
                    FriendId = friendUser.Id,
                    FirstName = friendUser.FirstName,
                    LastName = friendUser.LastName
                };
                friends.Add(friendViewModel);
            }
            return friends;
        }
        public async Task<List<string>> GetUserFriendsIds(string userId)
        {
            User user = await _userRepository.GetUserById(userId);
            List<string> friends = new List<string>();
            foreach (string friend in user.FriendList)
            {
                friends.Add(friend);
            }
            return friends;
        }
    }
}
