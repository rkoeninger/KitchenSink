using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ZedSharp.UnitTests
{
    [TestClass]
    public class XmlTests
    {
        [TestMethod]
        public void BuildXml()
        {
            String xml;
            String str;

            xml = Xml.Doc < "Person" >= "id" <= "123" < "Name" < "First" <= "John" < "Last" <= "Smith" > Xml.End;
            str = "<Person id=\"123\"><Name><First>John</First><Last>Smith</Last></Name></Person>";
            Assert.IsTrue(xml.EndsWith(str));

            xml = Xml.Doc < "Root" >= "attr" <= "123" >= "attr2" <= "456" >= "attr3" <= "789" > Xml.End;
            str = "<Root attr=\"123\" attr2=\"456\" attr3=\"789\" />";
            Assert.IsTrue(xml.EndsWith(str));
        }
    }
}
