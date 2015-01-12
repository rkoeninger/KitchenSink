using System;
using System.Collections.Generic;

namespace ZedSharp
{
    public static class Sample
    {
        private const int Imax = int.MaxValue;
        private const int Imin = int.MinValue;

        public static readonly IReadOnlyCollection<int> Ints = ReadOnly.Collection(
            0,
            Imax - 2, Imax - 1, Imax,
            Imin + 2, Imin + 1, Imin,
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

        private const double Dep = double.Epsilon;
        private const double Dmax = double.MaxValue;
        private const double Dmin = double.MinValue;

        public static readonly IReadOnlyCollection<double> Doubles = ReadOnly.Collection(
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            0.0,
            Dep, Dep * 2, Dep * 3,
            -Dep, -Dep * 2, -Dep * 3,
            Dmax, Dmax - Dep, Dmax - (Dep * 2),
            Dmin, Dmin + Dep, Dmin + (Dep * 2)
        );

        public static readonly IReadOnlyCollection<String> TopLevelDomains = ReadOnly.Collection(
            "com", "org", "net", "gov", "edu", "uk", "ca", "jp", "au", "de", "fr", "us"
        );
    }
}