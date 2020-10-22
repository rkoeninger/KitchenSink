using KitchenSink.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class MathExtensions
    {
        /// <summary>
        /// Returns true if argument is a finite number.
        /// </summary>
        public static bool IsReal(this double x) => Not(x.IsNotReal());

        /// <summary>
        /// Returns true if argument is NaN or infinite.
        /// </summary>
        public static bool IsNotReal(this double x) => double.IsInfinity(x) || double.IsNaN(x);

        /// <summary>
        /// Returns true if argument is evenly divisible by 2.
        /// </summary>
        public static bool IsEven(this int x) => (x & 1) == 0;

        /// <summary>
        /// Returns true if argument is not evenly divisible by 2.
        /// </summary>
        public static bool IsOdd(this int x) => (x & 1) != 0;

        /// <summary>
        /// Returns true if <c>(x / y) * y</c> would equal <c>x</c>.
        /// </summary>
        public static bool IsDivisibleBy(this int x, int y) => x % y == 0;

        /// <summary>
        /// Returns true if <c>(x / y) * y</c> would not equal <c>x</c>.
        /// </summary>
        public static bool IsNotDivisibleBy(this int x, int y) => x % y != 0;

        /// <summary>
        /// Returns first value, but no less than the second.
        /// </summary>
        public static A NoLessThan<A>(this A x, A min) where A : IComparable<A> => x.CompareTo(min) < 0 ? min : x;

        /// <summary>
        /// Returns first value, but no more than the second.
        /// </summary>
        public static A NoMoreThan<A>(this A x, A max) where A : IComparable<A> => x.CompareTo(max) > 0 ? max : x;

        /// <summary>
        /// Returns first value, but no less than the second.
        /// </summary>
        public static Func<A, A> NoLessThan<A>(this A x) where A : IComparable<A> => min => x.NoLessThan(min);

        /// <summary>
        /// Returns first value, but no more than the second.
        /// </summary>
        public static Func<A, A> NoMoreThan<A>(this A x) where A : IComparable<A> => max => x.NoMoreThan(max);

        /// <summary>
        /// Returns greater value, defaulting to the first.
        /// </summary>
        public static A Max<A>(this IComparer<A> comparer, A x, A y) => comparer.Compare(x, y) > 0 ? x : y;

        /// <summary>
        /// Returns lesser value, defaulting to the first.
        /// </summary>
        public static A Min<A>(this IComparer<A> comparer, A x, A y) => comparer.Compare(x, y) < 0 ? x : y;

        /// <summary>
        /// Returns greater value, defaulting to the first.
        /// </summary>
        public static A Max<A>(this A x, A y) where A : IComparable<A> => Comparer<A>.Default.Max(x, y);

        /// <summary>
        /// Returns lesser value, defaulting to the first.
        /// </summary>
        public static A Min<A>(this A x, A y) where A : IComparable<A> => Comparer<A>.Default.Min(x, y);

        /// <summary>
        /// Inclusive on start value, exclusive on end value.
        /// </summary>
        public static IReadOnlyList<int> To(this int start, int end) =>
            new ComputedList<int>(end - start, i => i + start);

        /// <summary>
        /// Inclusive on start and end value.
        /// </summary>
        public static IReadOnlyList<int> ToIncluding(this int start, int end) =>
            new ComputedList<int>(end - start + 1, i => i + start);

        /// <summary>
        /// Computes the factorial of n.
        /// <code>
        ///     n! = n * (n - 1) * (n - 2) * ... * 2 * 1
        /// </code>
        /// </summary>
        public static int Factorial(this int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Factorial not valid on integers less than 0");
            }

            if (n == 0 || n == 1)
            {
                return 1;
            }

            var result = 2;

            for (var i = 3; i <= n; ++i)
            {
                result *= i;
            }

            return result;
        }

        /// <summary>
        /// Computes the number of permutations of r elements from a set of n elements.
        /// Equivalent to Factorial when <c>n = r</c>.
        /// <code>
        ///     nPr = n! / (n - r)!
        /// </code>
        /// </summary>
        public static int PermutationCount(this int n, int r)
        {
            if (n < 0)
            {
                throw new ArgumentException("PermutationCount not valid on negative set sizes (n)");
            }

            if (r < 0)
            {
                throw new ArgumentException("PermutationCount not valid on negative set sizes (r)");
            }

            if (r > n)
            {
                throw new ArgumentException("Permutations not valid on take sizes greater than set sizes");
            }

            if (r == 0)
            {
                return 1;
            }

            if (n == r)
            {
                return Factorial(n);
            }

            var result = 1;

            for (var i = n; i > n - r; --i)
            {
                result *= i;
            }

            return result;
        }

        /// <summary>
        /// Computes the number of combinations of r elements from a set of n elements.
        /// <code>
        ///     nCr = n! / (r! * (n - r)!)
        /// </code>
        /// </summary>
        public static int CombinationCount(this int n, int r)
        {
            if (n < 0)
            {
                throw new ArgumentException("CombinationCount not valid on negative set sizes (n)");
            }

            if (r < 0)
            {
                throw new ArgumentException("CombinationCount not valid on negative set sizes (r)");
            }

            if (r > n)
            {
                throw new ArgumentException("CombinationCount not valid on take sizes greater than set sizes");
            }

            if (r == 0 || n == 0 || n == r)
            {
                return 1;
            }

            r = Math.Min(r, n - r);

            if (r == 1)
            {
                return n;
            }

            var result = 1;

            for (var i = n; i > n - r; --i)
            {
                result *= i;
            }

            for (var i = r; i > 1; --i)
            {
                result /= i;
            }

            return result;
        }

        /// <summary>
        /// Returns a sequence of all permutations of the input sequence.
        /// Length of returned sequence is equal to <c>seq.Count().Factorial()</c>.
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Permutations<A>(this IEnumerable<A> seq) =>
            RenderPermutations(seq, null);

        /// <summary>
        /// Returns a sequence of all permutations of given length subsets of the input sequence.
        /// Length of returned sequence is equal to <c>seq.Count().PermutationCount(r)</c>.
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Permutations<A>(this IEnumerable<A> seq, int r) =>
            RenderPermutations(seq, r);

        private static IEnumerable<IConsList<A>> RenderPermutations<A>(IEnumerable<A> seq, int? rd)
        {
            var array = seq.ToArray();
            var len = array.Length;
            var r = rd ?? array.Length;

            if (r > len)
            {
                throw new ArgumentException("Can't take subsequence longer than entire set");
            }

            if (r == 0 || len == 0)
            {
                yield break;
            }

            if (r == 1)
            {
                foreach (var item in array)
                {
                    yield return ConsList.Empty<A>().Cons(item);
                }

                yield break;
            }

            foreach (var i in Enumerable.Range(0, array.Length))
            {
                var sublist = array.ExceptAt(i);

                foreach (var subseq in RenderPermutations(sublist, r - 1))
                {
                    yield return subseq.Cons(array[i]);
                }
            }
        }

        /// <summary>
        /// Returns a sequence of all combinations of given length subsets of the input sequence.
        /// Length of returned sequence is equal to <c>seq.Count().CombinationCount(r)</c>.
        /// </summary>
        public static IEnumerable<IEnumerable<A>> Combinations<A>(this IEnumerable<A> seq, int r)
        {
            if (r < 0)
            {
                throw new ArgumentException();
            }

            var array = seq.ToArray();

            if (array.Length == r)
            {
                yield return array;
                yield break;
            }

            if (array.Length <= 64)
            {
                foreach (var flags in RenderFlags64(array.Length, r))
                {
                    yield return array.Where((_, i) => ((flags >> i) & 1ul) != 0);
                }
            }
            else
            {
                foreach (var flags in RenderFlagsN(array.Length, r))
                {
                    yield return array.ZipTuples(flags).Where(x => x.Item2).Select(x => x.Item1);
                }
            }
        }

        private static IEnumerable<ulong> RenderFlags64(int n, int r)
        {
            if (n == 0 && r == 0)
            {
                yield return 0;
                yield break;
            }

            if (n < r)
            {
                yield break;
            }

            if (r > 0)
            {
                foreach (var flags in RenderFlags64(n - 1, r - 1))
                {
                    yield return flags | (1ul << (n - 1));
                }
            }

            foreach (var flags in RenderFlags64(n - 1, r))
            {
                yield return flags;
            }
        }

        private static IEnumerable<IConsList<bool>> RenderFlagsN(int n, int r)
        {
            if (n == 0 && r == 0)
            {
                yield return ConsList.Empty<bool>();
                yield break;
            }

            if (n < r)
            {
                yield break;
            }

            if (r > 0)
            {
                foreach (var flags in RenderFlagsN(n - 1, r - 1))
                {
                    yield return flags.Cons(true);
                }
            }

            foreach (var flags in RenderFlagsN(n - 1, r))
            {
                yield return flags.Cons(false);
            }
        }
    }
}
