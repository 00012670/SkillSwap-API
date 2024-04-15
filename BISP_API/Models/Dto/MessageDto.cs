namespace BISP_API.Models.Dto
{
    public class MessageDto
    {
        public int SenderId { get; set; }
        public int? ImageId { get; set; }
        public int ReceiverId { get; set; }
        public string MessageText { get; set; }
        public bool IsEdited { get; set; }

    }
}
