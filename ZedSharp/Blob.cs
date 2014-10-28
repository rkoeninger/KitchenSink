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
    }

    /// <summary>Immutable, array-like structure.</summary>
    public struct Blob<A> : IEnumerable<A>
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
        private int Length { get; set; }

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
    }
}
