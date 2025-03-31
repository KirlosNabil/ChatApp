namespace ChatApp.DTOs
{
    public class ChatMessageDTO
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public string SenderFullName { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public bool IsRead { get; set; }
        public bool IsDelivered { get; set; }

    }
}
