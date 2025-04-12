using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class GroupChat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User Owner { get; set; }
    }
}
