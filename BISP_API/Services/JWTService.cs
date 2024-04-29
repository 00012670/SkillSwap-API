using BISP_API.Models;
using BISP_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BISP_API.Services
{
    // JWTService class implementing the IJWTService interface
    public class JWTService : IJWTService
    {
        // Configuration object to access app settings
        private readonly IConfiguration _config;

        // Constructor that takes a configuration object
        public JWTService(IConfiguration config)
        {
            _config = config; // Assign the configuration object to the private field
        }

        public string CreateJwt(User auth, bool isSubscriber)
        {
            // Create a new JWT handler
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            // Get the secret key from the configuration
            var key = Encoding.ASCII.GetBytes(_config["JWT:SecretKey"]);

            // Create a new identity with the necessary claims
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, auth.Role), // Role claim
                new Claim(ClaimTypes.Name, $"{auth.Username}"), // Name claim
                new Claim("userId", $"{auth.UserId}"), // User ID claim
                new Claim(ClaimTypes.NameIdentifier, $"{auth.UserId}"), // Name Identifier claim
                new Claim("isSubscriber", $"{isSubscriber}") // Subscriber status claim
            });

            // Create the signing credentials
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            // Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity, // The identity for the token
                Expires = DateTime.Now.AddMinutes(_config.GetValue<int>("JWT:ExpirationMinutes")), // The expiration time for the token
                SigningCredentials = credentials // The signing credentials for the token
            };

            // Create the token
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            // Return the written token
            return jwtTokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            // Get the secret key from the configuration
            var key = Encoding.ASCII.GetBytes(_config["JWT:SecretKey"]);

            // Define the token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, // Don't validate the audience
                ValidateIssuer = false, // Don't validate the issuer
                ValidateIssuerSigningKey = true, // Do validate the issuer signing key
                IssuerSigningKey = new SymmetricSecurityKey(key), // The issuer signing key
                ValidateLifetime = false // Don't validate the lifetime
            };

            // Create a new JWT handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Declare a security token
            SecurityToken securityToken;

            // Validate the token and get the principal
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            // Cast the security token to a JWT
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            // If the token is null or the algorithm doesn't match, throw an exception
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");

            // Return the principal
            return principal;
        }

        public string CreateRefreshToken()
        {
            // Generate a random number
            var tokenBytes = RandomNumberGenerator.GetBytes(64);

            // Convert the random number to a base64 string
            var refreshToken = Convert.ToBase64String(tokenBytes);

            // Return the refresh token
            return refreshToken;
        }
    }
}
