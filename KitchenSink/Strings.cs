using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace KitchenSink
{
    public static class Strings
    {
        public static string ToLF(this string s)
        {
            return s.Replace("\r\n", "\n");
        }

        public static string ToCRLF(this string s)
        {
            return s.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        public static string Format(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

        public static IEnumerable<string> SplitSeq(this string s, Regex r)
        {
            var m = r.Match(s);

            while (m.Success)
            {
                yield return m.Value;
                m = m.NextMatch();
            }
        }

        public static IEnumerable<string> SplitSeq(this string s, string sep, StringComparison comparison = StringComparison.InvariantCulture)
        {
            var i = 0;
            int j;

            while ((j = s.IndexOf(sep, i, comparison)) >= 0)
            {
                yield return s.Substring(i, j - i);
                i = j + sep.Length;
            }

            yield return s.Substring(i, s.Length - i);
        }

        public static bool EqualsIgnoreCase(this string x, string y)
        {
            return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public static Func<string, bool> EqualsIgnoreCase(this string x)
        {
            return y => EqualsIgnoreCase(x, y);
        }

        public static bool IsNotBlank(this string x)
        {
            return string.IsNullOrWhiteSpace(x).Not();
        }

        public static string IfEmpty(this string x, string y)
        {
            return string.IsNullOrEmpty(x) ? y : x;
        }

        public static string IfBlank(this string x, string y)
        {
            return string.IsNullOrWhiteSpace(x) ? y : x;
        }

        public static readonly Regex WhiteSpaceRegex = new Regex("\\s+");

        public static string CollapseSpace(this string x)
        {
            return WhiteSpaceRegex.Split(x.Trim()).Concat(" ");
        }

        public static string ToTitleCase(this string x)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x);
        }

        public static string ToCurrencyString(this decimal x)
        {
            return string.Format("{0:c}", x);
        }

        public static IEnumerable<string> TrimAll(this IEnumerable<string> seq)
        {
            return seq.Where(IsNotBlank).Select(x => x.Trim());
        }

        /// <summary>Joins the string representations of the elements in the sequence, separated by the given string (defaults to empty string).</summary>
        public static string Concat<A>(this IEnumerable<A> seq, string sep = null)
        {
            return string.Join(sep ?? "", seq);
        }
    }
}
