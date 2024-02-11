using BISP_API.Context;
using BISP_API.Helper;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController: ControllerBase
    {
        private readonly BISPdbContext _dbContext;
        private readonly IWebHostEnvironment hostingEnv;

        public ImageController(BISPdbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            hostingEnv = environment;

        }

        [HttpPost("UploadImage/{userId}")]
        public async Task<ActionResult> UploadImage(IFormFile formFile, int userId)
        {
            APIResponse response = new();
            try
            {
                var user = _dbContext.Users.Include(u => u.ProfileImage).FirstOrDefault(u => u.UserId == userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                using MemoryStream stream = new();
                await formFile.CopyToAsync(stream);
                if (user.ProfileImage != null)
                {
                    user.ProfileImage.Img = stream.ToArray();
                    response.Message = "Image updated successfully";
                }
                else
                {
                    var image = new Image()
                    {
                        Img = stream.ToArray(),
                        UserId = userId
                    };
                    this._dbContext.Images.Add(image);
                    user.ProfileImage = image;
                    response.Message = "Image uploaded successfully";
                }
                await this._dbContext.SaveChangesAsync();
                response.ResponseCode = 200;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        [HttpGet("GetImageByUserId/{userId}")]
        public async Task<IActionResult> GetImageByUserId(int userId)
        {
            try
            {
                var user = this._dbContext.Users.Include(u => u.ProfileImage).FirstOrDefault(u => u.UserId == userId);
                if (user != null && user.ProfileImage != null)
                {
                    return File(user.ProfileImage.Img, "image/png");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        [HttpDelete("RemoveImage/{userId}")]
        public async Task<IActionResult> RemoveImage(int userId)
        {
            try
            {
                var user = this._dbContext.Users.Include(u => u.ProfileImage).FirstOrDefault(u => u.UserId == userId);
                if (user != null && user.ProfileImage != null)
                {
                    _dbContext.Images.Remove(user.ProfileImage);
                    user.ProfileImage = null;
                    await _dbContext.SaveChangesAsync();
                    return Ok("pass");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

    }
}
