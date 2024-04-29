using Microsoft.AspNetCore.Identity;
using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BISP_API.Models
{
    public class User 
    {
        //public User()
        //{
        //    Skills = new HashSet<Skill>();
        //    SwapRequestsInitiated = new HashSet<SwapRequest>();
        //    SwapRequestsReceived = new HashSet<SwapRequest>();
        //    ReviewsSent = new HashSet<Review>();
        //    ReviewsReceived = new HashSet<Review>();
        //    SentMessages = new HashSet<Message>();
        //    ReceivedMessages = new HashSet<Message>();
        //}

        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; } 

        public string FullName { get; set; }

        public string Bio { get; set; }

        public string SkillInterested { get; set; }

        public string Token { get; set; }

        public string Role { get; set; }

        public Image ProfileImage { get; set; }

        public bool HasImage { get; set; }

        public bool IsPremium { get; set; }

        public bool IsSuspended { get; set; }
        public string RefreshToken { get; set; }
        public string GoogleId { get; set; }

        public string ResetPasswordToken { get; set; }
        public DateTime ResetPasswordExpiry { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public string CustomerId { get; set; }

        public ICollection<Skill> Skills { get; set; }

        public ICollection<SwapRequest> SwapRequestsInitiated { get; set; }

        public ICollection<SwapRequest> SwapRequestsReceived { get; set; }

        public ICollection<Review> ReviewsSent { get; set; }

        public ICollection<Review> ReviewsReceived { get; set; }

        public ICollection<Message> SentMessages { get; set; }

        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Notification> Notifications { get; set; }

    }

}
