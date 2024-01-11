using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Claims;

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
            var profile = await _profileContext.Users.FirstOrDefaultAsync(x => x.UserId == id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }





        [HttpPost()]
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
            profile.Picture = updateProfileRequest.Picture;

            await _profileContext.SaveChangesAsync();

            return Ok(profile);
        }


        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _profileContext.Users.ToListAsync());
        }

        //[HttpPost, DisableRequestSizeLimit]
        //public IActionResult UploadImage()
        //{
        //    try
        //    {
        //        var file = Request.Form.Files[0];
        //        var folderName = Path.Combine("Resources", "Images");
        //        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        //        if(file.Length > 0)
        //        {
        //            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //            var fullPath = Path.Combine(pathToSave, fileName);
        //            var dbPath = Path.Combine(folderName, fileName);

        //            using (var stream = new FileStream(fullPath, FileMode.Create))
        //            {
        //                file.CopyTo(stream);
        //            }
        //            return Ok(new { dbPath });
        //        }
        //        else
        //        {
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex}");
        //    }
        //}
    }
}
