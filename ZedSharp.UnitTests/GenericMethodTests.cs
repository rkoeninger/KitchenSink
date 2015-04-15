using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class GenericMethodTests
    {
        [TestMethod]
        public void PredicateDispatchTest()
        {
            var method = new GenericMethod<int, int>()
                .AddLast(Z.Neg, Z.Negate)
                .AddLast(_ => true, Z.Id)
                .AsFunc();

            Assert.AreEqual(5, method(-5));
            Assert.AreEqual(2, method(2));
            Assert.AreEqual(0, method(0));
            Check.That(Sample.Ints.Except(int.MinValue), x => method(x) >= 0);
            Check.Idempotent(method);
        }

        [TestMethod]
        public void SubtypeDispatchTest()
        {
            var speak = new GenericMethod<IAnimal, String>()
                .AddLast<Dog>(x => x.Bark())
                .AddLast<Cat>(x => x.Purr())
                .AddLast<Duck>(x => x.Honk())
                .AsFunc();

            Assert.AreEqual("Woof", speak(new Dog()));
            Assert.AreEqual("Meow", speak(new Cat()));
            Assert.AreEqual("Quack", speak(new Duck()));
        }

        interface IAnimal { }

        class Dog : IAnimal
        {
            public String Bark()
            {
                return "Woof";
            }
        }

        class Cat : IAnimal
        {
            public String Purr()
            {
                return "Meow";
            }
        }

        class Duck : IAnimal
        {
            public String Honk()
            {
                return "Quack";
            }
        }
    }
}
