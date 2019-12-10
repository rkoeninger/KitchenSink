using System;
using System.Collections.Generic;
using static KitchenSink.Operators;

namespace KitchenSink.Extensions
{
    public static class ObjectExtensions
    {
        public static A NonNull<A>(this A x) where A : class =>
            x ?? throw new NullReferenceException();

        public static A With<A>(this A x, Action<A> f)
        {
            f(x);
            return x;
        }

        public static A Copy<A>(this A x, Action<A> f) => Clone(x).With(f);

        public static B Use<A, B>(this A resource, Func<A, B> f) where A : IDisposable
        {
            using (resource)
            {
                return f(resource);
            }
        }

        public static void Use<A>(this A resource, Action<A> f) where A : IDisposable
        {
            using (resource)
            {
                f(resource);
            }
        }

        // TODO: use System.Runtime.CompilerServices.ITuple here

        public static IEnumerable<object> AsEnumerable<A, B>(
            this (A, B) t) => SeqOf<object>(
            t.Item1,
            t.Item2);

        public static IEnumerable<object> AsEnumerable<A, B, C>(
            this (A, B, C) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3);

        public static IEnumerable<object> AsEnumerable<A, B, C, D>(
            this (A, B, C, D) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3,
            t.Item4);

        public static IEnumerable<object> AsEnumerable<A, B, C, D, E>(
            this (A, B, C, D, E) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3,
            t.Item4,
            t.Item5);

        public static IEnumerable<object> AsEnumerable<A, B, C, D, E, F>(
            this (A, B, C, D, E, F) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3,
            t.Item4,
            t.Item5,
            t.Item6);

        public static IEnumerable<object> AsEnumerable<A, B, C, D, E, F, G>(
            this (A, B, C, D, E, F, G) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3,
            t.Item4,
            t.Item5,
            t.Item6,
            t.Item7);

        public static IEnumerable<object> AsEnumerable<A, B, C, D, E, F, G, H>(
            this (A, B, C, D, E, F, G, H) t) => SeqOf<object>(
            t.Item1,
            t.Item2,
            t.Item3,
            t.Item4,
            t.Item5,
            t.Item6,
            t.Item7,
            t.Item8);
    }
}
