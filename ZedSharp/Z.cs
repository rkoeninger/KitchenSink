using System;
using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class Z
    {
        public static readonly Func<int, bool> Even = x => x.Even();

        public static readonly Func<int, int, int> AddI = (x, y) => x + y;

        public static Func<int, int> Add(this int i)
        {
            return x => x + i;
        }

        public static readonly Func<Object, Type> Type = x => x.GetType();
    }
}
