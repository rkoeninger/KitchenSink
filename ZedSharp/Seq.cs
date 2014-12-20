using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            while (true)
                foreach (var item in seq)
                    yield return item;
        }

        /// <summary>Infinitely repeats item(s).</summary>
        public static IEnumerable<A> Repeat<A>(params A[] vals)
        {
            return vals.Cycle();
        }

        /// <summary>Infinitely enumerates items returned from provided function.</summary>
        public static IEnumerable<A> Forever<A>(Func<A> f)
        {
            while (true)
                yield return f();
        }

        /// <summary>Performs side-effecting Action on each item in sequence.</summary>
        public static void ForEach<A>(this IEnumerable<A> seq, Action<A> f)
        {
            foreach (var item in seq)
                f(item);
        }

        /// <summary>Forces sequence to enumerate.</summary>
        public static void DoAll<A>(this IEnumerable<A> seq)
        {
            foreach (var item in seq);
        }
    }
}
