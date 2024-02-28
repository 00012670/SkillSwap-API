using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models
{
    public class SwapRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int InitiatorId { get; set; }
        public User Initiator { get; set; }

        public int ReceiverId { get; set; }
        public User Receiver { get; set; }

        public int SkillOfferedId { get; set; }
        public Skill SkillOffered { get; set; }

        public int SkillRequestedId { get; set; }
        public Skill SkillRequested { get; set; }

        public string Details { get; set; }
        // public bool IsDeleted { get; set; }

        public Status StatusRequest { get; set; }
        public ICollection<Review> Reviews { get; set; }



        public enum Status
        {
            Pending,
            Accepted,
            Rejected,
            Invalid
        }
    }
}
