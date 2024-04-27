namespace BISP_API.Models.Dto
{
    public class MessageReadDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public int? ImageId { get; set; }
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsEdited { get; set; }
    }
}
