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

        [TestMethod]
        public void PrimitiveTypeChecks()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(is int 0)"));
            Assert.IsTrue(Schwa.Eval<bool>("(is string \"asdfds\")"));
            Assert.IsTrue(Schwa.Eval<bool>("(is string \"\")"));
            Assert.IsNotNull(Schwa.Eval<object>("(as object \"\")"));
            Assert.AreEqual(1, Schwa.Eval<int>("(cast int 1.34)"));
        }

        [TestMethod]
        public void BooleanOperations()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(! false)"));
            Assert.IsTrue(Schwa.Eval<bool>("(&& true true true true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| true false true true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| false false true false)"));
        }

        [TestMethod]
        public void CollectionInitializerAndIndexerAccess()
        {
            Assert.AreEqual(3, Schwa.Eval<int>("(# [int 1 2 3 4 5] 2)"));
            Assert.AreEqual(2, Schwa.Eval<int>("(# {string int \"one\" 1 \"two\" 2 \"three\" 3} \"two\")"));
        }
    }
}
