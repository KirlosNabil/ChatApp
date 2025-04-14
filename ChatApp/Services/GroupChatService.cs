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
        public async Task<GroupChatViewModel> GetGroupChat(int groupId)
        {
            GroupChat groupChat = await _groupChatRepository.GetGroup(groupId);
            List<GroupChatMember> groupMembers = await _groupChatRepository.GetGroupMembers(groupId);
            List<GroupChatMessage> groupMessages = await _groupChatRepository.GetGroupMessages(groupId);

            List<Tuple<GroupChatMessage, string>> messages = new List<Tuple<GroupChatMessage, string>>();
            foreach(GroupChatMessage message in groupMessages) 
            {
                Tuple<string, string> userName = await _userService.GetUserName(message.SenderId);
                messages.Add(new Tuple<GroupChatMessage, string>(message, userName.Item1 + " " + userName.Item2));
            }

            List<Tuple<string, string>> members = new List<Tuple<string, string>>();
            foreach(GroupChatMember member in groupMembers)
            {
                Tuple<string, string> userName = await _userService.GetUserName(member.UserId);
                members.Add(new Tuple<string, string>(member.UserId, userName.Item1 + " " + userName.Item2));
            }

            Tuple<string, string> ownerName = await _userService.GetUserName(groupChat.OwnerId);

            GroupChatViewModel groupChatViewModel = new GroupChatViewModel()
            {
                GroupChatMessages = messages,
                GroupMembers = members,
                GroupName = groupChat.Name,
                GroupOwnerId = groupChat.OwnerId,
                GroupOwnerName = ownerName.Item1 + " " + ownerName.Item2
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
                Tuple<string, string> lastMessageSenderName = await _userService.GetUserName(lastMessage.SenderId);
                GroupChatsViewModel groupChatsViewModel = new GroupChatsViewModel()
                {
                    GroupId = groupId,
                    GroupName = groupChat.Name,
                    LastMessage = lastMessage.Message,
                    LastMesasageSenderName = lastMessageSenderName.Item1 + " " + lastMessageSenderName.Item2
                };
                groupChatsViewModels.Add(groupChatsViewModel);
            }
            return groupChatsViewModels;
        }
    }
}
