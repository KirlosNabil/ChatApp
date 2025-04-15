using System.Collections.Concurrent;
using System.Reflection;
using ChatApp.Data;
using ChatApp.DTOs;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatApp.Hubs
{
    public class GroupChatHub : Hub
    {
        private readonly IGroupChatService _groupChatService;
        private static ConcurrentDictionary<string, List<string>> _connectedUsers = new ConcurrentDictionary<string, List<string>>();
        public GroupChatHub(IGroupChatService groupChatService)
        {
            _groupChatService = groupChatService;
        }
        public override async Task OnConnectedAsync()
        {
            try
            {
                string userId = Context.UserIdentifier;
                if (userId != null)
                {
                    _connectedUsers.GetOrAdd(userId, _ => new List<string>()).Add(Context.ConnectionId);

                    List<int> groupIds = await _groupChatService.GetUserGroupsIds(userId);
                    foreach(int groupId in groupIds)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"group-{groupId}");
                    }
                }
            }
            catch (Exception ex) { }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            try
            {
                string userId = Context.UserIdentifier;
                if (userId != null && _connectedUsers.TryGetValue(userId, out var connections))
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        _connectedUsers.TryRemove(userId, out _);
                    }

                    List<int> groupIds = await _groupChatService.GetUserGroupsIds(userId);
                    foreach(int groupId in groupIds)
                    {
                        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"group-{groupId}");
                    }
                }
            }
            catch (Exception ex) { }
            await base.OnDisconnectedAsync(exception);
        }
        public async Task SendGroupMessage(int groupId, string message)
        {
            try
            {
                string userId = Context.UserIdentifier;

                GroupChatMessageDTO groupChatMessageDTO = await _groupChatService.SendGroupMessage(userId, groupId, message);
                await Clients.Group($"group-{groupId}").SendAsync("ReceiveGroupMessage", groupChatMessageDTO);
            }
            catch (Exception ex) { }
        }
    }
}
