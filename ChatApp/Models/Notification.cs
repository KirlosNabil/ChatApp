namespace ChatApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public bool isRead { get; set; }
    }
}
