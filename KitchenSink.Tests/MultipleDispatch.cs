using System;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class MultipleDispatch
    {
        public class Shape {}
        public class Tri : Shape {}
        public class Quad : Shape {}
        public class Square : Quad {}
        public class Round : Shape {}
        public class Circle : Round {}

        [Test]
        public void ClassHierarchyDispatch()
        {
            var method = new MultiMethod<Shape, string>()
                .Extend((Tri _) => "tri")
                .Extend((Square _) => "square") // Extension order is important
                .Extend((Quad _) => "quad")
                .ExtendExact((Round _) => "round");

            Assert.AreEqual("tri", method.Apply(new Tri()));
            Assert.AreEqual("quad", method.Apply(new Quad()));
            Assert.AreEqual("square", method.Apply(new Square()));
            Assert.Throws<NotImplementedException>(() => method.Apply(new Shape()));
            Assert.AreEqual("round", method.Apply(new Round()));
            Assert.Throws<NotImplementedException>(() => method.Apply(new Circle()));
        }

        [Test]
        public void ValuePredicateDispatch()
        {
            var method = new MultiMethod<int, int>()
                .Extend(x => x == 0, Id)
                .Extend(Even, x => Inc(x) * 2)
                .Extend(Odd, x => Dec(x) * 2 - 1);

            Assert.AreEqual(0, method.Apply(0));
            Assert.AreEqual(10, method.Apply(4));
            Assert.AreEqual(11, method.Apply(7));
        }
    }
}
