using BISP_API.Models;

namespace BISP_API.UtilitySeervice
{
    public interface IEmailService
    {
        void SendEmail(EmailModel emailModel);
    }
}
