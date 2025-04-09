using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public string FirstUserId { get; set; }
        [ForeignKey("FirstUserId")]
        public User FirstUser { get; set; }
        public string SecondUserId { get; set; }
        [ForeignKey("SecondUserId")]
        public User SecondUser { get; set; }
    }
}
