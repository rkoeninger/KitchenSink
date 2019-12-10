using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// A Regex that identifies contiguous sequences of whitespace.
        /// </summary>
        public static readonly Regex WhiteSpaceRegex = new Regex("\\s+");

        /// <summary>
        /// Trims string and replaces sequences of whitespace with a single space.
        /// </summary>
        public static string CollapseSpace(this string s) => WhiteSpaceRegex.Replace(s.Trim(), " ");

        /// <summary>
        /// Returns string with words captialized.
        /// </summary>
        public static string ToTitleCase(this string x) => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x);

        /// <summary>
        /// Converts "MultiPartName" to "multi-part-name". Also handles "camelCase".
        /// </summary>
        public static string ToLispCase(this string s) =>
            EnumerateCamelPascalCaseTerms(s?.Trim()).Select(x => x.ToLower()).MkStr("-");

        /// <summary>
        /// Converts "MultiPartName" to "multi_part_name". Also handles "camelCase".
        /// </summary>
        public static string ToSnakeCase(this string s) =>
            EnumerateCamelPascalCaseTerms(s?.Trim()).Select(x => x.ToLower()).MkStr("_");

        /// <summary>
        /// Converts "multi-part-name" to "MultiPartName". Also handles "snake_case" with custom sep.
        /// </summary>
        public static string ToPascalCase(this string s, char sep = '-') =>
            (s ?? "").Split(ArrayOf(sep), StringSplitOptions.RemoveEmptyEntries)
                .Select(ss => ss.ToLower().ToTitleCase())
                .MkStr();

        /// <summary>
        /// Converts "multi-part-name" to "multiPartName". Also handles "snake_case" with custom sep.
        /// </summary>
        public static string ToCamelCase(this string s, char sep = '-') =>
            (s ?? "").Split(ArrayOf(sep), StringSplitOptions.RemoveEmptyEntries)
            .Select((ss, i) => i == 0 ? ss.ToLower() : ss.ToLower().ToTitleCase())
            .MkStr();

        private static IEnumerable<string> EnumerateCamelPascalCaseTerms(string s)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                for (int i = 0, j = 1; j < s.Length; ++j)
                {
                    var hasNext = j + 1 < s.Length;
                    var prevIsUpper = char.IsUpper(s[j - 1]);
                    var currentIsUpper = char.IsUpper(s[j]);
                    var nextIsUpper = hasNext && char.IsUpper(s[j + 1]);
                    var nextIsDigit = hasNext && char.IsDigit(s[j + 1]);

                    if (hasNext)
                    {
                        if (currentIsUpper && !prevIsUpper)
                        {
                            yield return s.Substring(i, j - i);
                            i = j;
                        }
                        else if (prevIsUpper && currentIsUpper && !nextIsUpper && !nextIsDigit)
                        {
                            yield return s.Substring(i, j - i);
                            i = j;
                        }
                    }
                    else
                    {
                        yield return s.Substring(i, j - i + 1);
                        i = j;
                    }
                }
            }
        }

        /// <summary>
        /// Formats decimal value as currency.
        /// </summary>
        public static string ToCurrencyString(this decimal x) => $"{x:c}";

        /// <summary>
        /// Attempts parse of string to int.
        /// </summary>
        public static Maybe<int> ToInt(this string s) => int.TryParse(s, out var i) ? Some(i) : None<int>();

        /// <summary>
        /// Attempts parse of string to double.
        /// </summary>
        public static Maybe<double> ToDouble(this string s) => double.TryParse(s, out var d) ? Some(d) : None<double>();

        /// <summary>
        /// Attempts parse of string to enum.
        /// </summary>
        public static Maybe<A> ToEnumMaybe<A>(this string s) where A : struct => Enum.TryParse(s, out A x) ? Some(x) : None<A>();

        /// <summary>
        /// Attempts parse of string to enum.
        /// </summary>
        public static A ToEnum<A>(this string s) where A : struct => s.ToEnumMaybe<A>().OrElseThrow($"Not a value of enum {typeof(A)}");

        /// <summary>
        /// Converts items in sequence to string and concats them
        /// separated by <c>sep</c>, which defaults to empty string.
        /// </summary>
        public static string MkStr<A>(this IEnumerable<A> seq, string sep = "") => string.Join(sep, seq);

        /// <summary>
        /// Transforms each item in sequence, converts it to a string,
        /// and concats them separated by <c>sep</c>, which defaults
        /// to empty string.
        /// </summary>
        public static string MkStr<A, B>(this IEnumerable<A> seq, string sep, Func<A, B> f) =>
            seq.Select(f).MkStr(sep);

        /// <summary>
        /// Transforms each item in sequence, converts it to a string,
        /// and concats them separated by <c>sep</c>, which defaults
        /// to empty string.
        /// </summary>
        public static string MkStr<A, B>(this IEnumerable<A> seq, Func<A, B> f) =>
            seq.Select(f).MkStr();

        /// <summary>
        /// Transforms each item in sequence, converts it to a string,
        /// and concats them separated by <c>sep</c>, which defaults
        /// to empty string.
        /// </summary>
        public static string MkStr<A>(this IEnumerable<A> seq, string sep, Func<A, object> f) =>
            seq.Select(f).MkStr(sep);

        /// <summary>
        /// Transforms each item in sequence, converts it to a string,
        /// and concats them separated by <c>sep</c>, which defaults
        /// to empty string.
        /// </summary>
        public static string MkStr<A>(this IEnumerable<A> seq, Func<A, object> f) =>
            seq.Select(f).MkStr();

        /// <summary>
        /// Converts sequence to character-separated string, using quotes
        /// to escape values containing the separator (comma by default).
        /// </summary>
        public static string ToCsv<A>(this IEnumerable<A> seq, string sep = ",") =>
            seq
                .Select(x =>
                {
                    var s = Str(x);
                    return s.Contains(sep) ? $"\"{s.Replace("\"", "\"\"")}\"" : s;
                })
                .MkStr(sep);

        /// <summary>
        /// Converts string from Windows-style CRLF to Unix-style LF.
        /// </summary>
        public static string ToLF(this string s) => s.Replace("\r\n", "\n");

        /// <summary>
        /// Converts string from Unix-style LF to Windows-style CRLF.
        /// </summary>
        public static string ToCRLF(this string s) => s.Replace("\r\n", "\n").Replace("\n", "\r\n");

        /// <summary>
        /// Returns true if two strings are equal ignoring case.
        /// </summary>
        public static bool IsSimilar(this string x, string y) => string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);

        /// <summary>
        /// Splits a string according to given Regex.
        /// </summary>
        public static IEnumerable<string> Split(this string s, Regex r)
        {
            var m = r.Match(s);

            while (m.Success)
            {
                yield return m.Value;
                m = m.NextMatch();
            }
        }

        /// <summary>
        /// Splits a string according to given separator and optional
        /// StringComparison method.
        /// </summary>
        public static IEnumerable<string> Split(
            this string s,
            string sep,
            StringComparison comparison = StringComparison.InvariantCulture)
        {
            var i = 0;

            for (int j; (j = s.IndexOf(sep, i, comparison)) >= 0; i = j + sep.Length)
            {
                yield return s.Substring(i, j - i);
            }

            yield return s.Substring(i, s.Length - i);
        }
    }
}
