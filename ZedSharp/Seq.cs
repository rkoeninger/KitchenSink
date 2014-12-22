using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Seq
    {
        public static IEnumerable<A> Of<A>(params A[] vals)
        {
            return vals;
        }

        /// <summary>Infinitely enumerates sequence.</summary>
        public static IEnumerable<A> Cycle<A>(this IEnumerable<A> seq)
        {
            var list = new List<A>();

            foreach (var item in seq)
            {
                list.Add(item);
                yield return item;
            }

            while (true)
                foreach (var item in list)
                    yield return item;

            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>Infinitely repeats item(s).</summary>
        public static IEnumerable<A> Cycle<A>(params A[] vals)
        {
            return vals.Cycle();
        }

        /// <summary>Infinitely enumerates items returned from provided function.</summary>
        public static IEnumerable<A> Forever<A>(Func<A> f)
        {
            while (true)
                yield return f();

            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>Performs side-effecting Action on each item in sequence.</summary>
        public static IEnumerable<A> ForEach<A>(this IEnumerable<A> seq, Action<A> f)
        {
            foreach (var item in seq)
            {
                f(item);
                yield return item;
            }
        }

        /// <summary>Forces sequence to enumerate.</summary>
        public static IEnumerable<A> Force<A>(this IEnumerable<A> seq)
        {
            return seq.ToArray();
        }
    }
}
