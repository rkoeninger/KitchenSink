using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;

namespace KitchenSink
{
    public static class Retry
    {
        public static A FixedAttempts<A>(int count, Func<A> f, Func<Exception, bool> p)
        {
            var n = count;
            var exceptions = new List<Exception>();

            while (n > 0)
            {
                try
                {
                    return f();
                }
                catch (Exception e)
                {
                    if (!p(e))
                    {
                        throw;
                    }

                    exceptions.Add(e);
                }

                n--;
            }

            throw new AggregateException($"{nameof(FixedAttempts)} failed after {count} attempts", exceptions);
        }

        public static void FixedAttempts(int count, Action f, Func<Exception, bool> p) =>
            FixedAttempts(count, f.AsFunc(), p);

        public static void FixedAttemptsSubdivide<A>(
            int count,
            List<A> items,
            Action<IReadOnlyList<A>> f,
            Func<Exception, bool> p)
        {

        }

        private static int FixedAttemptsSubdivideRecur<A>(
            int depth,
            IReadOnlyList<A> items,
            Action<IReadOnlyList<A>> f,
            Func<Exception, bool> p)
        {
            try
            {
                f(items);
                return items.Count;
            }
            catch (Exception e)
            {
                if (!p(e))
                {
                    throw;
                }

                var halfLength = items.Count / 2;
                var x = FixedAttemptsSubdivideRecur(depth - 1, items.Take(halfLength).ToList(), f, p);
                var y = FixedAttemptsSubdivideRecur(depth - 1, items.Skip(halfLength).ToList(), f, p);
                return x + y;
            }
        }
    }
}
