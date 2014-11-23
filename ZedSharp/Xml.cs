using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZedSharp
{
    /// <summary>A stateful XML building stream.</summary>
    public class Xml
    {
        /// <summary>Starts new XML document.</summary>
        public static XmlStart Doc
        {
            get { return default(XmlStart); }
        }

        /// <summary>Starts new indented XML document.</summary>
        public static XmlStart IndentedDoc
        {
            get { return new XmlStart(new XmlWriterSettings { Indent = true, IndentChars = "    " }); }
        }

        /// <summary>Used to close all remaining open tags with <code>&lt; Xml.End</code>.</summary>
        public static XmlEnd End
        {
            get { return default(XmlEnd); }
        }
        
        /// <summary>Undefined. Throws InvalidOperationException.</summary>
        public static String operator <(Xml xml, XmlEnd end)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Closes all preceding tags.</summary>
        public static String operator >(Xml xml, XmlEnd end)
        {
            return (xml > -1).ToString();
        }

        /// <summary>Opens new tag.</summary>
        public static Xml operator <(Xml xml, String tagName)
        {
            xml.Writer.WriteStartElement(tagName);
            xml.CurrentDepth++;
            return xml;
        }

        /// <summary>Closes current tag.</summary>
        public static Xml operator >(Xml xml, String tagName)
        {
            xml.Writer.WriteEndElement();
            xml.CurrentDepth--;
            return xml;
        }
        
        /// <summary>Undefined. Throws InvalidOperationException.</summary>
        public static Xml operator <(Xml xml, int depth)
        {
            throw new InvalidOperationException();
        }

        /// <summary>Closes <code>depth</code> number of preceding open tags. <code>-1</code> closes all previous tags.</summary>
        public static Xml operator >(Xml xml, int depth)
        {
            if (depth < -1)
            {
                throw new InvalidOperationException();
            }
            else if (depth == -1)
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
        public static Xml operator <=(Xml xml, String tagValue)
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
        public static Xml operator >=(Xml xml, String attrName)
        {
            xml.Writer.WriteStartAttribute(attrName);
            xml.CurrentDepth++;
            return xml;
        }

        public static implicit operator String(Xml xml)
        {
            return xml.ToString();
        }

        internal Xml(String rootTagName, XmlWriterSettings settings)
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

    public struct XmlStart
    {
        /// <summary>Opens root tag.</summary>
        public static Xml operator <(XmlStart start, String rootTagName)
        {
            return new Xml(rootTagName, start.Settings);
        }

        /// <summary>Undefined. Throws InvalidOperationException.</summary>
        public static Xml operator >(XmlStart start, String rootTagName)
        {
            throw new InvalidOperationException();
        }

        internal XmlStart(XmlWriterSettings settings) : this()
        {
            Settings = settings;
        }

        private readonly XmlWriterSettings Settings;
    }

    public struct XmlEnd
    {

    }
}
