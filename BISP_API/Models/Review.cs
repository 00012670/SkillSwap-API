using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public int FromUserId { get; set; }
        public User FromUser { get; set; }

        public int ToUserId { get; set; }
        public User ToUser { get; set; }

        public int SkillId { get; set; }
        public Skill Skill { get; set; }

        public int RequestId { get; set; }
        public SwapRequest Request { get; set; }

        public int Rating { get; set; }

        public string Text { get; set; }
    }
}
