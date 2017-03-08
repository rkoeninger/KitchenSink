using System;
using System.Linq;
using static KitchenSink.Operators;
using static KitchenSink.Testing.Expect;
using NUnit.Framework;
using static NUnit.Framework.Assert;

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
            AreEqual(some(3), Maybe.All(a, b, c, (x, y, z) => x + y + z));
            AreEqual(none<int>(), Maybe.All(a, b, none<int>(), (x, y, z) => x + y + z));
        }

        [Test]
        public void MaybeWrappers()
        {
            const string s = "";
            IsNone(maybeof((string) null));
            IsSome(maybeof(s));
            IsNone(none<string>());
        }

        [Test]
        public void MaybeParsing()
        {
            IsSome(123, "123".ToInt());
            IsNone("!@#".ToInt());
            IsNone("123.123".ToInt());
            IsSome(123.0, "123".ToDouble());
            IsNone("!@#".ToDouble());
            IsSome(123.123, "123.123".ToDouble());
        }

        [Test]
        public void MaybeJoining()
        {
            IsSome(3, maybeof(1).Join(maybeof(2), Add));
            IsNone(none<int>().Join(maybeof(2), Add));
            IsNone(maybeof(1).Join(none<int>(), Add));
            IsNone(none<int>().Join(none<int>(), Add));
        }

        [Test]
        public void MaybeCasting()
        {
            IsTrue(maybeof("").Cast<string>().HasValue); // same-type
            IsTrue(maybeof("").Cast<object>().HasValue); // up-casting
            IsFalse(maybeof(new object()).Cast<string>().HasValue); // down-casting
            IsFalse(maybeof("").Cast<int>().HasValue); // casting to unrelated type
        }

        [Test]
        public void MaybeEnumerableExtensions()
        {
            IsSome(0, SeqOf(0).FirstMaybe());
            IsNone(SeqOf<int>().FirstMaybe());
            IsSome(0, SeqOf(0).LastMaybe());
            IsNone(SeqOf<int>().LastMaybe());
            IsSome(0, SeqOf(0).SingleMaybe());
            IsNone(SeqOf<int>().SingleMaybe());
            IsSome(2, ArrayOf(0, 1, 2, 3).ElementAtMaybe(2));
            IsNone(ArrayOf(0, 1, 2, 3).ElementAtMaybe(5));
            IsNone(ArrayOf(0, 1, 2, 3).ElementAtMaybe(-1));
            AreEqual(5, ArrayOf("#", "3", "2", "1", "e", "3", "r", "3").Select(Maybe.ToInt).Flatten().Count());
            IsNone(ArrayOf("#", "3", "2", "1", "e", "3", "r", "3").Select(Maybe.ToInt).Sequence());
            IsSome(ArrayOf("9", "3", "2", "1", "6", "3", "5", "3").Select(Maybe.ToInt).Sequence());
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
            AreEqual("something missing", result);
        }

        [Test]
        public void ValidationTests()
        {
            var v = Validation.Of("abcdefg")
                .Is(x => x.StartsWith("abc"))
                .Is(x => x.EndsWith("abc"))
                .Is(x => { if (x.Length < 10) throw new Exception("asdfasdfa"); });
            AreEqual(2, v.Errors.Count());
        }
    }
}
