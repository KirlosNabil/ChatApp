namespace ChatApp.DTOs
{
    public class GroupChatMessageDTO
    {
        public int GroupId { get; set; }
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public string SenderFullName { get; set; }
        public string SenderId { get; set; }
        public string SenderProfilePicturePath { get; set; }
    }
}
