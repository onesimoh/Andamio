using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Pdf
{
    public static class iTextExtensions
    {
        public static PdfSharp.Pdf.PdfDocument Add(this PdfSharp.Pdf.PdfDocument document, FileInfo file)
        {
            if (document == null) throw new ArgumentNullException("document");
            if (file == null) throw new ArgumentNullException("file");

            PdfSharp.Pdf.PdfDocument reportDocument = CompatiblePdfReader.Open(file);
            foreach (PdfSharp.Pdf.PdfPage page in reportDocument.Pages)
            {
                document.AddPage(page); 
            }

            return document;
        }

        public static PdfSharp.Pdf.PdfDocument Add(this PdfSharp.Pdf.PdfDocument document, Stream stream)
        {
            if (document == null) throw new ArgumentNullException("document");
            if (stream == null) throw new ArgumentNullException("file");

            PdfSharp.Pdf.PdfDocument reportDocument = CompatiblePdfReader.Open(stream);
            foreach (PdfSharp.Pdf.PdfPage page in reportDocument.Pages)
            {
                document.AddPage(page);
            }

            return document;
        }

        public static FileInfo ToFile(this PdfSharp.Pdf.PdfDocument document, string path)
        {
            if (document == null) throw new ArgumentNullException("document");
            if (path.IsNullOrBlank()) throw new ArgumentNullException("path");

            if (document.PageCount == 0)
            { throw new InvalidOperationException("An attempt to generate the PDf File failed because no documents were available."); }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            document.Save(path);

            return new FileInfo(path);
        }
    }
}
