using BISP_API.Context;
using BISP_API.Models;
using BISP_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly BISPdbContext _context;

        private readonly ILogger<MessageController> _logger;

        public MessageController(BISPdbContext context, ILogger<MessageController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("ReceiveMessage/{senderId}/{receiverId}")]
        public IActionResult ReceiveMessages(int senderId, int receiverId)
        {
            var messages = _context.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId))
                .Select(m => new MessageReadDto
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    SenderUsername = m.Sender.Username,
                    ImageId = m.Sender.ProfileImage.ImgId, 
                    MessageText = m.MessageText,
                    Timestamp = m.Timestamp
                })
                .ToList();

            return Ok(messages);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto messageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sender = await _context.Users.FindAsync(messageDto.SenderId);
            if (sender == null)
            {
                return NotFound();
            }

            var message = new Message
            {
                SenderId = messageDto.SenderId,
                ImageId = sender.ProfileImage?.ImgId, 
                ReceiverId = messageDto.ReceiverId,
                MessageText = messageDto.MessageText,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return StatusCode(500, "Internal server error");
            }

            return Ok(message);
        }



        [HttpPut("UpdateMessage/{id}")]
        public async Task<IActionResult> UpdateMessage(int id, [FromBody] MessageDto messageDto)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId) || userId != message.SenderId)
            {
                return Forbid();
            }

            // Only allow updating the message text
            message.MessageText = messageDto.MessageText;

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("DeleteMessage/{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}



