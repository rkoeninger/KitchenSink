using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZedSharp.Test;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class LensTests
    {
        [TestMethod]
        public void LensComposition()
        {
            var fn = Lens.From<Person>().Of(x => x.FirstName, (x, y) => new Person(y, x.LastName, x.Address));
            var ln = Lens.From<Person>().Of(x => x.LastName, (x, y) => new Person(x.FirstName, y, x.Address));
            var addr = Lens.From<Person>().Of(x => x.Address, (x, y) => new Person(x.FirstName, x.LastName, y));
            var st = Lens.From<Address>().Of(x => x.Street, (x, y) => new Address(y, x.City));
            var ct = Lens.From<Address>().Of(x => x.City, (x, y) => new Address(x.Street, y));

            var addr_ct = addr.Compose(ct);

            var address = new Address("123 Fake Street", "Anytown");
            var person = new Person("John", "Doe", address);

            Assert.AreEqual("Anytown", addr_ct.Get(person));
            Assert.AreEqual("Someville", addr_ct.Set(person, "Someville").Address.City);
        }

        [TestMethod]
        public void LensGeneration()
        {
            var fn = Lens.Gen<Person, String>("FirstName");
            var ln = Lens.Gen<Person, String>("LastName");
            var addr = Lens.Gen<Person, Address>("Address");
            var st = Lens.Gen<Address, String>("Street");
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
    }

    public struct Person
    {
        public Person(String firstName, String lastName, Address address) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
        }

        public String FirstName { get; private set; }
        public String LastName { get; private set; }
        public Address Address { get; private set; }
    }

    public struct Address
    {
        public Address(String street, String city) : this()
        {
            Street = street;
            City = city;
        }

        public String Street { get; private set; }
        public String City { get; private set; }
    }
}
