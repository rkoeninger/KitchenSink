using System;
using System.Text;
using System.Xml;

namespace KitchenSink
{
    /// <summary>A stateful XML building stream.</summary>
    public class Xml
    {
        /// <summary>Starts new XML document.</summary>
        public static XmlStart Doc => default;

        /// <summary>Starts new indented XML document.</summary>
        public static XmlStart IndentedDoc =>
            new XmlStart(new XmlWriterSettings { Indent = true, IndentChars = "    " });

        /// <summary>Used to close open tag with <code>&lt; Xml.End</code>.</summary>
        public static int End => 1;

        /// <summary>Used to close <code>x</code> levels of open tags with <code>&lt; Xml.EndMany(3)</code>.</summary>
        public static int EndMany(int x) => x;

        /// <summary>Used to close all remaining open tags with <code>&lt; Xml.EndDoc</code>.</summary>
        public static int EndDoc => -1;

        /// <summary>Opens new tag.</summary>
        public static Xml operator <(Xml xml, string tagName)
        {
            xml.Writer.WriteStartElement(tagName);
            xml.currentDepth++;
            return xml;
        }

        /// <summary>Closes current tag.</summary>
        public static Xml operator >(Xml xml, string _)
        {
            xml.Writer.WriteEndElement();
            xml.currentDepth--;
            return xml;
        }
        
        /// <summary>Undefined. Throws InvalidOperationException.</summary>
        public static Xml operator <(Xml _0, int _1) => throw new InvalidOperationException();

        /// <summary>Closes <code>depth</code> number of preceding open tags. <code>-1</code> closes all previous tags.</summary>
        public static Xml operator >(Xml xml, int depth)
        {
            if (depth < -1)
            {
                throw new InvalidOperationException();
            }

            if (depth == -1)
            {
                while (xml.currentDepth > 0)
                {
                    xml.Writer.WriteEndElement();
                    xml.currentDepth--;
                }
            }
            else if (depth > 1)
            {
                while (xml.currentDepth > 0 || depth > 0)
                {
                    xml.Writer.WriteEndElement();
                    xml.currentDepth--;
                    depth--;
                }
            }

            return xml;
        }

        /// <summary>Inserts value into previous element/attribute and closes it.</summary>
        public static Xml operator <=(Xml xml, string tagValue)
        {
            // write value and close
            switch (xml.Writer.WriteState)
            {
            case WriteState.Attribute:
                xml.Writer.WriteValue(tagValue);
                xml.Writer.WriteEndAttribute();
                xml.currentDepth--;
                break;
            case WriteState.Element:
                xml.Writer.WriteValue(tagValue);
                xml.Writer.WriteEndElement();
                xml.currentDepth--;
                break;
            default:
                throw new InvalidOperationException();
            }

            return xml;
        }

        /// <summary>Starts an attribute. Value must next be specified with &lt;=.</summary>
        public static Xml operator >=(Xml xml, string attrName)
        {
            xml.Writer.WriteStartAttribute(attrName);
            xml.currentDepth++;
            return xml;
        }

        public static implicit operator string(Xml xml) => xml.ToString();

        internal Xml(string rootTagName, XmlWriterSettings settings)
        {
            currentDepth = 1;
            output = new StringBuilder();
            Writer = XmlWriter.Create(output, settings);
            Writer.WriteStartElement(rootTagName);
        }

        internal readonly XmlWriter Writer;
        private readonly StringBuilder output;
        private int currentDepth;

        public override string ToString()
        {
            Writer.Flush();
            return output.ToString();
        }
    }

    public readonly struct XmlStart
    {
        /// <summary>Opens root tag.</summary>
        public static Xml operator <(XmlStart start, string rootTagName) =>
            new Xml(rootTagName, start.settings);

        /// <summary>
        /// Undefined. Throws InvalidOperationException.
        /// 
        /// This operator is completely unnecessary but the compiler requires
        /// a matching &gt; operator for the defined &lt; operator.
        /// </summary>
        public static Xml operator >(XmlStart _0, string _1) =>
            throw new InvalidOperationException();

        internal XmlStart(XmlWriterSettings settings) : this() => this.settings = settings;

        private readonly XmlWriterSettings settings;
    }
}
