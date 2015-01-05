using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class TypeTreeTests
    {
        [TestMethod]
        public void TypeTreeLookups()
        {
            var tree = new TypeTree<int>();
            tree.Set<Shape>(1);
            tree.Set<Circle>(11);
            tree.Set<Para>(3);
            tree.Set<Isosceles>(9);
            tree.Set<Polygon>(6);
            tree.Set<IRegular>(15);
            tree.Set<IEssential>(20);

            Assert.AreEqual(1, tree.Get<Shape>());
            Assert.AreEqual(6, tree.Get<Quad>());
            Assert.AreEqual(1, tree.Get<Ellipse>());
            Assert.AreEqual(11, tree.Get<Circle>());
            Expect.Error<KeyNotFoundException>(() => tree.Get<Object>());
            Expect.Error<KeyNotFoundException>(() => tree.Get<String>());
            Assert.AreEqual(3, tree.Get<Rhombus>());
            Assert.AreEqual(9, tree.Get<Equilateral>());
        }

        public interface ISemiRegular { }
        public interface IRegular : ISemiRegular { }
        public interface IEssential { }
        public class Shape { }
        public class Polygon : Shape { }
        public class Quad : Polygon { }
        public class Para : Quad { }
        public class Rhombus : Para, ISemiRegular { }
        public class Trap : Quad { }
        public class Kite : Quad { }
        public class Rect : Quad, ISemiRegular, IEssential { }
        public class Square : Rect, IRegular { }
        public class Triangle : Polygon { }
        public class Isosceles : Triangle, ISemiRegular { }
        public class Equilateral : Isosceles, IRegular, IEssential { }
        public class Ellipse : Shape { }
        public class Circle : Ellipse, IRegular { }
    }
}
