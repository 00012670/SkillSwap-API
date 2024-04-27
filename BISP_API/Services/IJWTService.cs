using BISP_API.Models;
using System.Security.Claims;
using System.Security.Cryptography;

namespace BISP_API.Services
{
    public interface IJWTService
    {
        string CreateJwt(User auth, bool isSubscriber);
        ClaimsPrincipal GetPrincipleFromExpiredToken(string token);
        string CreateRefreshToken();

     

    }
}
