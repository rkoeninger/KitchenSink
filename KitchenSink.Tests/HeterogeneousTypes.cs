using System;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class HeterogeneousTypes
    {
        [TestFixture]
        public class HLists
        {
            [Test]
            public void ValuesCanBeRetrieved()
            {
                var h3 = HNil.It.Cons(1).Cons("a").Cons(DateTime.Now);
                Assert.IsTrue("a" == HList.Get(default(string), h3));
            }
        }

        [TestFixture]
        public class HSets
        {
            [Test]
            public void SubsetsCanBeExtracted()
            {
                var source = Tuple.Create(true, DateTime.MinValue, 1, "a");
                Assert.AreEqual(Tuple.Create(true, 1), HSet.Pick(source, Sig.Of<bool, int>()));
            }
        }
    }
}
