using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using BISP_API.Models.Stripe;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BISP_API.Models;
using BISP_API.Context;
using BISP_API.Repositories;
using Microsoft.EntityFrameworkCore;


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


        private readonly BISPdbContext _context;

        private readonly ILogger<PaymentController> _logger;

        private readonly ISubscriptionRepository _subscriberRepository;



        public PaymentController(IOptions<StripeSettings> stripeSettings, BISPdbContext context, ILogger<PaymentController> logger, ISubscriptionRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
            _stripeSettings = stripeSettings.Value;
            _context = context;
            _logger = logger;
            _subscriberRepository = subscriberRepository;
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



        private async Task<string> GetStripeCustomerIdForUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user?.CustomerId;
        }

        //[HttpPost]
        //public async Task<IActionResult> Index()
        //{
        //    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        //    try
        //    {
        //        var stripeEvent = EventUtility.ConstructEvent(json,
        //         Request.Headers["Stripe-Signature"], _stripeSettings.WHSecret);


        //        // Handle the event
        //        if (stripeEvent.Type == Events.PaymentIntentSucceeded)
        //        {
        //            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
        //            Console.WriteLine("PaymentIntent was successful!");
        //        }
        //        else if (stripeEvent.Type == Events.PaymentMethodAttached)
        //        {
        //            var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
        //            Console.WriteLine("PaymentMethod was attached to a Customer!");
        //        }
        //        // ... handle other event types
        //        else
        //        {
        //            Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
        //        }

        //        return Ok();
        //    }
        //    catch (StripeException e)
        //    {
        //        return BadRequest(e.Message);
        //    }
        //}
    

[Authorize]
        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal([FromBody] CustomerPortalRequest req)
        {
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User user = null;
            if (int.TryParse(userIdString, out int userId))
            {
                user = await _context.Users.FindAsync(userId);
            }
            else
            {
                return BadRequest("Invalid user ID");
            }
            _logger.LogInformation($"User ID: {userId}");

            if (user == null)
            {
                return BadRequest();
            }

            string customerId = await GetStripeCustomerIdForUser(userId); // Pass userId instead of userIdString
            _logger.LogInformation($"Stripe Customer ID: {customerId}");

            if (string.IsNullOrEmpty(customerId))
            {
                return BadRequest();
            }



            try
            {
                var options = new Stripe.BillingPortal.SessionCreateOptions
                {
                    Customer = customerId,
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


        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            _logger.LogInformation("WebHook method called");


            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                 json,
                 Request.Headers["Stripe-Signature"],
                 _stripeSettings.WHSecret
               );

                // Handle the event
                if (stripeEvent.Type == Events.CustomerSubscriptionCreated)
                {
                    var subscription = stripeEvent.Data.Object as Subscription;

                    //Do stuff
                    await addSubscriptionToDb(subscription);
                    _logger.LogInformation("Handling CustomerSubscriptionCreated event");

                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var session = stripeEvent.Data.Object as Subscription;

                    // Update Subsription
                    await updateSubscription(session);
                }
                else if (stripeEvent.Type == Events.CustomerCreated)
                {
                    _logger.LogInformation("Handling CustomerSubscriptionUpdated event");

                    var customer = stripeEvent.Data.Object as Customer;

                    //Do Stuff
                    await addCustomerIdToUser(customer);
                }
                // ... handle other event types
                else
                {
                    // Unexpected event type
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest();
            }
            catch (Exception e) // catch all other exceptions
            {
                Console.WriteLine(e.Message);
                return StatusCode(500, new { message = e.Message, stackTrace = e.StackTrace });
            }
        }

        private async Task updateSubscription(Subscription subscription)
        {
            try
            {
                var subscriptionFromDb = await _subscriberRepository.GetByIdAsync(subscription.Id);
                if (subscriptionFromDb != null)
                {
                    subscriptionFromDb.Status = subscription.Status;
                    subscriptionFromDb.CurrentPeriodEnd = subscription.CurrentPeriodEnd;
                    await _subscriberRepository.UpdateAsync(subscriptionFromDb);
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unable to update subscription");
            }

        }

        private async Task addCustomerIdToUser(Customer customer)
        {
            try
            {
                var userFromDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == customer.Email);

                if (userFromDb != null)
                {
                    userFromDb.CustomerId = customer.Id;
                    _context.Users.Update(userFromDb);
                    await _context.SaveChangesAsync();
                }

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unable to add customer id to user");
            }
        }

        private async Task addSubscriptionToDb(Subscription subscription)
        {
            _logger.LogInformation("addSubscriptionToDb called");

            try
            {
                var subscriber = new Subscriber
                {
                    Id = subscription.Id,
                    CustomerId = subscription.CustomerId,
                    Status = "active",
                    CurrentPeriodEnd = subscription.CurrentPeriodEnd
                };
                await _subscriberRepository.CreateAsync(subscriber);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unable to add new subscriber to Database");
            }
        }
    }
}




