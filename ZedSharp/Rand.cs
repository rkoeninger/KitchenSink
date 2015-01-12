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

        public static int Int(int max)
        {
            return Global.Next(max);
        }

        public static int Int(int min, int max)
        {
            return Global.Next(min, max);
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

        public static String UnicodeString()
        {
            return Chars().Take(Int(256)).Concat();
        }

        public static String UnicodeString(int length)
        {
            return Chars().Take(Int(length)).Concat();
        }

        public static IEnumerable<String> UnicodeStrings()
        {
            return Seq.Forever(UnicodeString);
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
            return AsciiChars().Take(Int(256)).Concat();
        }

        public static String AsciiString(int length)
        {
            return AsciiChars().Take(Int(length)).Concat();
        }

        public static String AsciiStringNoWhiteSpace(int minLength, int maxLength)
        {
            return Chars().Where(x => ! char.IsWhiteSpace(x)).Take(Int(minLength, maxLength)).Concat();
        }

        public static IEnumerable<String> AsciiStrings()
        {
            return Seq.Forever(AsciiString);
        }

        public static String Email()
        {
            return String.Format("{0}@{1}.{2}",
                AsciiStringNoWhiteSpace(16, 32),
                AsciiStringNoWhiteSpace(8, 16),
                Pick(Sample.TopLevelDomains));
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

        public static A Pick<A>(IEnumerable<A> seq)
        {
            return Global.Pick(seq);
        }

        public static A Pick<A>(this Random rand, params A[] vals)
        {
            return vals[rand.Next(vals.Length)];
        }

        public static A Pick<A>(this Random rand, IEnumerable<A> seq)
        {
            return rand.Pick(seq.ToArray());
        }
    }
}
