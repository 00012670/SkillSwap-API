namespace BISP_API.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateCreated { get; set; }
        public NotificationType Type { get; set; } 
        public User User { get; set; }
        public int? MessageId { get; set; }
        public Message Message { get; set; }

        public int? SwapRequestId { get; set; }
        public SwapRequest SwapRequest { get; set; }

    }

    public enum NotificationType
    {
        Message,
        SwapRequest
    }

}
