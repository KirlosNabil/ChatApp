using System.Text.RegularExpressions;
using ChatApp.Data;
using ChatApp.Models;

namespace ChatApp.Repositories
{
    public class GroupChatRepository : IGroupChatRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public GroupChatRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddGroup(GroupChat groupChat)
        {
            await _dbContext.GroupChats.AddAsync(groupChat);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddGroupMember(GroupChatMember groupChatMember)
        {
            await _dbContext.GroupChatMembers.AddAsync(groupChatMember);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddGroupMessage(GroupChatMessage groupChatMessage)
        {
            await _dbContext.GroupChatMessages.AddAsync(groupChatMessage);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateGroup(GroupChat groupChat)
        {
            _dbContext.GroupChats.Update(groupChat);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateGroupMember(GroupChatMember groupChatMember)
        {
            _dbContext.GroupChatMembers.Update(groupChatMember);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateGroupMessage(GroupChatMessage groupChatMessage)
        {
            _dbContext.GroupChatMessages.Update(groupChatMessage);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteGroup(int Id)
        {
            GroupChat groupChat = _dbContext.GroupChats.FirstOrDefault(g => g.Id == Id);
            _dbContext.GroupChats.Remove(groupChat);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteGroupMember(int Id)
        {
            GroupChatMember groupChatMember = _dbContext.GroupChatMembers.FirstOrDefault(m => m.Id == Id);
            _dbContext.GroupChatMembers.Remove(groupChatMember);
            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteGroupMessage(int Id)
        {
            GroupChatMessage groupChatMessage = _dbContext.GroupChatMessages.FirstOrDefault(m => m.Id == Id);
            _dbContext.GroupChatMessages.Remove(groupChatMessage);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<GroupChat> GetGroup(int Id)
        {
            GroupChat groupChat = _dbContext.GroupChats.FirstOrDefault(g => g.Id == Id);
            return groupChat;
        }
        public async Task<GroupChatMember> GetGroupMember(int Id)
        {
            GroupChatMember groupChatMember = _dbContext.GroupChatMembers.FirstOrDefault(m => m.Id == Id);
            return groupChatMember;
        }
        public async Task<GroupChatMessage> GetGroupMessage(int Id)
        {
            GroupChatMessage groupChatMessage = _dbContext.GroupChatMessages.FirstOrDefault(m => m.Id == Id);
            return groupChatMessage;
        }
    }
}
