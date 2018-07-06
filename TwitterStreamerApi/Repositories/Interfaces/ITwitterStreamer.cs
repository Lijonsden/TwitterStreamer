using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories.Interfaces
{
    public interface ITwitterStreamer
    {
        Task<bool> StartStreamer(string twitterId, string clientId, string[] tracks);
        Task<bool> RemoveCLientStream(string clientId);
        Task StopStreamer(string clientId);
        Task ResumeStreamer(string clientId);
        Task PauseStreamer(string clientId);
    }
}
