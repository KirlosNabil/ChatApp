﻿using System.Security.Claims;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IUserRepository _userRepository;
        public FriendService(IFriendRequestRepository friendRepository, IFriendshipRepository friendshipRepository, IUserRepository userRepository)
        {
            _friendRequestRepository = friendRepository;
            _friendshipRepository = friendshipRepository;
            _userRepository = userRepository;
        }
        public async Task SendFriendRequest(string senderId, string receiverId)
        {
            FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(senderId, receiverId);
            if(friendRequest == null ) 
            {
                FriendRequest newFriendRequest = new FriendRequest()
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Status = FriendRequestStatus.Sent
                };
                await _friendRequestRepository.AddFriendRequest(newFriendRequest);
            }
            else
            {
                await UpdateFriendRequestStatus(senderId, receiverId, FriendRequestStatus.Sent);
            }
        }
        public async Task<List<FriendRequestViewModel>> GetUserFriendRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _friendRequestRepository.GetUserFriendRequests(userId);
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
        public async Task<int> GetUserFriendRequestsCount(string userId)
        {
            List<FriendRequestViewModel> friendRequests = await GetUserFriendRequests(userId);
            return friendRequests.Count;
        }
        public async Task<List<SentRequestViewModel>> GetUserSentRequests(string userId)
        {
            List<FriendRequest> friendRequests = await _friendRequestRepository.GetUserFriendRequests(userId);
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
            List<FriendRequest> friendRequests = await _friendRequestRepository.GetUserFriendRequests(userId);
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
            FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(senderId, receiverId);
            friendRequest.Status = status;
            await _friendRequestRepository.UpdateFriendRequest(friendRequest);
        }
        public async Task RemoveRequest(string userId, string receiverId)
        {
            FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(userId, receiverId);
            await _friendRequestRepository.DeleteFriendRequest(friendRequest.Id);
        }
        public async Task RemoveFriend(string userId, string friendId)
        {
            Friendship friendship = await _friendshipRepository.GetFriendshipBetweenTwoUsers(userId, friendId);
            if(friendship != null)
            {
                await _friendshipRepository.DeleteFriendship(friendship.Id);
            }
            FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(userId, friendId);
            await _friendRequestRepository.DeleteFriendRequest(friendRequest.Id);
        }
        public async Task AcceptFriendRequest(string userId, string senderId)
        {
            await UpdateFriendRequestStatus(userId, senderId, FriendRequestStatus.Accepted);
            Friendship friendship = new Friendship()
            {
                FirstUserId = userId,
                SecondUserId = senderId
            };
            await _friendshipRepository.AddFriendship(friendship);
        }
        public async Task RejectFriendRequest(string userId, string senderId)
        {
            await UpdateFriendRequestStatus(userId, senderId, FriendRequestStatus.Rejected);
        }
        public async Task<List<FriendViewModel>> GetUserFriends(string userId)
        {
            List<Friendship> friendships = await _friendshipRepository.GetUserFriendships(userId);
            List<FriendViewModel> friends = new List<FriendViewModel>();
            foreach (Friendship friendship in friendships)
            {
                string friendId;
                if(friendship.FirstUserId == userId)
                {
                    friendId = friendship.SecondUserId;
                }
                else 
                {
                    friendId = friendship.FirstUserId;
                }
                User friendUser = await _userRepository.GetUserById(friendId);
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
            List<Friendship> friendships = await _friendshipRepository.GetUserFriendships(userId);
            List<string> friends = new List<string>();
            foreach (Friendship friendship in friendships)
            {
                string friendId;
                if (friendship.FirstUserId == userId)
                {
                    friendId = friendship.SecondUserId;
                }
                else
                {
                    friendId = friendship.FirstUserId;
                }
                friends.Add(friendId);
            }
            return friends;
        }
        public async Task<List<UserViewModel>> GetMutualFriends(string myId, string userId)
        {
            List<string> myFriends = await _friendshipRepository.GetUserFriendsIds(myId);
            List<string> userFriends = await _friendshipRepository.GetUserFriendsIds(userId);
            List<UserViewModel> mutualFriends = new List<UserViewModel>();
            foreach(string friendId in myFriends) 
            {
                if(userFriends.Contains(friendId))
                {
                    User friend = await _userRepository.GetUserById(friendId);
                    UserViewModel userViewModel = new UserViewModel()
                    {
                        Id = friendId,
                        FirstName = friend.FirstName,
                        LastName = friend.LastName,
                        ProfilePicturePath = friend.ProfilePicturePath
                    };
                    mutualFriends.Add(userViewModel);
                }
            }
            return mutualFriends;
        }
        public async Task<UserRelation> GetUserRelation(string myId, string userId)
        {
            FriendRequest friendRequest = await _friendRequestRepository.GetFriendRequestBetweenTwoUsers(myId, userId);
            UserRelation relation;
            if (friendRequest == null || friendRequest.Status == FriendRequestStatus.Rejected)
            {
                relation = UserRelation.NoRelation;
            }
            else if (friendRequest.Status == FriendRequestStatus.Accepted)
            {
                relation = UserRelation.Friend;
            }
            else
            {
                if (friendRequest.SenderId == myId)
                {
                    relation = UserRelation.SentFriendRequest;
                }
                else
                {
                    relation = UserRelation.ReceivedFriendRequest;
                }
            }
            return relation;
        }
    }
}
