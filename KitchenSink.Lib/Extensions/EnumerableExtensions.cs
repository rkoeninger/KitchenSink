using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns true if item is in sequence.
        /// </summary>
        public static bool IsIn<A>(this A val, params A[] vals) => IsIn(val, (IEnumerable<A>) vals);

        /// <summary>
        /// Returns true if item is in sequence.
        /// </summary>
        public static bool IsIn<A>(this A val, IEnumerable<A> seq) => seq.Any(Apply<A, A, bool>(Eq, val));

        /// <summary>
        /// Returns true if item is not in sequence.
        /// </summary>
        public static bool IsNotIn<A>(this A val, params A[] vals) => !IsIn(val, (IEnumerable<A>) vals);

        /// <summary>
        /// Returns true if item is not in sequence.
        /// </summary>
        public static bool IsNotIn<A>(this A val, IEnumerable<A> seq) => !IsIn(val, seq);

        /// <summary>
        /// Adapter for specialized collections that do not implement <see cref="IEnumerable{A}"/>.
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
        /// Adapter for specialized collections that do not implement <see cref="IEnumerable{A}"/>.
        /// </summary>
        public static IEnumerable<A> AsEnumerable<A>(this IEnumerator<A> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }

        /// <summary>
        /// Adapter for specialized collections that do not implement <see cref="IEnumerable{A}"/>.
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

        /// <summary>
        /// Sorts elements by their natural order.
        /// </summary>
        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq) where A : IComparable =>
            seq.OrderBy(x => x);

        /// <summary>
        /// Sorts elements descending by their natural order.
        /// </summary>
        public static IEnumerable<A> SortDescending<A>(this IEnumerable<A> seq) where A : IComparable =>
            seq.OrderByDescending(x => x);

        /// <summary>
        /// Sorts elements according to given comparer.
        /// </summary>
        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, IComparer<A> comparer) =>
            seq.OrderBy(x => x, comparer);

        /// <summary>
        /// Sorts elements descending according to given comparer.
        /// </summary>
        public static IEnumerable<A> SortDescending<A>(this IEnumerable<A> seq, IComparer<A> comparer) =>
            seq.OrderByDescending(x => x, comparer);

        /// <summary>
        /// Sorts elements according to given comparer.
        /// </summary>
        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, Func<A, A, Comparison> f) =>
            seq.OrderBy(x => x, new ComparisonComparer<A>(f));

        /// <summary>
        /// Sorts elements descending according to given comparer.
        /// </summary>
        public static IEnumerable<A> SortDescending<A>(this IEnumerable<A> seq, Func<A, A, Comparison> f) =>
            seq.OrderByDescending(x => x, new ComparisonComparer<A>(f));

        private class ComparisonComparer<A> : IComparer<A>
        {
            private readonly Func<A, A, Comparison> f;

            public ComparisonComparer(Func<A, A, Comparison> f)
            {
                this.f = f;
            }

            public int Compare(A x, A y)
            {
                switch (f(x, y))
                {
                    case Comparison.GT: return 1;
                    case Comparison.LT: return -1;
                    case Comparison.EQ: return 0;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Returns elements in given sequence as sub-sequences of given size.
        /// Example: <c>[1, 2, 3, 4, 5, 6, 7, 8], 3 => [[1, 2, 3], [4, 5, 6], [7, 8]]</c>
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Batch<A>(this IEnumerable<A> seq, int count)
        {
            var segment = new A[count];
            var i = 0;

            foreach (var item in seq)
            {
                segment[i] = item;
                i++;

                if (i == count)
                {
                    yield return segment;
                    segment = new A[count];
                    i = 0;
                }
            }

            if (i > 0)
            {
                yield return segment.Take(i);
            }
        }

        /// <summary>
        /// Returns sequence that eagerly reads from given sequence in groups of <c>count</c>.
        /// </summary>
        public static IEnumerable<A> Buffer<A>(this IEnumerable<A> seq, int count) =>
            seq.Batch(count).Flatten();

        /// <summary>
        /// Combines a sequence of sub-sequences into one long sequence.
        /// Example: <c>[[1, 2, 3], [4, 5], [6, 7, 8]] => [1, 2, 3, 4, 5, 6, 7, 8]</c>
        /// </summary>
        public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> seq) =>
            seq.SelectMany(x => x);

        /// <summary>
        /// Combines sub-sequences of arbitrary and varied depth, as determined
        /// by given <c>Either</c> function.
        /// Example: <c>[[1, [2, 3]], [[4], 5], [[6, 7], 8]] => [1, 2, 3, 4, 5, 6, 7, 8]</c>
        /// </summary>
        public static IEnumerable<B> Flatten<A, B>(this IEnumerable<A> seq, Func<A, Either<B, IEnumerable<A>>> f) =>
            seq.SelectMany(x => f(x).Branch(y => SeqOf(y), ys => Flatten(ys, f)));

        /// <summary>
        /// Returns sequence of overlapping pairs of elements in given sequence.
        /// Example: <c>[1, 2, 3, 4, 5] => [[1, 2], [2, 3], [3, 4], [4, 5]]</c>
        /// </summary>
        public static IEnumerable<(A, A)> OverlappingPairs<A>(this IEnumerable<A> seq)
        {
            using (var e = seq.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break;
                }

                var previous = e.Current;

                if (!e.MoveNext())
                {
                    throw new ArgumentException("too few elements");
                }

                var current = e.Current;
                yield return (previous, current);

                while (e.MoveNext())
                {
                    previous = current;
                    current = e.Current;
                    yield return (previous, current);
                }
            }
        }

        /// <summary>
        /// Returns a sequence with a copy of <c>separator</c> between each
        /// element of the original sequence.
        /// Example: <c>[1, 2, 3], 0 => [1, 0, 2, 0, 3]</c>
        /// </summary>
        public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> seq, A seperator)
        {
            using (var e = seq.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break;
                }

                yield return e.Current;

                while (e.MoveNext())
                {
                    yield return seperator;
                    yield return e.Current;
                }
            }
        }

        /// <summary>
        /// Returns a sequence with copies of <c>separators</c> between each
        /// element of the original sequence.
        /// Example: <c>[1, 2, 3], [4, 5, 6] => [1, 4, 5, 6, 2, 4, 5, 6, 3]</c>
        /// </summary>
        public static IEnumerable<A> IntersperseMany<A>(this IEnumerable<A> seq, IEnumerable<A> seperators)
        {
            var lazy = new Lazy<A[]>(seperators.ToArray);

            using (var e = seq.GetEnumerator())
            {
                if (!e.MoveNext())
                {
                    yield break;
                }

                yield return e.Current;

                while (e.MoveNext())
                {
                    foreach (var sep in lazy.Value)
                    {
                        yield return sep;
                    }

                    yield return e.Current;
                }
            }
        }

        /// <summary>
        /// Returns a sequence with a copy of <c>separator</c> between each
        /// element of the original sequence.
        /// Example: <c>[1, 2, 3], [7, 8, 9] => [1, 7, 2, 8, 3, 9]</c>
        /// </summary>
        public static IEnumerable<A> Interleave<A>(this IEnumerable<A> seq1, IEnumerable<A> seq2)
        {
            using (var e1 = seq1.GetEnumerator())
            using (var e2 = seq2.GetEnumerator())
            {
                while (true)
                {
                    if (!e1.MoveNext())
                    {
                        while (e2.MoveNext())
                        {
                            yield return e2.Current;
                        }

                        yield break;
                    }

                    yield return e1.Current;

                    if (!e2.MoveNext())
                    {
                        while (e1.MoveNext())
                        {
                            yield return e1.Current;
                        }

                        yield break;
                    }

                    yield return e2.Current;
                }
            }
        }

        /// <summary>
        /// Infinitely enumerates sequence.
        /// Example: <c>[1, 2, 3] => [1, 2, 3, 1, 2, 3, 1, 2, 3, 1, 2, ...]</c>
        /// </summary>
        public static IEnumerable<A> Cycle<A>(this IEnumerable<A> seq)
        {
            var list = new List<A>();

            foreach (var item in seq)
            {
                list.Add(item);
                yield return item;
            }

            while (true)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }

            // ReSharper disable once IteratorNeverReturns
        }

        /// <summary>
        /// Creates a <see cref="HashSet{A}"/> from an <see cref="IEnumerable{A}"/>.
        /// </summary>
        public static HashSet<A> ToSet<A>(this IEnumerable<A> seq) => new HashSet<A>(seq);

        /// <summary>
        /// Creates a <see cref="Func{B}"/> of <see cref="Maybe{A}"/> from an <see cref="IEnumerable{A}"/>.
        /// </summary>
        public static Func<Maybe<A>> AsFunc<A>(this IEnumerable<A> seq)
        {
            var e = seq.GetEnumerator();
            var done = false;
            return () =>
            {
                if (e.MoveNext())
                {
                    return Some(e.Current);
                }

                if (!done)
                {
                    e.Dispose();
                    done = true;
                }

                return None<A>();
            };
        }

        /// <summary>
        /// Forces entire sequence to be enumerated immediately.
        /// </summary>
        public static IEnumerable<A> Force<A>(this IEnumerable<A> seq) => seq.ToArray();

        /// <summary>
        /// Optimized version of Concat for Arrays.
        /// </summary>
        public static A[] Concat<A>(this A[] array, params A[] vals)
        {
            var result = new A[array.Length + vals.Length];
            array.CopyTo(result, 0);
            vals.CopyTo(result, array.Length);
            return result;
        }

        /// <summary>
        /// Optimized version of Concat for Lists.
        /// </summary>
        public static List<A> Concat<A>(this List<A> xs, List<A> ys)
        {
            var result = new List<A>(xs.Count + ys.Count);
            result.AddRange(xs);
            result.AddRange(ys);
            return result;
        }

        /// <summary>
        /// Combines two sequences by pairing off their elements into tuples.
        /// Example: <c>[1, 2, 3], [A, B, C] => [(1, A), (2, B), (3, C)]</c>
        /// </summary>
        public static IEnumerable<(A, B)> Zip<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys) => xs.Zip(ys, TupleOf);

        /// <summary>
        /// Returns a sequence of items paired with their index in the original sequence.
        /// Example: <c>[A, B, C] => [(0, A), (1, B), (2, C)]</c>
        /// </summary>
        public static IEnumerable<(int, A)> ZipWithIndex<A>(this IEnumerable<A> seq) => From(0).Zip(seq);

        /// <summary>
        /// Returns a dictionary of element counts indexed by an arbitrary property.
        /// Example: <c>[3, -2, 8, 0, -1, 4, -5, 6], Sign => {{-1, 3}, {0, 1}, {1, 4}}</c>
        /// </summary>
        public static IDictionary<B, int> CountBy<A, B>(this IEnumerable<A> seq, Func<A, B> f) =>
            seq.GroupBy(f).ToDictionary(x => x.Key, x => x.Count());

        /// <summary>
        /// Returns sequence, excluding elements at given indicies.
        /// Example: <c>[1, 2, 3, 4, 5, 6, 7, 8], 3, 5 => [1, 2, 3, 5, 7, 8]</c>
        /// </summary>
        public static IEnumerable<A> ExceptAt<A>(this IEnumerable<A> seq, params int[] indicies) =>
            seq.Where((_, i) => i.IsNotIn(indicies));

        /// <summary>
        /// Randomizes elements in sequence. This will enumerate the entire sequence.
        /// Example: <c>[1, 2, 3, 4, 5] => [3, 5, 2, 1, 4]</c>
        /// </summary>
        public static IEnumerable<A> Shuffle<A>(this IEnumerable<A> seq, Random rand = null)
        {
            rand = rand ?? new Random();
            var values = seq.ToArray();

            for (var i = 0; i < values.Length; ++i)
            {
                var j = rand.Next(i, values.Length);
                yield return values[j];
                values[j] = values[i];
            }
        }

        /// <summary>
        /// Performs side-effecting Action on each item in sequence.
        /// </summary>
        public static void ForEach<A>(this IEnumerable<A> seq, Action<A> f)
        {
            foreach (var item in seq)
            {
                f(item);
            }
        }

        /// <summary>
        /// Sets every value in array to a particular value.
        /// </summary>
        public static A[] Fill<A>(this A[] array, A value)
        {
            for (var i = 0; i < array.Length; ++i)
            {
                array[i] = value;
            }

            return array;
        }

        /// <summary>
        /// Returns enumerable that can be re-enumerated even if given enumerable can't be.
        /// </summary>
        public static IReplayableEnumerable<A> Replayable<A>(this IEnumerable<A> seq) =>
            new ReplayableEnumerable<A>(seq);
    }
    
    public interface IReplayableEnumerable<out A> : IEnumerable<A> { }

    internal class ReplayableEnumerable<A> : IReplayableEnumerable<A>
    {
        private readonly Lazy<IEnumerator<A>> source;
        private readonly Atom<(bool, List<A>)> items = Atom.Of((false, ListOf<A>()));

        public ReplayableEnumerable(IEnumerable<A> source) =>
            this.source = new Lazy<IEnumerator<A>>(source.GetEnumerator);

        public IEnumerator<A> GetEnumerator() => new Etor(this);

        private class Etor : IEnumerator<A>
        {
            private readonly ReplayableEnumerable<A> seq;
            private int index;
            private Maybe<A> current = None<A>();

            public Etor(ReplayableEnumerable<A> seq) => this.seq = seq;

            public A Current => current.OrElseThrow(new InvalidOperationException());

            public bool MoveNext() => seq.items.Update(t =>
            {
                var (done, list) = t;

                if (done)
                {
                    return (false, list);
                }

                if (index < list.Count)
                {
                    current = Some(list[index++]);
                    return (true, list);
                }

                if (seq.source.Value.MoveNext())
                {
                    var val = seq.source.Value.Current;
                    list.Add(val);
                    index++;
                    current = Some(val);
                    return (true, list);
                }

                return (false, list);
            }).Item1;

            public void Reset() => index = 0;

            public void Dispose() { }

            object IEnumerator.Current => Current;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
