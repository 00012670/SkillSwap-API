﻿using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : Controller
    {
        private readonly BISPdbContext _dbContext;

        public SkillsController(BISPdbContext dbContext)
        {
            _dbContext = dbContext;

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
        [Route("UpdateSkillBy/{id}")]
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
        [Route("DeleteSkillBy/{id}")]
        public async Task<IActionResult> DeleteSkill([FromRoute] int id)
        {
            var skill = await _dbContext.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            // Check if the skill is associated with any accepted swap requests
            var acceptedSwapRequests = await _dbContext.SwapRequests
                .Where(sr => (sr.SkillOfferedId == id || sr.SkillRequestedId == id) && sr.StatusRequest == SwapRequest.Status.Accepted)
                .ToListAsync();

            if (acceptedSwapRequests.Count > 0)
            {
                return BadRequest(new { Message= "This skill cannot be deleted because it is associated with accepted swap requests." });
            }

            // Get the swap requests associated with the skill
            var swapRequests = await _dbContext.SwapRequests
                .Where(sr => sr.SkillOfferedId == id || sr.SkillRequestedId == id)
                .ToListAsync();

            // Remove the associated swap requests
            _dbContext.SwapRequests.RemoveRange(swapRequests);

            _dbContext.Skills.Remove(skill);
            await _dbContext.SaveChangesAsync();

            return Ok(skill);
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



        [HttpGet]
        [Route("GetSkillAndUserBy/{id}")]
        public async Task<IActionResult> GetSkillAndUserById([FromRoute] int id)
        {
            var skill = await _dbContext.Skills.Include(s => s.User).FirstOrDefaultAsync(s => s.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skill);
        }


    }
}
