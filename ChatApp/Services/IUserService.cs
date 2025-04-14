using ChatApp.DTOs;
using ChatApp.ViewModels;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public Task<UserDTO> GetUser(string userId);
        public Task<UserViewModel> GetUserViewModel(string userId);
        public Task<ViewUserViewModel> GetViewUserViewModel(string myId, string userId);
        public Task<string> GetUserFullName(string userId);
    }
}
