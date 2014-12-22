using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Numbers
    {
        public static bool IsReal(this double x)
        {
            return ! (Double.IsInfinity(x) || Double.IsNaN(x));
        }

        public static bool IsNotReal(this double x)
        {
            return Double.IsInfinity(x) || Double.IsNaN(x);
        }

        public static bool IsEven(this int x)
        {
            return x % 2 == 0;
        }

        public static bool IsOdd(this int x)
        {
            return x % 2 != 0;
        }

        public static Func<int, int> Plus(this int i)
        {
            return x => x + i;
        }

        public static Func<int, int> Times(this int i)
        {
            return x => x * i;
        }

        public static bool IsDivisibleBy(this int x, int y)
        {
            return x % y == 0;
        }

        public static bool IsNotDivisibleBy(this int x, int y)
        {
            return x % y != 0;
        }

        public static IEnumerable<int> To(this int start, int end)
        {
            return Enumerable.Range(start, end - start);
        }

        public static bool CoinFlip(Random rand = null)
        {
            return (rand ?? new Random()).NextBoolean();
        }

        public static bool NextBoolean(this Random rand)
        {
            return rand.Next(1) == 0;
        }

        public static A Pick<A>(this Random rand, IEnumerable<A> seq)
        {
            return seq.ElementAt(rand.Next(seq.Count()));
        }

        public static IEnumerable<int> RandomInts(int max = Int32.MaxValue)
        {
            var rand = new Random();

            while (true)
            {
                yield return rand.Next(max);
            }
        }

        public static IEnumerable<double> RandomDoubles()
        {
            var rand = new Random();

            while (true)
            {
                yield return rand.NextDouble();
            }
        }

        public static int Factorial(this int n)
        {
            if (n < 0)
                throw new ArgumentException("Factorial not valid on integers less than 0");

            if (n == 0 || n == 1)
                return 1;

            int result = 2;

            for (var i = 3; i <= n; ++i)
                result *= i;

            return result;
        }

        public static int Permutations(this int n, int r)
        {
            if (n < 0)
                throw new ArgumentException("Permutations not valid on negative set sizes (n)");
            if (r < 0)
                throw new ArgumentException("Permutations not valid on negative set sizes (r)");
            if (r > n)
                throw new ArgumentException("Permutations not valid on take sizes greater than set sizes");
            if (r == 0)
                return 1;
            if (n == r)
                return Factorial(n);

            int result = 1;

            for (var i = n - r + 1; i <= n; ++i)
                result *= i;

            return result;
        }

        public static IEnumerable<IEnumerable<A>> Permutations<A>(this IEnumerable<A> list, int r)
        {
            var len = list.Count();

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
                foreach (var item in list)
                {
                    yield return Seq.Of(item);
                }

                yield break;
            }

            foreach (var i in list.Indicies())
            {
                var sublist = list.WithoutAt(i);

                foreach (var subseq in Permutations(sublist, r - 1))
                {
                    yield return Seq.Of(list.ElementAt(i)).Concat(subseq);
                }
            }
        }

        public static int Combinations(this int n, int r)
        {
            if (n < 0)
                throw new ArgumentException("Combinations not valid on negative set sizes (n)");
            if (r < 0)
                throw new ArgumentException("Combinations not valid on negative set sizes (r)");
            if (r > n)
                throw new ArgumentException("Combinations not valid on take sizes greater than set sizes");
            if (r == 0 || n == r)
                return 1;

            int result = 1;

            for (var i = n - r + 1; i <= n; ++i)
                result *= i;

            for (var i = 2; i <= r; ++i)
                result /= i;

            return result;
        }

        public static IEnumerable<IEnumerable<A>> Combinations<A>(this IEnumerable<A> seq, int r)
        {
            if (seq == null)
                throw new ArgumentNullException();
            if (r < 0)
                throw new ArgumentException();

            return CombHelper(seq.Count(), r).Select(ZipWhere(seq));
        }

        private static IEnumerable<IEnumerable<bool>> CombHelper(int n, int r)
        {
            if (n == 0 && r == 0)
            {
                yield return EmptyBoolSeq;
                yield break;
            }
            if (n < r)
            {
                yield break;
            }

            if (r > 0)
            {
                foreach (var subGroup in CombHelper(n - 1, r - 1))
                {
                    yield return OneTrue.Concat(subGroup);
                }
            }

            foreach (var subGroup in CombHelper(n - 1, r))
            {
                yield return OneFalse.Concat(subGroup);
            }
        }

        private static readonly IEnumerable<bool> OneTrue = Seq.Of(true);
        private static readonly IEnumerable<bool> OneFalse = Seq.Of(false);
        private static readonly IEnumerable<bool> EmptyBoolSeq = Seq.Of<bool>();

        private static Func<IEnumerable<bool>, IEnumerable<A>> ZipWhere<A>(IEnumerable<A> seq)
        {
            return selectors => seq.Zip(selectors, Row.Of).Where(x => x.Item2).Select(x => x.Item1);
        }
    }
}
