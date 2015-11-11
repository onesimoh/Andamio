using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Andamio
{
    static public class CompatiblePdfReader
    {
        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        static public PdfDocument Open(string pdfPath)
        {
            using (FileStream fileStream = new FileStream(pdfPath, FileMode.Open, FileAccess.Read))
            {
                PdfDocument document = Open(fileStream);
                fileStream.Close();
                return document;
            }
        }


        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        static public PdfDocument Open(FileStream fileStream)
        {
            int len = (int)fileStream.Length;
            Byte[] fileArray = new Byte[len];
            fileStream.Read(fileArray, 0, len);

            return Open(fileArray);
        }

        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        static public PdfDocument Open(byte[] fileArray)
        {
            return Open(new MemoryStream(fileArray));
        }


        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        static public PdfDocument Open(FileInfo file)
        {
            return Open(file.Bytes());
        }

        /// <summary>
        /// uses itextsharp 4.1.6 to convert any pdf to 1.4 compatible pdf, called instead of PdfReader.open
        /// </summary>
        static public PdfDocument Open(Stream stream)
        {
            PdfDocument outDoc = null;
            stream.Position = 0;

            try
            {
                outDoc = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
            }
            catch (PdfSharp.Pdf.IO.PdfReaderException)
            {
                //workaround if pdfsharp doesn't support this pdf
                stream.Position = 0;
                MemoryStream outputStream = new MemoryStream();
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(stream);
                iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
                pdfStamper.FormFlattening = true;
                pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
                pdfStamper.Writer.CloseStream = false;
                pdfStamper.Close();

                outDoc = PdfReader.Open(outputStream, PdfDocumentOpenMode.Import);
            }

            return outDoc;
        }
    }
}
