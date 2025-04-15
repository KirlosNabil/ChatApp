using System.Collections.Concurrent;
using System.Linq;
using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.ViewModels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class FriendHub : Hub
    {
        private readonly IFriendService _friendService;
        private readonly IUserService _userService;
        private static ConcurrentDictionary<string, List<string>> _connectedUsers = new ConcurrentDictionary<string, List<string>>();
        public FriendHub(IFriendService friendService, IUserService userService)
        {
            _friendService = friendService;
            _userService = userService;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.UserIdentifier;
                if (userId != null)
                {
                    _connectedUsers.GetOrAdd(userId, _ => new List<string>()).Add(Context.ConnectionId);

                    await NotifyOnline();
                    await ReceiveRequests();
                }
            }
            catch (Exception ex){}
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                string userId = Context.UserIdentifier;
                if (userId != null && _connectedUsers.TryGetValue(userId, out var connections))
                {
                    await NotifyOffline();

                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _connectedUsers.TryRemove(userId, out _);
                    }
                }
            }
            catch (Exception ex){}
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendFriendRequest(string receiverId)
        {
            try
            {
                string userId = Context.UserIdentifier;
                await _friendService.SendFriendRequest(userId, receiverId);
                if (_connectedUsers.ContainsKey(receiverId))
                {
                    await _friendService.UpdateFriendRequestStatus(userId, receiverId, FriendRequestStatus.Pending);
                    string userName = await _userService.GetUserFullName(userId);
                    await Clients.User(receiverId).SendAsync("NewFriendRequest", userId, userName);
                }
            }
            catch (Exception ex){}
        }
        public async Task<List<FriendViewModel>> GetOnlineFriends()
        {
            string userId = Context.UserIdentifier;
            List<FriendViewModel> friends = await _friendService.GetUserFriends(userId);
            List<FriendViewModel> onlineFriends = new List<FriendViewModel>();
            foreach (FriendViewModel friend in friends)
            {
                if(_connectedUsers.ContainsKey(friend.FriendId))
                {
                    onlineFriends.Add(friend);
                }
            }
            return onlineFriends;
        }
        public async Task NotifyOnline()
        {
            string userId = Context.UserIdentifier;
            List<string> friendsIds = await _friendService.GetUserFriendsIds(userId);
            foreach (string friendId in friendsIds)
            {
                if (_connectedUsers.ContainsKey(friendId))
                {
                    string userName = await _userService.GetUserFullName(userId);
                    await Clients.User(friendId).SendAsync("FriendOnline", userId, userName);
                }
            }
        }
        public async Task NotifyOffline()
        {
            string userId = Context.UserIdentifier;
            List<string> friendsIds = await _friendService.GetUserFriendsIds(userId);
            foreach (string friendId in friendsIds)
            {
                if (_connectedUsers.ContainsKey(friendId))
                {
                    await Clients.User(friendId).SendAsync("FriendOffline", userId);
                }
            }
        }
        public async Task ReceiveRequests()
        {
            string userId = Context.UserIdentifier;
            List<FriendRequestViewModel> friendRequests = await _friendService.GetUserUnreceivedRequests(userId);
            foreach (FriendRequestViewModel friendRequest in friendRequests)
            {
                await _friendService.UpdateFriendRequestStatus(friendRequest.SenderId, userId, FriendRequestStatus.Pending);
                await Clients.User(userId).SendAsync("NewFriendRequest", friendRequest.SenderId, friendRequest.SenderFirstName, friendRequest.SenderLastName);
            }
        }
    }
}
