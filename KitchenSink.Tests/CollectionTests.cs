using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void ListCreateWithOfMethod()
        {
            var xs = List.Of(1, 2, 3, 4, 5);
            var ys = new List<int> { 1, 2, 3, 4, 5 };
            Assert.IsTrue(xs.SequenceEqual(ys));
        }

        [Test]
        public void DictionaryCreateLongArgList()
        {
            var dict = Dictionary.Of(
                "a", 1,
                "b", 2,
                "c", 3,
                "d", 4,
                "e", 5,
                "f", 6,
                "g", 7,
                "h", 8,
                "i", 9,
                "j", 10,
                "k", 11,
                "l", 12,
                "m", 13,
                "n", 14,
                "o", 15,
                "p", 16,
                "q", 17,
                "r", 18);

            Assert.AreEqual(14, dict["n"]);
            Assert.AreEqual(6, dict["f"]);
        }

        [Test]
        public void DictionaryCreateByAnonObject()
        {
            var dict2 = Dictionary.Of(new
            {
                // ReSharper disable once RedundantAnonymousTypePropertyName
                Red = ConsoleColor.Red,
                ConsoleColor.Green,
                blue = "blue",
                Color_Yellow = Tuple.Create(255, 255, 0),
                Func1 = new Func<int, int>(x => x + 5)
            });
            Assert.AreEqual(5, dict2.Count);
            Assert.IsTrue(dict2.ContainsKey("Red"));
            Assert.IsTrue(dict2.ContainsKey("Green"));
            Assert.IsTrue(dict2.ContainsKey("blue"));
            Assert.IsTrue(dict2.ContainsKey("Color_Yellow"));
            Assert.IsTrue(dict2.ContainsKey("Func1"));
            Assert.AreEqual(ConsoleColor.Red, dict2["Red"]);
            Assert.AreEqual(ConsoleColor.Green, dict2["Green"]);
            Assert.AreEqual("blue", dict2["blue"]);
            Assert.AreEqual(Tuple.Create(255, 255, 0), dict2["Color_Yellow"]);
            Assert.IsInstanceOf<Func<int, int>>(dict2["Func1"]);

            // Can actually be any object
            var dict3 = Dictionary.Of(new Color(12, 23, 34));
            Assert.AreEqual(3, dict3.Count);
            Assert.IsTrue(dict3.ContainsKey("R"));
            Assert.IsTrue(dict3.ContainsKey("G"));
            Assert.IsTrue(dict3.ContainsKey("B"));
            Assert.AreEqual(12, dict3["R"]);
            Assert.AreEqual(23, dict3["G"]);
            Assert.AreEqual(34, dict3["B"]);
        }

        [Test]
        public void DictionaryCreateByArgList()
        {
            var dict = Dictionary.Of(
                    "red", ConsoleColor.Red,
                    "blue", ConsoleColor.Blue,
                    "green", ConsoleColor.Green
            );
            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.ContainsKey("red"));
            Assert.IsTrue(dict.ContainsKey("green"));
            Assert.IsTrue(dict.ContainsKey("blue"));
            Assert.AreEqual(ConsoleColor.Red, dict["red"]);
            Assert.AreEqual(ConsoleColor.Green, dict["green"]);
            Assert.AreEqual(ConsoleColor.Blue, dict["blue"]);
        }

        class Color
        {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Local
            // ReSharper restore MemberCanBePrivate.Local
            public Color(int r, int g, int b) { R = r; G = g; B = b; }
        }

        [Test]
        public void EnumerableForce()
        {
            /*
             * Enumerable methods like Where() and Select() make use of deferred execution so side-effects in
             * functions will not occur until the resulting Enumerable is iterated or a method like ToList() is called.
             */
            var x = 0;
            var y = 0;
            var z = 0;

            var e = Seq.Of<Func<Unit>>(
                () => { x = 1; return Unit.It; },
                () => { y = 2; return Unit.It; },
                () => { z = 3; return Unit.It; }).Select(f => f.Invoke());

            Assert.IsFalse(x == 1);
            Assert.IsFalse(y == 2);
            Assert.IsFalse(z == 3);

            /*
             * Force() causes an IEnumerable to be evaluated and side-effects to occur.
             */
            e.Force();

            Assert.IsTrue(x == 1);
            Assert.IsTrue(y == 2);
            Assert.IsTrue(z == 3);
        }

        [Test]
        public void EnumerablePartition()
        {
            var p = Seq.Of(1, 2, 3, 4, 5, 6, 7, 8);
            var actual = p.Partition(3);
            var expected = Seq.Of(
                Seq.Of(1, 2, 3),
                Seq.Of(4, 5, 6),
                Seq.Of(7, 8));
            Assert.IsTrue(actual.Zip(expected, Tuple.Create).All(t => t.Item1.SequenceEqual(t.Item2)));
        }
    }
}
