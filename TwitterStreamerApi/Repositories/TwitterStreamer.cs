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

            if (user == null)
                return false;

            var credentials = await _userDataManager.GetUserCredentials(twitterId);
            if (credentials == null/* || credentials.ValidUntil < DateTime.UtcNow*/)
            {
            }

            //TEMP
            if (tracks == null)
                tracks = new string[] { "Trump" };

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
                }),
                ProcessCancellationTokenSource = new CancellationTokenSource(),
            };

            CancellationToken cancellationToken = userStream.ProcessCancellationTokenSource.Token;

            if (!(await _taskManager.TryAddTask(clientId, userStream.ProcessCancellationTokenSource) == true))
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

            //Temp
            var temp = Task.Run(() =>
            {
                Thread.Sleep(2000);
                System.Diagnostics.Debug.WriteLine("Cancelling the operation");
                var result = _taskManager.CancelTask(clientId);
            });

            return true;
        }
    }
}
