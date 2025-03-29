using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public bool Delivered { get; set; }
        public bool IsRead { get; set; }
    }
}
