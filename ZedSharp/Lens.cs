using System;

namespace ZedSharp
{
    public static class Lens
    {
        public static Lens<A, B> Of<A, B>(Func<A, B> getter, Func<A, B, A> setter)
        {
            return new Lens<A, B>(getter, setter);
        }
    }

    public struct Lens<A, B>
    {
        internal Lens(Func<A, B> getter, Func<A, B, A> setter) : this()
        {
            Get = getter;
            Set = setter;
        }

        public Func<A, B> Get { get; private set; }
        public Func<A, B, A> Set { get; private set; }

        public Lens<A, C> Compose<C>(Lens<B, C> other)
        {
            var me = this;
            return new Lens<A, C>(
                a => other.Get(me.Get(a)),
                (a, c) => me.Set(a, other.Set(me.Get(a), c)));
        }
    }
}
