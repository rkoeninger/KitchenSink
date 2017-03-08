using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    // TODO: move into Operators/Extensions
    public static class Strings
    {
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
    }
}
