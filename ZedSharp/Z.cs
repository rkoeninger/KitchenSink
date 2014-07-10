using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace ZedSharp
{
    public static class Z
    {
        public static IEnumerable<String> SplitSeq(this String s, Regex r)
        {
            var m = r.Match(s);

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
            return seq.Where(NonBlank).Select(x => x.Trim());
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

        public static IEnumerable<IEnumerable<A>> Partition<A>(this IEnumerable<A> seq, int count)
        {
            while (seq.Count().Pos())
            {
                yield return seq.Take(count);
                seq = seq.Skip(count);
            }
        }

        public static Dictionary<A, B> Map<A, B>(params Row<A, B>[] pairs)
        {
            return pairs.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public static Dictionary<A, B> Map<A, B>(params Tuple<A, B>[] pairs)
        {
            return pairs.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public static Dictionary<A, B> Map<A, B>(params Object[] pairs)
        {
            if (pairs.Length.Even().Not())
                throw new ArgumentException("Argument list must have even number of values");

            return pairs.Partition(2).ToDictionary(x => x.First().As<A>(), x => x.Last().As<B>());
        }

        public static Dictionary<String, Object> Map(params Object[] pairs)
        {
            if (pairs.Length.Even().Not())
                throw new ArgumentException("Argument list must have even number of values");

            return pairs.Partition(2).ToDictionary(x => x.First().ToString(), x => x.Last());
        }

        public static Dictionary<String, Object> Map(Object obj)
        {
            if (obj == null)
                return new Dictionary<String, Object>();

            return obj.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0).ToDictionary(
                x => x.Name, x => x.GetValue(obj, null));
        }

        public static Dictionary<String, A> Map<A>(params Expression<Func<Object, A>>[] exprs)
        {
            return exprs.ToDictionary(x => x.Parameters.First().Name, x => x.Compile().Invoke(null));
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
            return list.Count.Pos();
        }

        public static bool NonEmpty<A>(this IEnumerable<A> seq)
        {
            return seq.Count().Pos();
        }

        public static Func<A, B> AsFunc<A, B>(this IDictionary<A, B> dict)
        {
            return x => dict[x];
        }

        public static Func<int, A> AsFunc<A>(this IList<A> list)
        {
            return list.ElementAt;
        }

        public static Func<A, bool> AsFunc<A>(this ISet<A> set)
        {
            return set.Contains;
        }

        public static bool Is<A>(this Object x)
        {
            return x is A;
        }

        public static Func<Object, bool> Is<A>()
        {
            return x => x is A;
        }

        public static A As<A>(this Object x)
        {
            return (A) x;
        }

        public static Func<Object, A> As<A>()
        {
            return x => (A) x;
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

        public static B Apply<A, B>(this Func<A, B> f, A x)
        {
            return f(x);
        }

        public static Func<B, C> Apply<A, B, C>(this Func<A, B, C> f, A x)
        {
            return y => f(x, y);
        }

        public static Func<B, C, D> Apply<A, B, C, D>(this Func<A, B, C, D> f, A x)
        {
            return (y, z) => f(x, y, z);
        }

        public static Func<B, C, D, E> Apply<A, B, C, D, E>(this Func<A, B, C, D, E> f, A x)
        {
            return (y, z, w) => f(x, y, z, w);
        }

        public static Func<A, C> Apply<A, B, C>(this Func<A, B> f, Func<B, C> g)
        {
            return x => g(f(x));
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

        public static bool Even(this int x)
        {
            return x % 2 == 0;
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
