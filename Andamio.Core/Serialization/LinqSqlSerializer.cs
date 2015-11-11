using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.IO;

namespace Andamio.Serialization
{
    /// <summary>
    /// Utility class that provides serialization functionality for types decorated with DataContract Attribute..
    /// </summary>
    /// <typeparam name="T">Generic Type of the object for serialization.</typeparam>
    public static class LinqSqlSerializer
    {
        /// <summary>
        /// Serializes the specified object into an Xml String using DataContractSerializer.
        /// </summary>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>
        /// <returns>Serialized Xml string.</returns>
        public static string Serialize<T>(T graph)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            StringBuilder sb = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(sb))
            {
                serializer.WriteObject(writer, graph);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Deserializes Serialized Xml string and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="serializedString">Serialized Xml string.</param>
        /// <returns>The top object of the deserialized graph.</returns>                
        public static T Deserialize<T>(string serializedString)
        {
            T graph = default(T);

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            using (XmlReader reader = XmlReader.Create(new StringReader(serializedString)))
            {
                graph = (T)serializer.ReadObject(reader);
            }
            return graph;
        }
    }
}
