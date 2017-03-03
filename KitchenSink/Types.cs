using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KitchenSink
{
    // TODO: move into Extensions
    public static class Types
    {
        public static bool IsAssignableTo(this Type x, Type y)
        {
            return y.IsAssignableFrom(x);
        }

        public static IEnumerable<Type> All()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
        }

        public static IEnumerable<Type> All(Func<Type, bool> predicate)
        {
            return All().Where(predicate);
        }

        public static Maybe<A> GetAttribute<A>(this Type type) where A : Attribute
        {
            return type.GetCustomAttribute<A>();
        }

        public static Maybe<A> GetAttribute<A>(this Type type, Func<A, bool> predicate) where A : Attribute
        {
            return type.GetCustomAttributes<A>().FirstMaybe(predicate);
        }

        public static bool HasAttribute<A>(this Type type) where A : Attribute
        {
            return type.GetAttribute<A>().HasValue;
        }

        public static bool HasAttribute<A>(this Type type, Func<A, bool> predicate) where A : Attribute
        {
            return type.GetAttribute(predicate).HasValue;
        }
    }
}
