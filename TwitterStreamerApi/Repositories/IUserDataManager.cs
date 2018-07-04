using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories
{
    public interface IUserDataManager
    {
        Task<string> StoreUserData(Models.ZeroDayTwitterUser user);
        Task<Models.ZeroDayTwitterUser> GetUserData(string twitterId);
        Task<Models.TwitterUserCredentials> GetUserCredentials(string twitterId);
        Task CreateOrUpdateCredentials(Models.TwitterUserCredentials newCredentials);
    }
}
