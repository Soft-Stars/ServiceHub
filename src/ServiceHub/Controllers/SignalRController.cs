using Microsoft.AspNetCore.Mvc;

namespace SignalRHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalRController : ControllerBase
    {        
        private IServiceProvider ServiceProvider
        { get; set; }


        public SignalRController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
       

        [HttpGet("GetUsers")]
        public IDictionary<string, string> GetUsers()
        {
            var signalHub = ServiceProvider.GetRequiredService<SignalHub>();
            var clients = signalHub.GetConnectedClients();
            return clients;
        }

        [HttpPost("DoAction")]
        public IActionResult DoAction(string clientId, string actionType, string script)
        {
            var result = false;
           var actionTypes = new[] { "message" };
            if (!actionTypes.Contains(actionType))
            {
                return BadRequest($"Possible actions {string.Join(",", actionType)} ");
            }
            var signalHub = ServiceProvider.GetRequiredService<SignalHub>();
            if (actionType == "message")
            {
                result = signalHub.SendMessageTo(clientId, script);
            }
            return result ? Ok() : NotFound();
        }

        [HttpPost("LogActivity")]
        public IActionResult LogActivity(string clientId, string action, string comment)
        {
            return Ok();
        }
    }
}
