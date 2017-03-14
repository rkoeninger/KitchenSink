using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace KitchenSink
{
    public class Scope
    {
        private static readonly ThreadLocal<DynamicScope> scopes = new ThreadLocal<DynamicScope>();

        /// <summary>
        /// Gets the current dynamic scope for this thread.
        /// </summary>
        public static DynamicScope Me
        {
            get
            {
                if (scopes.IsValueCreated)
                {
                    return scopes.Value;
                }

                var scope = new DynamicScope();
                scopes.Value = scope;
                return scope;
            }
        }

        ///// <summary>
        ///// Registers dependency in current dynamic scope.
        ///// </summary>
        //public static DynamicScope Add<T>(T impl) => Me.Add(impl);

        ///// <summary>
        ///// Resolves registered dependency <c>T</c> in current dynamic scope.
        ///// </summary>
        //public static T Get<T>() => Me.Get<T>();
    }

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

        /// <summary>
        /// Pushes value onto dynamic stack.
        /// </summary>
        public IDisposable Add(string key, object value)
        {
            return new Pop(index.AddOrUpdate(key, NewStack(value), ExistingStack(value)));
        }

        private static Func<string, Stack<object>> NewStack(object value) => key =>
            ExistingStack(value)(key, new Stack<object>());

        private static Func<string, Stack<object>, Stack<object>> ExistingStack(object value) => (key, stack) =>
        {
            stack.Push(value);
            return stack;
        };

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
