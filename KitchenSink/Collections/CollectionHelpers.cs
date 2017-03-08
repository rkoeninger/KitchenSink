using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Collections
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Adapter for specialized collections that do not implement IEnumerable&lt;A&gt;.
        /// Eagerly reads enumerator results into list.
        /// Result can be enumerated multiple times.
        /// </summary>
        public static IEnumerable AsEnumerable(this IEnumerator e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        /// <summary>
        /// Adapter for specialized collections that do not implement IEnumerable&lt;A&gt;.
        /// </summary>
        public static IEnumerable<A> AsEnumerable<A>(this IEnumerator<A> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        /// <summary>
        /// Adapter for specialized collections that do not implement IEnumerable&lt;A&gt;.
        /// Lazily reads enumerator results and returns them.
        /// Result can be enumerated only once.
        /// </summary>
        public static IEnumerable<A> AsEnumerableNonRepeatable<A>(this IEnumerator e)
        {
            while (e.MoveNext())
            {
                yield return (A) e.Current;
            }
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderBy(x => x);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderByDescending(x => x);
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderBy(x => x, comp);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderByDescending(x => x, comp);
        }

        public static IEnumerable<IEnumerable<A>> Partition<A>(this IEnumerable<A> seq, int count)
        {
            var array = seq.ToArray();

            for (var i = 0; i < array.Length; i += count)
            {
                var partSize = Math.Min(count, array.Length - i);
                var array2 = new A[partSize];

                for (var j = 0; j < partSize; ++j)
                {
                    array2[j] = array[i + j];
                }

                yield return array2;
            }
        }

        public static IEnumerable<List<A>> Partition<A>(this List<A> list, int count)
        {
            return Partition(list.AsEnumerable(), count).Select(x => x.ToList());
        }

        public static IEnumerable<A[]> Partition<A>(this A[] array, int count)
        {
            return Partition(array.AsEnumerable(), count).Select(x => x.ToArray());
        }

        public static IEnumerable<Tuple<A, A>> OverlappingPartition2<A>(this IEnumerable<A> seq)
        {
            var array = seq.ToArray();

            if (array.Length < 2)
                throw new Exception("too few elements");

            return Enumerable.Range(0, array.Length - 1).Select(i => Tuple.Create(array[i], array[i + 1]));
        }

        public static IEnumerable<A> Except<A>(this IEnumerable<A> seq, params A[] excludes)
        {
            var excludeSet = Set(excludes);
            return seq.Where(x => !excludeSet.Contains(x));
        }

        public static HashSet<A> Set<A>(this IEnumerable<A> seq)
        {
            return new HashSet<A>(seq);
        }

        public static A[] Add<A>(this A[] array, params A[] vals)
        {
            var result = new A[array.Length + vals.Length];
            array.CopyTo(result, 0);
            vals.CopyTo(result, array.Length);
            return result;
        }

        public static bool NotEmpty<A>(this IEnumerable<A> seq)
        {
            return seq.Any();
        }

        public static IEnumerable<int> Indicies<A>(this IEnumerable<A> seq)
        {
            return Enumerable.Range(0, seq.Count());
        }

        public static IEnumerable<A> WithoutAt<A>(this IEnumerable<A> seq, int index)
        {
            return seq.Where((_, i) => i != index);
        }

        public static IEnumerable<A> Shuffle<A>(this IEnumerable<A> seq)
        {
            var rand = new Random();
            var temp = seq.ToArray();

            foreach (var i in temp.Indicies())
            {
                var j = rand.Next(i, temp.Length);
                yield return temp[j];
                temp[j] = temp[i];
            }
        }

        public static A[] Fill<A>(this A[] array, A value)
        {
            for (var i = 0; i < array.Length; ++i)
                array[i] = value;

            return array;
        }
    }

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

        /// <summary>Performs side-effecting Action on each item in sequence.</summary>
        public static IEnumerable<A> ForEach<A>(this IEnumerable<A> seq, Action<A> f)
        {
            foreach (var item in seq)
            {
                f(item);
                yield return item;
            }
        }

        /// <summary>Forces sequence to enumerate.</summary>
        public static IEnumerable<A> Force<A>(this IEnumerable<A> seq)
        {
            return seq.ToArray();
        }

        public static IEnumerable<Tuple<A, B>> Zip<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys)
        {
            return xs.Zip(ys, TupleOf);
        }
    }

    /// <summary>Utility methods for building Dictionaries.</summary>
    public static class Dictionary
    {
        /// <summary>Creates a new Dictionary from a sequence of Tuples interpreted as key-value pairs.</summary>
        public static Dictionary<A, B> ToDictionary<A, B>(this IEnumerable<Tuple<A, B>> seq)
        {
            return seq.ToDictionary(x => x.Item1, x => x.Item2);
        }

        /// <summary>Creates a new Dictionary from a sequence of KeyValuePairs.</summary>
        public static Dictionary<A, B> ToDictionary<A, B>(this IEnumerable<KeyValuePair<A, B>> seq)
        {
            return seq.ToDictionary(x => x.Key, x => x.Value);
        }

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
