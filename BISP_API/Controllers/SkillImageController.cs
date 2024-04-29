using BISP_API.Context;
using BISP_API.Helpers;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillImageController : ControllerBase
    {
        private readonly BISPdbContext _dbContext;

        public SkillImageController(BISPdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("UploadSkillImage/{skillId}")]
        public async Task<ActionResult> UploadSkillImage(IFormFile formFile, int skillId)
        {
            APIResponse response = new();
            try
            {
                var skill = _dbContext.Skills.Include(u => u.SkillImage).SingleOrDefault(u => u.SkillId == skillId);
                if (skill == null)
                {
                    return NotFound("Skill not found");
                }

                using MemoryStream stream = new();
                await formFile.CopyToAsync(stream);
                var imageData = stream.ToArray();

                if (skill.SkillImage != null)
                {
                    // Update the existing image
                    skill.SkillImage.Img = imageData;
                    response.Message = "Image updated successfully";
                }
                else
                {
                    // Create a new image
                    var image = new SkillImage()
                    {
                        Img = imageData,
                        SkillId = skillId
                    };
                    _dbContext.SkillImages.Add(image);
                    skill.SkillImage = image;
                    response.Message = "Image uploaded successfully";
                }
                skill.HasImage = true;
                await _dbContext.SaveChangesAsync();
                response.ResponseCode = 200;
                response.Result = "pass";
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return Ok(response);
        }


        [HttpGet("GetSkillImageBySkillId/{skillId}")]
        public async Task<IActionResult> GetSkillImageBySkillId(int skillId)
        {
            try
            {
                var skill = await _dbContext.Skills.Include(u => u.SkillImage).SingleOrDefaultAsync(u => u.SkillId == skillId);
                if (skill == null || skill.SkillImage == null)
                {
                    return NotFound();
                }

                return File(skill.SkillImage.Img, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpGet("GetSkillImage/{id}")]
        public async Task<IActionResult> GetSkillImage(int id)
        {
            try
            {
                var image = await _dbContext.Images.FindAsync(id);
                if (image == null)
                {
                    return NotFound();
                }

                return File(image.Img, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }

        [HttpDelete("RemoveSkillImage/{skillId}")]
        public async Task<IActionResult> RemoveSkillImage(int skillId)
        {
            try
            {
                var skill = await _dbContext.Skills.Include(u => u.SkillImage).SingleOrDefaultAsync(u => u.SkillId == skillId);
                if (skill == null || skill.SkillImage == null)
                {
                    return NotFound();
                }

                _dbContext.SkillImages.Remove(skill.SkillImage);
                skill.SkillImage = null;
                skill.HasImage = false;
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }


    }
}