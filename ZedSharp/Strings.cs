using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    public static class Strings
    {
        public static String ToLF(this String s)
        {
            return s.Replace("\r\n", "\n");
        }

        public static String ToCRLF(this String s)
        {
            return s.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        public static String Format(this String s, params Object[] args)
        {
            return String.Format(s, args);
        }

        public static IEnumerable<String> SplitSeq(this String s, Regex r)
        {
            var m = r.Match(s);

            while (m.Success)
            {
                yield return m.Value;
                m = m.NextMatch();
            }
        }

        public static IEnumerable<String> SplitSeq(this String s, String sep, StringComparison comparison = StringComparison.InvariantCulture)
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

        public static bool EqualsIgnoreCase(this String x, String y)
        {
            return String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public static Func<String, bool> EqualsIgnoreCase(this String x)
        {
            return y => EqualsIgnoreCase(x, y);
        }

        public static bool IsNotBlank(this String x)
        {
            return String.IsNullOrWhiteSpace(x).Not();
        }

        public static String IfEmpty(this String x, String y)
        {
            return String.IsNullOrEmpty(x) ? y : x;
        }

        public static String IfBlank(this String x, String y)
        {
            return String.IsNullOrWhiteSpace(x) ? y : x;
        }

        public static readonly Regex WhiteSpaceRegex = new Regex("\\s+");

        public static String CollapseSpace(this String x)
        {
            return WhiteSpaceRegex.Split(x.Trim()).Concat(" ");
        }

        public static String ToTitleCase(this String x)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x);
        }

        public static IEnumerable<String> TrimAll(this IEnumerable<String> seq)
        {
            return seq.Where(IsNotBlank).Select(x => x.Trim());
        }

        /// <summary>Joins the string representations of the elements in the sequence, separated by the given string (defaults to empty string).</summary>
        public static String Concat<A>(this IEnumerable<A> seq, String sep = null)
        {
            return String.Join(sep ?? "", seq);
        }
    }
}
