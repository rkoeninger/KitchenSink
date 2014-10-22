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
            return seq.OrderBy(Funcs.Id);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq) where A : IComparable
        {
            return seq.OrderByDescending(Funcs.Id);
        }

        public static IEnumerable<A> Sort<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderBy(Funcs.Id, comp);
        }

        public static IEnumerable<A> SortDesc<A>(this IEnumerable<A> seq, IComparer<A> comp)
        {
            return seq.OrderByDescending(Funcs.Id, comp);
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

        public static bool Not(this bool x)
        {
            return !x;
        }
    }
}
