using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamerApi.Repositories.Interfaces;

namespace TwitterStreamerApi.Controllers
{
    [Route("api/twitter/[controller]/[action]")]
    public class StreamerController : Controller
    {
        private readonly IUserDataManager _userDataManager;
        private readonly ITwitterStreamer _twitterStreamer;

        public StreamerController(IUserDataManager userDataManager, ITwitterStreamer twitterStreamer)
        {
            _userDataManager = userDataManager;
            _twitterStreamer = twitterStreamer;
        }

        [HttpGet]
        public async Task<IActionResult> StartStreamer(string twitterId, string clientId, string tracks)
        {
            var result = await _twitterStreamer.StartStreamer(twitterId, clientId, tracks.Split(','));

            return new OkObjectResult(new { message = result }); 
        }

        [HttpGet]
        public async Task<IActionResult> StopStreamer(string clientId)
        {
            _twitterStreamer.StopStreamer(clientId);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> PauseStreamer(string clientId)
        {
            _twitterStreamer.PauseStreamer(clientId);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> ResumeStreamer(string clientId)
        {
            _twitterStreamer.ResumeStreamer(clientId);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveStreamer(string clientId)
        {
            var result = await _twitterStreamer.RemoveCLientStream(clientId);

            return new OkObjectResult(new { message = result });
        }
    }
}
