using KitchenSink.Extensions;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Enums
    {
        public enum Signal
        {
            Red,
            Yellow,
            Green
        }

        [Test]
        public void Parsing()
        {
            Assert.AreEqual(Signal.Green, "Green".ToEnum<Signal>());
        }
    }
}
