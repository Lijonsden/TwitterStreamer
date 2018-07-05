using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories.Interfaces
{
    public interface ITwitterStreamer
    {
        Task<bool> StartStreamer(string twitterId, string clientId, string[] tracks); 
    }
}
