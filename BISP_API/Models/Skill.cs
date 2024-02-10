namespace BISP_API.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public SkillLevel Level { get; set; }
        public string Prerequisity { get; set; }
        public int UserId { get; set; }  
        public User User { get; set; }  
    }

    public enum SkillLevel
    {
        Foundational,
        Competent,
        Expert,
        Master
    }
}
