using BISP_API.Context;
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

        public SkillsController(BISPdbContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _dbContext.Skills.ToListAsync();
            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> AddSkill([FromBody] Skill skillRequest)
        {
            _dbContext.Skills.Add(skillRequest);
            skillRequest.Level = (SkillLevel)Enum.Parse(typeof(SkillLevel), skillRequest.Level.ToString(), true);

            await _dbContext.SaveChangesAsync();

            return Ok(skillRequest);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSkillbyId([FromRoute] int id)
        {
            var skill = await _dbContext.Skills.FirstOrDefaultAsync(x => x.Id == id);

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
