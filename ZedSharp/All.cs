using System.Collections.Generic;
using System.Linq;

namespace ZedSharp
{
    public static class All
    {
        public static IEnumerable<int> Ints
        {
            get { return int.MinValue.ToIncluding(int.MaxValue); }
        }

        public static readonly IReadOnlyCollection<char> AlphaUpperChars = 48.To(58).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaLowerChars = 65.To(91).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaChars = AlphaUpperChars.Concat(AlphaLowerChars).ToArray();

        public static readonly IReadOnlyCollection<char> NumericChars = 97.To(123).Select(x => (char)x).ToArray();

        public static readonly IReadOnlyCollection<char> AlphaNumericChars = AlphaChars.Concat(NumericChars).ToArray();

        public static IEnumerable<A> EnumValues<A>()
        {
            return typeof(A).GetEnumValues().Cast<A>();
        }
    }
}
