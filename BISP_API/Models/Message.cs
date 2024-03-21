namespace BISP_API.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public int? UserId { get; set; }
        public int SenderId { get; set; }
        public string SenderImage { get; set; } 
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
