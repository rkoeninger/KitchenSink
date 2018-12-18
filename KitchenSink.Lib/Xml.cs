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
            xml.CurrentDepth++;
            return xml;
        }

        /// <summary>Closes current tag.</summary>
        public static Xml operator >(Xml xml, string tagName)
        {
            xml.Writer.WriteEndElement();
            xml.CurrentDepth--;
            return xml;
        }
        
        /// <summary>Undefined. Throws InvalidOperationException.</summary>
        public static Xml operator <(Xml xml, int depth) => throw new InvalidOperationException();

        /// <summary>Closes <code>depth</code> number of preceding open tags. <code>-1</code> closes all previous tags.</summary>
        public static Xml operator >(Xml xml, int depth)
        {
            if (depth < -1)
            {
                throw new InvalidOperationException();
            }

            if (depth == -1)
            {
                while (xml.CurrentDepth > 0)
                {
                    xml.Writer.WriteEndElement();
                    xml.CurrentDepth--;
                }
            }
            else if (depth > 1)
            {
                while (xml.CurrentDepth > 0 || depth > 0)
                {
                    xml.Writer.WriteEndElement();
                    xml.CurrentDepth--;
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
                xml.CurrentDepth--;
                break;
            case WriteState.Element:
                xml.Writer.WriteValue(tagValue);
                xml.Writer.WriteEndElement();
                xml.CurrentDepth--;
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
            xml.CurrentDepth++;
            return xml;
        }

        public static implicit operator string(Xml xml) => xml.ToString();

        internal Xml(string rootTagName, XmlWriterSettings settings)
        {
            CurrentDepth = 1;
            Output = new StringBuilder();
            Writer = XmlWriter.Create(Output, settings);
            Writer.WriteStartElement(rootTagName);
        }

        internal readonly XmlWriter Writer;
        private readonly StringBuilder Output;
        private int CurrentDepth;

        public override string ToString()
        {
            Writer.Flush();
            return Output.ToString();
        }
    }

    public readonly struct XmlStart
    {
        /// <summary>Opens root tag.</summary>
        public static Xml operator <(XmlStart start, string rootTagName) =>
            new Xml(rootTagName, start.Settings);

        /// <summary>
        /// Undefined. Throws InvalidOperationException.
        /// 
        /// This operator is completely unnecessary but the compiler requires
        /// a matching &gt; operator for the defined &lt; operator.
        /// </summary>
        public static Xml operator >(XmlStart start, string rootTagName) =>
            throw new InvalidOperationException();

        internal XmlStart(XmlWriterSettings settings) : this() => Settings = settings;

        private readonly XmlWriterSettings Settings;
    }
}
