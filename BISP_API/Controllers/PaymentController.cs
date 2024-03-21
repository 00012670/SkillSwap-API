using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BISP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreatePaymentIntent()
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 1000, 
                Currency = "usd",
            };

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = service.Create(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }
    }

}
