using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SchwaTests
    {
        [TestMethod]
        public void TestConstantExpressions()
        {
            var expr = Schwa.Parse("(+ 3 5)");
            Assert.AreEqual(ExpressionType.Add, expr.NodeType);
            Assert.AreEqual(typeof(decimal), expr.Type);
        }
    }
}
