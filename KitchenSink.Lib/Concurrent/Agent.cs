using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace KitchenSink.Concurrent
{
    /// <summary>
    /// Atomic value cell with asynchronous updates.
    /// </summary>
    public static class Agent
    {
        /// <summary>
        /// Create new Agent with given initial value.
        /// </summary>
        public static Agent<A> Of<A>(A initial) => new Agent<A>(initial);
    }

    /// <summary>
    /// Atomic value cell with asynchronous updates.
    /// </summary>
    public class Agent<A> : IDisposable
    {
        private readonly ConcurrentQueue<Task<A>> queue;
        private readonly Thread worker;
        private bool running = true;

        /// <summary>
        /// Create new Agent with given initial value.
        /// </summary>
        public Agent(A initial)
        {
            Value = initial;
            queue = new ConcurrentQueue<Task<A>>();
            worker = new Thread(() =>
            {
                while (running || !queue.IsEmpty)
                {
                    if (queue.TryDequeue(out var task))
                    {
                        task.RunSynchronously();
                    }
                    else
                    {
                        SpinWait.SpinUntil(() => !running || !queue.IsEmpty, 100);
                    }
                }
            });
            worker.Start();
        }

        /// <summary>
        /// Synchronously get contained value.
        /// </summary>
        public A Value { get; private set; }

        /// <summary>
        /// Asynchronously reset contained value to given value.
        /// </summary>
        public Task<A> Reset(A value) => Update(_ => value);

        /// <summary>
        /// Asynchronously transform contained value using given function.
        /// </summary>
        public Task<A> Update(Func<A, A> f)
        {
            if (!running)
            {
                throw new AgentDisposedException();
            }

            var task = new Task<A>(() => Value = f(Value));
            queue.Enqueue(task);
            return task;
        }

        /// <summary>
        /// Shuts down Agent, waiting for all pending operations to finish.
        /// </summary>
        public void Dispose()
        {
            running = false;
            worker.Join();
        }
    }
}
