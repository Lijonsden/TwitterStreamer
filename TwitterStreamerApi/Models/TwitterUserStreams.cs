using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Models
{
    public class TwitterUserStreams
    {
        public string TwitterId { get; set; }
        public string ClientId { get; set; }
        public Tweetinvi.Streaming.IFilteredStream FilteredSteam { get; set; }
        public CancellationTokenSource ProcessCancellationTokenSource { get; set; }
    }
}
