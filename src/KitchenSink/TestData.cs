using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class All
    {
        public static IEnumerable<int> Ints => int.MinValue.ToIncluding(int.MaxValue);

        public static readonly IReadOnlyCollection<char> AlphaUpperChars = 48.To(58).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaLowerChars = 65.To(91).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaChars = AlphaUpperChars.Concat(AlphaLowerChars).ToArray();

        public static readonly IReadOnlyCollection<char> NumericChars = 97.To(123).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaNumericChars = AlphaChars.Concat(NumericChars).ToArray();

        public static IEnumerable<A> EnumValues<A>() => typeof(A).GetEnumValues().Cast<A>();
    }

    public static class Rand
    {
        public static readonly Random Global = new Random();

        public static int Int() => Global.Next();

        public static int Int(int max) => Global.Next(max);

        public static int Int(int min, int max) => Global.Next(min, max);

        public static IEnumerable<int> Ints() => Forever(Int);

        public static double Double() => Global.NextDouble();

        public static IEnumerable<double> Doubles() => Forever(Double);

        public static char Char() => (char)Global.Next();

        public static IEnumerable<char> Chars() => Forever(Char);

        public static string UnicodeString() => Chars().Take(Int(256)).MkStr();

        public static string UnicodeString(int length) => Chars().Take(Int(length)).MkStr();

        public static IEnumerable<string> UnicodeStrings() => Forever(UnicodeString);

        public static char AsciiChar() => (char)Global.Next(128);

        public static IEnumerable<char> AsciiChars() => Forever(AsciiChar);

        public static string AsciiString() => AsciiChars().Take(Int(256)).MkStr();

        public static string AsciiString(int length) => AsciiChars().Take(Int(length)).MkStr();

        public static string AsciiStringNoWhiteSpace(int minLength, int maxLength) =>
            Chars().Where(x => !char.IsWhiteSpace(x)).Take(Int(minLength, maxLength)).MkStr();

        public static IEnumerable<string> AsciiStrings() => Forever(AsciiString);

        public static string Email()
        {
            var name = AsciiStringNoWhiteSpace(16, 32);
            var host = AsciiStringNoWhiteSpace(8, 16);
            var tld = Pick(Sample.TopLevelDomains);
            return $"{name}@{host}.{tld}";
        }

        public static IEnumerable<string> Emails() => Forever(Email);

        public static List<A> List<A>(IEnumerable<A> elements) => elements.Take(Int(0, 32)).ToList();

        public static IEnumerable<List<A>> Lists<A>(IEnumerable<A> elements) => Forever(() => List(elements));

        public static bool Bool() => Global.NextBoolean();

        public static IEnumerable<bool> Bools() => Forever(Bool);

        public static bool NextBoolean(this Random rand) => rand.Next(1) == 0;

        public static A Pick<A>(params A[] vals) => Global.Pick(vals);

        public static A Pick<A>(IEnumerable<A> seq) => Global.Pick(seq);

        public static A Pick<A>(this Random rand, params A[] vals) => vals[rand.Next(vals.Length)];

        public static A Pick<A>(this Random rand, IEnumerable<A> seq) => rand.Pick(seq.ToArray());
    }

    public static class Sample
    {
        public static readonly IReadOnlyCollection<bool> Booleans = ArrayOf(true, false);

        public static readonly IReadOnlyCollection<int> Ints = ArrayOf(
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

        public static readonly IReadOnlyCollection<uint> UnsignedInts = ArrayOf<uint>(
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

        public static readonly IReadOnlyCollection<double> Doubles = ArrayOf(
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            0.0,
            double.MaxValue, double.MaxValue - double.Epsilon, double.MaxValue - (double.Epsilon * 2),
            double.MinValue, double.MinValue + double.Epsilon, double.MinValue + (double.Epsilon * 2),
            double.Epsilon, double.Epsilon * 2, double.Epsilon * 3,
            -double.Epsilon, -double.Epsilon * 2, -double.Epsilon * 3
        );

        public static readonly IReadOnlyCollection<string> TopLevelDomains = ArrayOf(
            "com", "org", "net", "gov", "edu", "uk", "ca", "jp", "au", "de", "fr", "us"
        );
    }
}
