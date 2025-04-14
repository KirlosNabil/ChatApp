using ChatApp.DTOs;
using ChatApp.Models;
using ChatApp.ViewModels;

namespace ChatApp.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToDTO(User user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath
            };
        }
        public static UserViewModel ToViewModel(User user) 
        {
            return new UserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath
            };
        }
        public static ViewUserViewModel ToViewUserViewModel(User user, List<UserViewModel> mutualFriends, UserRelation relation)
        {
            return new ViewUserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePicturePath = user.ProfilePicturePath,
                Relation = relation,
                MutualFriends = mutualFriends
            };
        }
    }
}
