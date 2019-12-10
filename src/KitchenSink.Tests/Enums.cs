using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
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

        [Test]
        public void Enumerating()
        {
            Assert.AreEqual(3, ValuesOf<Signal>().Count());
        }
    }
}
