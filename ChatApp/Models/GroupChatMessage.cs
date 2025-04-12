using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class GroupChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public GroupChat Group { get; set; }
        public bool Delivered { get; set; }
        public bool IsRead { get; set; }
    }
}
