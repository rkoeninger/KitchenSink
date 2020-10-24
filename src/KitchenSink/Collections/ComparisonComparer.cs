using System;
using System.Collections.Generic;

namespace KitchenSink.Collections
{
    /// <summary>
    /// A comparer based on a function that returns a gt/lt/eq result.
    /// </summary>
    public class ComparisonComparer<A> : IComparer<A>
    {
        private readonly Func<A, A, Ordering> f;

        /// <summary>
        /// Builds a comparer out of a function.
        /// </summary>
        public ComparisonComparer(Func<A, A, Ordering> f) => this.f = f;

        public int Compare(A x, A y) =>
            f(x, y) switch
            {
                Ordering.Gt => 1,
                Ordering.Lt => -1,
                Ordering.Eq => 0,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
