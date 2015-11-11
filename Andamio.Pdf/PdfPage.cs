using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

namespace Andamio.Pdf
{
    public class PdfPages : CollectionBase<PdfPage>
    {
        #region Constructors
        public PdfPages()
            : base()
        {
        }

        public PdfPages(IEnumerable<PdfPage> pages)
            : base(pages)
        {
        }

        #endregion

        #region Serialization
        public void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            var pages = element.Elements(xmlns + "Page");
            pages.ForEach(p => Add(PdfPage.CreateFromXElement(p)));
        }

        #endregion

        #region Elements
        public IPdfElement[] GetAllElementsRecursive()
        {
            List<IPdfElement> elements = new List<IPdfElement>();
            elements.AddRange(this.SelectMany(p => p.Elements.GetAllRecursive()));

            return elements.ToArray();
        }

        #endregion
    }


    public class PdfPage
    {
        #region Constructors
        public PdfPage() : base()
        {
            Elements = new PdfElements();
            Elements.ItemsInserted += OnElementsInserted;
        }

        #endregion

        #region Manifest
        public PdfManifest Manifest { get; internal set; }

        #endregion

        #region Elements
        public PdfElements Elements { get; private set; }
        void OnElementsInserted(object sender, ItemEventArgs<IEnumerable<IPdfElement>> e)
        {
            if (e.Item != null)
            {
                PdfElements elements = new PdfElements(e.Item);
                elements.GetAllRecursive().ForEach(element => element.Manifest = Manifest); 
            }
        }

        #endregion

        #region Serialization
        internal static PdfPage CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            PdfPage page = new PdfPage();

            foreach (XElement e in element.Elements())
            {
                page.Elements.Add(PdfElementFactory.CreateFromXElement(e));
            }

            return page;
        }

        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;

            XElement element = new XElement(name);

            return element;
        }

        #endregion
    }
}
