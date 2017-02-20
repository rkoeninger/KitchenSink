using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class TypeTreeTests
    {
        [Test]
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

            Expect.Some(1, tree.Get<Shape>());
            Expect.Some(6, tree.Get<Quad>());
            Expect.Some(1, tree.Get<Ellipse>());
            Expect.Some(11, tree.Get<Circle>());
            Expect.None(tree.Get<object>());
            Expect.None(tree.Get<string>());
            Expect.Some(3, tree.Get<Rhombus>());
            Expect.Some(9, tree.Get<Equilateral>());
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
