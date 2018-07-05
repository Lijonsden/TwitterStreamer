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

        public TwitterStreamer(IOptions<Options.ApplicationConfiguration> options, IOptions<Options.TwitterApiConfiguration> twitterConfigurations,
            IUserDataManager userDataManager, IHubContext<SignalHubs.TwitterStreamerHub> signalRcontext)
        {
            _twitterConfigurations = twitterConfigurations.Value;
            _userDataManager = userDataManager;
            _signalRcontext = signalRcontext;
        }

        public async Task<string> StartStreamer(string twitterId, string clientId, string[] tracks)
        {
            var user = await _userDataManager.GetUserData(twitterId);

            if (user == null)
                return null;

            var credentials = await _userDataManager.GetUserCredentials(twitterId);
            if (credentials == null/* || credentials.ValidUntil < DateTime.UtcNow*/)
            {
                //No credentials
            }

            //TEMP
            if (tracks == null)
                tracks = new string[] { "Trump" };

            Models.TwitterUserStreams userStream = userStreams?.SingleOrDefault(o => o.TwitterId == twitterId);

            if (userStream == null)
            {
                userStream = new Models.TwitterUserStreams()
                {
                    TwitterId = twitterId,
                    ClientId = clientId,
                };
            }

            userStream.FilteredSteam = Tweetinvi.Stream.CreateFilteredStream(new Tweetinvi.Models.TwitterCredentials()
            {
                AccessToken = credentials.AccessToken,
                AccessTokenSecret = credentials.AccessTokenSecret,
                ConsumerKey = _twitterConfigurations.ConsumerKey,
                ConsumerSecret = _twitterConfigurations.ConsumerSecret
            });

            foreach (var track in tracks)
                userStream.FilteredSteam.AddTrack(track);

            userStream.ProcessCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = userStream.ProcessCancellationTokenSource.Token;

            var newTask = Task.Run(() =>
            {
                userStream.FilteredSteam.MatchingTweetReceived += (sender, args) =>
                {

                    if (cancellationToken.IsCancellationRequested)
                    {
                        userStream?.FilteredSteam.StopStream();
                        userStream = null;
                    }

                    System.Diagnostics.Debug.WriteLine("New tweet: " + args.Tweet.FullText);

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
                userStreams[0].ProcessCancellationTokenSource.Cancel();
            });

            return $"this would start streamer for user { user.ScreenName } with credentials { credentials.AccessToken }";
        }
    }
}
