using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Andamio
{
    public static class FileExtensions
    {
        #region Content
        public static byte[] Bytes(this FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (!file.Exists) throw new FileNotFoundException(file.FullName);

            using (FileStream fileStrean = file.OpenRead())
            {
                using (BinaryReader binReader = new BinaryReader(fileStrean))
                {
                    return binReader.ReadBytes((int) file.Length);
                }
            }
        }

        public static byte[] Bytes(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int) stream.Length);

            return bytes;
        }

        public static string Content(this FileInfo file)
        {
            using (FileStream stream = file.OpenRead())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string Content(this Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            stream.Rewind();
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string Content(this MemoryStream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            stream.Rewind();
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static void ToFile(this string text, string path)
        {
            using (StreamWriter outfile = new StreamWriter(path))
            {
                outfile.Write(text);
            }
        }

        public static void Rewind(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            stream.Position = 0;
        }

        public static IEnumerable<string> Lines(this string text)
        {
            if (text.IsNullOrBlank()) throw new ArgumentNullException("text");

            using (StringReader reader = new StringReader(text))
            {                
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line == null) break;
                    yield return line;
                }
            }
        }

        public static string Line(this string text, int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException("index");
            return text.Lines().ElementAt(index);
        }

        #endregion

        #region File Type
        public static bool IsDefined(this FileType fileType)
        {
            return fileType != FileType.Unknown;
        }

        public static bool IsPdf(this FileType fileType)
        {
            return fileType == FileType.Pdf;
        }

        public static FileType GetFileType(this FileInfo file)
        {
            if (file == null)
            { throw new ArgumentNullException("file"); }

            return GetFileType(file.Name);
        }

        public static FileType GetFileType(this string fileName)
        {
            if (fileName.IsNullOrBlank())
            { throw new ArgumentNullException("fileName"); }

            string fileExtension = Path.GetExtension(fileName);

            if (fileExtension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Pdf;
            }
            else if ((fileExtension.Equals(".doc", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase)))
            {
                return FileType.Word;
            }
            else if ((fileExtension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".xls", StringComparison.OrdinalIgnoreCase)))
            {
                return FileType.Excel;
            }
            else if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Text;
            }
            else if ((fileExtension.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".bmp", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".gif", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".ico", StringComparison.OrdinalIgnoreCase)))
            {
                return FileType.Image;
            }
            else if ((fileExtension.Equals(".htm", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".html", StringComparison.OrdinalIgnoreCase)))
            {
                return FileType.Html;
            }
            else if (fileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                return FileType.Xml;
            }
            else if ((fileExtension.Equals(".bin", StringComparison.OrdinalIgnoreCase))
                || (fileExtension.Equals(".binary", StringComparison.OrdinalIgnoreCase)))
            {
                return FileType.Binary;
            }
            else
            {
                return FileType.Unknown;
            }
        }

        public static string ToContentType(this FileType fileType)
        {
            string contentType;
            switch (fileType)
            {
                case FileType.Excel:
                    contentType = "application/vnd.ms-excel";
                    break;

                case FileType.Html:
                    contentType = "text/html";
                    break;

                case FileType.Image:
                    contentType = "application/octet-stream";
                    break;

                case FileType.Pdf:
                    contentType = "application/pdf";
                    break;

                case FileType.Text:
                    contentType = "text/plain";
                    break;
                
                case FileType.Word:
                    contentType = "application/msword";
                    break;

                case FileType.Xml:
                    contentType = "text/xml";
                    break;

                case FileType.Binary:
                case FileType.Unknown:
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            return contentType;
        }

        #endregion

        #region FileVersionInfo (Format)
        static public string Format(this FileVersionInfo fileVersion)
        {
            if (fileVersion.FileMajorPart == 0
                && fileVersion.FileMinorPart == 0
                && fileVersion.FileBuildPart == 0
                && fileVersion.FilePrivatePart == 0)
            {
                // There is no version
                return String.Empty;
            }
            else
            {
                return String.Format("{0}.{1}.{2}.{3}"
                    , fileVersion.FileMajorPart
                    , fileVersion.FileMinorPart
                    , fileVersion.FileBuildPart
                    , fileVersion.FilePrivatePart);
            }
        }
        #endregion

        #region FileInfo - Attributes
        static public void MakeWriteable(this FileInfo fileInfo)
        {
            fileInfo.RemoveAttributes(FileAttributes.ReadOnly);
        }

        static public void MakeCompressed(this FileInfo fileInfo)
        {
            fileInfo.ApplyAttributes(FileAttributes.Compressed);
        }

        static public void RemoveAttributes(this FileInfo fileInfo, FileAttributes attributes)
        {
            FileAttributes currentAttributes = fileInfo.Attributes;
            if ((currentAttributes & attributes) != 0)
            {
                fileInfo.Attributes = (fileInfo.Attributes & ~attributes);
            }
        }

        static public void ApplyAttributes(this FileInfo fileInfo, FileAttributes attributes)
        {
            FileAttributes currentAttributes = fileInfo.Attributes;
            if ((currentAttributes & attributes) != attributes)
            {
                fileInfo.Attributes = (fileInfo.Attributes | attributes);
            }
        }

        public static bool IsReadOnly(this FileAttributes attributes)
        {
            return (attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        #endregion

        #region FileInfo - File Version
        /// <summary>
        /// Sets the file version.
        /// </summary>
        /// <param name="fileInfo">The file to set the new verison to.</param>
        /// <param name="newVersion">The new version.</param>
        public static void SetVersion(this FileInfo fileInfo, Version newVersion)
        {
            throw new NotImplementedException();
            //.SetFileVersion(fileInfo, newVersion);
        }

        /// <summary>
        /// Sets the file version.
        /// </summary>
        /// <param name="fileInfo">The file to set the new verison to.</param>
        /// <param name="newVersion">The new version.</param>
        public static void SetVersion(this FileInfo fileInfo, string newVersion)
        {
            throw new NotImplementedException();
            //Version version = new Version(newVersion);
            //.SetFileVersion(fileInfo, version);
        }

        #endregion

        #region Manipulation
        public static bool Move(this FileInfo fileInfo, string targetDirectory, bool createTargetDirectory = true, bool throwException = true)
        {
            if (fileInfo == null) throw new ArgumentNullException("fileInfo");
            if (targetDirectory.IsNullOrBlank()) throw new ArgumentNullException("targetDirectory");
            if (!fileInfo.Exists) return false;

            try
            {
                if (createTargetDirectory)
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                string destinationPath = Path.Combine(targetDirectory, fileInfo.Name);
                if (File.Exists(destinationPath))
                {
                    string name = String.Format("{0}.{1:yymmddhhmmss}{2}"
                        , Path.GetFileNameWithoutExtension(fileInfo.Name)
                        , DateTime.Now
                        , Path.GetExtension(fileInfo.Name));
                    destinationPath = Path.Combine(targetDirectory, name);
                }

                fileInfo.MoveTo(destinationPath);

                return true;
            }
            catch (Exception e)
            {
                if (throwException) throw e;
                return false;
            }
        }

        /// <summary>
        /// Waits until a file can be opened with write permission
        /// </summary>
        public static FileStream OpenWhenReady(this FileInfo file
            , FileMode mode = FileMode.Open
            , FileAccess access = FileAccess.Read
            , FileShare share = FileShare.Read)
        {
            if (file == null) throw new ArgumentNullException("file");

            while (true)
            {
                try
                {
                    FileStream stream = file.Open(mode, access, share);
                    return stream;
                }
                catch
                {
                    System.Threading.Thread.Sleep(1000);   
                }
            }
        }

        #endregion
    }
}
