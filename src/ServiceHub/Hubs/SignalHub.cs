
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SignalRHub
{
    public sealed class SignalHub : Hub
    {

        ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

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
            var clientId = Context.ConnectionId;
            var httpCtx = Context.GetHttpContext();
            string clientCustomId = httpCtx.Request.Headers["CI"].ToString();
            string dateTime = httpCtx.Request.Headers["T"].ToString();
            string accessToken = httpCtx.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            HMACSHA256 hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes("ounmadhr"));
            string cipherText = System.Convert.ToBase64String(hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(clientId + "." + dateTime + ".XpErTsOfT")));

            if ((clientId == "" || clientId == null) && (dateTime == "" || dateTime == null))
            {
                throw new HubException("401: Missing Argument");
            }
            
            _connections.TryAdd(clientId, clientCustomId);

            await base.OnConnectedAsync();
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
