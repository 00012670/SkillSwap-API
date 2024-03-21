using BISP_API.Context;
using BISP_API.Helpers;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using BISP_API.Models.Dto;


namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly BISPdbContext _authContext;

        public AuthenticationController(BISPdbContext appDbContext)
        {
            _authContext = appDbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authContext.Users.Include(u => u.Skills).ToListAsync();
            return Ok(users);
        }


        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User authObj)
        {
            if (authObj == null)
                return BadRequest(new { Message = "Request body is missing" });

            var auth = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Username == authObj.Username);
            if (auth == null)
                return NotFound(new { Message = "User not found" });

            if (!PasswordHasher.VerifyPassword(authObj.Password, auth.Password))
            {
                return BadRequest(new { Message = "Incorrect password" });
            }

            auth.Token = CreateJwt(auth);
            var newAccessToken = auth.Token;
            var newRefreshToken = CreateRefreshToken();
            auth.RefreshToken = newRefreshToken;
            auth.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
            await _authContext.SaveChangesAsync();

            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }



        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User authObj)
        {
            if (authObj == null)
                return BadRequest(new { Message = "Request body is missing" });


            //check username
            if (await CheckUsernameExistAsync(authObj.Username))
                return BadRequest(new { Message = "Username already exist" });


            // check email
            if (await CheckEmailExistAsync(authObj.Email))
                return BadRequest(new { Message = "Email already exist" });


            var passMessage = CheckPasswordStrength(authObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                return BadRequest(new { Message = passMessage.ToString() });
            authObj.Password = PasswordHasher.HashPassword(authObj.Password);
            authObj.Role = "User";
            authObj.Token = "";
            await _authContext.AddAsync(authObj);
            await _authContext.SaveChangesAsync();

            authObj.Token = CreateJwt(authObj);


            return Ok(new
            {
                Status = 200,
                Message = "User Added!",
                authObj.Token,

            });
        }



        private Task<bool> CheckEmailExistAsync(string email)
             => _authContext.Users.AnyAsync(x => x.Email == email);

        private Task<bool> CheckUsernameExistAsync(string username)
            => _authContext.Users.AnyAsync(x => x.Username == username);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special character" + Environment.NewLine);
            return sb.ToString();
        }



        private string CreateJwt(User auth)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, auth.Role),
                new Claim(ClaimTypes.Name,$"{auth.Username}"),
                new Claim("userId", $"{auth.UserId}") 
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddMinutes(15), // Set token to expire after 10 seconds
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }



        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _authContext.Users
                .Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("veryverysceret.....");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("This is Invalid Token");
            return principal;

        }


        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = CreateJwt(user);
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _authContext.SaveChangesAsync();


            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }
    }
}

