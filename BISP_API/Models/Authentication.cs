using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models
{
    public class Authentication
    {
        [Key]

        public int AuthId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        public string Role { get; set; }
    }
}
