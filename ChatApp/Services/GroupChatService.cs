using ChatApp.DTOs;
using ChatApp.Mappers;
using ChatApp.Models;
using ChatApp.Repositories;
using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public class GroupChatService : IGroupChatService
    {
        private readonly IGroupChatRepository _groupChatRepository;
        private readonly IUserService _userService;
        public GroupChatService(IGroupChatRepository groupChatRepository, IUserService userService)
        {
            _groupChatRepository = groupChatRepository;
            _userService = userService;
        }
        public async Task<GroupChatViewModel> GetGroupChat(string userId, int groupId)
        {
            GroupChat groupChat = await _groupChatRepository.GetGroup(groupId);
            List<GroupChatMember> groupMembers = await _groupChatRepository.GetGroupMembers(groupId);
            List<GroupChatMessage> groupMessages = await _groupChatRepository.GetGroupMessages(groupId);

            List<GroupChatMessageDTO> messages = new List<GroupChatMessageDTO>();
            foreach(GroupChatMessage message in groupMessages) 
            {
                UserDTO userDTO = await _userService.GetUser(message.SenderId);
                GroupChatMessageDTO messageDTO = GroupChatMessageMapper.ToDTO(message, userDTO);
                messages.Add(messageDTO);
            }

            List<UserDTO> members = new List<UserDTO>();
            foreach(GroupChatMember member in groupMembers)
            {
                UserDTO user = await _userService.GetUser(member.UserId);
                members.Add(user);
            }

            string ownerFullName = await _userService.GetUserFullName(groupChat.OwnerId);
            string myFullName = await _userService.GetUserFullName(userId);

            GroupChatViewModel groupChatViewModel = new GroupChatViewModel()
            {
                GroupChatMessages = messages,
                GroupMembers = members,
                GroupName = groupChat.Name,
                GroupOwnerId = groupChat.OwnerId,
                GroupOwnerName = ownerFullName,
                UserName = myFullName
            };
            return groupChatViewModel;
        }
        public async Task<List<GroupChatsViewModel>> GetGroupChats(string userId)
        {
            List<GroupChatsViewModel> groupChatsViewModels = new List<GroupChatsViewModel>();
            List<int> groupChatsIds = await _groupChatRepository.GetUserGroupsIds(userId);
            foreach(int groupId in groupChatsIds) 
            {
                GroupChat groupChat = await _groupChatRepository.GetGroup(groupId);
                GroupChatMessage lastMessage = await _groupChatRepository.GetGroupLastMessage(groupId);
                string lastMessageSenderName = await _userService.GetUserFullName(lastMessage.SenderId);
                GroupChatsViewModel groupChatsViewModel = new GroupChatsViewModel()
                {
                    GroupId = groupId,
                    GroupName = groupChat.Name,
                    LastMessage = lastMessage.Message,
                    LastMessageSenderName = lastMessageSenderName
                };
                groupChatsViewModels.Add(groupChatsViewModel);
            }
            return groupChatsViewModels;
        }
    }
}
