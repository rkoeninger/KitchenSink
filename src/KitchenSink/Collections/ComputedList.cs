using System;
using System.Collections;
using System.Collections.Generic;

namespace KitchenSink.Collections
{
    /// <summary>
    /// A pseudo-collection of fixed size whose elements are computed on demand via given function.
    /// Elements are not cached and will be re-computed each time.
    /// </summary>
    public class ComputedList<A> : IReadOnlyList<A>
    {
        /// <summary>
        /// Creates a computed list from a given size (fixed index domain) and a function
        /// to generate the value at that point.
        /// </summary>
        public ComputedList(int count, Func<int, A> projector)
        {
            Count = count;
            Projector = projector;
        }

        public A this[int index] =>
            index < 0 || index >= Count
                ? throw new IndexOutOfRangeException(index.ToString())
                : Projector(index);

        /// <summary>
        /// Fixed size (index domain) of this computed list.
        /// </summary>
        public int Count { get; private set; }

        private Func<int, A> Projector { get; set; }

        public IEnumerator<A> GetEnumerator()
        {
            for (var i = 0; i < Count; ++i)
            {
                yield return Projector(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
