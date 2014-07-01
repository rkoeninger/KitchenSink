﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void DictionaryCreateByLambdas()
        {
            var dict = Map.New(
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
            var dict2 = Map.New(new
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
            var dict3 = Map.New(new Color(12, 23, 34));
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
            var dict = Map.New(
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