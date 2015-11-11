using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Andamio.Serialization
{
    /// <summary>
    /// Provides functionality for formatting serialized objects using the specified generic type formatter.
    /// </summary>
    /// <typeparam name="F">An object that implements the IFormatter interface.</typeparam>
    /// <remarks>Create other specific type for Formatters e.i SOAP</remarks>
    public class GenericFormatter<F> : IGenericFormatter where F : IFormatter, new()
    {
        protected readonly IFormatter _formatter = new F();

        /// <summary>
        /// Deserializes the data and reconstitutes the graph of objects.
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to deserialize.</typeparam>
        /// <param name="serializationStream">The stream that contains the data to deserialize.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        public T Deserialize<T>(Stream serializationStream)
        {
            return (T)_formatter.Deserialize(serializationStream);
        }

        /// <summary>
        /// Serializes an object, or graph of objects with the given root..
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to serialize.</typeparam>
        /// <param name="serializationStream">The stream where the formatter puts the serialized data.</param>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>
        public void Serialize<T>(Stream serializationStream, T graph)
        {
            _formatter.Serialize(serializationStream, graph);
        }
    }

    /// <summary>
    /// Provides functionality for formatting serialized objects to binary format.
    /// </summary>
    public class GenericBinaryFormatter : GenericFormatter<BinaryFormatter>
    {
        /// <summary>
        /// Serialize object into byte array.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A array of bytes that represent the serialized object.</returns>
        public byte[] Serialize<T>(T graph)
        {
            using (Stream stream = new MemoryStream())
            {
                Serialize<T>(stream, graph);
                stream.Seek(0, SeekOrigin.Begin);

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    byte[] buffer = new byte[reader.BaseStream.Length];
                    return reader.ReadBytes(buffer.Length);
                }
            }
        }

        public T Deserialize<T>(byte[] serialized)
        {            
            using (MemoryStream stream = new MemoryStream(serialized))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return Deserialize<T>(stream);                
            } 
        }
    }
}
