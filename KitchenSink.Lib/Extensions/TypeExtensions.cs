using System;
using System.Reflection;

namespace KitchenSink.Extensions
{
    /// <summary>
    /// Extensions for the type <see cref="Type"/>.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns true if type <c>x</c> can be assigned to type <c>y</c>.
        /// </summary>
        public static bool IsAssignableTo(this Type x, Type y)
        {
            return y.IsAssignableFrom(x);
        }

        /// <summary>
        /// Attempts to get instance of <see cref="Attribute"/> from type declaration.
        /// </summary>
        public static Maybe<A> GetAttribute<A>(this Type type) where A : Attribute
        {
            return type.GetCustomAttribute<A>();
        }
        
        /// <summary>
        /// Attempts to get instance of <see cref="Attribute"/> from type declaration
        /// that satisfies the given predicate.
        /// </summary>
        public static Maybe<A> GetAttribute<A>(this Type type, Func<A, bool> predicate) where A : Attribute
        {
            return type.GetCustomAttributes<A>().FirstMaybe(predicate);
        }

        /// <summary>
        /// Returns true if the attribute <c>A</c> is defined on the given type.
        /// </summary>
        public static bool HasAttribute<A>(this Type type) where A : Attribute
        {
            return type.GetAttribute<A>().HasValue;
        }
        
        /// <summary>
        /// Returns true if the attribute <c>A</c> is defined on the given type
        /// that satisfies the given predicate.
        /// </summary>
        public static bool HasAttribute<A>(this Type type, Func<A, bool> predicate) where A : Attribute
        {
            return type.GetAttribute(predicate).HasValue;
        }
    }
}
