using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class List
    {
        public static List<A> Of<A>(params A[] vals)
        {
            return new List<A>(vals);
        }

        public static List<A> Add<A>(this List<A> list, params A[] vals)
        {
            var result = new List<A>(list);
            list.AddRange(vals);
            return result;
        }

        public static bool IsEmpty<A>(this IList<A> list)
        {
            return list.Count == 0;
        }

        public static bool IsNotEmpty<A>(this IList<A> list)
        {
            return list.Count > 0;
        }

        public static IEnumerable<int> Indicies<A>(this IList<A> list)
        {
            return Enumerable.Range(0, list.Count);
        }
    }
}
