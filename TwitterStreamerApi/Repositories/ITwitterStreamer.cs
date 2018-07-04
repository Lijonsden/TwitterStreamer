using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories
{
    public interface ITwitterStreamer
    {
        Task<string> StartStreamer(string twitterId, string clientId, string[] tracks); 
    }
}
