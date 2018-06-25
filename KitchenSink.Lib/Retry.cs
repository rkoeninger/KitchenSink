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

        public static SubdivisonResult Subdivide<A>(
            int depth,
            int branchingFactor,
            IReadOnlyList<A> items,
            Action<IReadOnlyList<A>> f,
            Func<Exception, bool> p)
        {
            try
            {
                if (items.Count == 0)
                {
                    return SubdivisonResult.Empty();
                }

                f(items);
                return SubdivisonResult.Success(items.Count);
            }
            catch (Exception e)
            {
                if (!p(e) || depth <= 0)
                {
                    return SubdivisonResult.Failure(0, e);
                }

                var batchSize = items.Count / branchingFactor;
                var result = SubdivisonResult.Empty();

                foreach (var batch in items.Batch(batchSize))
                {
                    var currentResult = Subdivide(
                        depth - 1,
                        branchingFactor,
                        batch.ToList(),
                        f,
                        p);
                    result.SuccessCount += currentResult.SuccessCount;

                    if (currentResult.HasError)
                    {
                        result.Error = currentResult.Error;
                        return result;
                    }
                }

                return result;
            }
        }

        public class SubdivisonResult
        {
            public static SubdivisonResult Empty() => Success(0);

            public static SubdivisonResult Success(int count) => Failure(count, null);

            public static SubdivisonResult Failure(int count, Exception error) =>
                new SubdivisonResult
                {
                    SuccessCount = count,
                    Error = error
                };

            public int SuccessCount { get; set; }
            public Exception Error { get; set; }
            public bool HasError => Error != null;
        }
    }
}
