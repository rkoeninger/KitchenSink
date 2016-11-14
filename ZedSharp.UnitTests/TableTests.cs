using NUnit.Framework;

namespace ZedSharp.UnitTests
{
    [TestFixture]
    public class TableTests
    {
        [Test]
        public void OfAndEqualsMethods()
        {
            var x = Row.Of("ZXC", 2);
            var y = Row.Of("ZXC", 2);
            Assert.AreEqual(x, y);
            Assert.AreEqual(x.GetHashCode(), y.GetHashCode());

            var r = Table.Of(
                Row.Of("ABC", 4),
                Row.Of("QWE", 5));
            var s = Table.Of(
                Row.Of("ABC", 4),
                Row.Of("QWE", 5));
            Assert.AreEqual(r, s);
            Assert.AreEqual(r.GetHashCode(), s.GetHashCode());

            var t = Table.Of(
                Row.Of("abc", 1, true),
                Row.Of("dfg", 6, true),
                Row.Of("wert", 2, false),
                Row.Of("qwer", 4, true),
                Row.Of("rytu", 7, true));
            var v = Table.Of(
                "SCol", "NumCol", "BoolCol",
                Row.Of("abc", 1, true),
                Row.Of("dfg", 6, true),
                Row.Of("wert", 2, false),
                Row.Of("qwer", 4, true),
                Row.Of("rytu", 7, true));
            var u = Table.Of<string, int, bool>(
                "abc", 1, true,
                "dfg", 6, true,
                "wert", 2, false,
                "qwer", 4, true,
                "rytu", 7, true);
            var w = Table.Of<string, int, bool>(
                "SCol", "NumCol", "BoolCol",
                "abc", 1, true,
                "dfg", 6, true,
                "wert", 2, false,
                "qwer", 4, true,
                "rytu", 7, true);
            Assert.AreEqual(t, v);
            Assert.AreEqual(t, u);
            Assert.AreEqual(t, w);
            Assert.AreEqual(u, v);
            Assert.AreEqual(w, v);
            Assert.AreEqual(u, w);
            Assert.AreEqual(t.GetHashCode(), v.GetHashCode());
            Assert.AreEqual(t.GetHashCode(), u.GetHashCode());
            Assert.AreEqual(t.GetHashCode(), w.GetHashCode());
            Assert.AreEqual(u.GetHashCode(), v.GetHashCode());
            Assert.AreEqual(w.GetHashCode(), v.GetHashCode());
            Assert.AreEqual(u.GetHashCode(), w.GetHashCode());
            
            Expect.Error(() => Table.Of<string, int>("asd", 1, 3, "wer"));
            Expect.Error(() => Table.Of<string, int>("col1", "col2", "asd", 1, 3, "wer"));

            Expect.CompileFail(
                Common.Wrap(@"Table.Of(Row.Of(""asc"", 1), Row.Of(2, ""wer""))"),
                new [] {Common.ZedDll},
                new []
                {
                    "CS0411" // type arguments cannot be inferred from usage
                });
        }

        [Test]
        public void WhereMethods()
        {
            var t = Table.Of(
                Row.Of(1, "ABC"),
                Row.Of(2, "DEF"),
                Row.Of(3, "GHI"),
                Row.Of(4, "JKL"),
                Row.Of(5, "MNO"));
            var u = Table.Of(
                Row.Of(1, "ABC"),
                Row.Of(2, "DEF"));
            Assert.AreEqual(u, t.Where((x, y) => x < 3));
        }
    }
}
