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
    }
}
