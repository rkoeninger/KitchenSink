using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SchwaTests
    {
        [TestMethod]
        public void ConstantExpressions()
        {
            Assert.AreEqual(8, Schwa.Eval<int>("(+ 3 5)"));
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
        public void DefaultOperator()
        {
            Assert.IsNull(Schwa.Eval<string>("(default string)"));
            Assert.AreEqual(0, Schwa.Eval<int>("(default int)"));
        }

        [TestMethod]
        public void TypeOfOperator()
        {
            Assert.AreEqual(typeof(string), Schwa.Eval<Type>("(typeof string)"));
        }

        [TestMethod]
        public void CastAndAsOperators()
        {
            Assert.IsNotNull(Schwa.Eval<object>("(as object \"\")"));
            Assert.AreEqual(1, Schwa.Eval<int>("(cast int 1.34)"));
        }

        [TestMethod]
        public void PrimitiveTypeChecks()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(is int 0)"));
            Assert.IsTrue(Schwa.Eval<bool>("(is string \"asdfds\")"));
            Assert.IsTrue(Schwa.Eval<bool>("(is string \"\")"));
        }

        [TestMethod]
        public void NaryOperators()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(! false)"));
            Assert.IsTrue(Schwa.Eval<bool>("(&& true true true true)"));
            Assert.IsFalse(Schwa.Eval<bool>("(&& true true false true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| true false true true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| false false true false)"));
        }

        [TestMethod]
        public void ConditionalOperator()
        {
            Assert.AreEqual(5, Schwa.Eval<int>("(?: true 5 3)"));
            Assert.AreEqual(3, Schwa.Eval<int>("(?: false 5 3)"));
        }

        [TestMethod]
        public void CollectionInitializerAndIndexerAccess()
        {
            Assert.AreEqual(3, Schwa.Eval<int>("(# [int 1 2 3 4 5] 2)"));
            Assert.AreEqual(2, Schwa.Eval<int>("(# {string int \"one\" 1 \"two\" 2 \"three\" 3} \"two\")"));
        }

        [TestMethod]
        public void LambdaDeclaration()
        {
            var f = Schwa.Eval<Func<int, int>>("(=> ((int x)) (+ 1 x))");
            Check.That(x => f(x) == x + 1, Rand.Ints().Where(x => x != Int32.MaxValue).Take(100));
        }
    }
}
