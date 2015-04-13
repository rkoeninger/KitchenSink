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
                Date.On(2001, 4, 13),
                Date.On(1947, 1, 20),
                Date.On(2030, 11, 9),
                Date.On(1994, 9, 16),
                Date.On(2023, 8, 27)));

            Check.EqualsAndHashCode(Seq.Forever(() => Rand.Ints().Take(Rand.Int(64))).Take(16));
        }

        [TestMethod]
        public void CheckComparableAndOperators()
        {
            Check.Comparable(Sample.Ints);
            Check.CompareOperators(Sample.Ints);
        }

        [TestMethod]
        public void Idempotence()
        {
            Check.Idempotent(Sample.Ints.Where(x => x != int.MinValue), Math.Abs);
            Expect.FailedAssert(() => Check.Idempotent(Z.Inc));
        }
    }
}
