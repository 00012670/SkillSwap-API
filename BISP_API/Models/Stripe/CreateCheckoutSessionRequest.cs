using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BISP_API.Models.Stripe
{
    public class CreateCheckoutSessionRequest
    {
        [Required]
        public string PriceId { get; set; }
        [Required]
        public string SuccessUrl { get; set; }
        [Required]
        public string FailureUrl { get; set; }
    }
}
