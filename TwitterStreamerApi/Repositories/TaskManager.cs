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
        private static ConcurrentDictionary<string, CancellationTokenSource> tasks = new ConcurrentDictionary<string, CancellationTokenSource>();

        public async Task<bool> TryAddTask(string clientId, CancellationTokenSource tokenSource)
        {
            return tasks.TryAdd(clientId, tokenSource);
        }

        public async Task<bool> CancelTask(string clientId)
        {
            var ct = tasks.TryRemove(clientId, out var cancellationToken);
            cancellationToken?.Cancel();
            cancellationToken?.Dispose();

            return ct;
        }
    }
}
