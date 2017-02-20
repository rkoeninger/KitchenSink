using System.Collections.Generic;
using KitchenSink.Collections;

namespace KitchenSink.Testing
{
    public static class Sample
    {
        public static readonly IReadOnlyCollection<bool> Booleans = ReadOnly.Collection(true, false);

        public static readonly IReadOnlyCollection<int> Ints = ReadOnly.Collection(
            0,
            int.MinValue, int.MinValue + 1, int.MinValue + 2,
            int.MaxValue, int.MaxValue - 1, int.MaxValue - 2,
            1, 2, 3,
            -1, -2, -3,
            9, 10, 11,
            -9, -10, -11,
            99, 100, 101,
            -99, -100, -101,
            255, 256, 257,
            -255, -256, -257,
            999, 1000, 1001,
            -999, -1000, -1001,
            9999, 10000, 10001,
            -9999, -10000, -10001,
            65535, 65536, 65537,
            -65535, -65536, -65537
        );

        public static readonly IReadOnlyCollection<uint> UnsignedInts = ReadOnly.Collection<uint>(
            0,
            uint.MaxValue, uint.MaxValue - 1, uint.MaxValue - 2,
            1, 2, 3,
            9, 10, 11,
            99, 100, 101,
            255, 256, 257,
            999, 1000, 1001,
            9999, 10000, 10001,
            65535, 65536, 65537
        );

        public static readonly IReadOnlyCollection<double> Doubles = ReadOnly.Collection(
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            0.0,
            double.MaxValue, double.MaxValue - double.Epsilon, double.MaxValue - (double.Epsilon * 2),
            double.MinValue, double.MinValue + double.Epsilon, double.MinValue + (double.Epsilon * 2),
            double.Epsilon, double.Epsilon * 2, double.Epsilon * 3,
            -double.Epsilon, -double.Epsilon * 2, -double.Epsilon * 3
        );

        public static readonly IReadOnlyCollection<string> TopLevelDomains = ReadOnly.Collection(
            "com", "org", "net", "gov", "edu", "uk", "ca", "jp", "au", "de", "fr", "us"
        );
    }
}