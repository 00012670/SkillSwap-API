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

            _profileContext.Users.Remove(profile);
            await _profileContext.SaveChangesAsync();

            return Ok(profile);
        }
    }
}
