using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.SignalHubs
{
    public class TwitterStreamerHub : Hub
    {
        private readonly Repositories.TaskManager _taskManager;

        public TwitterStreamerHub(Repositories.TaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public override Task OnConnectedAsync()
        {
            var test = Context.UserIdentifier;
            Clients.Client(Context.ConnectionId).SendAsync("Contact", Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public async Task Handshake(string clientId)
        {
            bool verified = false;

            if (clientId == Context.ConnectionId && await _taskManager.TryAddClientStream(clientId, null))
                verified = true; 

            await Clients.Client(Context.ConnectionId).SendAsync("Response", verified, clientId);
        }


        public async Task SendStream(string user, string message)
        {
            var identifier = Context.UserIdentifier;
            await Clients.All.SendAsync("GetStream", user, message); 
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            var success = _taskManager.RemoveClientStream(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
