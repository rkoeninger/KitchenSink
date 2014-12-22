using System.Collections.Generic;

namespace ZedSharp
{
    public static class KeyValuePair
    {
        public static KeyValuePair<A, B> Of<A, B>(A key, B value)
        {
            return new KeyValuePair<A, B>(key, value);
        }
    }
}
