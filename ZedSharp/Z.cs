using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RegexMatch = System.Text.RegularExpressions.Match;

namespace ZedSharp
{
    public static class Z
    {
        public static IEnumerable<String> SplitSeq(this String s, Regex r)
        {
            RegexMatch m = r.Match(s);

            while (m.Success)
            {
                yield return m.Value;
                m = m.NextMatch();
            }
        }

        public static IEnumerable<String> SplitSeq(this String s, String sep)
        {
            int i = 0;
            int j = 0;

            while ((j = s.IndexOf(sep, i)).NonNeg())
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

        public static bool NonEmpty(this String x)
        {
            return String.IsNullOrEmpty(x).Not();
        }

        public static bool NonBlank(this String x)
        {
            return String.IsNullOrWhiteSpace(x).Not();
        }

        public static IEnumerable<String> TrimAll(this IEnumerable<String> seq)
        {
            return seq.Select(x => x == null ? null : x.Trim()).Where(NonBlank);
        }

        public static String StringJoin(this IEnumerable<Object> seq, String sep = null)
        {
            return String.Join(sep ?? "", seq);
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderBy(Id);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderByDescending(Id);
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderBy(Id, comp);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderByDescending(Id, comp);
        }

        public static IEnumerable<A> Seq<A>(params A[] vals)
        {
            return vals;
        }

        public static A[] Array<A>(params A[] vals)
        {
            return vals;
        }

        public static List<A> List<A>(params A[] vals)
        {
            return new List<A>(vals);
        }

        public static A[] Add<A>(this A[] array, params A[] vals)
        {
            var result = new A[array.Length + vals.Length];
            array.CopyTo(result, 0);
            vals.CopyTo(result, array.Length);
            return result;
        }

        public static List<A> Add<A>(this List<A> list, params A[] vals)
        {
            var result = new List<A>(list);
            list.AddRange(vals);
            return result;
        }

        public static bool NonEmpty<A>(this List<A> list)
        {
            return list.Count > 0;
        }

        public static bool NonEmpty<A>(this IEnumerable<A> seq)
        {
            return seq.Count() > 0;
        }

        public static Func<A, B> AsFunc<A, B>(this IDictionary<A, B> dict)
        {
            return x => dict[x];
        }

        public static Func<int, A> AsFunc<A>(this IList<A> list)
        {
            return x => list[x];
        }

        public static Func<A, bool> AsFunc<A>(this ISet<A> set)
        {
            return x => set.Contains(x);
        }

        public static Func<A, B> F<A, B>(Func<A, B> f)
        {
            return f;
        }

        public static A Id<A>(A x)
        {
            return x;
        }

        public static Func<Object, A> Const<A>(A x)
        {
            return _ => x;
        }

        public static bool Pos(this int x)
        {
            return x > 0;
        }

        public static bool Zero(this int x)
        {
            return x == 0;
        }

        public static bool NonNeg(this int x)
        {
            return x >= 0;
        }

        public static bool Neg(this int x)
        {
            return x < 0;
        }

        public static bool Neg1(this int x)
        {
            return x == -1;
        }

        public static bool Not(this bool x)
        {
            return !x;
        }

        public static Func<A, bool> NotF<A>(this Func<A, bool> f)
        {
            return x => !f(x);
        }

        public static Func<A, bool> OrF<A>(params Func<A, bool>[] fs)
        {
            return x => fs.Any(f => f(x));
        }

        public static Func<A, bool> AndF<A>(params Func<A, bool>[] fs)
        {
            return x => fs.All(f => f(x));
        }

        public static Func<A, B, bool> OrF_<A, B>(Func<A, bool> fa, Func<B, bool> fb)
        {
            return (a, b) => fa(a) || fb(b);
        }

        public static Func<A, B, C, bool> OrF_<A, B, C>(Func<A, bool> fa, Func<B, bool> fb, Func<C, bool> fc)
        {
            return (a, b, c) => fa(a) || fb(b) || fc(c);
        }

        public static Func<A, B, bool> AndF_<A, B>(Func<A, bool> fa, Func<B, bool> fb)
        {
            return (a, b) => fa(a) && fb(b);
        }

        public static Func<A, B, C, bool> AndF_<A, B, C>(Func<A, bool> fa, Func<B, bool> fb, Func<C, bool> fc)
        {
            return (a, b, c) => fa(a) && fb(b) && fc(c);
        }
    }
}
