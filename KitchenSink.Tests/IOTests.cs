using NUnit.Framework;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class IOTests
    {
        [Test]
        public void IOChaining()
        {
            Assert.AreEqual(1, IO.Of(1).Eval());
            Assert.AreEqual(2, IO.Of(1).Then(IO.Of(2)).Eval());
            Assert.AreEqual(3, IO.Of(1).Join(IO.Of(2), Add).Eval());
        }

        [Test]
        public void IOTransforms()
        {
            Assert.AreEqual(3, IO.Demote(IO.Of(() => Add.Apply(1))).Invoke(2).Eval());
        }
    }
}
