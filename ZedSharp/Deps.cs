using System;
using System.Collections.Generic;

namespace ZedSharp
{
    /// <summary>
    /// Simple dependency injection container.
    /// Glorified dictionary.
    /// Mutable and not thread safe.
    /// </summary>
    public class DepsBuilder
    {
        public DepsBuilder()
        {
            Values = new Dictionary<Type, Object>();
        }

        private Dictionary<Type, Object> Values { get; set; }

        public DepsBuilder Add<T>(T val)
        {
            Values.Add(typeof(T), val);
            return this;
        }

        public Deps End()
        {
            try
            {
                return new Deps(Values);
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
        internal Deps(Dictionary<Type, Object> values) : this()
        {
            Values = Ref.Of(values);
        }

        private Ref<Dictionary<Type, Object>> Values { get; set; }

        public Maybe<T> Get<T>()
        {
            return Values.ToMaybe().SelectMany(x => x.MaybeGet(typeof(T))).Cast<T>();
        }

        public T GetOrThrow<T>(String item)
        {
            return Get<T>().OrThrow("No dependency registered for " + item);
        }

        public T GetOrThrow<T>()
        {
            return GetOrThrow<T>("type " + typeof(T).Name);
        }
    }
}
