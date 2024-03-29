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

            // Check if the skill is connected to any swap requests, including those marked as deleted
            var swapRequests = await _dbContext.SwapRequests
                .Where(sr => (sr.SkillOfferedId == id || sr.SkillRequestedId == id) && (sr.StatusRequest == SwapRequest.Status.Pending || sr.IsDeleted))
                .ToListAsync();

            // If the skill is connected to any swap requests, update their status to "Rejected" and remove the association
            if (swapRequests.Any())
            {
                foreach (var swapRequest in swapRequests)
                {
                    swapRequest.StatusRequest = SwapRequest.Status.Rejected;
                    if (swapRequest.SkillOfferedId == id)
                    {
                        swapRequest.SkillOfferedId = null;
                    }
                    if (swapRequest.SkillRequestedId == id)
                    {
                        swapRequest.SkillRequestedId = null;
                    }
                }
            }

            // Save changes
            await _dbContext.SaveChangesAsync();

            // Now try to delete the skill
            _dbContext.Skills.Remove(skill);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // This is likely due to a foreign key constraint. Log the exception and return a specific error message.
                // Log the exception here
                return BadRequest("This skill cannot be deleted because it is associated with existing swap requests.");
            }

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
