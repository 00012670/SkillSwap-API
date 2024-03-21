using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : Controller
    {
        private readonly BISPdbContext _profileContext;

        public ProfileController(BISPdbContext dbContext)
        {
            _profileContext = dbContext;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetProfilebyId([FromRoute] int id)
        {
            var profile = await _profileContext.Users
                .Include(u => u.Skills)
                .FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet]
        [Route("{id}/username")]
        public async Task<ActionResult> GetUsername([FromRoute] int id)
        {
            var user = await _profileContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(new { username = user.Username });
        }

        [HttpPut()]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] int id, User updateProfileRequest)
        {
            var profile = await _profileContext.Users.FindAsync(id);

            if (profile == null)
            {
                return NotFound();
            }

            profile.FullName = updateProfileRequest.FullName;
            profile.Bio = updateProfileRequest.Bio;
            profile.SkillInterested = updateProfileRequest.SkillInterested;

            await _profileContext.SaveChangesAsync();

            return Ok(profile);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteProfile([FromRoute] int id)
        //{
        //    var profile = await _profileContext.Users.FindAsync(id);
        //    if (profile == null)
        //    {
        //        return NotFound();
        //    }

        //    _profileContext.Users.Remove(profile);
        //    await _profileContext.SaveChangesAsync();

        //    return Ok(profile);
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _profileContext.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            int anonymousUserId = 0; 

            var messagesSent = _profileContext.Messages.Where(m => m.SenderId == userId);
            var messagesReceived = _profileContext.Messages.Where(m => m.ReceiverId == userId);
            foreach (var message in messagesSent)
            {
                message.SenderId = anonymousUserId;
            }
            foreach (var message in messagesReceived)
            {
                message.ReceiverId = anonymousUserId;
            }

            _profileContext.Users.Remove(user);
            await _profileContext.SaveChangesAsync();

            return NoContent();
        }



    }
}
