using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Andamio;
using Andamio.Collections;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

using iText = iTextSharp.text;
using System.Xml.Linq;

namespace Andamio.Pdf
{
    #region PdfElementKind
    public enum PdfElementKind
    {
        [EnumDisplay("")]
        Unknown,        
        Paragraph,
        Phrase,
        Chunk,
        Image,
        Panel,
        Signature,
        Separator,
        NewLine,
        Table,
        TableCell,
        TableHeaderCell,
        TableRow,
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
    }

    public static class PdfElementKindExtensions
    {
        public static bool IsUnknown(this PdfElementKind elementKind)
        {
            return elementKind == PdfElementKind.Unknown;
        }
    }

    #endregion


    public class PdfElements : CollectionBase<IPdfElement>
    {
        #region Constructors
        public PdfElements()
            : base()
        {
        }

        public PdfElements(IEnumerable<IPdfElement> elements)
            : base(elements)
        {
        }

        #endregion

        #region Elements
        public IPdfElement[] GetAllRecursive()
        {
            List<IPdfElement> elements = new List<IPdfElement>();
            foreach (IPdfElement element in this)
            {
                elements.Add(element);
                if (element is PdfContainerElement)
                {
                    elements.AddRange(((PdfContainerElement)element).GetAllElementsRecursive());
                }
                else if (element is Table)
                {                    
                    var tableCells = ((Table) element).GetAllCells();
                    PdfElements allCells = new PdfElements(tableCells);
                    elements.AddRange(allCells.GetAllRecursive());
                }
                else if (element is TableCell)
                {
                    elements.AddRange(((TableCell) element).GetAllElementsRecursive());
                }
            }

            return elements.ToArray();
        }

        #endregion
    }

    public interface IPdfElement
    {
        iText.IElement GeneratePdfElement();
        PdfManifest Manifest { get; set; }
    }

    public abstract class PdfElement : IPdfElement
    {        
        public abstract iText.IElement GeneratePdfElement();
        public virtual PdfManifest Manifest { get; set; }
    }

    public abstract class PdfContainerElement : PdfElement
    {
        #region Constructor
        public PdfContainerElement() : base()
        {
            Content = new PdfElements();
            Content.ItemsInserted += OnContentItemsInserted;
        }

        #endregion

        #region Content
        public PdfElements Content { get; private set; }
        void OnContentItemsInserted(object sender, ItemEventArgs<IEnumerable<IPdfElement>> e)
        {
            if (e.Item != null)
            {
                PdfElements elements = new PdfElements(e.Item);
                elements.GetAllRecursive().ForEach(element => element.Manifest = this.Manifest);
            }
        }

        public IPdfElement[] GetAllElementsRecursive()
        {
            PdfElements elements = new PdfElements(Content);
            return elements.GetAllRecursive();
        }

        public override PdfManifest Manifest
        {
            get
            {
                return base.Manifest;
            }
            set
            {
                base.Manifest = value;
                var elements = GetAllElementsRecursive();

            }
        }
        #endregion
    }
}
