using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamerApi.Repositories.Interfaces;

namespace TwitterStreamerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TwitterApiController : Controller
    {
        private readonly IUserDataManager _userDataManager;
        private readonly ITwitterStreamer _twitterStreamer;

        public TwitterApiController(IUserDataManager userDataManager, ITwitterStreamer twitterStreamer)
        {
            _userDataManager = userDataManager;
            _twitterStreamer = twitterStreamer;
        }

        [HttpGet]
        public async Task<IActionResult> StartStreamer(string twitterId, string clientId)
        {
            var result = await _twitterStreamer.StartStreamer(twitterId, clientId, null);

            return new OkObjectResult(new { message = result }); 
        }
    }
}
