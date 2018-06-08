using System;
using NUnit.Framework;
using static KitchenSink.Operators;

namespace KitchenSink.Tests
{
    public class Caching
    {
        [Test]
        public void FunctionMemoization()
        {
            var rand = new Random();
            var f = Memo<string, int>(s => rand.Next());
            Assert.AreEqual(f("a"), f("a"));
            Assert.AreEqual(f("b"), f("b"));
            Assert.AreEqual(f("c"), f("c"));
        }
    }
}
