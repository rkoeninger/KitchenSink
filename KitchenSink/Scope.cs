using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace KitchenSink
{
    public class Scope
    {
        private static readonly ConcurrentDictionary<Thread, DynamicScope> scopes =
            new ConcurrentDictionary<Thread, DynamicScope>();

        private static readonly ConcurrentDictionary<Thread, Thread> parents =
            new ConcurrentDictionary<Thread, Thread>();

        /// <summary>
        /// Gets the current dynamic scope for this thread.
        /// </summary>
        public static DynamicScope Me => GetScope(Thread.CurrentThread);

        private static DynamicScope GetScope(Thread thread)
        {
            DynamicScope scope;
            if (scopes.TryGetValue(thread, out scope))
            {
                return scope;
            }

            Thread parent;
            if (parents.TryGetValue(thread, out parent))
            {
                return GetScope(parent);
            }

            throw new InvalidOperationException("Dynamic scope does not exist");
        }

        /// <summary>
        /// Starts a new thread with the current thread as its parent.
        /// </summary>
        public static Thread Fork(Action start)
        {
            var thread = new Thread(new ThreadStart(start));
            parents[thread] = Thread.CurrentThread;
            thread.Start();
            return thread;
        }

        /// <summary>
        /// Registers dependency in current dynamic scope.
        /// </summary>
        //public static DynamicScope Add<T>(T impl) => Me.Add(impl);

        /// <summary>
        /// Resolves registered dependency <c>T</c> in current dynamic scope.
        /// </summary>
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
            return new Pop(index.AddOrUpdate(
                key,
                _ =>
                {
                    var stack = new Stack<object>();
                    stack.Push(value);
                    return stack;
                },
                (_, stack) =>
                {
                    stack.Push(value);
                    return stack;
                }));
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
