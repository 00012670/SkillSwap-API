using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models
{
    public class SkillImage
    {
        [Key]
        public int SkillImageId { get; set; }
        public byte[] Img { get; set; }
        public int SkillId { get; set; }
        public Skill Skill { get; set; }

    }
}
