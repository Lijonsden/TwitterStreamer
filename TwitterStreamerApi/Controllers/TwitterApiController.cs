using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TwitterApiController : Controller
    {
        private readonly Repositories.IUserDataManager _userDataManager;
        private readonly Repositories.ITwitterStreamer _twitterStreamer;

        public TwitterApiController(Repositories.IUserDataManager userDataManager, Repositories.ITwitterStreamer twitterStreamer)
        {
            _userDataManager = userDataManager;
            _twitterStreamer = twitterStreamer;
        }

        [HttpGet]
        public async Task<IActionResult> StartStreamer(string twitterId)
        {
            var result = await _twitterStreamer.StartStreamer(twitterId, null, null);

            return new OkObjectResult(new { message = result }); 
        }
    }
}
