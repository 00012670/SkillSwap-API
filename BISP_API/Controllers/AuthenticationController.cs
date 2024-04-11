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
using BISP_API.UtilitySeervice;
using Google.Apis.Auth;



namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly BISPdbContext _authContext;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;


        public AuthenticationController(BISPdbContext appDbContext, IConfiguration configuration, IEmailService emailService)
        {
            _authContext = appDbContext;
            _config = configuration;
            _emailService = emailService;
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

            // Check if the user is suspended
            if (auth.IsSuspended)
                return BadRequest(new { Message = "This account has been suspended" });

            if (!PasswordHasher.VerifyPassword(authObj.Password, auth.Password))
            {
                return BadRequest(new { Message = "Incorrect password" });
            }

            auth.Token = CreateJwt(auth);
            var newAccessToken = auth.Token;
            var newRefreshToken = CreateRefreshToken();
            auth.RefreshToken = newRefreshToken;
            auth.RefreshTokenExpiryTime = DateTime.Now.AddDays(1);
            await _authContext.SaveChangesAsync();

            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = auth.UserId,
            });
        }



        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleAuthDto googleAuthDto)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(googleAuthDto.IdToken);
            var googleId = payload.Subject;

            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == payload.Email);

            if (user == null)
            {
                // Create a new user
                user = new User
                {
                    GoogleId = googleId,
                    Email = payload.Email,
                    Username = !string.IsNullOrEmpty(payload.Email) && payload.Email.Contains('@') ? payload.Email.Split('@')[0] : "defaultUsername",
                    Role = "User", // Set a default role

                };

                // Add checks for Username, Role, and UserId
                if (string.IsNullOrEmpty(user.Username))
                    user.Username = "defaultUsername"; // Set a default username if none is provided

                if (string.IsNullOrEmpty(user.Role))
                    user.Role = "User"; // Set a default role if none is provided

                _authContext.Users.Add(user);
                await _authContext.SaveChangesAsync();
            }

            // Create a JWT for the user and return it
            var token = CreateJwt(user);
            return Ok(new { token });
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

            //Ser "User" role
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
                UserId = authObj.UserId

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



        [HttpPost("SendResetEmail/{email}")] 
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email doesn't exist"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _config["EmailSettings:From"];
            var emailModel = new EmailModel(email, "Reset Password!!", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent!"
            }) ;
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> RessetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToke = resetPasswordDto.EmailToken.Replace(" ", "+");
            var user = await _authContext.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
            if (user is null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User doesn't exist"
                });
            }
             var tokenCode = user.ResetPasswordToken;
             DateTime emailTokenExpiry = user.ResetPasswordExpiry;
             if(tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
             {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid reset link"
                });
             }
            user.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _authContext.Entry(user).State = EntityState.Modified;
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Password reset successfully"
            });
        }
    }
}

