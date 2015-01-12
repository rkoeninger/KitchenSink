using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Collections
    {
        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderBy(Funcs.Id);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderByDescending(Funcs.Id);
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderBy(Funcs.Id, comp);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderByDescending(Funcs.Id, comp);
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
            return Partition(list.As<IEnumerable<A>>(), count).Select(x => x.ToList());
        }

        public static IEnumerable<A[]> Partition<A>(this A[] array, int count)
        {
            return Partition(array.As<IEnumerable<A>>(), count).Select(x => x.ToArray());
        }

        public static HashSet<A> Set<A>(params A[] vals)
        {
            return new HashSet<A>(vals);
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
    }

    public static class ReadOnly
    {
        public static IReadOnlyList<A> List<A>(params A[] values)
        {
            return new List<A>(values);
        }

        public static IReadOnlyCollection<A> Collection<A>(params A[] values)
        {
            return values;
        }
    }

    public static class KeyValuePair
    {
        public static KeyValuePair<A, B> Of<A, B>(A key, B value)
        {
            return new KeyValuePair<A, B>(key, value);
        }
    }

    public static class List
    {
        public static List<A> Of<A>(params A[] vals)
        {
            return new List<A>(vals);
        }

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

    public static class AnArray
    {
        public static A[] Of<A>(params A[] vals)
        {
            return vals;
        }
    }

    public static class Seq
    {
        public static IEnumerable<A> Of<A>(params A[] vals)
        {
            return vals;
        }

        public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> seq, A seperator)
        {
            var itr = seq.GetEnumerator();

            if (!itr.MoveNext())
                yield break;

            yield return itr.Current;

            while (itr.MoveNext())
            {
                yield return seperator;
                yield return itr.Current;
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

            // ReSharper disable once FunctionNeverReturns
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

            // ReSharper disable once FunctionNeverReturns
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
    }
}
