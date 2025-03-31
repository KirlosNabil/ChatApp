using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IHomeService
    {
        public Task<List<UserViewModel>> SearchUser(string userId, string searchedName);
    }
}
