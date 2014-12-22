using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Chain
    {
        public static Chain<A> Of<A>(params A[] vals)
        {
            var current = Chain<A>.Empty;

            for (int i = vals.Length - 1; i >= 0; --i)
                current = new Chain<A>(vals[i], current);

            return current;
        }

        public static Chain<A> Of<A>(IEnumerable<A> seq)
        {
            return ToChain(seq);
        }

        public static Chain<A> ToChain<A>(this IEnumerable<A> seq)
        {
            return seq.Reverse().Aggregate(Chain<A>.Empty, (chain, item) => new Chain<A>(item, chain));
        }

        /// <summary>A lexicographical comparison of two chains.</summary>
        public static int Compare<A>(Chain<A> x, Chain<A> y) where A : IComparable<A>
        {
            var xs = x.GetEnumerator();
            var ys = y.GetEnumerator();

            while (true)
            {
                var xHasNext = xs.MoveNext();
                var yHasNext = ys.MoveNext();

                if (xHasNext && yHasNext)
                {
                    var c = xs.Current.CompareTo(ys.Current);

                    if (c != 0)
                        return c;
                }
                else if (yHasNext)
                {
                    return -1;
                }
                else if (xHasNext)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    /// <summary>Immutable, singly-linked list. Provides sequential-access operations.</summary>
    public struct Chain<A> : IEnumerable<A>, IEquatable<Chain<A>>
    {
        public static bool operator ==(Chain<A> x, Chain<A> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Chain<A> x, Chain<A> y)
        {
            return !(x == y);
        }

        public static readonly Chain<A> Empty = default(Chain<A>);

        internal Chain(A val, Chain<A> next) : this()
        {
            Head = Maybe.Of(val);
            TailRef = Ref.Of(next);
            Length = next.Length + 1;
        }

        /// <summary>The first value in this chain.</summary>
        public Maybe<A> Head { get; private set; }

        /// <summary>The rest of this chain, following the first value.</summary>
        public Chain<A> Tail { get { return TailRef.OrElse(Empty); } }

        /// <summary>Number of items in this chain.</summary>
        public int Length { get; private set; }

        private Ref<Chain<A>> TailRef { get; set; }

        public IEnumerator<A> GetEnumerator()
        {
            var current = this;

            while (current.Head.HasValue)
            {
                yield return current.Head.OrElse(default(A));
                current = current.Tail;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(Object that)
        {
            return that.Is<Chain<A>>() && Equals((Chain<A>)that);
        }

        public bool Equals(Chain<A> that)
        {
            return Length == that.Length && this.SequenceEqual(that);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        /// <summary>Returns a string formatted like: "[Item1, Item2, Item3]".</summary>
        public override string ToString()
        {
            if (Length == 0)
                return "[]";

            return "[" + this.Concat(", ") + "]";
        }
    }
}
