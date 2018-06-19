using System;
using System.Collections.Concurrent;

namespace KitchenSink
{
    public class GenericCache<A, B>
    {
        private readonly ConcurrentDictionary<A, (DateTime, B)> cache;
        private readonly TimeSpan duration;
        private readonly Func<A, B> lookup;

        public GenericCache(Func<A, B> lookup) : this(TimeSpan.MaxValue, lookup) { }

        public GenericCache(TimeSpan duration, Func<A, B> lookup)
        {
            cache = new ConcurrentDictionary<A, (DateTime, B)>();
            this.duration = duration;
            this.lookup = lookup;
        }

        public B Get(A key) =>
            (duration == TimeSpan.MaxValue
            ? cache.GetOrAdd(key, k => (DateTime.UtcNow, lookup(k)))
            : cache.AddOrUpdate(
                key,
                k => (DateTime.UtcNow, lookup(k)),
                (k, v) => v.Item1 < DateTime.UtcNow + duration
                    ? (DateTime.UtcNow, lookup(k))
                    : v)).Item2;
    }
}
