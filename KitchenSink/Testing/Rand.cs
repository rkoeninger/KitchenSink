using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Testing
{
    public static class Rand
    {
        public static readonly Random Global = new Random();

        public static int Int() => Global.Next();

        public static int Int(int max) => Global.Next(max);

        public static int Int(int min, int max) => Global.Next(min, max);

        public static IEnumerable<int> Ints() => Forever(Int);

        public static double Double() => Global.NextDouble();

        public static IEnumerable<double> Doubles() => Forever(Double);

        public static char Char() => (char) Global.Next();

        public static IEnumerable<char> Chars() => Forever(Char);

        public static string UnicodeString() => Chars().Take(Int(256)).MakeString();

        public static string UnicodeString(int length) => Chars().Take(Int(length)).MakeString();

        public static IEnumerable<string> UnicodeStrings() => Forever(UnicodeString);

        public static char AsciiChar() => (char) Global.Next(128);

        public static IEnumerable<char> AsciiChars() => Forever(AsciiChar);

        public static string AsciiString() => AsciiChars().Take(Int(256)).MakeString();

        public static string AsciiString(int length) => AsciiChars().Take(Int(length)).MakeString();

        public static string AsciiStringNoWhiteSpace(int minLength, int maxLength) =>
            Chars().Where(x => ! char.IsWhiteSpace(x)).Take(Int(minLength, maxLength)).MakeString();

        public static IEnumerable<string> AsciiStrings() => Forever(AsciiString);

        public static string Email()
        {
            var name = AsciiStringNoWhiteSpace(16, 32);
            var host = AsciiStringNoWhiteSpace(8, 16);
            var tld = Pick(Sample.TopLevelDomains);
            return $"{name}@{host}.{tld}";
        }

        public static IEnumerable<string> Emails() => Forever(Email);

        public static List<A> List<A>(IEnumerable<A> elements)
        {
            var length = Int(0, 32);
            return elements.Take(length).ToList();
        }

        public static IEnumerable<List<A>> Lists<A>(IEnumerable<A> elements) => Forever(() => List(elements));

        public static bool Bool() => Global.NextBoolean();

        public static IEnumerable<bool> Bools() => Forever(Bool);

        public static bool NextBoolean(this Random rand) => rand.Next(1) == 0;

        public static A Pick<A>(params A[] vals) => Global.Pick(vals);

        public static A Pick<A>(IEnumerable<A> seq) => Global.Pick(seq);

        public static A Pick<A>(this Random rand, params A[] vals) => vals[rand.Next(vals.Length)];

        public static A Pick<A>(this Random rand, IEnumerable<A> seq) => rand.Pick(seq.ToArray());
    }
}
