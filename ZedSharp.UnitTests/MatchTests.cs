using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class MatchTests
    {
        [TestMethod]
        public void Tests()
        {
            Assert.IsFalse(Match.On("").Return<int>().End().HasValue);
            Assert.IsTrue(Match.On("").Return<int>().Case("").Then(0).End().HasValue);
        }
    }
}
