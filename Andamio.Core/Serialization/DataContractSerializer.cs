using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.IO;

namespace Andamio.Serialization
{
   public class DataContractSerializer<T> : XmlObjectSerializer
   {
        #region Private Fields
        DataContractSerializer _dataContractSerializer;

        #endregion

        #region Constructors
        public DataContractSerializer()
        {
            _dataContractSerializer = new DataContractSerializer(typeof(T));
        }      
        public DataContractSerializer(IList<Type> knownTypes)
        {
            _dataContractSerializer = new DataContractSerializer(typeof(T),knownTypes);
        }

        #endregion

        #region Read, Write
        public override object ReadObject(XmlDictionaryReader reader)
        {
            return _dataContractSerializer.ReadObject(reader);
        }
        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            return _dataContractSerializer.IsStartObject(reader);
        }
        public override object ReadObject(XmlDictionaryReader reader,bool verifyObjectName)
        {
            return _dataContractSerializer.ReadObject(reader,verifyObjectName);
        }
        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            _dataContractSerializer.WriteEndObject(writer);
        }
        public override void WriteObjectContent(XmlDictionaryWriter writer,object graph)
        {
            _dataContractSerializer.WriteObjectContent(writer,graph);
        }
        public override void WriteStartObject(XmlDictionaryWriter writer,object graph)
        {
            _dataContractSerializer.WriteStartObject(writer,graph);
        }
        public new T ReadObject(Stream stream)
        {
            return (T)_dataContractSerializer.ReadObject(stream);
        }
        public new T ReadObject(XmlReader reader)
        {
            return (T)_dataContractSerializer.ReadObject(reader);
        }
        public new bool IsStartObject(XmlReader reader)
        {
            return _dataContractSerializer.IsStartObject(reader);
        }
        public new T ReadObject(XmlReader reader,bool verifyObjectName)
        {
            return (T)_dataContractSerializer.ReadObject(reader,verifyObjectName);
        }
        public new void WriteEndObject(XmlWriter writer)
        {
            _dataContractSerializer.WriteEndObject(writer);
        }
        public void WriteObject(Stream stream,T graph)
        {
            _dataContractSerializer.WriteObject(stream,graph);
        }
        public void WriteObject(XmlDictionaryWriter writer,T graph)
        {
            _dataContractSerializer.WriteObject(writer,graph);
        }
        public void WriteObject(XmlWriter writer,T graph)
        {
            _dataContractSerializer.WriteObject(writer,graph);
        }
        public void WriteObjectContent(XmlWriter writer,T graph)
        {
            _dataContractSerializer.WriteObjectContent(writer,graph);
        }
        public void WriteStartObject(XmlWriter writer,T graph)
        {
            _dataContractSerializer.WriteStartObject(writer,graph);
        }

        #endregion
   }
}
