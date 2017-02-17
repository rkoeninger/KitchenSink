using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class NewTypeTests
    {
        public sealed class CustomerId : NewType<int> { public CustomerId(int x) : base(x) {} }
        public sealed class ProductCode : NewType<string> { public ProductCode(string x) : base(x) {} }

        [Test]
        public void NewTypeTypeChecking()
        {
            Expect.CompileFail(
                @"using KitchenSink;
                public sealed class CustomerId : NewType<int> { public CustomerId(int x) : base(x) {} }
                public static class TestClass
                {
                    static void SomethingWithCustomer(CustomerId x)
                    {
                    }
                    static void Test()
                    {
                        SomethingWithCustomer(123);
                    }
                }",
                new [] {Common.KsDll},
                new []
                {
                    "CS1502", // SomethingWithCustomer has some invalid arguments
                    "CS1503"  // Cannot convert from int to CustomerId
                });
        }

        [Test]
        public void NewTypeEquality()
        {
            var c1 = new CustomerId(453);
            var c2 = new CustomerId(453);
            var c3 = new CustomerId(521);
            const int x1 = 453;

            Assert.AreEqual(c1, c2);    // Same wrapped value
            Assert.AreNotEqual(c1, c3); // Different values
            Assert.AreNotEqual(c1, x1); // NewType<A> and A are never equal
        }

        [Test]
        public void NewTypeHashCode()
        {
            var c1 = new CustomerId(946);
            const int x1 = 946;

            Assert.AreEqual(x1.GetHashCode(), c1.GetHashCode());
        }

        [Test]
        public void NewTypeToString()
        {
            Assert.AreEqual("", new ProductCode(null).ToString());
            Assert.AreEqual("54F23N", new ProductCode("54F23N").ToString());
            Assert.AreEqual("123", new CustomerId(123).ToString());
        }
    }
}
