namespace ChatApp.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public User Sender { get; set; }
        public string ReceiverId { get; set; }
        public User Receiver { get; set; }
        public FriendRequestStatus Status { get; set; }
    }
}
