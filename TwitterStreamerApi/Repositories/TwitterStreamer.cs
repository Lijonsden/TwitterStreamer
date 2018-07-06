using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TwitterStreamerApi.Repositories.Interfaces;

namespace TwitterStreamerApi.Repositories
{
    public class TwitterStreamer : ITwitterStreamer
    {
        private readonly Options.TwitterApiConfiguration _twitterConfigurations;
        private readonly IUserDataManager _userDataManager;
        private readonly IHubContext<SignalHubs.TwitterStreamerHub> _signalRcontext;
        private static List<Models.TwitterUserStreams> userStreams = new List<Models.TwitterUserStreams>();
        private readonly TaskManager _taskManager;

        public TwitterStreamer(IOptions<Options.ApplicationConfiguration> options,
            IOptions<Options.TwitterApiConfiguration> twitterConfigurations,
            IUserDataManager userDataManager, IHubContext<SignalHubs.TwitterStreamerHub> signalRcontext,
            TaskManager taskManager)
        {
            _twitterConfigurations = twitterConfigurations.Value;
            _userDataManager = userDataManager;
            _signalRcontext = signalRcontext;
            _taskManager = taskManager;
        }

        public async Task<bool> StartStreamer(string twitterId, string clientId, string[] tracks)
        {
            var user = await _userDataManager.GetUserData(twitterId);

            if (user == null || tracks == null)
                return false;

            var credentials = await _userDataManager.GetUserCredentials(twitterId);
            if (credentials == null || credentials.ValidUntil < DateTime.UtcNow)
            {
                //reroute for authentication 
            }

            var userStream = new Models.TwitterUserStreams()
            {
                TwitterId = twitterId,
                ClientId = clientId,
                FilteredSteam = Tweetinvi.Stream.CreateFilteredStream(new Tweetinvi.Models.TwitterCredentials()
                {
                    AccessToken = credentials.AccessToken,
                    AccessTokenSecret = credentials.AccessTokenSecret,
                    ConsumerKey = _twitterConfigurations.ConsumerKey,
                    ConsumerSecret = _twitterConfigurations.ConsumerSecret,
                })
            };

            var processCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = processCancellationTokenSource.Token;

            if (!(await _taskManager.TryUpdateClientStream(clientId, processCancellationTokenSource) == true))
                return false;

            foreach (var track in tracks)
                userStream.FilteredSteam.AddTrack(track);

            var newTask = Task.Run(() =>
            {
                userStream.FilteredSteam.MatchingTweetReceived += (sender, args) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        userStream?.FilteredSteam?.StopStream();
                        userStream = null;
                    }

                    _signalRcontext.Clients.Client(clientId).SendAsync("GetStream", "Linus", args.Tweet.FullText);
                };

                userStream.FilteredSteam.StartStreamMatchingAllConditionsAsync();

            }, cancellationToken);

            userStreams.Add(userStream);

            return true;
        }

        public async Task<bool> RemoveCLientStream(string clientId)
        {
            System.Diagnostics.Debug.WriteLine("Removing the client stream");
            return await _taskManager.RemoveClientStream(clientId);
        }

        public async Task StopStreamer(string clientId)
        {
            System.Diagnostics.Debug.WriteLine("Stopping stream operation");

            var stream = userStreams.SingleOrDefault(o => o.ClientId == clientId);

            stream?.FilteredSteam?.StopStream();
        }

        public async Task PauseStreamer(string clientId)
        {
            System.Diagnostics.Debug.WriteLine("Pausing stream operation");

            var stream = userStreams.SingleOrDefault(o => o.ClientId == clientId);

            stream?.FilteredSteam?.PauseStream();
        }

        public async Task ResumeStreamer(string clientId)
        {
            System.Diagnostics.Debug.WriteLine("Resuming stream operation");

            var stream = userStreams.SingleOrDefault(o => o.ClientId == clientId);

            stream?.FilteredSteam?.ResumeStream();
        }
    }
}
