using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

using Andamio;
using Andamio.Serialization;


namespace Andamio
{
    public static class SerializationExtensions
    {
        #region Xml
        public static T DeserializeFromXml<T>(this XmlReader xmlReader)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xmlReader);
        }

        public static void XmlSerialize<T>(this XmlWriter xmlWriter, T graph)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(xmlWriter, graph);
        }

        public static void XmlSerialize<T>(this Stream stream, T graph)
        {
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true, IndentChars = ("\t") };
            XmlWriter xmlWriter = XmlWriter.Create(stream, settings);
            xmlWriter.XmlSerialize<T>(graph);
        }
        #endregion

        #region SerializationInfo
        public static void AddGuid(this SerializationInfo info, string key, Guid value)
        {
            info.AddValue(key, value, typeof(Guid)); 
        }

        public static Guid GetGuid(this SerializationInfo info, string key)
        {
            return (Guid) info.GetValue(key, typeof(Guid));
        }

        public static void AddEnum<EnumT>(this SerializationInfo info, string key, EnumT value)
            where EnumT : struct
        {
            info.AddValue(key, Convert.ToInt32(value)); 
        }

        public static EnumT GetEnum<EnumT>(this SerializationInfo info, string key)
            where EnumT : struct
        {
            return (EnumT) Enum.Parse(typeof(EnumT), info.GetString(key));
        }

        public static Nullable<T> GetNullable<T>(this SerializationInfo info, string key)
            where T : struct
        {
            var value = info.GetValue(key, typeof(T));
            if (value != null)
            {
                return (T) value;
            }
            else
            {
                return null;
            }
        }

        public static void AddSerializableValue<T>(this SerializationInfo info, string key, T value)
            where T : ISerializable
        {
            byte[] serializedValue = value.BinarySerialize<T>();
            info.AddValue(key, serializedValue, typeof(byte[]));
        }

        public static T GetSerializableValue<T>(this SerializationInfo info, string key)
            where T : ISerializable
        {            
            byte[] serializedData = info.GetValue(key, typeof(byte[])) as byte[];
            if (serializedData != null)
            {
                GenericBinaryFormatter formatter = new GenericBinaryFormatter();
                return formatter.Deserialize<T>(serializedData);
            }
            else
            {
                return default(T);
            }
        }

        public static void AddSerializableArray<T>(this SerializationInfo info, string key, T[] arrayValue)
            where T : ISerializable
        {
            GenericBinaryFormatter formatter = new GenericBinaryFormatter();
            byte[] serializedValue = formatter.Serialize<T[]>(arrayValue);
            info.AddValue(key, serializedValue, typeof(byte[]));
        }

        public static T[] GetSerializableArray<T>(this SerializationInfo info, string key)
            where T : ISerializable
        {
            byte[] serializedData = info.GetValue(key, typeof(byte[])) as byte[];

            T[] arrayValue = null;
            if (serializedData != null)
            {
                GenericBinaryFormatter formatter = new GenericBinaryFormatter();
                arrayValue = formatter.Deserialize<T[]>(serializedData);
            }

            return arrayValue;
        }

        #endregion

        #region Binary Serialization
        public static byte[] BinarySerialize<T>(this T graph)
            where T : ISerializable
        {
            GenericBinaryFormatter formatter = new GenericBinaryFormatter();
            return formatter.Serialize<T>(graph);
        }

        public static BinaryContent ToBinaryContent(this ISerializable serializable)
        {
            return serializable.BinarySerialize();
        }

        #endregion

        #region CSV
        public static string ToCSV<T>(this IEnumerable<T> items
            , string delimiter = ","
            , string defaultNullOrEmpty = ""
            , BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // Header
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties(bindingFlags);
            if (properties != null && properties.Any())
            {
                stringBuilder.AppendLine(properties.Select(p => String.Format("\"{0}\"", p.Name)).ToArray()
                    .JoinStrings(delimiter));
            }

            // Items
            if (items != null && items.Any())
            {
                foreach (T item in items)
                {
                    List<object> propertyValues = new List<object>();
                    foreach (PropertyInfo property in properties)
                    {
                        propertyValues.Add(property.GetValue(item, null));
                    }

                    stringBuilder.AppendLine(propertyValues.Select(v => String.Format("\"{0}\"", (v != null) ? v.ToString().Trim() : defaultNullOrEmpty))
                        .ToArray().JoinStrings(delimiter, ignoreNullOrEmpty: false));
                }
            }

            return stringBuilder.ToString();
        }

        public static void ToCSV<T>(this IEnumerable<T> items
            , string outputFile
            , string delimiter = ","
            , string defaultNullOrEmpty = ""
            , BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            if (outputFile.IsNullOrBlank()) throw new ArgumentNullException("outputFile");

            if (File.Exists(outputFile))
            { File.Delete(outputFile); }

            if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
            { Directory.CreateDirectory(Path.GetDirectoryName(outputFile)); }

            using (FileStream stream = File.OpenWrite(outputFile))
            {
                using (StreamWriter outfile = new StreamWriter(stream))
                {
                    outfile.Write(items.ToCSV<T>(delimiter, defaultNullOrEmpty, bindingFlags));
                }
            }
        }

        #endregion

        #region Dynamic
        public static dynamic Dynamic(this IDynamicMarshaling value)
        {
            return (value != null) ? value.Write() : null;
        }

        public static dynamic DynamicCollection(this IEnumerable<IDynamicMarshaling> values)
        {
            return (values != null) ? values.Select(value => value.Write()) : null;
        }

        #endregion
    }
}
