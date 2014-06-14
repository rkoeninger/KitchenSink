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
        public static Dictionary<A, B> New<A, B>(params Object[] pairs)
        {
            if (pairs.Length % 2 != 0)
                throw new ArgumentException("Argument list must have even number of values");

            var dict = new Dictionary<A, B>();

            for (int i = 0; i < pairs.Length; ++i)
                dict.Add((A) pairs[i], (B) pairs[i + 1]);

            return dict;
        }

        public static Dictionary<String, Object> New(params Object[] pairs)
        {
            if (pairs.Length % 2 != 0)
                throw new ArgumentException("Argument list must have even number of values");

            var dict = new Dictionary<String, Object>();

            for (int i = 0; i < pairs.Length; i += 2)
                dict.Add(pairs[i].ToString(), pairs[i + 1]);

            return dict;
        }

        public static Dictionary<String, Object> New(Object obj)
        {
            if (obj == null)
                return new Dictionary<String, Object>();

            return obj.GetType().GetProperties().Where(x => x.GetIndexParameters().Length == 0).ToDictionary(
                x => x.Name, x => x.GetValue(obj, null));
        }

        public static Dictionary<String, A> New<A>(params Expression<Func<Object, A>>[] exprs)
        {
            return exprs.ToDictionary(x => x.Parameters.First().Name, x => x.Compile().Invoke(null));
        }
    }
}
