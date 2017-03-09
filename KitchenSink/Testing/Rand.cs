﻿using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Collections;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Testing
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
            return Forever(Int);
        }

        public static double Double()
        {
            return Global.NextDouble();
        }

        public static IEnumerable<double> Doubles()
        {
            return Forever(Double);
        }

        public static char Char()
        {
            return (char) Global.Next();
        }

        public static IEnumerable<char> Chars()
        {
            return Forever(Char);
        }

        public static string UnicodeString()
        {
            return Chars().Take(Int(256)).MakeString();
        }

        public static string UnicodeString(int length)
        {
            return Chars().Take(Int(length)).MakeString();
        }

        public static IEnumerable<string> UnicodeStrings()
        {
            return Forever(UnicodeString);
        }

        public static char AsciiChar()
        {
            return (char) Global.Next(128);
        }

        public static IEnumerable<char> AsciiChars()
        {
            return Forever(AsciiChar);
        }

        public static string AsciiString()
        {
            return AsciiChars().Take(Int(256)).MakeString();
        }

        public static string AsciiString(int length)
        {
            return AsciiChars().Take(Int(length)).MakeString();
        }

        public static string AsciiStringNoWhiteSpace(int minLength, int maxLength)
        {
            return Chars().Where(x => ! char.IsWhiteSpace(x)).Take(Int(minLength, maxLength)).MakeString();
        }

        public static IEnumerable<string> AsciiStrings()
        {
            return Forever(AsciiString);
        }

        public static string Email()
        {
            return $"{AsciiStringNoWhiteSpace(16, 32)}@{AsciiStringNoWhiteSpace(8, 16)}.{Pick(Sample.TopLevelDomains)}";
        }

        public static IEnumerable<string> Emails()
        {
            return Forever(Email);
        }

        public static List<A> List<A>(IEnumerable<A> elements)
        {
            var length = Int(0, 32);
            return elements.Take(length).ToList();
        }

        public static IEnumerable<List<A>> Lists<A>(IEnumerable<A> elements)
        {
            return Forever(() => List(elements));
        }

        public static bool Bool()
        {
            return Global.NextBoolean();
        }

        public static IEnumerable<bool> Bools()
        {
            return Forever(Bool);
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
