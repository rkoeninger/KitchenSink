using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void ListCreateWithOfMethod()
        {
            var xs = List.Of(1, 2, 3, 4, 5);
            var ys = new List<int>() { 1, 2, 3, 4, 5 };
            Assert.IsTrue(xs.SequenceEqual(ys));
        }

        [TestMethod]
        public void DictionaryCreateLongArgList()
        {
            var dict = Map.Of(
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

        [TestMethod]
        public void DictionaryCreateByAnonObject()
        {
            var dict2 = Map.Of(new
            {
                Red = ConsoleColor.Red,
                ConsoleColor.Green,
                blue = "blue",
                Color_Yellow = Tuple.Create(255, 255, 0),
                Func1 = 5.Plus()
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
            Assert.IsInstanceOfType(dict2["Func1"], typeof(Func<int, int>));

            // Can actually be any object
            var dict3 = Map.Of(new Color(12, 23, 34));
            Assert.AreEqual(3, dict3.Count);
            Assert.IsTrue(dict3.ContainsKey("R"));
            Assert.IsTrue(dict3.ContainsKey("G"));
            Assert.IsTrue(dict3.ContainsKey("B"));
            Assert.AreEqual(12, dict3["R"]);
            Assert.AreEqual(23, dict3["G"]);
            Assert.AreEqual(34, dict3["B"]);
        }

        [TestMethod]
        public void DictionaryCreateByArgList()
        {
            var dict = Map.Of(
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
            public int R { get; private set; }
            public int G { get; private set; }
            public int B { get; private set; }
            public Color(int r, int g, int b) { R = r; G = g; B = b; }
        }
    }
}
