using ChatApp.Models;

namespace ChatApp.ViewModels
{
    public class SearchUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRelation Relation { get; set; }
    }
}
