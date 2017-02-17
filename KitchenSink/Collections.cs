using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KitchenSink
{
    public static class Collections
    {
        /// <summary>
        /// Adapter for specialized collections that do not implement IEnumerable&lt;A&gt;.
        /// Eagerly reads enumerator results into list.
        /// Result can be enumerated multiple times.
        /// </summary>
        public static IEnumerable<A> AsEnumerable<A>(this IEnumerator e)
        {
            var list = new List<A>();

            while (e.MoveNext())
            {
                list.Add((A) e.Current);
            }

            return list;
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

        public static A[] Fill<A>(this A[] array, A value)
        {
            for (var i = 0; i < array.Length; ++i)
                array[i] = value;

            return array;
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

    public static class EnumeratorLinq
    {
        public static IEnumerator<B> Select<A, B>(this IEnumerator<A> e, Func<A, B> f)
        {
            while (e.MoveNext())
                yield return f(e.Current);
        }

        public static IEnumerator<B> SelectMany<A, B>(this IEnumerator<A> e, Func<A, IEnumerator<B>> f)
        {
            while (e.MoveNext())
            {
                var ee = f(e.Current);

                while (ee.MoveNext())
                    yield return ee.Current;
            }
        }

        public static IEnumerator<A> Where<A>(this IEnumerator<A> e, Func<A, bool> f)
        {
            while (e.MoveNext())
                if (f(e.Current))
                    yield return e.Current;
        }

        public static IEnumerable<A> ToEnumerable<A>(this IEnumerator<A> e)
        {
            while (e.MoveNext())
                yield return e.Current;
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

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>()
        {
            return new Dictionary<A, B>();
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0)
        {
            return new Dictionary<A, B> { { key0, val0 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14, A key15, B val15)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 }, { key15, val15 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14, A key15, B val15, A key16, B val16)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 }, { key15, val15 }, { key16, val16 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14, A key15, B val15, A key16, B val16, A key17, B val17)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 }, { key15, val15 }, { key16, val16 }, { key17, val17 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14, A key15, B val15, A key16, B val16, A key17, B val17, A key18, B val18)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 }, { key15, val15 }, { key16, val16 }, { key17, val17 }, { key18, val18 } };
        }

        /// <summary>Creates a new Dictionary from the provided arguments. Arguments are provided in key-value-key-value order.</summary>
        /// <remarks>Key and value types can be inferred.</remarks>
        public static Dictionary<A, B> Of<A, B>(A key0, B val0, A key1, B val1, A key2, B val2, A key3, B val3, A key4, B val4, A key5, B val5, A key6, B val6, A key7, B val7, A key8, B val8, A key9, B val9, A key10, B val10, A key11, B val11, A key12, B val12, A key13, B val13, A key14, B val14, A key15, B val15, A key16, B val16, A key17, B val17, A key18, B val18, A key19, B val19)
        {
            return new Dictionary<A, B> { { key0, val0 }, { key1, val1 }, { key2, val2 }, { key3, val3 }, { key4, val4 }, { key5, val5 }, { key6, val6 }, { key7, val7 }, { key8, val8 }, { key9, val9 }, { key10, val10 }, { key11, val11 }, { key12, val12 }, { key13, val13 }, { key14, val14 }, { key15, val15 }, { key16, val16 }, { key17, val17 }, { key18, val18 }, { key19, val19 } };
        }
    }
}
