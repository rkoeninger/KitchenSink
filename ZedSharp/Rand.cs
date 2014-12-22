using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Rand
    {
        public static readonly Random Global = new Random();

        public static int Int()
        {
            return Global.Next();
        }

        public static IEnumerable<int> Ints()
        {
            return Seq.Forever(Int);
        }

        public static double Double()
        {
            return Global.NextDouble();
        }

        public static IEnumerable<double> Doubles()
        {
            return Seq.Forever(Double);
        }

        public static char Char()
        {
            return (char) Global.Next();
        }

        public static IEnumerable<char> Chars()
        {
            return Seq.Forever(Char);
        }

        public static String String()
        {
            return Chars().Take(Int()).Concat();
        }

        public static IEnumerable<String> Strings()
        {
            return Seq.Forever(String);
        }

        public static char AsciiChar()
        {
            return (char) Global.Next(128);
        }

        public static IEnumerable<char> AsciiChars()
        {
            return Seq.Forever(AsciiChar);
        }

        public static String AsciiString()
        {
            return AsciiChars().Take(Int()).Concat();
        }

        public static IEnumerable<String> AsciiStrings()
        {
            return Seq.Forever(AsciiString);
        }

        public static String Email()
        {
            return AsciiString() + "@" + AsciiString() + "." + Pick(Sample.TopLevelDomains);
        }

        public static IEnumerable<String> Emails()
        {
            return Seq.Forever(Email);
        }

        public static bool Bool()
        {
            return Global.NextBoolean();
        }

        public static IEnumerable<bool> Bools()
        {
            return Seq.Forever(Bool);
        }

        public static bool NextBoolean(this Random rand)
        {
            return rand.Next(1) == 0;
        }

        public static A Pick<A>(params A[] vals)
        {
            return Global.Pick(vals);
        }

        public static A Pick<A>(Blob<A> blob)
        {
            return Global.Pick(blob);
        }

        public static A Pick<A>(IEnumerable<A> seq)
        {
            return Global.Pick(seq);
        }

        public static A Pick<A>(this Random rand, params A[] vals)
        {
            return vals[rand.Next(vals.Length)];
        }

        public static A Pick<A>(this Random rand, Blob<A> blob)
        {
            return blob[rand.Next(blob.Length)];
        }

        public static A Pick<A>(this Random rand, IEnumerable<A> seq)
        {
            return rand.Pick(seq.ToArray());
        }
    }
}
