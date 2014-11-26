using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Nullable
    {
        public static Nullable<B> Select<A, B>(this Nullable<A> na, Func<A, B> f) where A : struct where B : struct
        {
            return na.HasValue ? f(na.Value) : (Nullable<B>)null;
        }

        public static Nullable<B> SelectMany<A, B>(this Nullable<A> na, Func<A, Nullable<B>> f) where A : struct where B : struct
        {
            return na.HasValue ? f(na.Value) : (Nullable<B>)null;
        }

        public static Nullable<A> Where<A>(this Nullable<A> na, Func<A, bool> f) where A : struct
        {
            return na.HasValue && f(na.Value) ? na : (Nullable<A>)null;
        }

        public static void ForEach<A>(this Nullable<A> x, Action<A> f) where A : struct
        {
            if (x.HasValue)
                f(x.Value);
        }
    }
}
