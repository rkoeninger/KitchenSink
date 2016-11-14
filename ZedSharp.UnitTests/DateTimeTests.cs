using System;
using NUnit.Framework;

namespace ZedSharp.UnitTests
{
    [TestFixture]
    public class DateTimeTests
    {
        [Test]
        public void DateRangeTests()
        {
            Assert.IsTrue(DateTimeRange.GetEntireMonth(2000, Month.April).Contains(new DateTime(2000, 4, 5, 13, 24, 53)));
            Assert.IsTrue(DateTimeRange.GetEntireYear(2005).Contains(DateTimeRange.GetEntireMonth(2005, Month.July)));
            Assert.AreEqual(new DateTime(2000, 4, 10), DateTimeRange.Parse("2000-4-10 to 2000-5-10").Begin);
        }
    }
}
