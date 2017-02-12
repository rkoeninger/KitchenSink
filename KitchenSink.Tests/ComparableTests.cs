using NUnit.Framework;

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
        public void RangeOperators()
        {
            Assert.IsTrue(Z.Cmp- 0 < 3 <= 5);
            Assert.IsFalse(Z.Cmp- 0 < 3 >= 5);
            Assert.IsTrue(Z.Cmp- 10 > 5 < 9);
        }
    }
}
