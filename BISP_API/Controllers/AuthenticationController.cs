using BISP_API.Context;
using BISP_API.Helpers;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using System.Text;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly dbContext _authContext;

        public AuthenticationController(dbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Authentication authObj)
        {
            if (authObj == null)
                return BadRequest();

            var auth = await _authContext.Authentications
                .FirstOrDefaultAsync(x => x.Username == authObj.Username);
            if (auth == null)
                return NotFound(new { Message = "User not found?" });

            if (!PasswordHasher.VerifyPassword(authObj.Password, auth.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect" });
            }

            return Ok(new
            {
                Message = "Login success!"
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Authentication authObj)
        {
            if (authObj == null)
                return BadRequest();

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
            await _authContext.AddAsync(authObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Status = 200,
                Message = "User Added!"
            });
        }

        private Task<bool> CheckEmailExistAsync(string? email)
             => _authContext.Authentications.AnyAsync(x => x.Email == email);

        private Task<bool> CheckUsernameExistAsync(string? username)
            => _authContext.Authentications.AnyAsync(x => x.Username == username);

        private static string CheckPasswordStrength(string pass)
        {
            StringBuilder sb = new StringBuilder();
            if (pass.Length < 9)
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            if (!Regex.IsMatch(pass, "[<,>,@,!,#,$,%,^,&,*,(,),_,+,\\[,\\],{,},?,:,;,|,',\\,.,/,~,`,-,=]"))
                sb.Append("Password should contain special charcter" + Environment.NewLine);
            return sb.ToString();
        }
    }
}
