namespace BISP_API.Models.Dto
{
    public class MessageReadDto
    {
        public int MessageId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderImage { get; set; } 
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
