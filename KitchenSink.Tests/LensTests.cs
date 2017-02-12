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
            var fn = Lens.Gen<Person, String>("FirstName");
            var ln = Lens.Gen<Person, String>("LastName");
            var ct = Lens.Gen<Address, String>("City");

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
            public Person(String firstName, String lastName, Address address)
            {
                FirstName = firstName;
                LastName = lastName;
                Address = address;
            }

            public String FirstName { get; private set; }
            public String LastName { get; private set; }
            public Address Address { get; private set; }
        }

        public class Address
        {
            public Address(String street, String city)
            {
                Street = street;
                City = city;
            }

            public String Street { get; private set; }
            public String City { get; private set; }
        }
    }
}
