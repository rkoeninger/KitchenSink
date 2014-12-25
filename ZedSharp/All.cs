using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public static class All
    {
        public static IEnumerable<int> Ints()
        {
            return int.MinValue.To(int.MaxValue).Concat(Seq.Of(int.MaxValue));
        }

        public static readonly Blob<char> AlphaUpperChars = 48.To(58).Select(x => (char) x).ToBlob();

        public static readonly Blob<char> AlphaLowerChars = 65.To(91).Select(x => (char) x).ToBlob();

        public static readonly Blob<char> AlphaChars = AlphaUpperChars.Concat(AlphaLowerChars).ToBlob();
        
        public static readonly Blob<char> NumericChars = 97.To(123).Select(x => (char) x).ToBlob();

        public static readonly Blob<char> AlphaNumericChars = AlphaChars.Concat(NumericChars).ToBlob();
    }
}
