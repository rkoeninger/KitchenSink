using NUnit.Framework;
using static KitchenSink.Comparison;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class ComparableTests
    {
        [Test]
        public void IsBetween()
        {
            Assert.IsTrue(5.IsBetween(3, 10));
            Assert.IsTrue(5.IsNotBetween(1, 5));
            Assert.IsTrue(5.IsBetween(5, 13));
        }

        [Test]
        public void CompareOperator()
        {
            Assert.AreEqual(GT, Compare(6, -1));
            Assert.AreEqual(LT, Compare(-6, -1));
            Assert.AreEqual(EQ, Compare(6, 6));
        }

        [Test]
        public void RangeOperators()
        {
            Assert.IsTrue(Cmp- 0 < 3 <= 5);
            Assert.IsFalse(Cmp- 0 < 3 >= 5);
            Assert.IsTrue(Cmp- 10 > 5 < 9);
        }
    }
}
