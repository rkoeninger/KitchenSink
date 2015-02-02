using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class SchwaTests
    {
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
        public void UnicodeParsing()
        {
            Assert.AreEqual("\u0000", Schwa.Eval<string>("\"\\u0000\""));
            Assert.AreEqual("\u0065", Schwa.Eval<string>("\"\\u0065\""));
            Assert.AreEqual("\uffff", Schwa.Eval<string>("\"\\uffff\""));
            Assert.AreEqual("\U00000000", Schwa.Eval<string>("\"\\U00000000\""));
            Assert.AreEqual("\U00000065", Schwa.Eval<string>("\"\\U00000065\""));
            Assert.AreEqual("\U0002a601", Schwa.Eval<string>("\"\\U0002a601\""));
            Assert.AreEqual("\x0000", Schwa.Eval<string>("\"\\x0000\""));
            Assert.AreEqual("\t", Schwa.Eval<string>("\"\\x9\""));
            Assert.AreEqual("\xffff", Schwa.Eval<string>("\"\\xffff\""));
            Expect.Error<SchwaLexException>(() => Schwa.Eval<string>("\"\\x\""));
        }

        [TestMethod]
        public void DefaultOperator()
        {
            Assert.IsNull(Schwa.Eval<string>("(default string)"));
            Assert.AreEqual(0, Schwa.Eval<int>("(default int)"));
            Assert.AreEqual(null, Schwa.Eval<int?>("(default int?)"));
        }

        [TestMethod]
        public void TypeOfOperator()
        {
            Assert.AreEqual(typeof(string), Schwa.Eval<Type>("(typeof string)"));
            Assert.AreEqual(typeof(int?), Schwa.Eval<Type>("(typeof int?)"));
        }

        [TestMethod]
        public void NullableTypeLiterals()
        {
            Expect.Error<ArgumentException>(() => Schwa.Eval<Type>("(typeof int??)"));
        }

        [TestMethod]
        public void NewOperator()
        {
            Assert.AreEqual(0, Schwa.Eval<int?>("(new int? 0)"));
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
        public void UnaryOperators()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(! false)"));
            Assert.IsFalse(Schwa.Eval<bool>("(! true)"));
            Expect.Error<InvalidOperationException>(() => Schwa.Eval<bool>("(! null)"));

            Assert.AreEqual(0, Schwa.Eval<int>("(~ -1)"));
            Assert.AreEqual(4, Schwa.Eval<int>("(-- 5)"));
            Assert.AreEqual(9, Schwa.Eval<int>("(++ 8)"));
        }

        [TestMethod]
        public void NaryOperators()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(&& true true true true)"));
            Assert.IsFalse(Schwa.Eval<bool>("(&& true true false true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| true false true true)"));
            Assert.IsTrue(Schwa.Eval<bool>("(|| false false true false)"));
            Assert.AreEqual(8, Schwa.Eval<int>("(+ 3 5)"));
            Assert.AreEqual(new [] {6, 4, -2, 3, -9, 5}.Sum(), Schwa.Eval<int>("(+ 6 4 -2 3 -9 5)"));
            Assert.AreEqual(false, Schwa.Eval<bool>("(?? (cast bool? null) false)"));
        }

        [TestMethod]
        public void NaryCompareOperators()
        {
            Assert.IsTrue(Schwa.Eval<bool>("(< 1 3 6 7)"));
            Assert.IsFalse(Schwa.Eval<bool>("(< 1 5 4 7)"));
            Assert.IsFalse(Schwa.Eval<bool>("(< 1 4 4 8)"));
            Assert.IsTrue(Schwa.Eval<bool>("(> 8 7 6 -4)"));
            Assert.IsTrue(Schwa.Eval<bool>("(>= 9 5 5 2)"));
            Assert.IsTrue(Schwa.Eval<bool>("(<= 1 4 4 8)"));
            Assert.IsFalse(Schwa.Eval<bool>("(>= 9 5 6 2)"));
        }

        [TestMethod]
        public void NaryEqualityOperators()
        {
            Expect.Error(() => Schwa.Eval<bool>("(==)"));
            Expect.Error(() => Schwa.Eval<bool>("(== 1)"));
            Assert.IsTrue(Schwa.Eval<bool>("(== 1 1)"));
            Assert.IsTrue(Schwa.Eval<bool>("(== 1 1 1)"));
            Assert.IsFalse(Schwa.Eval<bool>("(== 1 1 1 2 1)"));
            Expect.Error(() => Schwa.Eval<bool>("(!=)"));
            Expect.Error(() => Schwa.Eval<bool>("(!= 1)"));
            Assert.IsFalse(Schwa.Eval<bool>("(!= 1 1)"));
            Assert.IsFalse(Schwa.Eval<bool>("(!= 1 1 1)"));
            Assert.IsFalse(Schwa.Eval<bool>("(!= 1 1 1 2 1)"));
            Assert.IsTrue(Schwa.Eval<bool>("(!= 4 2 7 5)"));
        }

        [TestMethod]
        public void ConditionalOperator()
        {
            Assert.AreEqual(5, Schwa.Eval<int>("(?: true 5 3)"));
            Assert.AreEqual(3, Schwa.Eval<int>("(?: false 5 3)"));
        }

        [TestMethod]
        public void CollectionInitializerTypeInference()
        {
            Assert.IsInstanceOfType(Schwa.Eval<object>("[1 2 3]"), typeof(List<int>));
            Assert.IsInstanceOfType(Schwa.Eval<object>("{\"one\" 1 \"two\" 2 \"three\" 3}"), typeof(Dictionary<string, int>));
            Expect.Error(() => Schwa.Eval<object>("[]"));
            Expect.Error(() => Schwa.Eval<object>("{}"));
        }

        [TestMethod]
        public void CollectionInitializerAndIndexerAccess()
        {
            Assert.AreEqual(3, Schwa.Eval<int>("(# [1 2 3 4 5] 2)"));
            Assert.AreEqual(2, Schwa.Eval<int>("(# {\"one\" 1 \"two\" 2 \"three\" 3} \"two\")"));
        }

        [TestMethod]
        public void LambdaDeclaration()
        {
            var f = Schwa.Eval<Func<int, int>>("(=> ((int x)) (+ 1 x))");
            Check.That(x => f(x) == x + 1, Sample.Ints.Where(x => x != Int32.MaxValue));
        }

        [TestMethod]
        public void SyntaxNestingDepth()
        {
            const int depth = 256;
            Syntax.Read(
                  Enumerable.Repeat("(", depth).Concat()
                + Enumerable.Repeat(")", depth).Concat());
        }

        [TestMethod]
        public void NestedString()
        {
            var stringList = Schwa.Eval<IReadOnlyList<String>>("[\"abc\" \'\"def\"\' «\"sdf«345\'«fgh\"»678»\'yui»]");
            Assert.AreEqual(3, stringList.Count);
            Assert.AreEqual("abc", stringList[0]);
            Assert.AreEqual("\"def\"", stringList[1]);
            Assert.AreEqual("\"sdf\"345\'\"fgh\"\"678\"\'yui", stringList[2]);
        }
    }
}
