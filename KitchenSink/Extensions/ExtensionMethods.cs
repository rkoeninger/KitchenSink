using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// A Regex that identifies contiguous sequences of whitespace.
        /// </summary>
        public static readonly Regex WhiteSpaceRegex = new Regex("\\s+");

        /// <summary>
        /// Trims string and replaces sequences of whitespace with a single space.
        /// </summary>
        public static string CollapseSpace(this string s)
        {
            return WhiteSpaceRegex.Replace(s.Trim(), " ");
        }

        /// <summary>
        /// Converts items in sequence to string and concats them
        /// separated by <c>sep</c>, which defaults to empty string.
        /// </summary>
        public static string MakeString<A>(this IEnumerable<A> seq, string sep = "")
        {
            return string.Join(sep, seq);
        }

        /// <summary>
        /// Converts sequence to comma-separated string, using quotes
        /// to escape values containing commas.
        /// </summary>
        public static string ToCsv(this IEnumerable<object> seq)
        {
            return seq
                .Select(Str)
                .Select(s => s.Contains(",")
                    ? "\"" + s.Replace("\"", "\"\"") + "\""
                    : s)
                .MakeString(",");
        }

        /// <summary>
        /// Converts string from Windows-style CRLF to Unix-style LF.
        /// </summary>
        public static string ToLF(this string s)
        {
            return s.Replace("\r\n", "\n");
        }

        /// <summary>
        /// Converts string from Unix-style LF to Windows-style CRLF.
        /// </summary>
        public static string ToCRLF(this string s)
        {
            return s.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        /// <summary>
        /// Returns true if two strings are equal ignoring case.
        /// </summary>
        public static bool IsSimilar(this string x, string y)
        {
            return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
