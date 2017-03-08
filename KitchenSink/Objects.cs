using System;
using System.Collections.Generic;
using System.Linq;
using static KitchenSink.Operators;

namespace KitchenSink
{
    // TODO: move into Operators/Extensions
    public static class Objects
    {
        public static A To<A>(this IConvertible obj)
        {
            return (A)Convert.ChangeType(obj, typeof(A));
        }

        public static bool IsIn<A>(this A val, params A[] vals)
        {
            return IsIn(val, (ICollection<A>) vals);
        }

        public static bool IsIn<A>(this A val, ICollection<A> coll)
        {
            return coll.Any(x => Eq(x, val));
        }

        public static bool IsNotIn<A>(this A val, params A[] vals)
        {
            return ! IsIn(val, (ICollection<A>) vals);
        }

        public static bool IsNotIn<A>(this A val, ICollection<A> coll)
        {
            return ! IsIn(val, coll);
        }
    }
}
