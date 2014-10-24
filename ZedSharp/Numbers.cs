using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Numbers
    {
        public static bool Pos(this int x)
        {
            return x > 0;
        }

        public static bool Zero(this int x)
        {
            return x == 0;
        }

        public static bool NotNeg(this int x)
        {
            return x >= 0;
        }

        public static bool Neg(this int x)
        {
            return x < 0;
        }

        public static bool Neg1(this int x)
        {
            return x == -1;
        }

        public static bool Even(this int x)
        {
            return x % 2 == 0;
        }

        public static Func<int, int> Plus(this int i)
        {
            return x => x + i;
        }

        public static Func<int, int> Times(this int i)
        {
            return x => x * i;
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
    }
}
