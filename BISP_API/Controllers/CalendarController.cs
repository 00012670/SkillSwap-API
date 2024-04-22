using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BISP_API.Context;
using BISP_API.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace BISP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {


        static string[] Scopes = { CalendarService.Scope.Calendar };
        static string ApplicationName = "Google Calendar API .NET Quickstart";



        [HttpPost("createevent")]
        public void CreateEvent()
        {
            UserCredential credential;

            using (var stream = new FileStream("client_secret_1079640577230-2rsqh8j2bs6c43vamc1eetmtvkp5i240.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None).Result;
            }

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var @event = new Event
            {
                Summary = "Google I/O 2015",
                Location = "800 Howard St., San Francisco, CA 94103",
                Description = "A chance to hear more about Google's developer products.",
                Start = new EventDateTime()
                {
                    DateTimeRaw = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    TimeZone = "America/Los_Angeles",
                },
                End = new EventDateTime()
                {
                    DateTimeRaw = DateTime.Now.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    TimeZone = "America/Los_Angeles",
                },
                ConferenceData = new ConferenceData
                {
                    CreateRequest = new CreateConferenceRequest
                    {
                        RequestId = "sample123", // Any unique ID
                        ConferenceSolutionKey = new ConferenceSolutionKey
                        {
                            Type = "hangoutsMeet"
                        }
                    }
                },
                Attendees = new EventAttendee[] {
                new EventAttendee() { Email = "attendee1@example.com" },
                new EventAttendee() { Email = "attendee2@example.com" },
            },
                Reminders = new Event.RemindersData
                {
                    UseDefault = false,
                    Overrides = new EventReminder[] {
                    new EventReminder() { Method = "email", Minutes = 24 * 60 },
                    new EventReminder() { Method = "popup", Minutes = 10 },
                }
                }
            };

            EventsResource.InsertRequest request = service.Events.Insert(@event, "primary");
            request.ConferenceDataVersion = 1;
            Event createdEvent = request.Execute();
            Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
        }



        //private readonly BISPdbContext _context;

        //public CalendarController(BISPdbContext context)
        //{
        //    _context = context;
        //}

        //[HttpGet("GetAllEvents")]
        //public async Task<ActionResult<IEnumerable<Calendar>>> GetCalendars()
        //{
        //    return await _context.Calendars.ToListAsync();
        //}


        //[HttpGet("GetEvent{id}")]
        //public async Task<ActionResult<Calendar>> GetCalendar(int id)
        //{
        //    var calendar = await _context.Calendars.FindAsync(id);

        //    if (calendar == null)
        //    {
        //        return NotFound();
        //    }

        //    return calendar;
        //}


        //[HttpPut("UpdateEvent{id}")]
        //public async Task<IActionResult> UpdateEvent(int id, Calendar calendar)
        //{
        //    if (id != calendar.EventId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(calendar).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CalendarExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //[HttpPost("CreateEvent")]
        //public async Task<ActionResult<Calendar>> CreateEvent(Calendar calendar)
        //{
        //    _context.Calendars.Add(calendar);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetCalendar", new { id = calendar.EventId }, calendar);
        //}


        //[HttpDelete("DeleteEvent{id}")]
        //public async Task<IActionResult> DeleteEvent(int id)
        //{
        //    var calendar = await _context.Calendars.FindAsync(id);
        //    if (calendar == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Calendars.Remove(calendar);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool CalendarExists(int id)
        //{
        //    return _context.Calendars.Any(e => e.EventId == id);
        //}
    }
}
