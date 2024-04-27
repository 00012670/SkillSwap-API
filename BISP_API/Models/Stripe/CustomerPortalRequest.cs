using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models.Stripe
{
    public class CustomerPortalRequest
    {
        [Required]
        public string ReturnUrl { get; set; }
    }
}
