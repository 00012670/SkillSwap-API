namespace BISP_API.Models.Stripe
{
    public class CreateCheckoutSessionResponse
    {
        public string SessionId { get; set; }
        public string PublicKey { get; set; }   
    }
}
