using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Andamio.Serialization
{
    /// <summary>
    /// Provides Xml Serialization utilities. 
    /// </summary>
    /// <typeparam name="T">Generic Type of the object for serialization.</typeparam>
    public static class GenericXmlSerializer
    {
        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects. 
        /// </summary>
        /// <param name="serializationStream">The stream that contains the data to deserialize.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        public static T Deserialize<T>( TextReader reader )
        {
            if( reader == null )
                throw new ArgumentNullException( "TextReader reader" );
            
            XmlSerializer serializer = new XmlSerializer( typeof(T) );
            T graph = (T) serializer.Deserialize( reader );
            
            return graph;
        }

        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <param name="serializationStream">The stream where the formatter puts the serialized data.</param>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>
        public static void Serialize<T>( TextWriter writer, T graph )
        {
            if( writer == null )
                throw new ArgumentNullException( "TextWriter writer" );
                
            if( graph != null )
            {                
                XmlSerializer serializer = new XmlSerializer( typeof(T) );                
                serializer.Serialize( writer, graph );
            }
        }
        
        /// <summary>
        /// Serializes the specified object into an Xml String using XmlSerializer.
        /// </summary>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>
        /// <returns>Serialized Xml string.</returns>
        public static string Serialize<T>( T graph )
        {
            string serializedString = null;
            
            if( graph != null )
            {
                using( StringWriter writer = new StringWriter() )
                {
                    Serialize<T>( writer, graph );
                    serializedString = writer.ToString();
                }
            }           
            
            return serializedString;         
        
        }
        
        /// <summary>
        /// Deserializes Serialized Xml string and reconstitutes the graph of objects.
        /// </summary>
        /// <param name="serializedXmlString">Serialized Xml string.</param>
        /// <returns>The top object of the deserialized graph.</returns>                
        public static T Deserialize<T>( string serializedXmlString )
        {
            T graph = default(T);
            
            if( !String.IsNullOrEmpty( serializedXmlString ) )
            {
                using( StringReader reader = new StringReader( serializedXmlString ) )
                {
                    graph = Deserialize<T>( reader );
                }
            }
            
            return graph;
        }
    }
}
