using System;
using System.Collections;
using System.Collections.Generic;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class ReplayableExtensions
    {
        /// <summary>
        /// Returns enumerable that can be re-enumerated even if given enumerable can't be.
        /// </summary>
        public static IReplayableEnumerable<A> Replayable<A>(this IEnumerable<A> seq) =>
            new ReplayableEnumerable<A>(seq);
    }

    /// <summary>
    /// An <see cref="IEnumerable{T}"/> that can be iterated multiple times
    /// without error and will iterate the same values each time.
    /// </summary>
    public interface IReplayableEnumerable<out A> : IEnumerable<A> { }

    internal class ReplayableEnumerable<A> : IReplayableEnumerable<A>
    {
        private readonly Lazy<IEnumerator<A>> source;
        private readonly Atom<(bool, List<A>)> items = Atom.Of((false, ListOf<A>()));

        public ReplayableEnumerable(IEnumerable<A> source) =>
            this.source = new Lazy<IEnumerator<A>>(source.GetEnumerator);

        public IEnumerator<A> GetEnumerator() => new ReplayableEnumerator(this);

        private class ReplayableEnumerator : IEnumerator<A>
        {
            private readonly ReplayableEnumerable<A> seq;
            private int index;
            private Maybe<A> current = None<A>();

            public ReplayableEnumerator(ReplayableEnumerable<A> seq) => this.seq = seq;

            public A Current => current.OrElseThrow(new InvalidOperationException());

            public bool MoveNext() => seq.items.Update(t =>
            {
                var (done, list) = t;

                if (done)
                {
                    return (false, list);
                }

                if (index < list.Count)
                {
                    current = Some(list[index++]);
                    return (true, list);
                }

                if (!seq.source.Value.MoveNext()) return (false, list);

                var val = seq.source.Value.Current;
                list.Add(val);
                index++;
                current = Some(val);
                return (true, list);
            }).Item1;

            public void Reset() => index = 0;

            public void Dispose() { }

            object IEnumerator.Current => Current;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
