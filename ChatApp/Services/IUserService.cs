using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public Task<Tuple<string, string>> GetUserName(string userId);
        public Task<UserViewModel> GetUser(string userId);
        public Task<ViewUserViewModel> GetUserView(string myId, string userId);
    }
}
