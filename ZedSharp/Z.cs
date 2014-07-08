using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class Z
    {
        public static readonly Func<String, String, bool> EqualsIC = (x, y) => String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);

        public static bool EqualsIgnoreCase(this String x, String y)
        {
            return EqualsIC(x, y);
        }
    }
}
