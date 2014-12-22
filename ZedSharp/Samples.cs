using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Samples
    {
        public static readonly int[] Ints = new []
        {
            0,
            1, 2, 3,
            9, 10, 11,
            99, 100, 101,
            255, 256, 257,
            999, 1000, 1001,
            9999, 10000, 10001,
            65535, 65536, 65537,
            int.MaxValue - 2, int.MaxValue - 1, int.MaxValue,
            -1, -2, -3,
            -9, -10, -11,
            -99, -100, -101,
            -255, -256, -257,
            -999, -1000, -1001,
            -9999, -10000, -10001,
            -65535, -65536, -65537,
            int.MinValue + 2, int.MinValue + 1, int.MinValue
        };

        public static readonly IEnumerable<int> RandomInts = Numbers.RandomInts();

        public static readonly double[] Doubles = new []
        {
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            0.0,
            double.Epsilon, double.Epsilon * 2, double.Epsilon * 3,
            double.MaxValue, double.MaxValue - double.Epsilon, double.MaxValue - (double.Epsilon * 2),
            -double.Epsilon, -double.Epsilon * 2, -double.Epsilon * 3,
            double.MinValue, double.MinValue + double.Epsilon, double.MinValue + (double.Epsilon * 2)
        };

        public static readonly IEnumerable<double> RandomDoubles = Numbers.RandomDoubles();
    }
}
