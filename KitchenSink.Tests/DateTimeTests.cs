using System;
using NUnit.Framework;
using KitchenSink.Timekeeping;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class DateTimeTests
    {
        [Test]
        public void DateRangeTests()
        {
            Assert.IsTrue(DateSpan.EntireMonth(2000, 4).Contains(new DateTime(2000, 4, 5, 13, 24, 53)));
            Assert.IsTrue(DateSpan.EntireYear(2005).Contains(DateSpan.EntireMonth(2005, 7)));
            Assert.AreEqual(new DateTime(2000, 4, 10), DateSpan.Parse("2000-4-10 to 2000-5-10").Begin);
        }
    }
}
