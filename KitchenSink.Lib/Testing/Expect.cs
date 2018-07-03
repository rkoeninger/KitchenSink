using System;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Testing
{
    public class ExpectationFailedException : Exception
    {
        public ExpectationFailedException(string message) : base(message) { }
    }

    public class ExceptionExpectationException : ExpectationFailedException
    {
        public ExceptionExpectationException(Type type) : base(type.Name + " expected") { }
    }

    public class SomeExpectedException : ExpectationFailedException
    {
        public SomeExpectedException() : base("Maybe was supposed to have a value") { }

        public SomeExpectedException(object expected, object actual)
            : base($"{actual} is not the expected {Some(expected)}") { }
    }

    public class NoneExpectedException : ExpectationFailedException
    {
        public NoneExpectedException(object val) : base("Maybe was not supposed to have a value, but does: " + val) { }
    }

    public class PropertyRefutedException : ExpectationFailedException
    {
        public PropertyRefutedException(params object[] vals) : base($"Property refuted with ({vals.MakeString(", ")})") { }
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
        public static Exception Error(Action f, Exception toThrow = null) => Error<Exception>(f, toThrow);

        /// <summary>
        /// Catches exception thrown by <code>f</code> and returns it.
        /// Throws exception if none thrown by <code>f</code>.
        /// </summary>
        public static E Error<E>(Action f, Exception toThrow = null) where E : Exception =>
            Either.Try<E>(f).OrElseThrow(() => new ExceptionExpectationException(typeof(E)));

        public static ExpectationFailedException FailedAssert(Action f) => Error<ExpectationFailedException>(f);

        /// <summary>
        /// Asserts that the actual maybe has a value and the value is equal to the expected value.
        /// </summary>
        public static void IsSome<A>(A expected, Maybe<A> actual)
        {
            if (Some(expected) != actual)
            {
                throw new SomeExpectedException(expected, actual);
            }
        }

        /// <summary>
        /// Asserts that the given maybe has a value.
        /// </summary>
        public static void IsSome<A>(Maybe<A> maybe) =>
            maybe.OrElseThrow<SomeExpectedException>();

        /// <summary>
        /// Asserts that the given maybe does not have a value.
        /// </summary>
        public static void IsNone<A>(Maybe<A> maybe) =>
            maybe.Reverse().OrElseThrow(new NoneExpectedException(maybe));
    }
}
