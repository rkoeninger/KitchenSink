using System;
using NUnit.Framework;

namespace KitchenSink.Tests
{
    [TestFixture]
    public class LensTests
    {
        [Test]
        public void LensComposition()
        {
            var addr = Lens.From<Person>().Of(x => x.Address, (x, y) => new Person(x.FirstName, x.LastName, y));
            var ct = Lens.From<Address>().Of(x => x.City, (x, y) => new Address(x.Street, y));

            var addrCt = addr.Compose(ct);

            var address = new Address("123 Fake Street", "Anytown");
            var person = new Person("John", "Doe", address);

            Assert.AreEqual("Anytown", addrCt.Get(person));
            Assert.AreEqual("Someville", addrCt.Set(person, "Someville").Address.City);
        }

        [Test]
        public void LensGen()
        {
            var fn = Lens.Gen<Person, string>("FirstName");
            var ln = Lens.Gen<Person, string>("LastName");
            var ct = Lens.Gen<Address, string>("City");

            var address = new Address("123 Fake Street", "Anytown");
            var person = new Person("John", "Doe", address);

            Assert.AreEqual("Anytown", ct.Get(address));
            Assert.AreEqual("Doe", ln.Get(person));
            Assert.AreEqual("Someville", ct.Set(address, "Someville").City);
            Assert.AreEqual("John", fn.Set(person, "John").FirstName);

            Expect.Error(() => Lens.Gen<Person, DateTime>("Birthday"));
            Expect.Error(() => Lens.Gen<Address, int>("Street"));
        }

        [Test]
        public void GenFromGetterExprUsingBuilder()
        {
            var fn = Lens.From<Person>().Gen(x => x.FirstName);
            var ln = Lens.From<Person>().Gen(x => x.LastName);
            var ct = Lens.From<Address>().Gen(x => x.City);

            var address = new Address("123 Fake Street", "Anytown");
            var person = new Person("John", "Doe", address);

            Assert.AreEqual("Anytown", ct.Get(address));
            Assert.AreEqual("Doe", ln.Get(person));
            Assert.AreEqual("Someville", ct.Set(address, "Someville").City);
            Assert.AreEqual("John", fn.Set(person, "John").FirstName);
        }

        [Test]
        public void GenFromGetterExpr()
        {
            var fn = Lens.Gen((Person x) => x.FirstName);
            var ln = Lens.Gen((Person x) => x.LastName);
            var ct = Lens.Gen((Address x) => x.City);

            var address = new Address("123 Fake Street", "Anytown");
            var person = new Person("John", "Doe", address);

            Assert.AreEqual("Anytown", ct.Get(address));
            Assert.AreEqual("Doe", ln.Get(person));
            Assert.AreEqual("Someville", ct.Set(address, "Someville").City);
            Assert.AreEqual("John", fn.Set(person, "John").FirstName);
        }

        public class Person
        {
            public Person(string firstName, string lastName, Address address)
            {
                FirstName = firstName;
                LastName = lastName;
                Address = address;
            }

            public string FirstName { get; }
            public string LastName { get; }
            public Address Address { get; }
        }

        public class Address
        {
            public Address(string street, string city)
            {
                Street = street;
                City = city;
            }

            public string Street { get; }
            public string City { get; }
        }
    }
}
