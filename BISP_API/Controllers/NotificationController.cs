using BISP_API.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]

    public class NotificationController : Controller
    {
        private readonly BISPdbContext _context;
        public NotificationController(BISPdbContext context)
        {
            _context = context;
        }

        [HttpGet("GetNotifications/{userId}")]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            var notifications = await _context.Notifications
                 .Include(n => n.Message)
                 .ThenInclude(m => m.Sender)
                 .Include(n => n.SwapRequest)
                 .ThenInclude(sr => sr.Initiator)
                 .Where(n => n.UserId == userId)
                 .OrderByDescending(n => n.DateCreated)
                 .ToListAsync();

            var result = notifications.Select(n => new
            {
                n.NotificationId,
                n.UserId,
                n.SenderId,
                n.Content,
                n.IsRead,
                n.DateCreated,
                n.Type
            });

            return Ok(result);
        }



        [HttpDelete("DeleteAllNotifications/{userId}")]
        public async Task<IActionResult> DeleteAllNotifications(int userId)
        {
            var userNotifications = _context.Notifications.Where(n => n.UserId == userId);
            if (!userNotifications.Any())
            {
                return NotFound();
            }

            _context.Notifications.RemoveRange(userNotifications);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        //[HttpPut("ReadNotification/{notificationId}")]
        //public async Task<IActionResult> ReadNotification(int notificationId)
        //{
        //    var notification = await _context.Notifications.FindAsync(notificationId);
        //    if (notification == null)
        //    {
        //        return NotFound();
        //    }

        //    notification.IsRead = true;
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //[HttpPut("MarkAllAsRead/{userId}")]
        //public async Task<IActionResult> MarkAllAsRead(int userId)
        //{
        //    var unreadNotifications = _context.Notifications.Where(n => n.UserId == userId && !n.IsRead);
        //    if (!unreadNotifications.Any())
        //    {
        //        return NotFound();
        //    }

        //    foreach (var notification in unreadNotifications)
        //    {
        //        notification.IsRead = true;
        //    }

        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}



    }
}
