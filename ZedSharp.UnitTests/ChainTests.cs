using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class ChainTests
    {
        private static IEnumerable<Chain<int>> Chains()
        {
            yield return default(Chain<int>);
            yield return Chain.Of<int>();

            foreach (var len in Seq.Forever(() => Rand.Int(32)).Take(32))
                yield return Chain.Of(Rand.Ints().Take(len));
        }

        [TestMethod]
        public void ChainProperties()
        {
            Check.EqualsAndHashCode(Chains());
            Check.ReflexiveEquality(Chains());
            Check.That((x, y) => Equals(x, y).Implies(Chain.Compare(x, y) == 0), Chains(), Chains());
            Check.That((x, y) => Chain.Compare(y, x) == -Chain.Compare(x, y), Chains(), Chains());
        }
    }
}
