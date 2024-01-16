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


namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly BISPdbContext _userContext;

        public AuthenticationController(BISPdbContext appDbContext)
        {
            _userContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User authObj)
        {
            if (authObj == null)
                return BadRequest(new { Message = "Request body is missing" });


            var auth = await _userContext.Users
                .FirstOrDefaultAsync(x => x.Username == authObj.Username);
            if (auth == null)
                return NotFound(new { Message = "User not found" });

            if (!PasswordHasher.VerifyPassword(authObj.Password, auth.Password))
            {
                return BadRequest(new { Message = "Incorrect password" });
            }

            auth.Token = CreateJwt(auth);

            return Ok(new
            {
                auth.Token,
                Message = "Login success!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User authObj)
        {
            if (authObj == null)
                return BadRequest(new { Message = "Request body is missing" });


            // check email
            if (await CheckEmailExistAsync(authObj.Email))
                return BadRequest(new { Message = "Email already exist" });

            //check username
            if (await CheckUsernameExistAsync(authObj.Username))
                return BadRequest(new { Message = "Username already exist" });

            var passMessage = CheckPasswordStrength(authObj.Password);
            if (!string.IsNullOrEmpty(passMessage))
                return BadRequest(new { Message = passMessage.ToString() });
            authObj.Password = PasswordHasher.HashPassword(authObj.Password);
            authObj.Role = "User";
            authObj.Token = "";
            await _userContext.AddAsync(authObj);
            await _userContext.SaveChangesAsync();

            authObj.Token = CreateJwt(authObj);


            return Ok(new
            {
                Status = 200,
                Message = "User Added!",
                authObj.Token,

            });
        }

        private Task<bool> CheckEmailExistAsync(string email)
             => _userContext.Users.AnyAsync(x => x.Email == email);

        private Task<bool> CheckUsernameExistAsync(string username)
            => _userContext.Users.AnyAsync(x => x.Username == username);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
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

            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(10),
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _userContext.Users.ToListAsync());
        }
    }

}
