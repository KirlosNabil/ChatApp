using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class GroupChatMember
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public GroupChat Group { get; set; }
    }
}
