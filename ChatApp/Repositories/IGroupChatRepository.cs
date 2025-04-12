using ChatApp.Models;

namespace ChatApp.Repositories
{
    public interface IGroupChatRepository
    {
        public Task AddGroup(GroupChat groupChat);
        public Task AddGroupMember(GroupChatMember groupChatMember);
        public Task AddGroupMessage(GroupChatMessage groupChatMessage);
        public Task UpdateGroup(GroupChat groupChat);
        public Task UpdateGroupMember(GroupChatMember groupChatMember);
        public Task UpdateGroupMessage(GroupChatMessage groupChatMessage);
        public Task DeleteGroup(int Id);
        public Task DeleteGroupMember(int Id);
        public Task DeleteGroupMessage(int Id);
        public Task<GroupChat> GetGroup(int Id);
        public Task<GroupChatMember> GetGroupMember(int Id);
        public Task<GroupChatMessage> GetGroupMessage(int Id);
    }
}
