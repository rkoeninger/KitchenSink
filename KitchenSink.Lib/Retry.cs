using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KitchenSink.Extensions;

namespace KitchenSink
{
    public static class Retry
    {
        /// <summary>
        /// Repeatedly attempts operation, doubling the wait time between each successive attempt.
        /// Useful for dealing with momentary connectivity issues.
        /// </summary>
        public static Either<A, Exception> Exponential<A>(
            int count,
            TimeSpan delay,
            Func<A> action,
            Func<Exception, bool> retryableError)
        {
            var n = count;
            var exceptions = new List<Exception>();

            while (n > 0)
            {
                try
                {
                    return action();
                }
                catch (Exception e)
                {
                    if (!retryableError(e))
                    {
                        return e;
                    }

                    exceptions.Add(e);
                    Thread.Sleep(delay);
                    delay = delay + delay;
                }

                n--;
            }

            return new RetryExhaustedException(count, exceptions);
        }

        /// <summary>
        /// Repeatedly attempts operation, doubling the wait time between each successive attempt.
        /// Useful for dealing with momentary connectivity issues.
        /// </summary>
        public static Maybe<Exception> Exponential(
            int count,
            TimeSpan delay,
            Action action,
            Func<Exception, bool> retryableError) =>
            Exponential(count, delay, action.AsFunc(), retryableError).RightMaybe;

        /// <summary>
        /// Recursively subdivides a workload for an operation as attempts fail.
        /// Useful for dealing with timeouts on batch operations.
        /// </summary>
        public static (int, B, Exception) Fractal<A, B>(
            int depth,
            int branchingFactor,
            IReadOnlyList<A> items,
            Func<IReadOnlyList<A>, B> action,
            Func<B, B, B> reducer,
            Func<Exception, bool> retryableError)
        {
            try
            {
                if (items.Count == 0)
                {
                    return (0, default, null);
                }

                return (items.Count, action(items), null);
            }
            catch (Exception e)
            {
                if (!retryableError(e) || depth <= 0)
                {
                    return (0, default, e);
                }

                var batchSize = items.Count / branchingFactor;
                var total = 0;
                var result = default(B);

                foreach (var batch in items.Batch(batchSize))
                {
                    var (count, r0, error) = Fractal(
                        depth - 1,
                        branchingFactor,
                        batch.ToList(),
                        action,
                        reducer,
                        retryableError);
                    total += count;
                    result = reducer(result, r0);

                    if (error != null)
                    {
                        return (total, result, error);
                    }
                }

                return (total, result, null);
            }
        }

        /// <summary>
        /// Recursively subdivides a workload for an operation as attempts fail.
        /// Useful for dealing with timeouts on batch operations.
        /// </summary>
        public static (int, Exception) Fractal<A>(
            int depth,
            int branchingFactor,
            IReadOnlyList<A> items,
            Action<IReadOnlyList<A>> action,
            Func<Exception, bool> retryableError)
        {
            var (count, _, error) = Fractal(
                depth,
                branchingFactor,
                items,
                action.AsFunc(),
                (x, _) => x,
                retryableError);
            return (count, error);
        }

        /// <summary>
        /// Repeatedly attempts the same operation, providing a series of alternate arguments.
        /// Useful for dealing with uncertain or varying details.
        /// </summary>
        public static Either<(A, B), Exception> Sequential<A, B>(
            IReadOnlyList<A> options,
            Func<A, B> action,
            Func<Exception, bool> retryableError)
        {
            var exceptions = new List<Exception>();

            foreach (var option in options)
            {
                try
                {
                    return (option, action(option));
                }
                catch (Exception e)
                {
                    if (!retryableError(e))
                    {
                        return e;
                    }

                    exceptions.Add(e);
                }
            }

            return new RetryExhaustedException(options.Count, exceptions);
        }

        /// <summary>
        /// Repeatedly attempts the same operation, providing a series of alternate arguments.
        /// Useful for dealing with uncertain or varying details.
        /// </summary>
        public static Either<A, Exception> Sequential<A>(
            IReadOnlyList<A> options,
            Action<A> action,
            Func<Exception, bool> retryableError) =>
            Sequential(options, action.AsFunc(), retryableError).Select(x => x.Item1);
    }
}
