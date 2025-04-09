using Microsoft.AspNetCore.Identity;

namespace ChatApp.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfilePicturePath { get; set; }
        public Gender Gender { get; set; }
    }
}
