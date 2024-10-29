﻿
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
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendCommand(string command, string crmID = null, string connectionId = null, string groupName = null)
        {
            throw new NotImplementedException();
        }


        public async Task SendSqlCommand(string command, string crmID = null, string connectionId = null, string groupName = null)
        {
            throw new NotImplementedException();
        }
    }
}
