using System;

namespace KitchenSink.Extensions
{
    public static class ObjectExtensions
    {
        public static A NonNull<A>(this A x) where A : class =>
            x ?? throw new NullReferenceException();
    }
}
