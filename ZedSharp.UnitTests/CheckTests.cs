using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class CheckTests
    {
        [TestMethod]
        public void CheckWithDefaultInputs()
        {
            Check.That((int x, int y) => (x == y) == (y == x));
            Check.ReflexiveEquality<int>();
        }

        [TestMethod]
        public void CheckWithProvidedInputs()
        {
            Check.That(
                x => Math.Abs(x) >= 0,
                Sample.Ints.Where(x => x != int.MinValue));

            Check.EqualsAndHashCode(Seq.Of(
                2001.Apr(13),
                1947.Feb(20),
                2030.Nov(9),
                1994.Sep(16),
                2023.Aug(27)));

            Check.EqualsAndHashCode(Seq.Forever(() => Rand.Ints().Take(Rand.Int(64))).Take(16));
        }

        [TestMethod]
        public void CheckComparableAndOperators()
        {
            Check.Comparable(Sample.Ints);
            Check.CompareOperators(Sample.Ints);
        }
    }
}
