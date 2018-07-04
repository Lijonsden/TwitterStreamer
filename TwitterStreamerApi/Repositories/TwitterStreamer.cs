using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories
{
    public class TwitterStreamer : ITwitterStreamer
    {
        private readonly Options.TwitterApiConfiguration _twitterConfigurations;
        private readonly IUserDataManager _userDataManager;

        public TwitterStreamer(IOptions<Options.ApplicationConfiguration> options, IOptions<Options.TwitterApiConfiguration> twitterConfigurations,
            IUserDataManager userDataManager)
        {
            _twitterConfigurations = twitterConfigurations.Value;
            _userDataManager = userDataManager;
        }

        public async Task<string> StartStreamer(string twitterId, string clientId, string[] tracks)
        {
            var user = await _userDataManager.GetUserData(twitterId);

            if (user == null)
                return null;

            Models.TwitterUserStreams userStream = new Models.TwitterUserStreams()
            {
                TwitterId = twitterId,
                ClientId = clientId,
            };

            var credentials = await _userDataManager.GetUserCredentials(twitterId);
            if(credentials == null || credentials.ValidUntil < DateTime.UtcNow)
            {
                //Create credentials
            }

            userStream.FilteredSteam = Tweetinvi.Stream.CreateFilteredStream(new Tweetinvi.Models.TwitterCredentials()
            {
                AccessToken = credentials.AccessToken,
                AccessTokenSecret = credentials.AccessTokenSecret,
                ConsumerKey = _twitterConfigurations.ConsumerKey,
                ConsumerSecret = _twitterConfigurations.ConsumerSecret
            });

            tracks = new string[] { "Ronaldo" };

            foreach (var track in tracks)
                userStream.FilteredSteam.AddTrack(track);

            userStream.ProcessCancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = userStream.ProcessCancellationTokenSource.Token;


            Task newStream = Task.Run(() =>
            {
                userStream.FilteredSteam.MatchingTweetReceived += (sender, args) =>
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            userStream.FilteredSteam.StopStream();
                            userStream = null;
                        }

                        System.Diagnostics.Debug.WriteLine("New tweet: " + args.Tweet.FullText);
                    }
                    catch (Exception ex)
                    {
                        //log error
                        userStream = null;
                    }
                    finally
                    {
                        if (userStream != null)
                            userStream = null; 
                    }
                };

                userStream.FilteredSteam.StartStreamMatchingAllConditionsAsync();

            }, cancellationToken);

            //Temp
            Task tempTask = Task.Run(() =>
            {
                Task.Delay(5000);
                System.Diagnostics.Debug.WriteLine("Cancelling the operation");
                userStream.ProcessCancellationTokenSource.Cancel();
            });

            return $"this would start streamer for user { user.ScreenName } with credentials { credentials.AccessToken }";
        }
    }
}
