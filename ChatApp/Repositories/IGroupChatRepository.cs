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
        public Task<GroupChatMember> GetMember(string Id);
        public Task<GroupChatMessage> GetMessage(int Id);
        public Task<List<GroupChatMember>> GetGroupMembers(int Id);
        public Task<List<GroupChatMessage>> GetGroupMessages(int Id);
        public Task<List<int>> GetUserGroupsIds(string userId);
        public Task<GroupChatMessage> GetGroupLastMessage(int groupId);
    }
}
