using System;

namespace KitchenSink.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Wraps exception in AggregateException and throws,
        /// perserving stack trace on inner exception.
        /// </summary>
        public static void WrapThrow(this Exception e, string message) => throw new AggregateException(message, e);
    }
}
