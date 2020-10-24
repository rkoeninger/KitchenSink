using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using static KitchenSink.Operators;

namespace KitchenSink
{
    /// <summary>
    /// Creation methods for <see cref="Batcher{A}"/>'s.
    /// </summary>
    public static class Batcher
    {
        /// <summary>
        /// Creates a buffer which flushes when batch size is reached.
        /// </summary>
        public static Batcher<A> Of<A>(long limit, Action<IReadOnlyCollection<A>> handler) =>
            new Batcher<A>(limit, TimeSpan.Zero, handler);

        /// <summary>
        /// Creates a buffer which flushes when time limit is reached.
        /// </summary>
        public static Batcher<A> Of<A>(TimeSpan timeout, Action<IReadOnlyCollection<A>> handler) =>
            new Batcher<A>(long.MaxValue, timeout, handler);

        /// <summary>
        /// Creates a buffer which flushes when either batch size or time limit is reached.
        /// </summary>
        public static Batcher<A> Of<A>(long limit, TimeSpan timeout, Action<IReadOnlyCollection<A>> handler) =>
            new Batcher<A>(limit, timeout, handler);
    }

    /// <summary>
    /// Multi-threaded batcher that batches items over time and writes batches
    /// to an arbitrary handler.
    /// </summary>
    /// <example>
    /// using var b = Batcher.Of(1000, lines => File.AppendAllLines(path, lines));
    /// foreach (var x in xs) { b.Write(x); } // buffer will occassionally flush
    /// // some time later...
    /// // buffer will dispose and flush remaining lines at end of scope
    /// </example>
    public class Batcher<A> : IDisposable
    {
        private readonly long limit;
        private readonly Action<IReadOnlyCollection<A>> handler;
        private readonly ConcurrentQueue<A> queue = new ConcurrentQueue<A>();
        private readonly IDisposable worker;
        private bool disposal;

        internal Batcher(long limit, TimeSpan timeout, Action<IReadOnlyCollection<A>> handler)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw new ArgumentException($"{nameof(timeout)} must be positive in duration");
            }

            this.limit = limit;
            this.handler = handler;

            if (timeout > TimeSpan.Zero)
            {
                worker = Repeat(timeout, FlushInternal);
            }
        }

        /// <summary>
        /// Writes an item to the buffer which will immediately be written as a batch
        /// if the batch size has been reached or batched when criteria are met.
        /// </summary>
        /// <exception cref="InvalidOperationException">If Dispose has been called.</exception>
        public void Push(A item)
        {
            if (disposal)
            {
                throw new InvalidOperationException("Already disposed or disposal in progress");
            }

            queue.Enqueue(item);

            if (queue.Count >= limit)
            {
                FlushInternal();
            }
        }

        /// <summary>
        /// Writes a batch of all currently buffered items.
        /// </summary>
        /// <exception cref="InvalidOperationException">If Dispose has been called.</exception>
        public void Flush()
        {
            if (disposal)
            {
                throw new InvalidOperationException("Already disposed or disposal in progress");
            }

            FlushInternal();
        }

        /// <summary>
        /// Begins the disposal process. Buffer will flush and then be unusable.
        /// Does nothing on subsequent calls.
        /// </summary>
        public void Dispose()
        {
            if (!disposal)
            {
                disposal = true;
                worker?.Dispose();
                FlushInternal();
            }
        }

        private void FlushInternal()
        {
            lock (this)
            {
                var batch = new List<A>();

                while (queue.TryDequeue(out var item))
                {
                    batch.Add(item);
                }

                if (batch.Count > 0)
                {
                    handler(batch);
                }
            }
        }
    }
}
