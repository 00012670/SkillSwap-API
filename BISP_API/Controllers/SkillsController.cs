using BISP_API.Context;
using BISP_API.Helper;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;

namespace BISP_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : Controller
    {
        private readonly BISPdbContext _dbContext;

        public SkillsController(BISPdbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;

        }

        [HttpGet]
        [Route("GetAllSkills")]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _dbContext.Skills.Include(s => s.User).ToListAsync();
            return Ok(skills);
        }

        [HttpGet]
        [Route("GetSkillsByUserId/{userId}")]
        public async Task<IActionResult> GetSkillsByUserId([FromRoute] int userId)
        {
            var user = await _dbContext.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Skills);
        }


        [HttpPost]
        [Route("AddSkillToUser/{userId}")]
        public async Task<IActionResult> AddSkillToUser([FromRoute] int userId, [FromBody] Skill addSkillRequest)
        {
            var user = await _dbContext.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            addSkillRequest.User = user;
            _dbContext.Skills.Add(addSkillRequest);
            await _dbContext.SaveChangesAsync();

            return Ok(addSkillRequest);
        }

        [HttpDelete]
        [Route("RemoveSkillFromUser/{userId}/{skillId}")]
        public async Task<IActionResult> RemoveSkillFromUser([FromRoute] int userId, [FromRoute] int skillId)
        {
            var user = await _dbContext.Users.Include(u => u.Skills).FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var skill = user.Skills.FirstOrDefault(s => s.SkillId == skillId);

            if (skill == null)
            {
                return NotFound();
            }

            user.Skills.Remove(skill);
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }


        [HttpGet]
        [Route("GetSkillBy/{id}")]
        public async Task<IActionResult> GetSkillbyId([FromRoute] int id)
        {
            var skill = await _dbContext.Skills.FirstOrDefaultAsync(x => x.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skill);
        }

        [HttpPut]
        [Route("UpdateSkillBy{id}")]
        public async Task<IActionResult> UpdateSkill([FromRoute] int id, Skill updateSkillRequest)
        {
            var skill = await _dbContext.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            skill.Name = updateSkillRequest.Name;
            skill.Description = updateSkillRequest.Description;
            skill.Category = updateSkillRequest.Category;
            skill.Level = updateSkillRequest.Level;
            skill.Prerequisity = updateSkillRequest.Prerequisity;
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSkill([FromRoute] int id)
        {
            var skill = await _dbContext.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            _dbContext.Remove(skill);
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }

    }
}
