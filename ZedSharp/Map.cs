using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Map
    {
        public static Dictionary<A, B> Of<A, B>(params Row<A, B>[] pairs)
        {
            return pairs.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public static Dictionary<A, B> Of<A, B>(params Tuple<A, B>[] pairs)
        {
            return pairs.ToDictionary(x => x.Item1, x => x.Item2);
        }

        public static Dictionary<A, B> Of<A, B>(params Object[] pairs)
        {
            if (pairs.Length % 2 != 0)
                throw new ArgumentException("Argument list must have even number of values");

            var dict = new Dictionary<A, B>();

            for (int i = 0; i < pairs.Length; ++i)
                dict.Add((A) pairs[i], (B) pairs[i + 1]);

            return dict;
        }

        public static Dictionary<String, Object> Of(params Object[] pairs)
        {
            if (pairs.Length % 2 != 0)
                throw new ArgumentException("Argument list must have even number of values");

            var dict = new Dictionary<String, Object>();

            for (int i = 0; i < pairs.Length; i += 2)
                dict.Add(pairs[i].ToString(), pairs[i + 1]);

            return dict;
        }

        public static Dictionary<String, Object> Of(Object obj)
        {
            if (obj == null)
                return new Dictionary<String, Object>();

            return obj.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0).ToDictionary(
                x => x.Name, x => x.GetValue(obj, null));
        }

        public static Dictionary<String, A> Of<A>(params Expression<Func<Object, A>>[] exprs)
        {
            return exprs.ToDictionary(x => x.Parameters.First().Name, x => x.Compile().Invoke(null));
        }
    }
}
