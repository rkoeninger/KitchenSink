using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    /// <summary>
    /// Mutable dependency collection builder.
    /// </summary>
    public class DepsBuilder
    {
        public DepsBuilder()
        {
            Values = new List<Object>();
        }

        private List<Object> Values { get; set; }

        public DepsBuilder Add(Object val)
        {
            Values.Add(val);
            return this;
        }

        public Deps End()
        {
            try
            {
                return Deps.Of(Values);
            }
            finally
            {
                Values = null;
            }
        }
    }

    /// <summary>
    /// Immutable dependency dictionary.
    /// </summary>
    public struct Deps
    {
        public static Deps Of(params Object[] values)
        {
            return new Deps(values);
        }

        private Deps(IEnumerable<Object> values) : this()
        {
            Values = values.ToArray();
        }

        private Object[] Values { get; set; }

        public Maybe<T> Get<T>() where T : class
        {
            return Maybe.Of(Values.FirstOrDefault(x => x is T) as T);
        }

        public T GetOrThrow<T>(String item) where T : class
        {
            return Get<T>().OrElseThrow("No dependency registered for " + item);
        }

        public T GetOrThrow<T>() where T : class
        {
            return GetOrThrow<T>("type " + typeof(T).Name);
        }
    }
}
