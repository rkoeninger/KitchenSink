using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Blob
    {
        public static Blob<A> Of<A>(params A[] vals)
        {
            return new Blob<A>(vals);
        }

        public static Blob<A> Of<A>(IEnumerable<A> vals)
        {
            return new Blob<A>(vals);
        }

        public static Blob<A> ToBlob<A>(this IEnumerable<A> vals)
        {
            return new Blob<A>(vals);
        }

        /// <summary>A lexicographical comparison of two blobs.</summary>
        public static int Compare<A>(Blob<A> x, Blob<A> y) where A : IComparable<A>
        {
            for (int i = 0; i < Math.Min(x.Length, y.Length); ++i)
            {
                int c = x[i].CompareTo(y[i]);

                if (c != 0)
                    return c;
            }

            if (x.Length < y.Length)
                return -1;

            if (x.Length > y.Length)
                return 1;

            return 0;
        }
    }

    /// <summary>Immutable, array-like structure. Provides random-access operations.</summary>
    public struct Blob<A> : IEnumerable<A>, IEquatable<Blob<A>>
    {
        public static bool operator ==(Blob<A> x, Blob<A> y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Blob<A> x, Blob<A> y)
        {
            return ! (x == y);
        }

        internal Blob(IEnumerable<A> vals) : this()
        {
            Values = vals.ToArray();
        }

        private A[] Values { get; set; }
        public int Length { get { return Values == null ? 0 : Values.Length; } }

        public A this[int index]
        {
            get { return Get(index); }
        }

        /// <summary>Returns the value at the given index in this blob.</summary>
        public A Get(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException("Index {0} is out of range for an array of length {1}".Format(index, Length));

            return Values[index];
        }

        public Maybe<A> GetMaybe(int index)
        {
            var len = Length;
            return Maybe.If(Values, x => index >= 0 && index < len, x => x[index]);
        }

        public IEnumerator<A> GetEnumerator()
        {
            if (Length == 0)
                yield break;

            foreach (var item in Values)
                yield return item;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(Object that)
        {
            return that.Is<Blob<A>>() && Equals((Blob<A>) that);
        }

        public bool Equals(Blob<A> that)
        {
            if (Object.ReferenceEquals(this, that))
                return true;

            if (Length != that.Length)
                return false;

            for (int i = 0; i < Length; ++i)
                if (! Object.Equals(Values[i], that.Values[i]))
                    return false;

            return true;
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

            return "[" + Values.Concat(", ") + "]";
        }
    }
}
