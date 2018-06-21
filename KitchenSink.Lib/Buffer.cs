﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KitchenSink.Concurrent;

namespace KitchenSink
{
    /// <summary>
    /// Generic buffer that passes accumulated values to given handler
    /// when size limit is reached and can auto-flush after a given timeout.
    /// </summary>
    /// <example>
    /// var b = Buffer.Of(1000, lines => File.AppendAllLines(path, lines));
    /// foreach (var x in xs) { b.Write(x); } // Buffer will occassionally flush
    /// b.Flush(); // Flush anything that's left at the end
    /// </example>
    public static class Buffer
    {
        public static Buffer<A> Of<A>(long limit, Action<IReadOnlyList<A>> handler) =>
            new Buffer<A>(limit, TimeSpan.Zero, handler);

        public static Buffer<A> Of<A>(TimeSpan timeout, Action<IReadOnlyList<A>> handler) =>
            new Buffer<A>(long.MaxValue, timeout, handler);

        public static Buffer<A> Of<A>(long limit, TimeSpan timeout, Action<IReadOnlyList<A>> handler) =>
            new Buffer<A>(limit, timeout, handler);
    }

    public class Buffer<A> : IDisposable
    {
        private readonly long limit;
        private readonly Action<IReadOnlyList<A>> handler;
        private readonly List<A> items = new List<A>();
        private readonly Lock @lock = Lock.New();
        private readonly CancellationTokenSource cancel;
        private readonly Task flusher;
        private bool running = true;

        public Buffer(long limit, TimeSpan timeout, Action<IReadOnlyList<A>> handler)
        {
            this.limit = limit;
            this.handler = handler;

            if (timeout > TimeSpan.Zero)
            {
                cancel = new CancellationTokenSource();
                flusher = Task.Run(() =>
                {
                    while (running)
                    {
                        Task.Delay(timeout, cancel.Token).ContinueWith(_ => Flush()).Wait();
                    }
                });
            }
        }

        public bool IsAsyncFlushRunning => running && (flusher?.IsCompleted ?? false);

        public void Write(A item)
        {
            @lock.Do(() =>
            {
                items.Add(item);

                if (items.Count >= limit)
                {
                    Flush();
                }
            });
        }

        public void Flush()
        {
            @lock.Do(() =>
            {
                if (items.Count > 0)
                {
                    handler(items);
                    items.Clear();
                }
            });
        }

        public void Dispose()
        {
            running = false;
            cancel?.Cancel();
            flusher?.Wait();
            Flush();
        }
    }
}