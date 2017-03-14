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

        public static IDisposable Push(string key, object value) => Me.Push(key, value);

        public static object Get(string key) => Me.Get(key);
    }

    /// <remarks>
    /// Extensions can be defined on this type for conveinence methods.
    /// </remarks>
    public class DynamicScope
    {
        private readonly ConcurrentDictionary<string, Stack<object>> index =
            new ConcurrentDictionary<string, Stack<object>>();

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

        private static Func<string, Stack<object>, Stack<object>> Existing(object value) => (key, stack) =>
        {
            stack.Push(value);
            return stack;
        };

        private static Func<string, Stack<object>> New(object value) => key =>
            Existing(value)(key, new Stack<object>());

        /// <summary>
        /// Pushes value onto dynamic stack.
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
    }
}
