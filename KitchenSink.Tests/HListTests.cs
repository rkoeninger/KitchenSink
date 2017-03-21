using System;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class HListTests
    {
        [Test]
        public void EmptinessCanBeDetermined()
        {
            var h0 = HList.Empty;
            var h1 = HList.Singleton(0);
            var h2 = HList.Singleton(0).Cons("");

            Assert.IsTrue(h0.IsEmpty());
            Assert.IsFalse(h1.IsEmpty());
            Assert.IsFalse(h2.IsEmpty());
        }

        [Test]
        public void LengthCanBeDetermined()
        {
            var h0 = HList.Empty;
            var h1 = HList.Singleton(0);
            var h2 = HList.Singleton(0).Cons("");

            Assert.AreEqual(0, h0.Length());
            Assert.AreEqual(1, h1.Length());
            Assert.AreEqual(2, h2.Length());
        }

        [Test]
        public void ValuesCanBeCheckedForMembership()
        {
            var h3 = HList.Singleton(0).Cons("").Cons(DateTime.Now);
            Assert.IsTrue(h3.Contains(""));
        }
    }
}
