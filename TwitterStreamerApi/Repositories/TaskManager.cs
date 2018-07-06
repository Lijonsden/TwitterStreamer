using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TwitterStreamerApi.Repositories
{
    public class TaskManager
    {
        private static ConcurrentDictionary<string, CancellationTokenSource> verifiedStreams = new ConcurrentDictionary<string, CancellationTokenSource>();

        public async Task<bool> TryAddClientStream(string clientId, CancellationTokenSource tokenSource)
        {
            return verifiedStreams.TryAdd(clientId, tokenSource);
        }

        public async Task<bool> TryUpdateClientStream(string clientId, CancellationTokenSource tokenSource)
        {
            return verifiedStreams.TryUpdate(clientId, tokenSource, null);
        }

        public async Task<bool> RemoveClientStream(string clientId)
        {
            var ct = verifiedStreams.TryRemove(clientId, out var cancellationToken);
            cancellationToken?.Cancel();
            cancellationToken?.Dispose();

            return ct;
        }
    }
}
