using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Z
    {

        public static bool EqualsAny(this Object obj, params Object[] vals)
        {
            return vals.Any(x => x == obj);
        }

        public static bool EqualsAny(this Object obj, IEnumerable<Object> vals)
        {
            return vals.Any(x => x == obj);
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

        public static HashSet<A> Set<A>(params A[] vals)
        {
            return new HashSet<A>(vals);
        }

        public static HashSet<A> Set<A>(this IEnumerable<A> seq)
        {
            return new HashSet<A>(seq);
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

        public static bool NotEmpty<A>(this List<A> list)
        {
            return list.Count.Pos();
        }

        public static bool NotEmpty<A>(this IEnumerable<A> seq)
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

        public static bool Null(this Object obj)
        {
            return obj == null;
        }

        public static bool NotNull(this Object obj)
        {
            return obj != null;
        }

        public static Func<A, bool> Eq<A>(this A x)
        {
            return y => Equals(x, y);
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

        public static Func<Unit> UnitF(Action f)
        {
            return () => { f(); return Unit.It; };
        }

        public static Action Action(Func<Unit> f)
        {
            return () => { f(); };
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

        public static Func<A, C> Compose<A, B, C>(this Func<A, B> f, Func<B, C> g)
        {
            return x => g(f(x));
        }

        public static Func<A, R, C> ComposeMany<A, B, C, R>(this Func<A, R, B> f, Func<B, R, C> g)
        {
            return (a, r) => g(f(a, r), r);
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
