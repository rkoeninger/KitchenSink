using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ZedSharp.UnitTests
{
    [TestFixture]
    public class StreamTests
    {
        [Test]
        public void StreamSerialization()
        {
            var people = Seq.Of(
                new Person
                {
                    FirstName = "Rusty",
                    LastName = "Shackelford"
                },
                new Person
                {
                    FirstName = "Art",
                    LastName = "Vandelay"
                },
                new Person
                {
                    FirstName = "Kal",
                    LastName = "Varnsen"
                }
            );
            var stream = people.ToStream(Person.ToBytes);
            var bytes = new byte[4096];
            Assert.AreEqual(people.Sum(x => x.FirstName.Length + x.LastName.Length + 1), stream.Read(bytes, 0, bytes.Length));
        }

        public class Person
        {
            public String FirstName;
            public String LastName;

            public static IEnumerable<byte> ToBytes(Person p)
            {
                return Encoding.UTF8.GetBytes(p.FirstName + "|" + p.LastName);
            }

            public static Person FromBytes(byte[] b)
            {
                var split = Encoding.UTF8.GetString(b).Split('|');
                return new Person
                {
                    FirstName = split[0],
                    LastName = split[1]
                };
            }
        }
    }
}
