using NUnit.Framework;
using static NUnit.Framework.Assert;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class NewTypeTests
    {
        public sealed class CustomerId : NewType<int> { public CustomerId(int x) : base(x) {} }
        public sealed class ProductCode : NewType<string> { public ProductCode(string x) : base(x) {} }

        [Test]
        public void NewTypeEquality()
        {
            var c1 = new CustomerId(453);
            var c2 = new CustomerId(453);
            var c3 = new CustomerId(521);
            const int x1 = 453;

            AreEqual(c1, c2);    // Same wrapped value
            AreNotEqual(c1, c3); // Different values
            AreNotEqual(c1, x1); // NewType<A> and A are never equal
        }

        [Test]
        public void NewTypeHashCode()
        {
            var c1 = new CustomerId(946);
            const int x1 = 946;

            AreEqual(x1.GetHashCode(), c1.GetHashCode());
        }

        [Test]
        public void NewTypeToString()
        {
            AreEqual("", new ProductCode(null).ToString());
            AreEqual("54F23N", new ProductCode("54F23N").ToString());
            AreEqual("123", new CustomerId(123).ToString());
        }
    }
}
