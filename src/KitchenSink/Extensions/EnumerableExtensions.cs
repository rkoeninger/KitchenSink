using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Collections;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns true if item is in sequence.
        /// </summary>
        public static bool IsIn<A>(this A val, params A[] vals) => IsIn(val, vals.AsEnumerable());

        /// <summary>
        /// Returns true if item is in sequence.
        /// </summary>
        public static bool IsIn<A>(this A val, IEnumerable<A> seq) => seq.Contains(val);

        /// <summary>
        /// Returns true if any <paramref name="vals"/> are in sequence.
        /// </summary>
        public static bool ContainsAny<A>(this IEnumerable<A> seq, params A[] vals) => ContainsAny(seq, vals.AsEnumerable());

        /// <summary>
        /// Returns true if any <paramref name="vals"/> are in sequence.
        /// </summary>
        public static bool ContainsAny<A>(this IEnumerable<A> seq, IEnumerable<A> vals) => ContainsAny(seq, vals.ToHashSet());

        /// <summary>
        /// Returns true if any <paramref name="vals"/> are in sequence.
        /// </summary>
        public static bool ContainsAny<A>(this IEnumerable<A> seq, ISet<A> vals) => seq.Any(vals.Contains);

        /// <summary>
        /// Returns true if no <paramref name="vals"/> are missing from sequence.
        /// </summary>
        public static bool ContainsAll<A>(this IEnumerable<A> seq, params A[] vals) => ContainsAllInternal(seq, vals.ToHashSet());

        /// <summary>
        /// Returns true if no <paramref name="vals"/> are missing from sequence.
        /// </summary>
        public static bool ContainsAll<A>(this IEnumerable<A> seq, IEnumerable<A> vals) => ContainsAllInternal(seq, vals.ToHashSet());

        /// <summary>
        /// Returns true if no <paramref name="vals"/> are missing from sequence.
        /// </summary>
        public static bool ContainsAll<A>(this IEnumerable<A> seq, ISet<A> vals) => ContainsAllInternal(seq, vals.ToHashSet());

        // Modifies passed-in set so it needs to be given a fresh copy
        private static bool ContainsAllInternal<A>(this IEnumerable<A> xs, ISet<A> vals)
        {
            if (vals.Count == 0)
            {
                return true;
            }

            foreach (var x in xs)
            {
                vals.Remove(x);

                if (vals.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// If sequence is empty, replace with sequence of given value(s).
        /// </summary>
        public static IEnumerable<A> IfEmpty<A>(this IEnumerable<A> xs, params A[] vals) => IfEmpty(xs, vals.AsEnumerable());

        /// <summary>
        /// If sequence is empty, replace with given sequence.
        /// </summary>
        public static IEnumerable<A> IfEmpty<A>(this IEnumerable<A> xs, IEnumerable<A> ys)
        {
            var any = false;

            foreach (var x in xs)
            {
                any = true;
                yield return x;
            }

            if (!any)
            {
                foreach (var y in ys)
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// If <paramref name="xs"/> is non-empty, append with sequence of <paramref name="vals"/>.
        /// </summary>
        public static IEnumerable<A> IfNonEmpty<A>(this IEnumerable<A> xs, params A[] vals) => IfNonEmpty(xs, vals.AsEnumerable());

        /// <summary>
        /// If <paramref name="xs"/> is non-empty, append with <paramref name="ys"/>.
        /// </summary>
        public static IEnumerable<A> IfNonEmpty<A>(this IEnumerable<A> xs, IEnumerable<A> ys)
        {
            var any = false;

            foreach (var x in xs)
            {
                any = true;
                yield return x;
            }

            if (any)
            {
                foreach (var y in ys)
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// Finds first 2 items in sequence that match predicate and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain at least 2 items matching predicate.
        /// </exception>
        public static (A, A) First2<A>(this IEnumerable<A> xs, Func<A, bool> predicate)
        {
            A x0 = default;
            var count = 0;

            foreach (var x in xs)
            {
                if (predicate(x))
                {
                    switch (count++)
                    {
                        case 0:
                            x0 = x;
                            break;
                        default:
                            return (x0, x);
                    }
                }
            }

            throw new InvalidOperationException("Sequence contains less than 2 items");
        }

        /// <summary>
        /// Finds first 2 items in sequence and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain at least 2 items.
        /// </exception>
        public static (A, A) First2<A>(this IEnumerable<A> xs) => First2(xs, _ => true);

        /// <summary>
        /// Finds first 3 items in sequence that match predicate and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain at least 3 items matching predicate.
        /// </exception>
        public static (A, A, A) First3<A>(this IEnumerable<A> xs, Func<A, bool> predicate)
        {
            A x0 = default;
            A x1 = default;
            var count = 0;

            foreach (var x in xs)
            {
                if (predicate(x))
                {
                    switch (count++)
                    {
                        case 0:
                            x0 = x;
                            break;
                        case 1:
                            x1 = x;
                            break;
                        default:
                            return (x0, x1, x);
                    }
                }
            }

            throw new InvalidOperationException("Sequence contains less than 3 items");
        }

        /// <summary>
        /// Finds first 3 items in sequence and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain at least 3 items.
        /// </exception>
        public static (A, A, A) First3<A>(this IEnumerable<A> xs) => First3(xs, _ => true);

        /// <summary>
        /// Finds exactly 2 items in sequence that match predicate and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain exactly 2 items matching predicate.
        /// </exception>
        public static (A, A) Couple<A>(this IEnumerable<A> xs, Func<A, bool> predicate)
        {
            A x0 = default;
            A x1 = default;
            var count = 0;

            foreach (var x in xs)
            {
                if (predicate(x))
                {
                    switch (count++)
                    {
                        case 0:
                            x0 = x;
                            break;
                        case 1:
                            x1 = x;
                            break;
                        default:
                            throw new InvalidOperationException("Sequence contains more than 2 items");
                    }
                }
            }

            if (count < 2)
            {
                throw new InvalidOperationException("Sequence contains less than 2 items");
            }

            return (x0, x1);
        }

        /// <summary>
        /// Finds exactly 2 items in sequence and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain exactly 2 items.
        /// </exception>
        public static (A, A) Couple<A>(this IEnumerable<A> xs) => Couple(xs, _ => true);

        /// <summary>
        /// Finds exactly 3 items in sequence that match predicate and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain exactly 3 items matching predicate.
        /// </exception>
        public static (A, A, A) Triple<A>(this IEnumerable<A> xs, Func<A, bool> predicate)
        {
            A x0 = default;
            A x1 = default;
            A x2 = default;
            var count = 0;

            foreach (var x in xs)
            {
                if (predicate(x))
                {
                    switch (count++)
                    {
                        case 0:
                            x0 = x;
                            break;
                        case 1:
                            x1 = x;
                            break;
                        case 2:
                            x2 = x;
                            break;
                        default:
                            throw new InvalidOperationException("Sequence contains more than 3 items");
                    }
                }
            }

            if (count < 3)
            {
                throw new InvalidOperationException("Sequence contains less than 3 items");
            }

            return (x0, x1, x2);
        }

        /// <summary>
        /// Finds exactly 3 items in sequence and returns them as a tuple.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// If sequence does not contain exactly 3 items.
        /// </exception>
        public static (A, A, A) Triple<A>(this IEnumerable<A> xs) => Triple(xs, _ => true);

        /// <summary>
        /// Returns greatest item in sequence according to comparer, preferring earlier elements.
        /// </summary>
        public static A MaxBy<A>(this IEnumerable<A> xs, IComparer<A> comparer) =>
            xs.Aggregate(comparer.Max);

        /// <summary>
        /// Returns least item in sequence according to comparer, preferring earlier elements.
        /// </summary>
        public static A MinBy<A>(this IEnumerable<A> xs, IComparer<A> comparer) =>
            xs.Aggregate(comparer.Min);

        /// <summary>
        /// Returns greatest item in sequence according to comparer, preferring earlier elements.
        /// </summary>
        public static Maybe<A> MaxByMaybe<A>(this IEnumerable<A> xs, IComparer<A> comparer) =>
            xs.Aggregate(None<A>(), (m, x2) => m.Select(x1 => comparer.Max(x1, x2)).Or(Some(x2)));

        /// <summary>
        /// Returns least item in sequence according to comparer, preferring earlier elements.
        /// </summary>
        public static Maybe<A> MinByMaybe<A>(this IEnumerable<A> xs, IComparer<A> comparer) =>
            xs.Aggregate(None<A>(), (m, x2) => m.Select(x1 => comparer.Min(x1, x2)).Or(Some(x2)));

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
        /// Returns sequence casted as array if it's an array, otherwise collects to an array.
        /// </summary>
        public static A[] AsArray<A>(this IEnumerable<A> seq) => seq is A[] arr ? arr : seq.ToArray();

        /// <summary>
        /// Returns sequence casted as list if it's a list, otherwise collects to a list.
        /// </summary>
        public static A[] AsList<A>(this IEnumerable<A> seq) => seq is A[] arr ? arr : seq.ToArray();

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
        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, Func<A, A, Ordering> f) =>
            seq.OrderBy(x => x, new ComparisonComparer<A>(f));

        /// <summary>
        /// Sorts elements descending according to given comparer.
        /// </summary>
        public static IEnumerable<A> SortDescending<A>(this IEnumerable<A> seq, Func<A, A, Ordering> f) =>
            seq.OrderByDescending(x => x, new ComparisonComparer<A>(f));

        /// <summary>
        /// Aggregates enumerable sequence using given monoid.
        /// </summary>
        public static A Aggregate<A>(this IEnumerable<A> seq, Monoid<A> m) => m.Aggregate(seq);

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
            seq.Batch(count).Select(x => x.ToArray()).Flatten();

        /// <summary>
        /// Combines a sequence of sub-sequences into one long sequence.
        /// Example: <c>[[1, 2, 3], [4, 5], [6, 7, 8]] => [1, 2, 3, 4, 5, 6, 7, 8]</c>
        /// </summary>
        public static IEnumerable<A> Flatten<A>(this IEnumerable<IEnumerable<A>> seq) =>
            seq.SelectMany(Id);

        /// <summary>
        /// Combines sub-sequences of arbitrary and varied depth, as determined
        /// by given <c>Either</c> function.
        /// Example: <c>[[1, [2, 3]], [[4], 5], [[6, 7], 8]] => [1, 2, 3, 4, 5, 6, 7, 8]</c>
        /// </summary>
        public static IEnumerable<B> Flatten<A, B>(this IEnumerable<A> seq, Func<A, Either<B, IEnumerable<A>>> f) =>
            seq.SelectMany(x => f(x).Branch(y => SeqOf(y), ys => Flatten(ys, f)));

        /// <summary>
        /// Returns sequence of overlapping subsequences of given size.
        /// Example: <c>[1, 2, 3, 4, 5], 3 => [[1, 2, 3], [2, 3, 4], [3, 4, 5]]</c>
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Clump<A>(this IEnumerable<A> seq, int size)
        {
            var buffer = new A[size];

            foreach (var (i, x) in seq.ZipWithIndex())
            {
                buffer[i % size] = x;

                if (i >= size - 1)
                {
                    var offset = (i + 1) % size;
                    yield return 0.To(size)
                        .Select(j => buffer[(j + offset) % size])
                        .ToArray();
                }
            }
        }

        /// <summary>
        /// Returns a sequence with a copy of <c>separator(s)</c> between each
        /// element of the original sequence.
        /// Example: <c>[1, 2, 3], [A, B, C] => [1, A, B, C, 2, A, B, C, 3]</c>
        /// </summary>
        public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> seq, params A[] separators) =>
            Intersperse(seq, separators.AsEnumerable());

        /// <summary>
        /// Returns a sequence with copies of <c>separator(s)</c> between each
        /// element of the original sequence.
        /// Example: <c>[1, 2, 3], [A, B, C] => [1, A, B, C, 2, A, B, C, 3]</c>
        /// </summary>
        public static IEnumerable<A> Intersperse<A>(this IEnumerable<A> seq, IEnumerable<A> seperators)
        {
            A[] seps = null;
            var first = true;

            foreach (var item in seq)
            {
                if (!first)
                {
                    seps ??= seperators.ToArray();

                    foreach (var sep in seps)
                    {
                        yield return sep;
                    }
                }

                yield return item;
                first = false;
            }
        }

        /// <summary>
        /// Returns a sequence of the elements of given sequences in round-robin order.
        /// Example: <c>[1, 2], [3, 4], [5, 6] => [1, 3, 5, 2, 4, 6]</c>
        /// </summary>
        public static IEnumerable<A> Interleave<A>(this IEnumerable<A> seq, params IEnumerable<A>[] seqs) =>
            Interleave(seqs.Prepend(seq));

        /// <summary>
        /// Returns a sequence of the elements of given sequences in round-robin order.
        /// Example: <c>[1, 2], [3, 4], [5, 6] => [1, 3, 5, 2, 4, 6]</c>
        /// </summary>
        public static IEnumerable<A> Interleave<A>(this IEnumerable<IEnumerable<A>> seqs)
        {
            var enumerators = seqs.Select(x => x.GetEnumerator()).ToArray();
            using var _ = new AggregateDisposable(enumerators);
            var running = true;

            while (running)
            {
                running = false;

                foreach (var enumerator in enumerators)
                {
                    if (enumerator.MoveNext())
                    {
                        running = true;
                        yield return enumerator.Current;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a sequence of the elements of given sequences in round-robin order.
        /// Example: <c>[1, 2], [3, 4], [5, 6] => [[1, 3], [5, 2], [4, 6]]</c>
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Transpose<A>(this IEnumerable<IEnumerable<A>> seqs)
        {
            var enumerators = seqs.Select(x => x.GetEnumerator()).ToArray();
            using var _ = new AggregateDisposable(enumerators);

            while (enumerators.All(e => e.MoveNext()))
            {
                yield return enumerators.Select(e => e.Current);
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
                if (e.MoveNext()) return Some(e.Current);
                if (done) return None<A>();

                e.Dispose();
                done = true;
                return None<A>();
            };
        }

        public static IEnumerable<A> AsEnumerable<A>(this Func<Maybe<A>> f)
        {
            var x = f();

            while (x.HasValue)
            {
                yield return x.Value;
                x = f();
            }
        }

        /// <summary>
        /// Forces entire sequence to be enumerated immediately, returning sequence.
        /// </summary>
        public static IEnumerable<A> Force<A>(this IEnumerable<A> seq) => seq.ToArray();

        /// <summary>
        /// Forces entire sequence to be enumerated immediately, returning nothing.
        /// </summary>
        public static void DoAll<A>(this IEnumerable<A> seq)
        {
            foreach (var _ in seq) { }
        }

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
        /// Sames as the standard <see cref="Enumerable.Zip{A, B, C}"/>, but
        /// raises exception if sequences are not of the same length.
        /// </summary>
        public static IEnumerable<C> ZipExact<A, B, C>(this IEnumerable<A> xs, IEnumerable<B> ys, Func<A, B, C> f)
        {
            using var ex = xs.GetEnumerator();
            using var ey = ys.GetEnumerator();
            bool xHasNext;
            bool yHasNext;

            while ((xHasNext = ex.MoveNext()) | (yHasNext = ey.MoveNext()))
            {
                if (xHasNext != yHasNext)
                {
                    throw new InvalidOperationException("Enumerables are of different length");
                }

                yield return f(ex.Current, ey.Current);
            }
        }

        /// <summary>
        /// Sames as the standard <see cref="Zip{A, B}"/>, but
        /// raises exception if sequences are not of the same length.
        /// </summary>
        public static IEnumerable<(A, B)> ZipExact<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys) => xs.ZipExact(ys, TupleOf);

        /// <summary>
        /// Returns a sequence of items paired with their index in the original sequence.
        /// Example: <c>[A, B, C] => [(0, A), (1, B), (2, C)]</c>
        /// </summary>
        public static IEnumerable<(int, A)> ZipWithIndex<A>(this IEnumerable<A> seq) => seq.Select((x, i) => (i, x));

        /// <summary>
        /// Returns the cross product of two sequences, combining elements with the given function.
        /// Example: <c>[1, 2, 3], [4, 5], (*) => [4, 5, 8, 10, 12, 15]</c>
        /// </summary>
        public static IEnumerable<C> CrossJoin<A, B, C>(this IEnumerable<A> xs, IEnumerable<B> ys, Func<A, B, C> f) =>
            xs.SelectMany(x => ys.Select(y => f(x, y)));

        /// <summary>
        /// Returns the cross product of two sequences, combining elements into tuples.
        /// Example: <c>[A, B, C], [1, 2] => [(A, 1), (A, 2), (B, 1), (B, 2), (C, 1), (C, 2)]</c>
        /// </summary>
        public static IEnumerable<(A, B)> CrossJoin<A, B>(this IEnumerable<A> xs, IEnumerable<B> ys) =>
            xs.CrossJoin(ys, TupleOf);

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
            seq.Where((_, i) => !i.IsIn(indicies));

        /// <summary>
        /// Filters out <c>null</c> values.
        /// </summary>
        public static IEnumerable<A> Sift<A>(this IEnumerable<A> seq) =>
            seq.Where(x => x != null);

        /// <summary>
        /// Filters out elements for which the given selector returns <c>null</c>.
        /// </summary>
        public static IEnumerable<A> Sift<A, B>(this IEnumerable<A> seq, Func<A, B> f) =>
            seq.Where(x => f(x) != null);

        /// <summary>
        /// Randomizes elements in sequence. This will enumerate the entire sequence.
        /// Example: <c>[1, 2, 3, 4, 5] => [3, 5, 2, 1, 4]</c>
        /// </summary>
        public static IEnumerable<A> Shuffle<A>(this IEnumerable<A> seq, Random rand = null)
        {
            rand ??= new Random();
            var values = seq.ToArray();

            for (var i = 0; i < values.Length; ++i)
            {
                var j = rand.Next(i, values.Length);
                yield return values[j];
                values[j] = values[i];
            }
        }

        /// <summary>
        /// Splits sequence into <c>n</c> sub-sequences, each containing every <c>n</c>th element.
        /// Example: <c>[1, 2, 3, 4, 5, 6], 2 => [[1, 3, 5], [2, 4, 6]]</c>
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Deal<A>(this IEnumerable<A> seq, int n)
        {
            var count = 0L;
            var etor = new Lazy<IEnumerator<A>>(seq.GetEnumerator);
            var queues = Enumerable.Range(0, n).Select(_ => new Queue<A>()).ToList();

            IEnumerable<A> TakeEveryNth(int offset)
            {
                var queue = queues[offset % n];

                while (true)
                {
                    if (queue.Count > 0)
                    {
                        yield return queue.Dequeue();
                    }
                    else if (etor.Value.MoveNext())
                    {
                        if (count % n == offset)
                        {
                            count++;
                            yield return etor.Value.Current;
                        }
                        else
                        {
                            queues[(int) (count % n)].Enqueue(etor.Value.Current);
                            count++;
                        }
                    }
                    else
                    {
                        yield break;
                    }
                }
            }

            return Enumerable.Range(0, n).Select(TakeEveryNth);
        }

        /// <summary>
        /// Performs side-effecting Action on each item in sequence and then yield it.
        /// Like <see cref="ForEach{A}"/>, but lazy and yields the values of the input sequence.
        /// Example: <c>...Where(Filter).Tap(LogValue).Select(Transform).</c>
        /// </summary>
        public static IEnumerable<A> Tap<A>(this IEnumerable<A> seq, Action<A> f) =>
            seq.Select(x => x.With(f));

        /// <summary>
        /// Performs side-effecting Action on each item in sequence.
        /// Like <see cref="Tap{A}"/>, but eager and returns void.
        /// </summary>
        public static void ForEach<A>(this IEnumerable<A> seq, Action<A> f) =>
            seq.Tap(f).DoAll();
    }
}
