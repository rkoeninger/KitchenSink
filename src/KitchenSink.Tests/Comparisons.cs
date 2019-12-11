using NUnit.Framework;
using static KitchenSink.Comparison;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    public class Comparisons
    {
        [Test]
        public void CompareOperator()
        {
            Assert.AreEqual(Gt, Compare(6, -1));
            Assert.AreEqual(Lt, Compare(-6, -1));
            Assert.AreEqual(Comparison.Eq, Compare(6, 6));
        }

        [Test]
        public void RangeOperators()
        {
            Assert.IsTrue(0 < Cmp(3) <= 5);
            Assert.IsFalse(0 < Cmp(3) >= 5);
            Assert.IsTrue(10 > Cmp(5) < 9);
        }
    }
}
