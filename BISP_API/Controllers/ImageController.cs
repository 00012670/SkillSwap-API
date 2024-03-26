using BISP_API.Context;
using BISP_API.Helpers;
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
        public ImageController(BISPdbContext dbContext)
        {
            _dbContext = dbContext;
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
                    user.HasImage = true;
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
                    user.HasImage = true;
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

        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _dbContext.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            return File(image.Img, "image/jpeg");
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
                    user.HasImage = false;
                    await _dbContext.SaveChangesAsync();
                    return NoContent();
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
