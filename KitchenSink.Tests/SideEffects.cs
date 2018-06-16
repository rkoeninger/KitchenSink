using NUnit.Framework;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using KitchenSink.Purity;

namespace KitchenSink.Tests
{
    public class SideEffects
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
            Assert.AreEqual(3, IO.Demote(IO.Of(() => Add.Invoke(1))).Invoke(2).Eval());
        }
    }
}
