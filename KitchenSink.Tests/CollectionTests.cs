using System;
using System.Collections.Generic;
using System.Linq;
using KitchenSink.Collections;
using KitchenSink.Extensions;
using static KitchenSink.Operators;
using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void ListCreateWithOfMethod()
        {
            var xs = ListOf(1, 2, 3, 4, 5);
            var ys = new List<int> { 1, 2, 3, 4, 5 };
            Assert.IsTrue(xs.SequenceEqual(ys));
        }

        [Test]
        public void DictionaryCreateLongArgList()
        {
            var dict = DictOf(
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
            var dict2 = ToDictionary(new
            {
                ConsoleColor.Green,
                Crimson = ConsoleColor.Red,
                blue = "blue",
                Color_Yellow = TupleOf(255, 255, 0),
                Func1 = new Func<int, int>(x => x + 5)
            });
            Assert.AreEqual(5, dict2.Count);
            Assert.IsTrue(dict2.ContainsKey("Crimson"));
            Assert.IsTrue(dict2.ContainsKey("Green"));
            Assert.IsTrue(dict2.ContainsKey("blue"));
            Assert.IsTrue(dict2.ContainsKey("Color_Yellow"));
            Assert.IsTrue(dict2.ContainsKey("Func1"));
            Assert.AreEqual(ConsoleColor.Red, dict2["Crimson"]);
            Assert.AreEqual(ConsoleColor.Green, dict2["Green"]);
            Assert.AreEqual("blue", dict2["blue"]);
            Assert.AreEqual(TupleOf(255, 255, 0), dict2["Color_Yellow"]);
            Assert.IsInstanceOf<Func<int, int>>(dict2["Func1"]);

            // Can actually be any object
            var dict3 = ToDictionary(new Color(12, 23, 34));
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
            var dict = DictOf(
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

        public class Color
        {
            public int R { get; }
            public int G { get; }
            public int B { get; }
            public Color(int r, int g, int b) { R = r; G = g; B = b; }
        }

        [Test]
        public void EnumerableForce()
        {
            // Enumerable methods like Where() and Select() make use of
            // deferred execution so side-effects in functions will not
            // occur until the resulting Enumerable is iterated or
            // a method like ToList() is called.
            var x = 0;
            var y = 0;
            var z = 0;

            var e = SeqOf<Func<Unit>>(
                () => { x = 1; return Unit.It; },
                () => { y = 2; return Unit.It; },
                () => { z = 3; return Unit.It; }).Select(f => f.Invoke());

            Assert.IsFalse(x == 1);
            Assert.IsFalse(y == 2);
            Assert.IsFalse(z == 3);

            // Force() causes an IEnumerable to be evaluated and side-effects to occur.
            e.Force();

            Assert.IsTrue(x == 1);
            Assert.IsTrue(y == 2);
            Assert.IsTrue(z == 3);
        }

        [Test]
        public void EnumerablePartition()
        {
            var p = SeqOf(1, 2, 3, 4, 5, 6, 7, 8);
            var actual = p.Batch(3);
            var expected = SeqOf(
                SeqOf(1, 2, 3),
                SeqOf(4, 5, 6),
                SeqOf(7, 8));
            Assert.IsTrue(actual.Zip(expected, TupleOf).All(t => t.Item1.SequenceEqual(t.Item2)));
        }

        [TestFixture]
        public class RadixDictionaryTests
        {
            [Test]
            public void SimpleAddContainsGet()
            {
                var tree = new RadixDictionary<int>
                {
                    {"zero", 0},
                    {"one", 1},
                    {"two", 2},
                    {"three", 3},
                    {"four", 4},
                    {"five", 5},
                    {"six", 6},
                    {"seven", 7},
                    {"eight", 8},
                    {"nine", 9},
                    {"ten", 10},
                    {"eleven", 11},
                    {"twelve", 12}
                };
                Assert.IsTrue(tree.ContainsKey("zero"));
                Assert.IsTrue(tree.ContainsKey("one"));
                Assert.IsTrue(tree.ContainsKey("two"));
                Assert.IsTrue(tree.ContainsKey("three"));
                Assert.IsTrue(tree.ContainsKey("four"));
                Assert.IsTrue(tree.ContainsKey("five"));
                Assert.IsTrue(tree.ContainsKey("six"));
                Assert.IsTrue(tree.ContainsKey("seven"));
                Assert.IsTrue(tree.ContainsKey("eight"));
                Assert.IsTrue(tree.ContainsKey("nine"));
                Assert.IsTrue(tree.ContainsKey("ten"));
                Assert.IsTrue(tree.ContainsKey("eleven"));
                Assert.IsTrue(tree.ContainsKey("twelve"));
                Assert.AreEqual(0, tree["zero"]);
                Assert.AreEqual(1, tree["one"]);
                Assert.AreEqual(2, tree["two"]);
                Assert.AreEqual(3, tree["three"]);
                Assert.AreEqual(4, tree["four"]);
                Assert.AreEqual(5, tree["five"]);
                Assert.AreEqual(6, tree["six"]);
                Assert.AreEqual(7, tree["seven"]);
                Assert.AreEqual(8, tree["eight"]);
                Assert.AreEqual(9, tree["nine"]);
                Assert.AreEqual(10, tree["ten"]);
                Assert.AreEqual(11, tree["eleven"]);
                Assert.AreEqual(12, tree["twelve"]);
            }

            [Test]
            public void AddRemoveTracksCount()
            {
                var tree = new RadixDictionary<int>();
                Assert.AreEqual(0, tree.Count);
                tree.Add("one", 1);
                Assert.AreEqual(1, tree.Count);
                tree.Remove("one");
                Assert.AreEqual(0, tree.Count);
            }

            [Test]
            public void Enumeration()
            {
                var tree = new RadixDictionary<int>
                {
                    { "zero", 0 },
                    { "one", 1 },
                    { "two", 2 },
                    { "three", 3 }
                };
                var list = tree.ToList();
                Assert.AreEqual(4, list.Count);
                Assert.Contains(new KeyValuePair<string, int>("zero", 0), list);
                Assert.Contains(new KeyValuePair<string, int>("one", 1), list);
                Assert.Contains(new KeyValuePair<string, int>("two", 2), list);
                Assert.Contains(new KeyValuePair<string, int>("three", 3), list);
            }
        }

        [TestFixture]
        public class BankersQueueTests
        {
            [Test]
            public void EnqueueAndDequeue()
            {
                var q = BankersQueue.Empty<int>();
                Maybe<int> m;
                q = q.Enqueue(1);
                q = q.Enqueue(2);
                q = q.Enqueue(3);
                Expect.IsSome(1, q.Current);
                (q, m) = q.Dequeue();
                Expect.IsSome(1, m);
                Expect.IsSome(2, q.Current);
                (q, m) = q.Dequeue();
                Expect.IsSome(2, m);
                Expect.IsSome(3, q.Current);
                (q, m) = q.Dequeue();
                Expect.IsSome(3, m);
                Expect.IsNone(q.Current);
                q = q.Enqueue(4);
                q = q.Enqueue(5);
                Expect.IsSome(4, q.Current);
                (q, m) = q.Dequeue();
                Expect.IsSome(4, m);
                Expect.IsSome(5, q.Current);
                (q, m) = q.Dequeue();
                Expect.IsSome(5, m);
                Expect.IsNone(q.Current);
            }
        }

        [TestFixture]
        public class FingerTreeTests
        {
            [Test]
            public void EnqueueAndDequeue()
            {
                var q = FingerTree.Empty<int>();
                Maybe<int> m;
                q = q.EnqueuePrefix(1);
                q = q.EnqueuePrefix(2);
                q = q.EnqueuePrefix(3);
                Expect.IsSome(3, q.CurrentPrefix);
                Expect.IsSome(1, q.CurrentSuffix);
                (q, m) = q.DequeueSuffix();
                Expect.IsSome(1, m);
                Expect.IsSome(2, q.CurrentSuffix);
                (q, m) = q.DequeueSuffix();
                Expect.IsSome(2, m);
                Expect.IsSome(3, q.CurrentSuffix);
                (q, m) = q.DequeueSuffix();
                Expect.IsSome(3, m);
                Expect.IsNone(q.CurrentSuffix);
                q = q.EnqueuePrefix(4);
                q = q.EnqueuePrefix(5);
                Expect.IsSome(4, q.CurrentSuffix);
                (q, m) = q.DequeueSuffix();
                Expect.IsSome(4, m);
                Expect.IsSome(5, q.CurrentSuffix);
                (q, m) = q.DequeueSuffix();
                Expect.IsSome(5, m);
                Expect.IsNone(q.CurrentSuffix);
            }
        }

        [TestFixture]
        public class BitmappedTrieTests
        {
            [Test]
            public void SuffixAndEnumerate()
            {
                var v = BitmappedTrie.Empty<string>();
                Assert.AreEqual(0, v.Count);
                v = v.Suffix("abc");
                Assert.AreEqual(1, v.Count);
                Assert.AreEqual("abc", v[0]);
                v = v.Suffix("def");
                Assert.AreEqual(2, v.Count);
                Assert.AreEqual("def", v[1]);
                v = v
                    .Suffix("qwe")
                    .Suffix("wer")
                    .Suffix("ert")
                    .Suffix("rty")
                    .Suffix("tyu")
                    .Suffix("yui")
                    .Suffix("iuo")
                    .Suffix("iop")
                    .Suffix("asd")
                    .Suffix("sdf")
                    .Suffix("dfg")
                    .Suffix("fgh")
                    .Suffix("ghj")
                    .Suffix("hjk");
                Assert.AreEqual(16, v.Count);
                Assert.AreEqual("hjk", v[15]);
                v = v.Suffix("zxc");
                Assert.AreEqual(17, v.Count);
                Assert.AreEqual("zxc", v[16]);
            }
        }
    }
}
