
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace SignalRHub
{
    public sealed class SignalHub : Hub
    {
        private readonly ILogger<SignalHub> _logger;

        ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        public SignalHub(ILogger<SignalHub> logger)
        {
            _logger = logger;
        }


        public ConcurrentDictionary<string, string> GetConnectedClients()
        {
            return _connections;
        }

        public string GetConnectedClientName(string connectionId)
        {
            return _connections.FirstOrDefault(x => x.Key == connectionId).Value;
        }

        public KeyValuePair<string, string> GetConnectedClientByConnectionId(string connectionId)
        {
            return _connections.FirstOrDefault(x => x.Key == connectionId);
        }

        public KeyValuePair<string, string>? GetConnectedClientByClientId(string clientId)
        {
            return _connections.FirstOrDefault(x => x.Value == clientId);
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var httpCtx = Context.GetHttpContext();
            string clientId = httpCtx.Request.Headers["CI"].ToString();
            string dateTime = httpCtx.Request.Headers["T"].ToString();
            string accessToken = httpCtx.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            HMACSHA256 hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes("ounmadhr"));
            string cipherText = System.Convert.ToBase64String(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(clientId + "." + dateTime + ".XpErTsOfT")));

            if ((clientId == "" || clientId == null) && (dateTime == "" || dateTime == null))
            {
                throw new HubException("401: Missing Argument");
            }

            var client = GetConnectedClientByClientId(connectionId);
            if (client == null || !client.HasValue || string.IsNullOrEmpty(client.Value.Key))
            {
                _connections.TryAdd(connectionId, clientId);
                await base.OnConnectedAsync();
            }
            else
            {
                _logger.LogError($"{clientId} already connected");
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            var client = GetConnectedClientByConnectionId(connectionId);
            _connections.TryRemove(client);
            return base.OnDisconnectedAsync(exception);
        }

        public bool SendMessageTo(string clientId, string message)
        {
            var client = GetConnectedClientByClientId(clientId);
            if (client != null && client.HasValue && !string.IsNullOrEmpty(client.Value.Key))
            {
                Clients.Client(client.Value.Key).SendAsync("Execute", "message", message);
                return true;
            }

            return false;
            
        }
    }
}
