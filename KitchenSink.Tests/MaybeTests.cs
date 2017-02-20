using System;
using System.Linq;
using KitchenSink.Testing;
using static KitchenSink.Collections.ConstructionOperators;
using static KitchenSink.Z;
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
            Expect.None(maybeof((string) null));
            Expect.Some(maybeof(s));
            Expect.None(none<string>());
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
            Expect.Some(3, maybeof(1).Join(maybeof(2), Add));
            Expect.None(none<int>().Join(maybeof(2), Add));
            Expect.None(maybeof(1).Join(none<int>(), Add));
            Expect.None(none<int>().Join(none<int>(), Add));
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
            Expect.Some(0, seqof(0).FirstMaybe());
            Expect.None(seqof<int>().FirstMaybe());
            Expect.Some(0, seqof(0).LastMaybe());
            Expect.None(seqof<int>().LastMaybe());
            Expect.Some(0, seqof(0).SingleMaybe());
            Expect.None(seqof<int>().SingleMaybe());
            Expect.Some(2, arrayof(0, 1, 2, 3).ElementAtMaybe(2));
            Expect.None(arrayof(0, 1, 2, 3).ElementAtMaybe(5));
            Expect.None(arrayof(0, 1, 2, 3).ElementAtMaybe(-1));
            Assert.AreEqual(5, arrayof("#", "3", "2", "1", "e", "3", "r", "3").Select(Maybe.ToInt).Flatten().Count());
            Expect.None(arrayof("#", "3", "2", "1", "e", "3", "r", "3").Select(Maybe.ToInt).Sequence());
            Expect.Some(arrayof("9", "3", "2", "1", "6", "3", "5", "3").Select(Maybe.ToInt).Sequence());
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
