using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IHomeService
    {
        public Task<List<SearchUserViewModel>> SearchUser(string userId, string searchedName);
    }
}
