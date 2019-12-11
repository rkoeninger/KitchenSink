using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink
{
    public static class Scope
    {
        private static readonly ThreadLocal<DynamicScope> Scopes = new ThreadLocal<DynamicScope>();

        /// <summary>
        /// Gets the current dynamic scope for this thread.
        /// </summary>
        public static DynamicScope Me =>
            Scopes.IsValueCreated
                ? Scopes.Value
                : (Scopes.Value = new DynamicScope());

        /// <summary>
        /// Pushes value onto this thread's dynamic stack.
        /// </summary>
        public static IDisposable Push(string key, object value) => Me.Push(key, value);

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope.
        /// </summary>
        public static object Get(string key) => Me.Get(key);

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope.
        /// </summary>
        public static Maybe<object> GetMaybe(string key) => Me.GetMaybe(key);

        /// <summary>
        /// Pushes value onto this thread's dynamic stack, named after its type.
        /// </summary>
        public static IDisposable Push<TValue>(TValue value) => Me.Push(value);

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope by its type.
        /// </summary>
        public static TValue Get<TValue>() => Me.Get<TValue>();

        /// <summary>
        /// Resolves registered value in this thread's dynamic scope by its type.
        /// </summary>
        public static Maybe<TValue> GetMaybe<TValue>() => Me.GetMaybe<TValue>();
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
            var stack = index.AddOrUpdate(key, New(value), Existing(value));
            var size = stack.Count;

            return Disposable(() =>
            {
                if (stack.Count != size)
                {
                    throw new DynamicScopeStackException(size, stack.Count);
                }

                stack.Pop();
            });
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
        /// Resolves registered value in this scope.
        /// </summary>
        public Maybe<object> GetMaybe(string key)
        {
            var stack = index.GetOrAdd(key, _ => new Stack<object>());
            return stack.Count > 0 ? Some(stack.Peek()) : None<object>();
        }

        /// <summary>
        /// Pushes value onto stack, named after its type.
        /// </summary>
        public IDisposable Push<TValue>(TValue value) => Push(typeof(TValue).FullName, value);

        /// <summary>
        /// Resolves registered value in this scope by its type.
        /// </summary>
        public TValue Get<TValue>() => (TValue) Get(typeof(TValue).FullName);

        /// <summary>
        /// Resolves registered value in this scope by its type.
        /// </summary>
        public Maybe<TValue> GetMaybe<TValue>() => GetMaybe(typeof(TValue).FullName).OfType<TValue>();

        private static Func<string, Stack<object>, Stack<object>> Existing(object value) =>
            (key, stack) => stack.With(s => s.Push(value));

        private static Func<string, Stack<object>> New(object value) => key =>
            Existing(value)(key, new Stack<object>());
    }
}
