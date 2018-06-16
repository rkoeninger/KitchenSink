using System.Linq;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class Maybes
    {
        [Test]
        public void AllDefined()
        {
            var a = Some(0);
            var b = Some(1);
            var c = Some(2);
            Assert.AreEqual(Some(3), AllSome(a, b, c, (x, y, z) => x + y + z));
            Assert.AreEqual(None<int>(), AllSome(a, b, None<int>(), (x, y, z) => x + y + z));
        }

        [Test]
        public void MaybeWrappers()
        {
            const string s = "";
            Expect.IsNone(MaybeOf((string) null));
            Expect.IsSome(MaybeOf(s));
            Expect.IsNone(None<string>());
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
            Expect.IsSome(3, MaybeOf(1).Join(MaybeOf(2), Add));
            Expect.IsNone(None<int>().Join(MaybeOf(2), Add));
            Expect.IsNone(MaybeOf(1).Join(None<int>(), Add));
            Expect.IsNone(None<int>().Join(None<int>(), Add));
        }

        [Test]
        public void MaybeCasting()
        {
            Assert.IsTrue(MaybeOf("").Cast<string>().HasValue); // same-type
            Assert.IsTrue(MaybeOf("").Cast<object>().HasValue); // up-casting
            Assert.IsFalse(MaybeOf(new object()).Cast<string>().HasValue); // down-casting
            Assert.IsFalse(MaybeOf("").Cast<int>().HasValue); // casting to unrelated type
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
            var result = AllSome(
                    Some("hello"),
                    None<string>(),
                    Some("world!"),
                    (x, y, z) => x + y + z)
                .OrElse("something missing");
            Assert.AreEqual("something missing", result);
        }

        [Test]
        public void MaybeDefaultOperator()
        {
            Assert.AreEqual(3, None<int>() | None<int>() | 3);
            Assert.AreEqual(2, None<int>() | Some(2) | 3);
            Expect.IsSome(2, None<int>() | Some(2) | None<int>());
            Expect.IsNone(None<string>() | None<string>());
        }
    }
}
