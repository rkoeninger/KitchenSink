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
            for (int i = 0; i < Math.Max(x.Length, y.Length); ++i)
            {
                if (i == x.Length && i == y.Length)
                    return 0;

                if (i == x.Length)
                    return -1;

                if (i == y.Length)
                    return 1;

                int c = x[i].CompareTo(y[i]);

                if (c != 0)
                    return c;
            }

            return 0;
        }
    }

    /// <summary>Immutable, array-like structure.</summary>
    public struct Blob<A> : IEnumerable<A>, IEquatable<Blob<A>>
    {
        public static implicit operator A[](Blob<A> blob)
        {
            return blob.ToArray();
        }

        public static implicit operator Blob<A>(A[] array)
        {
            return new Blob<A>(array);
        }

        internal Blob(params A[] vals) : this((IEnumerable<A>) vals)
        {
        }

        internal Blob(IEnumerable<A> vals) : this()
        {
            Values = vals.ToArray();
            Length = Values.Length;
        }

        private A[] Values { get; set; }
        public int Length { get; private set; }

        public A this[int index]
        {
            get { return Get(index); }
        }

        public A Get(int index)
        {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException("Index of {0} is out of range for an array of length {1}".Format(index, Length));

            return Values[index];
        }

        public IEnumerator<A> GetEnumerator()
        {
            return Length == 0 ? Enumerable.Empty<A>().GetEnumerator() : Values.AsEnumerable().GetEnumerator();
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
            if (Length != that.Length)
                return false;

            if (Length == 0)
                return true;

            for (int i = 0; i < Length; ++i)
                if (! Object.Equals(Values[i], that.Values[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            if (Length == 0)
                return "[]";

            return "[" + String.Join(", ", Values) + "]";
        }
    }
}
