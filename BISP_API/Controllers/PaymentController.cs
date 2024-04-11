using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace BISP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {

        [HttpGet("Products")]
        public IActionResult Product() 
        {
            StripeConfiguration.ApiKey = "sk_test_51OuxWhP2zd9Sg1qK2AZzWnAtnjbxd069o0EVoSgDwaSc8GI98keZQkv8YADGO6dibvoZRYCDORS0bxQcQ1afDvLz00IEedfrQX";

            var service = new ProductService();
            var options = new ProductListOptions { Limit = 3 };
            StripeList<Product> products = service.List(options);

            return Ok(products);
        }


        //[HttpPost]
        //public IActionResult CreatePaymentIntent()
        //{
        //    var options = new PaymentIntentCreateOptions
        //    {
        //        Amount = 1000, 
        //        Currency = "usd",
        //    };

        //    var service = new PaymentIntentService();
        //    PaymentIntent paymentIntent = service.Create(options);

        //    return Ok(new { clientSecret = paymentIntent.ClientSecret });
        //}
    }

}
