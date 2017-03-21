using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace KitchenSink
{
    public static class Scope
    {
        private static readonly ThreadLocal<DynamicScope> scopes = new ThreadLocal<DynamicScope>();

        /// <summary>
        /// Gets the current dynamic scope for this thread.
        /// </summary>
        public static DynamicScope Me =>
            scopes.IsValueCreated
                ? scopes.Value
                : (scopes.Value = new DynamicScope());

        /// <summary>
        /// Pushes value onto this thread's dynamic stack.
        /// </summary>
        public static IDisposable Push(string key, object value) => Me.Push(key, value);

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope.
        /// </summary>
        public static object Get(string key) => Me.Get(key);

        /// <summary>
        /// Pushes value onto this thread's dynamic stack, named after its type.
        /// </summary>
        public static IDisposable Push<TValue>(TValue value) => Me.Push(value);

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope by its type.
        /// </summary>
        public static TValue Get<TValue>() => Me.Get<TValue>();
    }

    /// <remarks>
    /// Extensions can be defined on this type for conveinence methods.
    /// </remarks>
    public class DynamicScope
    {
        private readonly ConcurrentDictionary<string, Stack<object>> index =
            new ConcurrentDictionary<string, Stack<object>>();

        /// <summary>
        /// Pushes value onto stack.
        /// </summary>
        public IDisposable Push(string key, object value)
        {
            return new Pop(index.AddOrUpdate(key, New(value), Existing(value)));
        }

        /// <summary>
        /// Resolves registered value in this scope.
        /// </summary>
        public object Get(string key)
        {
            var stack = index.GetOrAdd(key, _ => new Stack<object>());
            return stack.Peek();
        }

        /// <summary>
        /// Pushes value onto stack, named after its type.
        /// </summary>
        public IDisposable Push<TValue>(TValue value) => Push(typeof(TValue).FullName, value);

        /// <summary>
        /// Resolves registered value in this scope by its type.
        /// </summary>
        public TValue Get<TValue>() => (TValue) Get(typeof(TValue).FullName);

        private static Func<string, Stack<object>, Stack<object>> Existing(object value) => (key, stack) =>
        {
            stack.Push(value);
            return stack;
        };

        private static Func<string, Stack<object>> New(object value) => key =>
        {
            var stack = new Stack<object>();
            stack.Push(value);
            return stack;
        };

        private class Pop : IDisposable
        {
            private readonly Stack<object> stack;

            public Pop(Stack<object> stack)
            {
                this.stack = stack;
            }

            public void Dispose()
            {
                stack.Pop();
            }
        }
    }
}
