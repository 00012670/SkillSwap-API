﻿using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    public class ReviewController : Controller
    {
        private readonly BISPdbContext _dbContext;
        public ReviewController(BISPdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] Review review)
        {
            // Check if a review from this user for this skill already exists
            var existingReview = await _dbContext.Reviews
                .FirstOrDefaultAsync(r => r.FromUserId == review.FromUserId && r.SkillId == review.SkillId);
            if (existingReview != null)
            {
                return BadRequest(new { Message = "You've already reviewed this skill "});
            }

            // Fetch the associated SwapRequest
            var swapRequest = await _dbContext.SwapRequests.FindAsync(review.RequestId);

            // Check if the SwapRequest exists
            if (swapRequest == null)
            {
                return NotFound(new { Message = "SwapRequest not found" });
            }

            // Check if the SwapRequest has been accepted
            if (swapRequest.StatusRequest != SwapRequest.Status.Accepted)
            {
                return BadRequest(new { Message = "Cannot create review for a SwapRequest that has not been accepted" });
            }

            // Validate and add review to the database
            _dbContext.Reviews.Add(review);
            await _dbContext.SaveChangesAsync();

            return Ok(review);
        }


        [HttpGet("GetReviewsByUserIdAndSkillId/{userId}/{skillId}")]
        public async Task<IActionResult> GetReviewsByUserIdAndSkillId(int userId, int skillId)
        {
            
            // Fetch reviews written for the user and associated with the skill
            var reviews = await _dbContext.Reviews
                .Where(r => r.ToUserId == userId && r.SkillId == skillId)
                .Select(r => new
                {
                    r.ReviewId,
                    FromUserName = r.FromUser.Username,
                    ToUserName = r.ToUser.Username,
                    r.SkillId,
                    SkillName = r.Skill.Name,
                    r.RequestId,
                    RequestStatus = r.Request.StatusRequest.ToString(),
                    r.Rating,
                    r.Text
                })
                .ToListAsync();

            return Ok(reviews);
        }



        [HttpGet("GetReviewsByUserId/{userId}")]
        public async Task<IActionResult> GetReviewsByUserId(int userId)
        {
            // Fetch reviews written for the user
            var reviews = await _dbContext.Reviews
                .Where(r => r.ToUserId == userId)
                .Select(r => new
                {
                    r.ReviewId,
                    FromUserName = r.FromUser.Username,
                    ToUserName = r.ToUser.Username,
                    r.SkillId,
                    SkillName = r.Skill.Name,
                    r.RequestId,
                    RequestStatus = r.Request.StatusRequest.ToString(),
                    r.Rating,
                    r.Text
                })
                .ToListAsync();

            return Ok(reviews);
        }


        [HttpDelete("DeleteReview/{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            var review = await _dbContext.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return NotFound();
            }

            _dbContext.Reviews.Remove(review);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("DeleteAllReviews")]
        public async Task<IActionResult> DeleteAllReviews()
        {
            var reviews = await _dbContext.Reviews.ToListAsync();

            _dbContext.Reviews.RemoveRange(reviews);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }


        //[HttpPut("UpdateReview/{reviewId}")]
        //public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] Review updatedReview)
        //{
        //    var review = await _dbContext.Reviews.FindAsync(reviewId);
        //    if (review == null)
        //    {
        //        return NotFound();
        //    }

        //    // Get the ID of the currently authenticated user
        //    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    // Check if the current user is the one who created the review
        //    if (review.FromUserId.ToString() != currentUserId)
        //    {
        //        return Forbid();
        //    }

        //    review.Rating = updatedReview.Rating;
        //    review.Text = updatedReview.Text;

        //    await _dbContext.SaveChangesAsync();

        //    return NoContent();
        //}


    }
}

