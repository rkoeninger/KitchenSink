using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void DictionaryCreateLongArgList()
        {
            var dict = Z.Map(
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
        public void DictionaryCreateByLambdas()
        {
            var dict = Z.Map(
                red => ConsoleColor.Red,
                green => ConsoleColor.Green,
                blue => ConsoleColor.Blue,
                yellow => ConsoleColor.Yellow);

            Assert.AreEqual(4, dict.Count);
            Assert.IsTrue(dict.ContainsKey("red"));
            Assert.IsTrue(dict.ContainsKey("green"));
            Assert.IsTrue(dict.ContainsKey("blue"));
            Assert.IsTrue(dict.ContainsKey("yellow"));
            Assert.AreEqual(ConsoleColor.Red, dict["red"]);
            Assert.AreEqual(ConsoleColor.Green, dict["green"]);
            Assert.AreEqual(ConsoleColor.Blue, dict["blue"]);
            Assert.AreEqual(ConsoleColor.Yellow, dict["yellow"]);
        }

        [TestMethod]
        public void DictionaryCreateByAnonObject()
        {
            var dict2 = Z.Map(new
            {
                Red = ConsoleColor.Red,
                ConsoleColor.Green,
                blue = "blue",
                Color_Yellow = Tuple.Create(255, 255, 0),
                Func1 = (Func<int, int>)((int x) => x + 5)
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
            var dict3 = Z.Map(new Color(12, 23, 34));
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
            var dict = Z.Map(
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

        [TestMethod]
        public void DictionaryCreateByRows()
        {
            var dict = Z.Map(
                Row.Of("red", ConsoleColor.Red),
                Row.Of("blue", ConsoleColor.Blue),
                Row.Of("green", ConsoleColor.Green));
            Assert.AreEqual(3, dict.Count);
            Assert.IsTrue(dict.ContainsKey("red"));
            Assert.IsTrue(dict.ContainsKey("green"));
            Assert.IsTrue(dict.ContainsKey("blue"));
            Assert.AreEqual(ConsoleColor.Red, dict["red"]);
            Assert.AreEqual(ConsoleColor.Green, dict["green"]);
            Assert.AreEqual(ConsoleColor.Blue, dict["blue"]);
        }

        [TestMethod]
        public void DictionaryCreateByTuples()
        {
            var dict = Z.Map(
                Tuple.Create("red", ConsoleColor.Red),
                Tuple.Create("blue", ConsoleColor.Blue),
                Tuple.Create("green", ConsoleColor.Green));
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
