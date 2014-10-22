using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Seq
    {
        public static IEnumerable<A> Of<A>(params A[] vals)
        {
            return vals;
        }
    }
}
