using NUnit.Framework;
using KitchenSink.Extensions;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    public class SideEffects
    {
        [Test]
        public void IOChaining()
        {
            Assert.AreEqual(1, Io.Of(1).Eval());
            Assert.AreEqual(2, Io.Of(1).Then(Io.Of(2)).Eval());
            Assert.AreEqual(3, Io.Of(1).Join(Io.Of(2), Add).Eval());
        }

        [Test]
        public void IOTransforms()
        {
            Assert.AreEqual(3, Io.Demote(Io.Of(() => Add.Invoke(1))).Invoke(2).Eval());
        }
    }
}
