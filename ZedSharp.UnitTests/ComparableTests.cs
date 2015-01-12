using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class ComparableTests
    {
        [TestMethod]
        public void IsBetween()
        {
            Assert.IsTrue(5.IsBetween(3, 10));
            Assert.IsTrue(5.IsNotBetween(1, 5));
            Assert.IsTrue(5.IsBetween(5, 13));
        }
    }
}
