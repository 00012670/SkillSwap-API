using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using BISP_API.Models.Stripe;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using BISP_API.Models;


//public class StripeOptions
//{
//    public string option { get; set; }
//}


namespace BISP_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly StripeSettings _stripeSettings;

        private readonly UserManager<User> _userManager;
        public PaymentController(IOptions<StripeSettings> stripeSettings, UserManager<User> userManager)
        {
            _stripeSettings = stripeSettings.Value;
            _userManager = userManager;

        }
        //[HttpGet("Products")]
        //public IActionResult Product() 
        //{
        //    StripeConfiguration.ApiKey = "sk_test_51OuxWhP2zd9Sg1qK2AZzWnAtnjbxd069o0EVoSgDwaSc8GI98keZQkv8YADGO6dibvoZRYCDORS0bxQcQ1afDvLz00IEedfrQX";

        //    var service = new ProductService();
        //    var options = new ProductListOptions { Limit = 3 };
        //    StripeList<Product> products = service.List(options);

        //    return Ok(products);
        //}


        //[HttpPost]
        //public ActionResult Create()
        //{
        //    var domain = "http://localhost:4242";

        //    var priceOptions = new PriceListOptions
        //    {
        //        LookupKeys = new List<string> {
        //            Request.Form["lookup_key"]
        //        }
        //    };
        //    var priceService = new PriceService();
        //    StripeList<Price> prices = priceService.List(priceOptions);

        //    var options = new SessionCreateOptions
        //    {
        //        LineItems = new List<SessionLineItemOptions>
        //        {
        //          new SessionLineItemOptions
        //          {
        //            Price = prices.Data[0].Id,
        //            Quantity = 1,
        //          },
        //        },
        //        Mode = "subscription",
        //        SuccessUrl = domain + "/success.html?session_id={CHECKOUT_SESSION_ID}",
        //        CancelUrl = domain + "/cancel.html",
        //    };
        //    var service = new SessionService();
        //    Session session = service.Create(options);

        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {

                SuccessUrl = req.SuccessUrl,
                CancelUrl = req.FailureUrl,
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = req.PriceId,
                        Quantity = 1,
                    },
                },
            };

            var service = new SessionService();
            service.Create(options);
            try
            {
                var session = await service.CreateAsync(options);
                return Ok(new CreateCheckoutSessionResponse
                {
                    SessionId = session.Id,
                    PublicKey = _stripeSettings.PublicKey,
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new ErrorResponse
                {
                    ErrorMessage = new ErrorMessage
                    {
                        Message = e.StripeError.Message,
                    }
                });
            }
        }

        [Authorize]

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal([FromBody] CustomerPortalRequest req)
        {
            //Customer Portal API -- Inside try block
            ClaimsPrincipal principal = HttpContext.User as ClaimsPrincipal;
            var claim = principal.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname");
            var userFromDb = await _userManager.FindByNameAsync(claim.Value);

            if (userFromDb == null)
            {
                return BadRequest();
            }


            try
            {
                var options = new Stripe.BillingPortal.SessionCreateOptions
                {
                    Customer = userFromDb.CustomerId,
                    ReturnUrl = req.ReturnUrl,
                };
                var service = new Stripe.BillingPortal.SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new
                {
                    url = session.Url
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new ErrorResponse
                {
                    ErrorMessage = new ErrorMessage
                    {
                        Message = e.StripeError.Message,
                    }
                });
            }
        }

        //[Route("create-portal-session")]
        //[ApiController]
        //public class PortalApiController : Controller
        //{
        //    [HttpPost]
        //    public ActionResult Create()
        //    {
        //        // For demonstration purposes, we're using the Checkout session to retrieve the customer ID.
        //        // Typically this is stored alongside the authenticated user in your database.
        //        var checkoutService = new SessionService();
        //        var checkoutSession = checkoutService.Get(Request.Form["session_id"]);

        //        // This is the URL to which your customer will return after
        //        // they are done managing billing in the Customer Portal.
        //        var returnUrl = "http://localhost:4242";

        //        var options = new Stripe.BillingPortal.SessionCreateOptions
        //        {
        //            Customer = checkoutSession.CustomerId,
        //            ReturnUrl = returnUrl,
        //        };
        //        var service = new Stripe.BillingPortal.SessionService();
        //        var session = service.Create(options);

        //        Response.Headers.Add("Location", session.Url);
        //        return new StatusCodeResult(303);
        //    }
        //}

        //[Route("webhook")]
        //[ApiController]
        //public class WebhookController : Controller
        //{
        //    [HttpPost]
        //    public async Task<IActionResult> Index()
        //    {
        //        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        //        // Replace this endpoint secret with your endpoint's unique secret
        //        // If you are testing with the CLI, find the secret by running 'stripe listen'
        //        // If you are using an endpoint defined with the API or dashboard, look in your webhook settings
        //        // at https://dashboard.stripe.com/webhooks
        //        const string endpointSecret = "whsec_12345";
        //        try
        //        {
        //            var stripeEvent = EventUtility.ParseEvent(json);
        //            var signatureHeader = Request.Headers["Stripe-Signature"];
        //            stripeEvent = EventUtility.ConstructEvent(json,
        //                    signatureHeader, endpointSecret);
        //            if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
        //            {
        //                var subscription = stripeEvent.Data.Object as Subscription;
        //                Console.WriteLine("A subscription was canceled.", subscription.Id);
        //                // Then define and call a method to handle the successful payment intent.
        //                // handleSubscriptionCanceled(subscription);
        //            }
        //            else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
        //            {
        //                var subscription = stripeEvent.Data.Object as Subscription;
        //                Console.WriteLine("A subscription was updated.", subscription.Id);
        //                // Then define and call a method to handle the successful payment intent.
        //                // handleSubscriptionUpdated(subscription);
        //            }
        //            else if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
        //            {
        //                var subscription = stripeEvent.Data.Object as Subscription;
        //                Console.WriteLine("A subscription was created.", subscription.Id);
        //                // Then define and call a method to handle the successful payment intent.
        //                // handleSubscriptionUpdated(subscription);
        //            }
        //            else if (stripeEvent.Type == Events.CustomerSubscriptionTrialWillEnd)
        //            {
        //                var subscription = stripeEvent.Data.Object as Subscription;
        //                Console.WriteLine("A subscription trial will end", subscription.Id);
        //                // Then define and call a method to handle the successful payment intent.
        //                // handleSubscriptionUpdated(subscription);
        //            }
        //            else
        //            {
        //                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
        //            }
        //            return Ok();
        //        }
        //        catch (StripeException e)
        //        {
        //            Console.WriteLine("Error: {0}", e.Message);
        //            return BadRequest();
        //        }
        //    }
        //}
    }
}


