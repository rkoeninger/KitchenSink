using System;
using static KitchenSink.Operators;

namespace KitchenSink.Testing
{
    public class ExpectationFailedException : Exception
    {
        public ExpectationFailedException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Expect contains methods that are used to assert failures,
    /// like exception throwing and failed compiliation/type-checking.
    /// 
    /// It is a companion to Assert.
    /// </summary>
    public static class Expect
    {
        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        public static Exception Error(Action f, Exception toThrow = null)
        {
            return Error<Exception>(f, toThrow);
        }

        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        public static E Error<E>(Action f, Exception toThrow = null) where E : Exception
        {
            try
            {
                f();
            }
            catch (E e)
            {
                return e;
            }

            throw toThrow ?? new ExpectationFailedException(typeof(E).Name + " expected");
        }

        public static ExpectationFailedException FailedAssert(Action f)
        {
            return Error<ExpectationFailedException>(f);
        }

        /// <summary>Asserts that the actual maybe has a value and the value is equal to the expected value.</summary>
        public static void IsSome<A>(A expected, Maybe<A> actual)
        {
            if (Some(expected) != actual)
            {
                throw new ExpectationFailedException($"{actual} is not the expected {Some(expected)}");
            }
        }

        /// <summary>Asserts that the given maybe has a value.</summary>
        public static void IsSome<A>(Maybe<A> maybe)
        {
            if (! maybe.HasValue)
            {
                throw new ExpectationFailedException("Maybe was supposed to have a value");
            }
        }

        /// <summary>Asserts that the given maybe does not have a value.</summary>
        public static void IsNone<A>(Maybe<A> maybe)
        {
            if (maybe.HasValue)
            {
                throw new ExpectationFailedException("Maybe was not supposed to have a value, but does: " + maybe);
            }
        }
    }
}
