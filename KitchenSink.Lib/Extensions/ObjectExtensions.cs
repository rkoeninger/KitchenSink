using System;

namespace KitchenSink.Extensions
{
    public static class ObjectExtensions
    {
        public static A NonNull<A>(this A x) where A : class =>
            x ?? throw new NullReferenceException();

        public static A With<A>(this A x, Action<A> f)
        {
            f(x);
            return x;
        }

        public static A Copy<A>(this A x, Action<A> f) where A : ICloneable
        {
            var copy = (A) x.Clone();
            f(copy);
            return copy;
        }
    }
}
