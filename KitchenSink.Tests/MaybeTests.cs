using System;
using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void AllDefined()
        {
            var a = some(0);
            var b = some(1);
            var c = some(2);
            Assert.AreEqual(some(3), Maybe.All(a, b, c, (x, y, z) => x + y + z));
            Assert.AreEqual(none<int>(), Maybe.All(a, b, none<int>(), (x, y, z) => x + y + z));
        }

        [Test]
        public void MaybeWrappers()
        {
            const string s = "";
            Expect.IsNone(maybeof((string) null));
            Expect.IsSome(maybeof(s));
            Expect.IsNone(none<string>());
        }

        [Test]
        public void MaybeParsing()
        {
            Expect.IsSome(123, "123".ToInt());
            Expect.IsNone("!@#".ToInt());
            Expect.IsNone("123.123".ToInt());
            Expect.IsSome(123.0, "123".ToDouble());
            Expect.IsNone("!@#".ToDouble());
            Expect.IsSome(123.123, "123.123".ToDouble());
        }

        [Test]
        public void MaybeJoining()
        {
            Expect.IsSome(3, maybeof(1).Join(maybeof(2), Add));
            Expect.IsNone(none<int>().Join(maybeof(2), Add));
            Expect.IsNone(maybeof(1).Join(none<int>(), Add));
            Expect.IsNone(none<int>().Join(none<int>(), Add));
        }

        [Test]
        public void MaybeCasting()
        {
            Assert.IsTrue(maybeof("").Cast<string>().HasValue); // same-type
            Assert.IsTrue(maybeof("").Cast<object>().HasValue); // up-casting
            Assert.IsFalse(maybeof(new object()).Cast<string>().HasValue); // down-casting
            Assert.IsFalse(maybeof("").Cast<int>().HasValue); // casting to unrelated type
        }

        [Test]
        public void MaybeEnumerableExtensions()
        {
            Expect.IsSome(0, SeqOf(0).FirstMaybe());
            Expect.IsNone(SeqOf<int>().FirstMaybe());
            Assert.AreEqual(5, ArrayOf("#", "3", "2", "1", "e", "3", "r", "3").Select(ToInt).WhereSome().Count());
            Expect.IsNone(ArrayOf("#", "3", "2", "1", "e", "3", "r", "3").Select(ToInt).Sequence());
            Expect.IsSome(ArrayOf("9", "3", "2", "1", "6", "3", "5", "3").Select(ToInt).Sequence());
        }

        [Test]
        public void MaybeAllTests()
        {
            var result = Maybe.All(
                    some("hello"),
                    none<string>(),
                    some("world!"),
                    (x, y, z) => x + y + z)
                .OrElse("something missing");
            Assert.AreEqual("something missing", result);
        }

        [Test]
        public void MaybeDefaultOperator()
        {
            Assert.AreEqual(3, none<int>() | none<int>() | 3);
            Assert.AreEqual(2, none<int>() | some(2) | 3);
            Expect.IsSome(2, none<int>() | some(2) | none<int>());
            Expect.IsNone(none<string>() | none<string>());
        }

        [Test]
        public void ValidationTests()
        {
            var v = Validation.Of("abcdefg")
                .Is(x => x.StartsWith("abc"))
                .Is(x => x.EndsWith("abc"))
                .Is(x => { if (x.Length < 10) throw new Exception("asdfasdfa"); });
            Assert.AreEqual(2, v.Errors.Count());
        }
    }
}
