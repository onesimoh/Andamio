using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Andamio;
using Andamio.Serialization;

namespace Andamio
{
    public class BinaryContent
    {
        #region Constructors
        protected BinaryContent()
        {
        }

        public BinaryContent(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (!file.Exists) throw new FileNotFoundException("File Not Found.", file.FullName);

            Value = file.Bytes();
            Size = (int) file.Length;
        }

        public BinaryContent(string filePath)
        {
            if (filePath.IsNullOrBlank()) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException("File Not Found.", filePath);

            FileInfo file = new FileInfo(filePath);
            Value = file.Bytes();
            Size = (int) file.Length;
        }

        public BinaryContent(byte[] content)
        {
            if (content == null) throw new ArgumentNullException("content");
            Value = content;
            Size = content.Length;
        }

        public BinaryContent(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.Position = 0;

                Size = (int) reader.BaseStream.Length;                                
                byte[] buffer = new byte[Size];
                Value = reader.ReadBytes(Size);
            }
        }

        #endregion

        #region Content
        public byte[] Value { get; private set; }
        public int Size { get; private set; }

        #endregion

        #region Conversion
        public static implicit operator BinaryContent(FileInfo file)
        {
            return new BinaryContent(file);
        }

        public static implicit operator BinaryContent(byte[] content)
        {
            return new BinaryContent(content);
        }

        public static implicit operator byte[](BinaryContent binaryContent)
        {
            return binaryContent.Value;
        }

        #endregion

        #region IO
        public void WritoToFile(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                WritoToStream(stream);
            }
        }

        public void WritoToStream(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Value, 0, Size);
                writer.Close();
            }
        }

        public void CopyTo(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream(Value);
            memoryStream.CopyTo(stream);
        }

        public static BinaryContent ReadFromStream(Stream stream)
        {
            byte[] resourceContent = new byte[(int) stream.Length];
            stream.Read(resourceContent, 0, resourceContent.Length);
            return new BinaryContent(resourceContent);
        }

        #endregion

        #region Serialization
        public T Deserialize<T>()
        {
            GenericBinaryFormatter formatter = new GenericBinaryFormatter();
            return formatter.Deserialize<T>(Value);
        }

        #endregion
    }
}
