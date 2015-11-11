using System;
using System.Collections.Generic;
using System.IO;

namespace Andamio.Serialization
{
    /// <summary>
    /// Provides functionality for formatting serialized objects.
    /// </summary>
    public interface IGenericFormatter
    {
        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects. 
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to deserialize.</typeparam>
        /// <param name="serializationStream">The stream that contains the data to deserialize.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        T Deserialize<T>( Stream serializationStream );
        
        /// <summary>
        /// Serializes an object, or graph of objects with the given root to the provided stream.
        /// </summary>
        /// <typeparam name="T">Generic Type of the object to serialize.</typeparam>
        /// <param name="serializationStream">The stream where the formatter puts the serialized data.</param>
        /// <param name="graph">The object, or root of the object graph, to serialize.</param>
        void Serialize<T>( Stream serializationStream, T graph );
    }
}
