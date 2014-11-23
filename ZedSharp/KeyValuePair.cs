using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
