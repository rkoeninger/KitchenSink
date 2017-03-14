using KitchenSink.Testing;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class DynamicScoping
    {
        [Test]
        public void ValuesArePoppedWhenUsingEnds()
        {
            var scope = new DynamicScope();

            using (scope.Add("x", 1))
            {
                Assert.AreEqual(1, scope.Get("x"));

                using (scope.Add("x", 2))
                {
                    Assert.AreEqual(2, scope.Get("x"));

                    using (scope.Add("x", 3))
                    {
                        Assert.AreEqual(3, scope.Get("x"));
                    }
                    
                    Assert.AreEqual(2, scope.Get("x"));
                }
                
                Assert.AreEqual(1, scope.Get("x"));
            }

            Expect.Error(() => scope.Get("x"));
        }
    }
}
