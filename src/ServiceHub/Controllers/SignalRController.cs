using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceHub.Data;
using ServiceHub.Models;

namespace SignalRHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalRController : ControllerBase
    {        
        private IServiceProvider _serviceProvider { get; set; }


        public SignalRController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
       

        [HttpGet("GetUsers")]
        public IDictionary<string, string> GetUsers()
        {
            var signalHub = _serviceProvider.GetRequiredService<SignalHub>();
            var clients = signalHub.GetConnectedClients();
            return clients;
        }

        [HttpPost("DoAction")]
        public IActionResult DoAction(string clientId, string actionType, string script)
        {
            var result = false;
           var actionTypes = new[] { "script", "message" };
            if (!actionTypes.Contains(actionType))
            {
                return BadRequest($"Possible actions {string.Join(",", actionType)} ");
            }
            var signalHub = _serviceProvider.GetRequiredService<SignalHub>();
            result = signalHub.SendMessageTo(clientId, actionType, script);
            return result ? Ok() : NotFound();
        }

        [HttpPost("LogActivity")]
        public IActionResult LogActivity([FromBody] LogActivityRequest request)
        {
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Activities.Add(new Activity() { Date = DateTime.UtcNow, Action = request.Action, Comment = request.Comment, ClientId = request.ClientId });
            dbContext.SaveChanges();
            return Ok("Activity added");
        }

        [HttpGet("Activities")]
        public async Task<IActionResult> ActivitiesAsync(int pageIndex, int pageSize, string? searchTerm)
        {
            pageSize = pageSize == 0 ? 10 : pageSize;
            var dbContext = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            IQueryable<Activity> query = dbContext.Activities;

            // Apply search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e => e.Action.Contains(searchTerm) ||
                                         e.ClientId.Contains(searchTerm) ||
                                         e.ConnectionId.Contains(searchTerm) ||
                                         e.Comment.Contains(searchTerm));
            }

            query = query.OrderByDescending(x => x.Date);

            var activities = await query.Skip(pageIndex * pageSize)
                              .Take(pageSize)
                              .ToListAsync();


            return Ok(activities);
        }


       
    }
}
