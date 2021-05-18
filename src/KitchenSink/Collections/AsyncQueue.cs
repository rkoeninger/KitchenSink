using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KitchenSink.Collections
{
    public class AsyncQueue<A>
    {
        private readonly ConcurrentQueue<A> queue = new ConcurrentQueue<A>();
        private readonly ConcurrentQueue<TaskCompletionSource<A>> backlog =
            new ConcurrentQueue<TaskCompletionSource<A>>();

        public void Enqueue(A item)
        {
            lock (this)
            {
                if (backlog.TryDequeue(out var continuation))
                {
                    continuation.SetResult(item);
                }
                else
                {
                    queue.Enqueue(item);
                }
            }
        }

        public Task<A> DequeueAsync()
        {
            lock (this)
            {
                if (queue.TryDequeue(out var item))
                {
                    return Task.FromResult(item);
                }
                else
                {
                    var continuation = new TaskCompletionSource<A>();
                    backlog.Enqueue(continuation);
                    return continuation.Task;
                }
            }
        }

        public int Count => queue.Count - backlog.Count;
    }
}
