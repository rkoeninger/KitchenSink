using System;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class ListExtensions
    {
        public static List<A> Concat<A>(this List<A> list, params A[] vals)
        {
            var result = new List<A>(list);
            list.AddRange(vals);
            return result;
        }

        public static bool IsEmpty<A>(this IList<A> list)
        {
            return list.Count == 0;
        }

        public static bool IsNotEmpty<A>(this IList<A> list)
        {
            return list.Count > 0;
        }

        public static IEnumerable<int> Indicies<A>(this IList<A> list)
        {
            return Enumerable.Range(0, list.Count);
        }

        public static IList<A> RemoveAll<A>(this IList<A> list, IEnumerable<A> seq)
        {
            foreach (var item in seq)
                list.Remove(item);

            return list;
        }
    }

    public static class Seq
    {
        public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> seq, A seperator)
        {
            using (var itr = seq.GetEnumerator())
            {
                if (!itr.MoveNext())
                    yield break;

                yield return itr.Current;

                while (itr.MoveNext())
                {
                    yield return seperator;
                    yield return itr.Current;
                }
            }
        }

        /// <summary>Infinitely enumerates sequence.</summary>
        public static IEnumerable<A> Cycle<A>(this IEnumerable<A> seq)
        {
            var list = new List<A>();

            foreach (var item in seq)
            {
                list.Add(item);
                yield return item;
            }

            while (true)
                foreach (var item in list)
                    yield return item;

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>Infinitely repeats item(s).</summary>
        public static IEnumerable<A> Cycle<A>(params A[] vals)
        {
            return vals.Cycle();
        }

        /// <summary>Infinitely enumerates items returned from provided function.</summary>
        public static IEnumerable<A> Forever<A>(Func<A> f)
        {
            while (true)
                yield return f();

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Generates a sequence based on given function.
        /// Functions returns None to indicate end of sequence.
        /// </summary>
        public static IEnumerable<A> Unaggregate<A>(Func<Maybe<A>> f)
        {
            while (true)
            {
                var x = f();

                if (x.HasValue)
                {
                    yield return x.Value;
                }
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>Performs side-effecting Action on each item in sequence.</summary>
        public static IEnumerable<A> ForEach<A>(this IEnumerable<A> seq, Action<A> f)
        {
            foreach (var item in seq)
            {
                f(item);
                yield return item;
            }
        }

        public static IEnumerable<Tuple<A, B>> Zip<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys)
        {
            return xs.Zip(ys, TupleOf);
        }
    }

    /// <summary>Utility methods for building Dictionaries.</summary>
    public static class Dictionary
    {
        /// <summary>Creates a new Dictionary from the properties of an object.</summary>
		/// <remarks>Intended to be used with an anonymous object, but can be used with any object.</remarks>
        public static Dictionary<string, object> Of(object obj)
        {
            if (obj == null)
                return new Dictionary<string, object>();

            return obj.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0).ToDictionary(
                x => x.Name, x => x.GetValue(obj, null));
        }
    }
}
