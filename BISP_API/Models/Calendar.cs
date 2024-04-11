using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models
{
    public class Calendar
    {
        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string EventDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int UserId { get; set; } 
        public User User { get; set; }
    }
}
