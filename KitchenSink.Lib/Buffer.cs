using System;
using System.Collections.Generic;
using KitchenSink.Concurrent;

namespace KitchenSink
{
    /// <summary>
    /// Generic buffer that passes accumulated values to given handler
    /// when size limit is reached.
    /// </summary>
    /// <example>
    /// var b = Buffer.Of(1000, lines => File.AppendAllLines(path, lines));
    /// foreach (var x in xs) { b.Write(x); } // Buffer will occassionally flush
    /// b.Flush(); // Flush anything that's left at the end
    /// </example>
    public static class Buffer
    {
        public static Buffer<A> Of<A>(long limit, Action<IReadOnlyList<A>> handler) =>
            new Buffer<A>(limit, handler);
    }

    public class Buffer<A>
    {
        private readonly long limit;
        private readonly Action<IReadOnlyList<A>> handler;
        private readonly List<A> items = new List<A>();
        private readonly Lock @lock = Lock.New();

        public Buffer(long limit, Action<IReadOnlyList<A>> handler)
        {
            this.limit = limit;
            this.handler = handler;
        }

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
    }
}
