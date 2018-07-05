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

        public async Task SendStream(string user, string message)
        {
            var identifier = Context.UserIdentifier;

            await Clients.All.SendAsync("GetStream", user, message); 
        }

        public override Task OnConnectedAsync()
        {
            var test = Context.UserIdentifier;
            string userIdentifier = "test";

            Clients.Client(Context.ConnectionId).SendAsync("Handshake", Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var success = _taskManager.CancelTask(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
