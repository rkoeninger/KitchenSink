using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SchwaTests
    {
        [TestMethod]
        public void ConstantExpressions()
        {
            var expr = Schwa.Parse("(+ 3 5)");
            Assert.AreEqual(ExpressionType.Add, expr.NodeType);
            Assert.AreEqual(typeof(int), expr.Type);

            var lambda = Expression.Lambda<Func<int>>(expr);
            Assert.AreEqual(8, lambda.Compile().Invoke());
        }

        [TestMethod]
        public void NumericExpressionTypes()
        {
            Assert.AreEqual(typeof(int), Schwa.Parse("143").Type);
            Assert.AreEqual(typeof(int), Schwa.Parse("-2434345").Type);
            Assert.AreEqual(typeof(uint), Schwa.Parse("143u").Type);
            Assert.AreEqual(typeof(long), Schwa.Parse("1434532344l").Type);
            Assert.AreEqual(typeof(ulong), Schwa.Parse("1434532344ul").Type);
            Assert.AreEqual(typeof(float), Schwa.Parse("-3454.5453f").Type);
            Assert.AreEqual(typeof(double), Schwa.Parse("-3454.5453").Type);
            Assert.AreEqual(typeof(decimal), Schwa.Parse("4573945723849579.2345m").Type);
        }
    }
}
