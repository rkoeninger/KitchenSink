using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Funcs
    {
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
    }
}
