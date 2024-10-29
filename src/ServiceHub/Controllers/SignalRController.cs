using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

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
        
    }
}
