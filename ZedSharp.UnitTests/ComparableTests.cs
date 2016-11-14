using NUnit.Framework;

namespace ZedSharp.UnitTests
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
    }
}
