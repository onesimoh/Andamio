using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Web;

using Andamio;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class PdfManifest
    {
        #region Constructors
        private PdfManifest()
        {
            Styles = new PdfManifestStyles();

            Pages = new PdfPages();
            Pages.ItemsInserted += OnPagesInserted;

            DocumentSize = PdfDocumentSize.None;
        }

        #endregion

        #region Styles
        public PdfManifestStyles Styles { get; private set; }

        #endregion

        #region Attributes
        public PdfDocumentSize DocumentSize { get; private set; }
        public float[] Margin { get; private set; }

        #endregion

        #region Pages
        public PdfPages Pages { get; private set; }
        void OnPagesInserted(object sender, ItemEventArgs<IEnumerable<PdfPage>> e)
        {
            var pages = e.Item;
            if (pages != null)
            { pages.ForEach(page => page.Manifest = this); }
        }

        #endregion

        #region Pdf Generation
        public void GeneratePdf(string filePath)
        {
            using (FileStream stream = File.OpenWrite(filePath))
            {
                GeneratePdf(stream);
                stream.Close();
            }
        }

        public void GeneratePdf(Stream stream)
        {
            using (iText.Document document = CreatePdfDocument())
            {
                PdfWriter writer = PdfWriter.GetInstance(document, stream);
                writer.CloseStream = false;
                // Open the Document for writing
                document.Open();

                // Fonts
                Styles.EmbeddedFonts.ForEach(f => iText.FontFactory.Register(f.Path, f.Name));

                // Pdf Content
                foreach (PdfPage page in Pages)
                {
                    // New Page
                    document.NewPage();

                    // Page Elements
                    page.Elements.ForEach(pdfElement => document.Add(pdfElement.GeneratePdfElement()));
                }

                document.Close();
            }
        }

        public void WriteTo(PdfSharp.Pdf.PdfDocument outputPdfDocument)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                GeneratePdf(stream);
                PdfSharp.Pdf.PdfDocument inputDocument = CompatiblePdfReader.Open(stream);
                foreach (PdfSharp.Pdf.PdfPage page in inputDocument.Pages)
                {
                    outputPdfDocument.AddPage(page);
                }
            }
        }

        private iText.Document CreatePdfDocument()
        {
            float marginLeft = 0, marginRight = 0, marginTop = 0, marginBottom = 0;
            if (Margin != null)
            {
                if (Margin.Length == 1)
                {
                    marginLeft = marginRight = marginTop = marginBottom = Margin[0];
                }
                else if (Margin.Length == 4)
                {
                    marginLeft = Margin[0];
                    marginRight = Margin[1];
                    marginTop = Margin[2];
                    marginBottom = Margin[3];
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            // Create a Document object
            return new iText.Document(PageSize.A4, marginLeft, marginRight, marginTop, marginBottom);
        }

        #endregion

        #region Serialization
        public static PdfManifest Load(string dataModelPath)
        {            
            using (FileStream stream = File.OpenRead(dataModelPath))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    return Load(xmlReader);
                }
            }
        }

        public static PdfManifest Load(XmlReader xmlReader)
        {
            XDocument document = XDocument.Load(xmlReader);
            XElement root = document.Root;
            PdfManifest pdfManifest = CreateFromXElement(root);

            return pdfManifest;
        }

        public static PdfManifest Parse(string pdfManifestXml)
        {
            if (pdfManifestXml == null)
            { throw new ArgumentNullException("pdfManifestXml"); }

            using (StringReader stringReader = new StringReader(pdfManifestXml.Trim()))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return Load(xmlReader);
                }
            }
        }

        internal static PdfManifest CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            // Document Attributes
            PdfManifest pdfManifest = new PdfManifest();
            pdfManifest.DocumentSize = element.Attribute("size").Enum<PdfDocumentSize>(PdfDocumentSize.None);

            string margin = element.Attribute("margin").Optional();
            if (!margin.IsNullOrBlank())
            {
                pdfManifest.Margin = margin.Split(new char[] { ',' }).Select(m => float.Parse(m)).ToArray();               
            }

            // Styles
            XElement stylesElement;
            if (element.TryGetElement(xmlns + "Styles", out stylesElement))
            {
                pdfManifest.Styles.PopulateFromXElement(stylesElement);
            }

            // Pages
            pdfManifest.Pages.PopulateFromXElement(element);

            var allElements = pdfManifest.Pages.SelectMany(p => p.Elements.GetAllRecursive());
            allElements.ForEach(e => e.Manifest = pdfManifest);

            return pdfManifest;
        }

        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            // Document Attributes
            if (DocumentSize.IsDefined())
            {
                element.Add(new XAttribute("size", DocumentSize));
            }
            if (Margin != null && Margin.Any())
            {
                element.Add(new XAttribute("margin", Margin.Select(m => m.ToString()).JoinStrings(", ")));
            }

            // Styles
            element.Add(Styles.GenerateXElement(xmlns + "Styles"));

            // Pages
            Pages.ForEach(p => element.Add(p.GenerateXElement(xmlns + "Page")));

            return element;
        }

        public XDocument ToXmlDocument()
        {
            XElement rootElement = GenerateXElement("Pdf");
            XDocument xml = new XDocument(rootElement);
            return xml;
        }

        #endregion
    }
}
