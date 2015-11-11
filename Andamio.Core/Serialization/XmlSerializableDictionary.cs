using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;

namespace Andamio.Serialization
{
    /// <summary>
    /// Provides XML Serialization for Dictionary class.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [XmlRoot("Andamio.Serialization.XmlSerializableDictionary")]
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable
        //  interface, you should return null (Nothing in Visual Basic) from this method,
        //  and instead, if specifying a custom schema is required, apply the System.Xml.Serialization.XmlSchemaProviderAttribute
        //  to the class.
        /// </summary>
        /// <returns>An XmlSchema that describes the XML representation of the object that is produced by the WriteXml method and consumed by the ReadXml method.</returns>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader == null || reader.IsEmptyElement)
            { return; }

            XElement rootXElem = XElement.Load(reader);
            if (rootXElem.HasElements)
            {
                XElement itemsXElem = rootXElem.Element(XName.Get("Items"));
                if (itemsXElem != null && itemsXElem.HasElements)
                {
                    var items = itemsXElem.Descendants(XName.Get("Item"))
                        .Select(elem => new KeyValuePair<TKey, TValue>(GenericXmlSerializer.Deserialize<TKey>(elem.Element("Key").Value)
                            , GenericXmlSerializer.Deserialize<TValue>(elem.Element("Value").Value)));

                    foreach (KeyValuePair<TKey, TValue> item in items)
                    {
                        Add(item.Key, item.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation. 
        /// </summary>
        /// <param name="writer">The XmlWriter stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            XElement element = new XElement("Items", new XAttribute("Count", Count));
            if (Count > 0)
            {
                element.Add(this.Select(kvpItem => new XElement("Item",
                    new XElement("Key", GenericXmlSerializer.Serialize<TKey>(kvpItem.Key)),
                    new XElement("Value", GenericXmlSerializer.Serialize<TValue>(kvpItem.Value)))));
            }
            element.WriteTo(writer);
        }

        #endregion
    }
}


