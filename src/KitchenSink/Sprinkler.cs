using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// Creation methods for <see cref="Sprinkler{A}"/>'s.
    /// </summary>
    public static class Sprinkler
    {
        /// <summary>
        /// Creates a sprinkler which emits items continuously.
        /// </summary>
        public static Sprinkler<A> Of<A>(Action<A> handler) =>
            new Sprinkler<A>(long.MaxValue, TimeSpan.Zero, handler);

        /// <summary>
        /// Creates a sprinkler which emits items with given delay between them.
        /// </summary>
        public static Sprinkler<A> Of<A>(TimeSpan delay, Action<A> handler) =>
            new Sprinkler<A>(long.MaxValue, delay, handler);

        /// <summary>
        /// Creates a sprinkler which emits items with given delay between them
        /// or emits all items continuously once queued item limit is reached.
        /// </summary>
        public static Sprinkler<A> Of<A>(long limit, TimeSpan delay, Action<A> handler) =>
            new Sprinkler<A>(limit, delay, handler);
    }

    /// <summary>
    /// Multi-threaded sprinkler that batches items over time and writes batches
    /// to an arbitrary handler.
    /// </summary>
    /// <example>
    /// using var s = Sprinker.Of(TimeSpan.FromSeconds(1), line => Console.WriteLine(line));
    /// s.Push(File.ReadAllText("huge.txt"));
    /// // some time later...
    /// // sprinkler will dispose and flush remaining lines at end of scope
    /// </example>
    public class Sprinkler<A> : IDisposable
    {
        private readonly long limit;
        private readonly Action<A> handler;
        private readonly ConcurrentQueue<A> queue = new ConcurrentQueue<A>();
        private readonly IDisposable worker;
        private bool disposing;

        internal Sprinkler(long limit, TimeSpan delay, Action<A> handler)
        {
            if (delay < TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(delay)} must be positive in duration");
            }

            this.limit = limit;
            this.handler = handler;
            worker = Repeat(delay, Sprinkle);
        }

        private void Sprinkle()
        {
            if (queue.Count >= limit)
            {
                FlushInternal();
            }
            else if (queue.TryDequeue(out var item))
            {
                handler(item);
            }
        }

        /// <summary>
        /// Insert a batch of items into the sprinkler to be dispersed over time.
        /// </summary>
        /// <exception cref="InvalidOperationException">If Dispose has been called.</exception>
        public void Push(IEnumerable<A> batch)
        {
            if (disposing)
            {
                throw new InvalidOperationException("Already disposed or disposal in progress");
            }

            foreach (var item in batch)
            {
                queue.Enqueue(item);
            }

            if (queue.Count > limit)
            {
                FlushInternal();
            }
        }

        /// <summary>
        /// Writes all currently buffered items.
        /// </summary>
        /// <exception cref="InvalidOperationException">If Dispose has been called.</exception>
        public void Flush()
        {
            if (disposing)
            {
                throw new InvalidOperationException("Already disposed or disposal in progress");
            }

            FlushInternal();
        }

        /// <summary>
        /// Begins the disposal process. Sprinkler will flush and then be unusable.
        /// Does nothing on subsequent calls.
        /// </summary>
        public void Dispose()
        {
            disposing = true;
            worker?.Dispose();
            FlushInternal();
        }

        private void FlushInternal()
        {
            foreach (var item in queue)
            {
                handler(item);
            }
        }
    }
}
