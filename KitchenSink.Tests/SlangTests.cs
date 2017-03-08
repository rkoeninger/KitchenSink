using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Extensions;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class SlangTests
    {
        [Test]
        public void NumericExpressionTypes()
        {
            Assert.AreEqual(typeof(int), Slang.Parse("143").Type);
            Assert.AreEqual(typeof(int), Slang.Parse("-2434345").Type);
            Assert.AreEqual(typeof(uint), Slang.Parse("143u").Type);
            Assert.AreEqual(typeof(long), Slang.Parse("1434532344l").Type);
            Assert.AreEqual(typeof(ulong), Slang.Parse("1434532344ul").Type);
            Assert.AreEqual(typeof(float), Slang.Parse("-3454.5453f").Type);
            Assert.AreEqual(typeof(double), Slang.Parse("-3454.5453").Type);
            Assert.AreEqual(typeof(decimal), Slang.Parse("4573945723849579.2345m").Type);
        }

        [Test]
        public void UnicodeParsing()
        {
            Assert.AreEqual("\u0000", Slang.Eval<string>("\"\\u0000\""));
            Assert.AreEqual("\u0065", Slang.Eval<string>("\"\\u0065\""));
            Assert.AreEqual("\uffff", Slang.Eval<string>("\"\\uffff\""));
            Assert.AreEqual("\U00000000", Slang.Eval<string>("\"\\U00000000\""));
            Assert.AreEqual("\U00000065", Slang.Eval<string>("\"\\U00000065\""));
            Assert.AreEqual("\U0002a601", Slang.Eval<string>("\"\\U0002a601\""));
            Assert.AreEqual("\x0000", Slang.Eval<string>("\"\\x0000\""));
            Assert.AreEqual("\t", Slang.Eval<string>("\"\\x9\""));
            Assert.AreEqual("\xffff", Slang.Eval<string>("\"\\xffff\""));
            Expect.Error<SlangLexException>(() => Slang.Eval<string>("\"\\x\""));
        }

        [Test]
        public void DefaultOperator()
        {
            Assert.IsNull(Slang.Eval<string>("(default string)"));
            Assert.AreEqual(0, Slang.Eval<int>("(default int)"));
            Assert.AreEqual(null, Slang.Eval<int?>("(default int?)"));
        }

        [Test]
        public void TypeOfOperator()
        {
            Assert.AreEqual(typeof(string), Slang.Eval<Type>("(typeof string)"));
            Assert.AreEqual(typeof(int?), Slang.Eval<Type>("(typeof int?)"));
        }

        [Test]
        public void NullableTypeLiterals()
        {
            Expect.Error<ArgumentException>(() => Slang.Eval<Type>("(typeof int??)"));
        }

        [Test]
        public void NewOperator()
        {
            Assert.AreEqual(0, Slang.Eval<int?>("(new int? 0)"));
        }

        [Test]
        public void CastAndAsOperators()
        {
            Assert.IsNotNull(Slang.Eval<object>("(as object \"\")"));
            Assert.AreEqual(1, Slang.Eval<int>("(cast int 1.34)"));
        }

        [Test]
        public void PrimitiveTypeChecks()
        {
            Assert.IsTrue(Slang.Eval<bool>("(is int 0)"));
            Assert.IsTrue(Slang.Eval<bool>("(is string \"asdfds\")"));
            Assert.IsTrue(Slang.Eval<bool>("(is string \"\")"));
        }

        [Test]
        public void UnaryOperators()
        {
            Assert.IsTrue(Slang.Eval<bool>("(! false)"));
            Assert.IsFalse(Slang.Eval<bool>("(! true)"));
            Expect.Error<InvalidOperationException>(() => Slang.Eval<bool>("(! null)"));

            Assert.AreEqual(0, Slang.Eval<int>("(~ -1)"));
            Assert.AreEqual(4, Slang.Eval<int>("(-- 5)"));
            Assert.AreEqual(9, Slang.Eval<int>("(++ 8)"));
        }

        [Test]
        public void NaryOperators()
        {
            Assert.IsTrue(Slang.Eval<bool>("(&& true true true true)"));
            Assert.IsFalse(Slang.Eval<bool>("(&& true true false true)"));
            Assert.IsTrue(Slang.Eval<bool>("(|| true false true true)"));
            Assert.IsTrue(Slang.Eval<bool>("(|| false false true false)"));
            Assert.AreEqual(8, Slang.Eval<int>("(+ 3 5)"));
            Assert.AreEqual(new [] {6, 4, -2, 3, -9, 5}.Sum(), Slang.Eval<int>("(+ 6 4 -2 3 -9 5)"));
            Assert.AreEqual(false, Slang.Eval<bool>("(?? (cast bool? null) false)"));
        }

        [Test]
        public void NaryCompareOperators()
        {
            Assert.IsTrue(Slang.Eval<bool>("(< 1 3 6 7)"));
            Assert.IsFalse(Slang.Eval<bool>("(< 1 5 4 7)"));
            Assert.IsFalse(Slang.Eval<bool>("(< 1 4 4 8)"));
            Assert.IsTrue(Slang.Eval<bool>("(> 8 7 6 -4)"));
            Assert.IsTrue(Slang.Eval<bool>("(>= 9 5 5 2)"));
            Assert.IsTrue(Slang.Eval<bool>("(<= 1 4 4 8)"));
            Assert.IsFalse(Slang.Eval<bool>("(>= 9 5 6 2)"));
        }

        [Test]
        public void NaryEqualityOperators()
        {
            Expect.Error<ArgumentException>(() => Slang.Eval<bool>("(==)"));
            Expect.Error<ArgumentException>(() => Slang.Eval<bool>("(== 1)"));
            Assert.IsTrue(Slang.Eval<bool>("(== 1 1)"));
            Assert.IsTrue(Slang.Eval<bool>("(== 1 1 1)"));
            Assert.IsFalse(Slang.Eval<bool>("(== 1 1 1 2 1)"));
            Expect.Error<ArgumentException>(() => Slang.Eval<bool>("(!=)"));
            Expect.Error<ArgumentException>(() => Slang.Eval<bool>("(!= 1)"));
            Assert.IsFalse(Slang.Eval<bool>("(!= 1 1)"));
            Assert.IsFalse(Slang.Eval<bool>("(!= 1 1 1)"));
            Assert.IsTrue(Slang.Eval<bool>("(!= 1 1 1 2 1)"));
            Assert.IsTrue(Slang.Eval<bool>("(!= 4 2 7 5)"));
            Check.That(Rand.Lists(Rand.Ints()).Where(x => x.Count >= 2).Take(100),
            (IEnumerable<int> xs) =>
            {
                var vals = string.Join(" ", xs);
                return Slang.Eval<bool>(string.Format("(== (== {0}) (! (!= {0})))", vals));
            });
        }

        [Test]
        public void NaryMinMaxOperators()
        {
            Assert.AreEqual(4, Slang.Eval<int>("(min 6 12 4 15 8)"));
            Assert.AreEqual(15, Slang.Eval<int>("(max 6 12 4 15 8)"));
        }

        [Test]
        public void ConditionalOperator()
        {
            Assert.AreEqual(5, Slang.Eval<int>("(?: true 5 3)"));
            Assert.AreEqual(3, Slang.Eval<int>("(?: false 5 3)"));
        }

        [Test]
        public void CollectionInitializerTypeInference()
        {
            Assert.IsInstanceOf<List<int>>(Slang.Eval<object>("[1 2 3]"));
            Assert.IsInstanceOf<Dictionary<string, int>>(Slang.Eval<object>("{\"one\" 1 \"two\" 2 \"three\" 3}"));
            Expect.Error(() => Slang.Eval<object>("[]"));
            Expect.Error(() => Slang.Eval<object>("{}"));
        }

        [Test]
        public void CollectionInitializerAndIndexerAccess()
        {
            Assert.AreEqual(3, Slang.Eval<int>("(# [1 2 3 4 5] 2)"));
            Assert.AreEqual(2, Slang.Eval<int>("(# {\"one\" 1 \"two\" 2 \"three\" 3} \"two\")"));
        }

        [Test]
        public void LambdaDeclaration()
        {
            var f = Slang.Eval<Func<int, int>>("(=> ((int x)) (+ 1 x))");
            Check.That(Sample.Ints.Where(x => x != int.MaxValue), x => f(x) == x + 1);
        }

        [Test]
        public void SyntaxNestingDepth()
        {
            const int depth = 256;
            Syntax.Read(
                  Enumerable.Repeat("(", depth).MakeString()
                + Enumerable.Repeat(")", depth).MakeString());
        }

        [Test]
        public void NestedString()
        {
            var stringList = Slang.Eval<IReadOnlyList<string>>("[\"abc\" \'\"def\"\' «\"sdf«345\'«fgh\"»678»\'yui»]");
            Assert.AreEqual(3, stringList.Count);
            Assert.AreEqual("abc", stringList[0]);
            Assert.AreEqual("\"def\"", stringList[1]);
            Assert.AreEqual("\"sdf\"345\'\"fgh\"\"678\"\'yui", stringList[2]);
        }
    }
}
