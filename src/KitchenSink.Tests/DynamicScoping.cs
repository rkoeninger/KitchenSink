using NUnit.Framework;

namespace KitchenSink.Tests
{
    public class DynamicScoping
    {
        [Test]
        public void ValuesArePoppedWhenUsingBlockEnds()
        {
            using (Scope.Push("x", 1))
            {
                Assert.AreEqual(1, Scope.Get("x"));

                using (Scope.Push("x", 2))
                {
                    OtherMethod();
                }

                Assert.AreEqual(1, Scope.Get("x"));
            }

            Expect.Error(() => Scope.Get("x"));
        }

        private static void OtherMethod()
        {
            Assert.AreEqual(2, Scope.Get("x"));

            using (Scope.Push("x", 3))
            {
                Assert.AreEqual(3, Scope.Get("x"));
            }

            Assert.AreEqual(2, Scope.Get("x"));
        }

        public interface IRepository { }
        public class Repository : IRepository { }

        [Test]
        public void ValuesCanBeIndexedByStaticType()
        {
            using (Scope.Push<IRepository>(new Repository()))
            {
                Assert.IsInstanceOf<Repository>(Scope.Get<IRepository>());
            }

            Expect.Error(() => Scope.Get<IRepository>());
        }
    }
}
