using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Funcs
    {
        public static Func<Unit> UnitF(Action f)
        {
            return () => { f(); return Unit.It; };
        }

        public static Action Action(Func<Unit> f)
        {
            return () => f();
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

        public static Func<A, D> Zip<A, B, C, D>(this Func<A, B> f, Func<A, C> g, Func<B, C, D> zipper)
        {
            return x => zipper(f(x), g(x));
        }

        public static Func<A, C, E> Join<A, B, C, D, E>(this Func<A, B> f, Func<C, D> g, Func<B, D, E> zipper)
        {
            return (x, y) => zipper(f(x), g(y));
        }

        public static Func<A, A, C> On<A, B, C>(this Func<B, B, C> combine, Func<A, B> selector)
        {
            return (x, y) => combine(selector(x), selector(y));
        }

        public static Func<A, C> Then<A, B, C>(this Func<A, B> f, Func<B, C> g)
        {
            return g.Compose(f);
        }

        public static Func<A, C> Compose<A, B, C>(this Func<B, C> f, Func<A, B> g)
        {
            return x => f(g(x));
        }

        public static Func<A, R, C> ComposeMany<A, B, C, R>(this Func<A, R, B> f, Func<B, R, C> g)
        {
            return (a, r) => g(f(a, r), r);
        }

        public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> f)
        {
            return a => b => f(a, b);
        }

        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> f)
        {
            return a => b => c => f(a, b, c);
        }

        public static Func<A, Func<B, Func<C, Func<D, E>>>> Curry<A, B, C, D, E>(this Func<A, B, C, D, E> f)
        {
            return a => b => c => d => f(a, b, c, d);
        }
    }
}
