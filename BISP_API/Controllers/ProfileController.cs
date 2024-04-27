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



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile([FromRoute] int id)
        {
            var profile = await _profileContext.Users.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }

            // Delete or reassign all messages associated with the user
            var messagesSent = _profileContext.Messages.Where(m => m.SenderId == id);
            var messagesReceived = _profileContext.Messages.Where(m => m.ReceiverId == id);
            _profileContext.Messages.RemoveRange(messagesSent);
            _profileContext.Messages.RemoveRange(messagesReceived);

            // Delete or reassign all SwapRequests associated with the user
            var swapRequestsInitiated = _profileContext.SwapRequests.Where(sr => sr.InitiatorId == id);
            var swapRequestsReceived = _profileContext.SwapRequests.Where(sr => sr.ReceiverId == id);
            _profileContext.SwapRequests.RemoveRange(swapRequestsInitiated);
            _profileContext.SwapRequests.RemoveRange(swapRequestsReceived);

            _profileContext.Users.Remove(profile);
            await _profileContext.SaveChangesAsync();

            return Ok(profile);
        }



        [HttpPut("{id}/suspend")]
        public async Task<IActionResult> SuspendUser([FromRoute] int id)
        {
            var user = await _profileContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsSuspended = true;
            await _profileContext.SaveChangesAsync();

            return NoContent();
        }



        [HttpPut("{id}/unsuspend")]
        public async Task<IActionResult> UnsuspendUser([FromRoute] int id)
        {
            var user = await _profileContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsSuspended = false;
            await _profileContext.SaveChangesAsync();

            return NoContent();
        }


    }
}
