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
        public ComputedList(int count, Func<int, A> projector)
        {
            Count = count;
            Projector = projector;
        }

        public A this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException("");
                }

                return Projector(index);
            }
        }

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
