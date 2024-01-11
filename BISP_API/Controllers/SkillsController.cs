using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

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

        [HttpGet]

        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _dbContext.Profiles.ToListAsync();
            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] Skill skillRequest)
        {
            _dbContext.Profiles.Add(skillRequest);
            await _dbContext.SaveChangesAsync();

            return Ok(skillRequest);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSkillbyId([FromRoute] int id)
        {
            var skill = await _dbContext.Profiles.FirstOrDefaultAsync(x => x.SkillId == id);

            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skill);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateSkill([FromRoute] int id, Skill updateSkillRequest)
        {
            var skill = await _dbContext.Profiles.FindAsync(id);

            if (skill == null)
            {
                return NotFound();
            }

            skill.Name = updateSkillRequest.Name;
            skill.Description = updateSkillRequest.Description;
            skill.Category = updateSkillRequest.Category;
            skill.Level = updateSkillRequest.Level;
            skill.Prerequisity = updateSkillRequest.Prerequisity;
            skill.Picture = updateSkillRequest.Picture;

            await _dbContext.SaveChangesAsync();

            return Ok(skill);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSkill([FromRoute] int id)
        {
            var skill = await _dbContext.Profiles.FindAsync(id);

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
