using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using KitchenSink.Injection;

namespace KitchenSink
{
// TODO: make dynamic vars follow stack
    /*
if scopes has entry for current thread
    then if scope stack for current thread is not empty
         then if top stack frame can resolve key
              then return value
              else recur this process for rest of stack
          else raise error
    else if thread has a parent thread
         then recur this process for parent thread
         else raise error
     */

    public class StackUnderflowException : Exception
    {
    }

    public class Scope
    {
        private static readonly ConcurrentDictionary<Thread, Stack<DynamicScope>> scopes =
            new ConcurrentDictionary<Thread, Stack<DynamicScope>>();

        /// <summary>
        /// Gets the current dynamic scope for this thread.
        /// </summary>
        public static DynamicScope Me
        {
            get
            {
                var stack = scopes.GetOrAdd(Thread.CurrentThread, _ => new Stack<DynamicScope>());

                if (stack.Count == 0)
                {
                    throw new StackUnderflowException();
                }

                return stack.Peek();
            }
        }

        /// <summary>
        /// Registers dependency in current dynamic scope.
        /// </summary>
        public static DynamicScope Add<T>(T impl) => Me.Add(impl);

        /// <summary>
        /// Resolves registered dependency <c>T</c> in current dynamic scope.
        /// </summary>
        public static T Get<T>() => Me.Get<T>();
    }

    public class DynamicScope
    {
        private readonly Needs needs = new Needs();

        /// <summary>
        /// Registers dependency.
        /// </summary>
        public DynamicScope Add<T>(T impl)
        {
            needs.Add(impl);
            return this;
        }

        /// <summary>
        /// Resolves registered dependency <c>T</c> in this scope.
        /// </summary>
        public T Get<T>() => needs.Get<T>();
    }
}
