using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class DateTimeTests
    {
        [TestMethod]
        public void DateRangeTests()
        {
            Assert.IsTrue(DateTimeRange.GetEntireMonth(2000, Month.April).Contains(new DateTime(2000, 4, 5, 13, 24, 53)));
            Assert.IsTrue(DateTimeRange.GetEntireYear(2005).Contains(DateTimeRange.GetEntireMonth(2005, Month.July)));
            Assert.AreEqual(new DateTime(2000, 4, 10), DateTimeRange.Parse("2000-4-10 to 2000-5-10").Begin);
        }

        [TestMethod]
        public void DateBuilderMethods()
        {
            var d = 2014.Apr(20).At(5, 30, 12).In(TimeZoneInfo.Utc);
            var e = 2014.Apr(20).At(5, 30, 12).In(TimeZoneInfo.FindSystemTimeZoneById("Dateline Standard Time"));

            Assert.IsTrue(d < e);
        }
    }
}
