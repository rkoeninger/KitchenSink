using System;

namespace ZedSharp
{
    public static class Nullable
    {
        public static B? Select<A, B>(this A? na, Func<A, B> f) where A : struct where B : struct
        {
            return na.HasValue ? f(na.Value) : (B?)null;
        }

        public static B? SelectMany<A, B>(this A? na, Func<A, B?> f) where A : struct where B : struct
        {
            return na.HasValue ? f(na.Value) : null;
        }

        public static A? Where<A>(this A? na, Func<A, bool> f) where A : struct
        {
            return na.HasValue && f(na.Value) ? na : null;
        }

        public static void ForEach<A>(this A? x, Action<A> f) where A : struct
        {
            if (x.HasValue)
                f(x.Value);
        }
    }
}
