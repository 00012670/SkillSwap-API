using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BISP_API.Models
{
    [Table("User")]
    public class User
    {
        [Key]

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }

        public string Bio { get; set; }

        public string SkillInterested { get; set; }

        public string Token { get; set; }

        public string Role { get; set; }

        public Image ProfileImage { get; set; }

        public ICollection<Skill> Skills { get; set; }

    }
}
