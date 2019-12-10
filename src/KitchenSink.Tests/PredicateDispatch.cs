using System;
using static KitchenSink.Operators;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class PredicateDispatch
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
            Func<Shape, string> f = new MultiMethod<Shape, string>()
                .Extend((Tri _) => "tri")
                .Extend((Square _) => "square") // Extension order is important
                .Extend((Quad _) => "quad")
                .ExtendExact((Round _) => "round")
                .Apply;

            Assert.AreEqual("tri", f(new Tri()));
            Assert.AreEqual("quad", f(new Quad()));
            Assert.AreEqual("square", f(new Square()));
            Assert.Throws<NotImplementedException>(() => f(new Shape()));
            Assert.AreEqual("round", f(new Round()));
            Assert.Throws<NotImplementedException>(() => f(new Circle()));
        }

        [Test]
        public void ValuePredicateDispatch()
        {
            Func<int, int> f = new MultiMethod<int, int>()
                .Extend(Zero, Id)
                .Extend(Even, Inc)
                .Extend(Odd, Dec)
                .Apply;

            Assert.AreEqual(0, f(0));
            Assert.AreEqual(4, f(5));
            Assert.AreEqual(11, f(10));
        }

        [Test]
        public void MultiParameterDispatch()
        {
            Func<int, int, int> f = new MultiMethod<int, int, int>()
                .Extend(Even, Even, Mul)
                .Extend(Odd, Odd, Add)
                .Apply;

            Assert.AreEqual(0, f(0, 2));
            Assert.AreEqual(72, f(12, 6));
            Assert.AreEqual(32, f(4, 8));
            Assert.AreEqual(12, f(3, 9));
            Assert.AreEqual(12, f(5, 7));
            Assert.Throws<NotImplementedException>(() => f(2, 5));
        }
    }
}
