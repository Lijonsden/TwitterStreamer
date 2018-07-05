using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterStreamerApi.Repositories.Interfaces;

namespace TwitterStreamerApi.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IUserDataManager _userDataManager;

        public AccountsController(IUserDataManager userDataManager)
        {
            _userDataManager = userDataManager;
        }

        [HttpPost]
        [Route("users")]
        public IActionResult CreateUser([FromBody] string user)
        {
            var model = JsonConvert.DeserializeObject<Models.ZeroDayTwitterUser>(user.ToString());

            var userId = _userDataManager.StoreUserData(new Models.ZeroDayTwitterUser()
            {
                Name = model.Name,
                ScreenName = model.ScreenName,
                TwitterId = model.TwitterId,
            });

            return Ok(); 
        }

        [HttpPost]
        [Route("credentials")]
        public IActionResult CreateCredentials([FromBody] string credentials)
        {
            var model = JsonConvert.DeserializeObject<Models.TwitterUserCredentials>(credentials.ToString());
            _userDataManager.CreateOrUpdateCredentials(model);

            return Ok();
        }

        [HttpPatch]
        [Route("credentials")]
        public IActionResult UpdateCredentials(string twitterId, string credentials)
        {
            var model = JsonConvert.DeserializeObject<Models.TwitterUserCredentials>(credentials.ToString());
            _userDataManager.CreateOrUpdateCredentials(model);

            return Ok();

        }

        [HttpDelete]
        [Route("credentials")]
        public IActionResult DeleteCredentials(string twitterId)
        {
            //Delete credentials
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("users/{id}")]
        public IActionResult GetUser(string id)
        {
            var user = _userDataManager.GetUserData(id);

            if (user == null)
                return NotFound();

            return new OkObjectResult(user);
        }
    }
}
