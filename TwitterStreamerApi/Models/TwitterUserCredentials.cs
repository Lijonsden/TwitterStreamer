using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Models
{
    public class TwitterUserCredentials
    {
        public string TwitterId { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
        public DateTime ValidUntil { get; set; }

    }
}
