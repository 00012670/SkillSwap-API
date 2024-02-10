using BISP_API.Context;
using BISP_API.Helper;
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
        private readonly IWebHostEnvironment hostingEnv;

        public ProfileController(BISPdbContext dbContext, IWebHostEnvironment environment)
        {
            _profileContext = dbContext;
            hostingEnv = environment;

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


        //[HttpPut("UploadImage")]
        //public async Task<ActionResult> UploadImage(IFormFile formFile, string imgId)
        //{
        //    APIResponse response = new();
        //    try
        //    {
        //        string Filepath = GetFilePath(imgId);
        //        if (!System.IO.Directory.Exists(Filepath))
        //        {
        //            System.IO.Directory.CreateDirectory(Filepath);
        //        }
        //        string imagepath = Filepath + "\\" + imgId + ".png";
        //        if (System.IO.File.Exists(imagepath))
        //        {
        //            System.IO.File.Delete(imagepath);
        //        }
        //        using FileStream stream = System.IO.File.Create(imagepath);
        //        await formFile.CopyToAsync(stream);
        //        response.ResponseCode = 200;
        //        response.Result = "pass";
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Message = ex.Message;
        //    }
        //    return Ok(response);
        //}


        //[HttpGet("GetImage")]
        //public async Task<IActionResult> GetImage(string imgId)
        //{
        //    string Imageurl = string.Empty;
        //    string hosturl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        //    try
        //    {
        //        string Filepath = GetFilePath(imgId);
        //        string imagepath = Filepath + "\\" + imgId + ".png";
        //        if (System.IO.File.Exists(imagepath))
        //        {
        //            Imageurl = hosturl + "/Uploads/Profile/" + imgId + "/" + imgId + ".png";
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return Ok(Imageurl);
        //}

        //[HttpDelete("RemoveImage")]
        //public async Task<IActionResult> RemoveImage(string imgId)
        //{
        //    try
        //    {
        //        string Filepath = GetFilePath(imgId);
        //        string Imagepath = Filepath + "\\" + imgId + ".png";
        //        if (System.IO.File.Exists(Imagepath))
        //        {
        //            System.IO.File.Delete(Imagepath);
        //            return Ok("pass");
        //        }
        //        else
        //        {
        //            return NotFound();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return NotFound();
        //    }
        //}

       
        [NonAction]
        public string GetFilePath(string imgId)
        {
            return this.hostingEnv.WebRootPath + "\\Uploads\\Profile\\" + imgId;
        }

    }
}
