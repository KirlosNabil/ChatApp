using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class ViewUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicturePath { get; set; }
        public UserRelation Relation { get; set; }
        public List<UserViewModel> MutualFriends { get; set; }
    }
}
