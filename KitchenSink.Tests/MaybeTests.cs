using System;
using System.Linq;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class MaybeTests
    {
        [Test]
        public void AllDefined()
        {
            var a = Maybe.Some(0);
            var b = Maybe.Some(1);
            var c = Maybe.Some(2);
            Assert.AreEqual(Maybe.Some(3), Maybe.All(a, b, c, (x, y, z) => x + y + z));
            Assert.AreEqual(Maybe<int>.None, Maybe.All(a, b, Maybe<int>.None, (x, y, z) => x + y + z));
        }

        [Test]
        public void MaybeWrappers()
        {
            const string s = "";
            Expect.None(Maybe.Of((string) null));
            Expect.Some(Maybe.Of(s));
            Expect.None(Maybe<string>.None);
        }

        [Test]
        public void MaybeParsing()
        {
            Expect.Some(123, "123".ToInt());
            Expect.None("!@#".ToInt());
            Expect.None("123.123".ToInt());
            Expect.Some(123.0, "123".ToDouble());
            Expect.None("!@#".ToDouble());
            Expect.Some(123.123, "123.123".ToDouble());
        }

        [Test]
        public void MaybeJoining()
        {
            Expect.Some(3, Maybe.Of(1).Join(Maybe.Of(2), Z.Add));
            Expect.None(Maybe<int>.None.Join(Maybe.Of(2), Z.Add));
            Expect.None(Maybe.Of(1).Join(Maybe<int>.None, Z.Add));
            Expect.None(Maybe<int>.None.Join(Maybe<int>.None, Z.Add));
        }

        [Test]
        public void MaybeCasting()
        {
            Assert.IsTrue(Maybe.Of("").Cast<string>().HasValue); // same-type
            Assert.IsTrue(Maybe.Of("").Cast<object>().HasValue); // up-casting
            Assert.IsFalse(Maybe.Of(new object()).Cast<string>().HasValue); // down-casting
            Assert.IsFalse(Maybe.Of("").Cast<int>().HasValue); // casting to unrelated type
        }

        [Test]
        public void MaybeEnumerableExtensions()
        {
            Expect.Some(0, new[] { 0 }.FirstMaybe());
            Expect.None(new int[0].FirstMaybe());
            Expect.Some(0, new[] { 0 }.LastMaybe());
            Expect.None(new int[0].LastMaybe());
            Expect.Some(0, new[] { 0 }.SingleMaybe());
            Expect.None(new int[0].SingleMaybe());
            Expect.Some(2, new[] { 0, 1, 2, 3 }.ElementAtMaybe(2));
            Expect.None(new[] { 0, 1, 2, 3 }.ElementAtMaybe(5));
            Expect.None(new[] { 0, 1, 2, 3 }.ElementAtMaybe(-1));
            Assert.AreEqual(5, new [] {"#", "3", "2", "1", "e", "3", "r", "3"}.Select(Maybe.ToInt).Flatten().Count());
            Expect.None(new[] { "#", "3", "2", "1", "e", "3", "r", "3" }.Select(Maybe.ToInt).Sequence());
            Expect.Some(new[] { "9", "3", "2", "1", "6", "3", "5", "3" }.Select(Maybe.ToInt).Sequence());
        }

        [Test]
        public void MaybeAllTests()
        {
            var result = Maybe.All(
                    Maybe.Some("hello"),
                    Maybe<string>.None,
                    Maybe.Some("world!"),
                    (x, y, z) => x + y + z)
                .OrElse("something missing");
            Assert.AreEqual("something missing", result);
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
