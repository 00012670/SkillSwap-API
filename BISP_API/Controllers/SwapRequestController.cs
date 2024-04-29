using BISP_API.Context;
using BISP_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]     // Setting the route for the controller
    public class SwapRequestController : Controller     // Inheriting from the base Controller class
    {
        // Declaring a private readonly field for the database context
        private readonly BISPdbContext _dbContext;
        // Constructor for the controller, accepting a database context as a parameter
        public SwapRequestController(BISPdbContext dbContext)
        {
            // Assigning the passed in database context to the private field
            _dbContext = dbContext;
        }

        [HttpPost("CreateSwapRequest")]
        public async Task<IActionResult> CreateSwapRequest([FromBody] SwapRequest swapRequest)
        {
            // Checking if the requested skill exists in the database
            var skillExists = await _dbContext.Skills.AnyAsync(x => x.SkillId == swapRequest.SkillRequestedId);
            if (!skillExists) 
            {
                // If the skill does not exist, return a BadRequest response
                return BadRequest("SkillRequestedId does not exist");
            }

            // Add the swap request to the database
            _dbContext.SwapRequests.Add(swapRequest);

            // Create a new notification
            var notification = new Notification
            {
                UserId = swapRequest.ReceiverId,
                SenderId = swapRequest.InitiatorId,
                Content = "You have a new swap request",
                IsRead = false,
                DateCreated = DateTime.UtcNow,
                Type = NotificationType.SwapRequest
            };

            // Add the notification to the database
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();

            // Load the related entities for the swap request
            await _dbContext.Entry(swapRequest)
      .Reference(sr => sr.Initiator).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.Receiver).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.SkillOffered).LoadAsync();
            await _dbContext.Entry(swapRequest)
                .Reference(sr => sr.SkillRequested).LoadAsync();

            // Return the swap request in the response
            return Ok(swapRequest);
        }

        [HttpGet("ReceiveSwapRequests")]
        public async Task<IActionResult> GetSwapRequests(int userId)
        {
            // Querying the database for swap requests for the specified user
            var swapRequests = await _dbContext.SwapRequests
                .Where(sr => sr.ReceiverId == userId && sr.IsDeleted == false)
                .Select(sr => new
                {
                    sr.RequestId,
                    sr.InitiatorId,
                    InitiatorName = sr.Initiator.Username,
                    sr.ReceiverId,
                    ReceiverName = sr.Receiver.Username,
                    sr.SkillOfferedId,
                    SkillOfferedName = sr.SkillOffered.Name,
                    sr.SkillRequestedId,
                    SkillRequestedName = sr.SkillRequested.Name,
                    sr.Details,
                    sr.StatusRequest
                })
                .ToListAsync();

            // Return the swap requests in the response
            return Ok(swapRequests);
        }

        [HttpGet("GetSentSwapRequests")]
        public async Task<IActionResult> GetSentSwapRequests(int userId)
        {
            // Querying the database for swap requests sent by the specified user
            var sentSwapRequests = await _dbContext.SwapRequests
                .Where(sr => sr.InitiatorId == userId && sr.IsDeleted == false)
                .Select(sr => new
                {
                    sr.RequestId,
                    sr.InitiatorId,
                    InitiatorName = sr.Initiator.Username,
                    sr.ReceiverId,
                    ReceiverName = sr.Receiver.Username,
                    sr.SkillOfferedId,
                    SkillOfferedName = sr.SkillOffered.Name,
                    sr.SkillRequestedId,
                    SkillRequestedName = sr.SkillRequested.Name,
                    sr.Details,
                    sr.StatusRequest
                })
                .ToListAsync();

            // Return the sent swap requests in the response
            return Ok(sentSwapRequests);
        }

        [HttpGet("GetAcceptedSwapRequests")]
        public async Task<IActionResult> GetAcceptedSwapRequests(int userId)
        {
            // Querying the database for accepted swap requests for the specified user
            var acceptedSwapRequests = await _dbContext.SwapRequests
           .Where(sr => (sr.InitiatorId == userId || sr.ReceiverId == userId) && sr.StatusRequest == SwapRequest.Status.Accepted)
                .ToListAsync();

            // Return the accepted swap requests in the response
            return Ok(acceptedSwapRequests);
        }

        [HttpGet("GetBySkillId/{skillId}")]
        public async Task<IActionResult> GetBySkillId(int skillId)
        {
            // Querying the database for swap requests with the specified skill id
            var swapRequests = await _dbContext.SwapRequests
                .Where(sr => sr.SkillOfferedId == skillId || sr.SkillRequestedId == skillId)
                .ToListAsync();

            // If no swap requests are found, return an empty list
            if (swapRequests == null || swapRequests.Count == 0)
            {
                return Ok(new List<SwapRequest>());
            }

            // Return the swap requests in the response
            return Ok(swapRequests);
        }

        [HttpPut("UpdateSwapRequest/{requestId}")]
        public async Task<IActionResult> UpdateSwapRequest(int requestId, [FromBody] SwapRequest updatedSwapRequest)
        {
            // Finding the swap request in the database
            var swapRequest = await _dbContext.SwapRequests.FindAsync(requestId);
            // If the swap request is not found, return a NotFound response
            if (swapRequest == null)
            {
                return NotFound();
            }

            // Update the status of the swap request
            swapRequest.StatusRequest = updatedSwapRequest.StatusRequest;

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("DeleteSwapRequest/{requestId}")]
        public async Task<IActionResult> DeleteSwapRequest(int requestId)
        {
            // Finding the swap request in the database
            var swapRequest = await _dbContext.SwapRequests.FindAsync(requestId);
            // If the swap request is not found, return a NotFound response
            if (swapRequest == null)
            {
                return NotFound();
            }

            // Check if the swap request is accepted
            if (swapRequest.StatusRequest == SwapRequest.Status.Accepted)
            {
                // If the swap request is accepted, return a BadRequest response
                return BadRequest(new { Message = "Cannot delete an accepted swap request" });
            }

            // Mark the swap request as deleted
            swapRequest.IsDeleted = true;
            // Save the changes to the database
            await _dbContext.SaveChangesAsync();
            // Return a NoContent response
            return NoContent();
        }

        [HttpDelete("DeleteAllSwapRequests")]
        public async Task<IActionResult> DeleteAllSwapRequests()
        {
            // Getting all swap requests from the database
            var swapRequests = await _dbContext.SwapRequests.ToListAsync();
            // If no swap requests are found, return a NotFound response
            if (swapRequests == null || swapRequests.Count == 0)
            {
                return NotFound();
            }

            // Remove all swap requests from the database
            _dbContext.SwapRequests.RemoveRange(swapRequests);
            await _dbContext.SaveChangesAsync();
            // Return a NoContent response
            return NoContent();
        }

        [HttpGet("GetAllSwapRequests")]
        public async Task<IActionResult> GetAllSwapRequests()
        {
            // Querying the database for all swap requests
            var swapRequests = await _dbContext.SwapRequests
                .Select(sr => new
                {
                    sr.RequestId,
                    sr.InitiatorId,
                    InitiatorName = sr.Initiator.Username,
                    sr.ReceiverId,
                    ReceiverName = sr.Receiver.Username,
                    sr.SkillOfferedId,
                    SkillOfferedName = sr.SkillOffered.Name,
                    sr.SkillRequestedId,
                    SkillRequestedName = sr.SkillRequested.Name,
                    sr.Details,
                    sr.StatusRequest
                })
                .ToListAsync();

            // Return all swap requests in the response
            return Ok(swapRequests);
        }
    }
}