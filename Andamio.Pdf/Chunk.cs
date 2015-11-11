using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class Chunk : PdfElement
    {
        #region Constructors
        public Chunk() : base()
        {
            Style = new ChunkStyle();
        }

        public Chunk(string text) : this()
        {
            Text = text;
        }

        #endregion

        #region Text
        public string Text { get; set; }
        #endregion

        #region Style
        public ChunkStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static Chunk CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            
            Chunk chunk = new Chunk() { Text = element.Value };
            chunk.Style.PopulateFromXElement(element);
            
            return chunk;
        }

        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;

            XElement element = new XElement(name);

            return element;
        }

        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            ChunkStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            iTextSharp.text.Chunk chunk = new iTextSharp.text.Chunk(Text, (iText.Font)style.Font);
            return chunk;
        }

        #endregion
    }
}
