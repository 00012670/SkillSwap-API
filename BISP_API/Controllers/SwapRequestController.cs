﻿using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    public class SwapRequestController : Controller
    {
        private readonly BISPdbContext _dbContext;
        public SwapRequestController(BISPdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateSwapRequest")]
        public async Task<IActionResult> CreateSwapRequest([FromBody] SwapRequest swapRequest)
        {
            var skillExists = await _dbContext.Skills.AnyAsync(x => x.SkillId == swapRequest.SkillRequestedId);
            if (!skillExists)
            {
                return BadRequest("SkillRequestedId does not exist");
            }

            _dbContext.SwapRequests.Add(swapRequest);
            await _dbContext.SaveChangesAsync();

            await _dbContext.Entry(swapRequest)
      .Reference(sr => sr.Initiator).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.Receiver).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.SkillOffered).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.SkillRequested).LoadAsync();

            return Ok(swapRequest);
        }

        [HttpGet("GetSwapRequests")]
        public async Task<IActionResult> GetSwapRequests(int userId)
        {
            var swapRequests = await _dbContext.SwapRequests
                .Where(sr => sr.InitiatorId == userId || sr.ReceiverId == userId)
                .ToListAsync();
            return Ok(swapRequests);
        }

        [HttpPut("UpdateSwapRequest/{requestId}")]
        public async Task<IActionResult> UpdateSwapRequest(int requestId, [FromBody] SwapRequest updatedSwapRequest)
        {
            var swapRequest = await _dbContext.SwapRequests.FindAsync(requestId);
            if (swapRequest == null)
            {
                return NotFound();
            }

            swapRequest.StatusRequest = updatedSwapRequest.StatusRequest;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

    }
}