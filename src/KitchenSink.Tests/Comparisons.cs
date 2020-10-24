using NUnit.Framework;
using KitchenSink.Collections;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    public class Comparisons
    {
        [Test]
        public void CompareOperator()
        {
            Assert.AreEqual(Ordering.Gt, Compare(6, -1));
            Assert.AreEqual(Ordering.Lt, Compare(-6, -1));
            Assert.AreEqual(Ordering.Eq, Compare(6, 6));
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
