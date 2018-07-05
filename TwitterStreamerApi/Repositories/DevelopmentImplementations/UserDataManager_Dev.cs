using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamerApi.Models;
using TwitterStreamerApi.Repositories.Interfaces;

namespace TwitterStreamerApi.Repositories.Development
{
    public class UserDataManager_Dev : IUserDataManager
    {
        List<Models.ZeroDayTwitterUser> Users = new List<Models.ZeroDayTwitterUser>();
        List<Models.TwitterUserCredentials> Credentials = new List<TwitterUserCredentials>();
        
        public UserDataManager_Dev()
        {
        }

        public async Task CreateOrUpdateCredentials(TwitterUserCredentials newCredentials)
        {
            var oldCredentials = Credentials.SingleOrDefault(o => o.TwitterId == newCredentials.TwitterId);

            if (oldCredentials == null)
                Credentials.Add(newCredentials);
            else
                oldCredentials = newCredentials;
        }

        public async Task<TwitterUserCredentials> GetUserCredentials(string twitterId)
        {
            return Credentials.SingleOrDefault(o => o.TwitterId == twitterId); 
        }

        public async Task<Models.ZeroDayTwitterUser> GetUserData(string twitterId)
        {
            var user = Users.SingleOrDefault(o => o.TwitterId == twitterId);

            return user;
        }

        public async Task<string> StoreUserData(Models.ZeroDayTwitterUser user)
        {
            var storedUser = Users.Find(o => o.TwitterId == user.TwitterId);

            if (storedUser != null)
                Users.RemoveAll(o => o.TwitterId == user.TwitterId);

            Users.Add(user);

            return user.TwitterId;
        }
    }
}
