using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class ChunkStyle : PdfElementStyle
    {
        #region Constructors
        public ChunkStyle() : base()
        {
            Font = new PdfFont();
        }

        public static readonly ChunkStyle Default = new ChunkStyle() { Font = PdfFont.Default };

        #endregion

        #region Properties
        public PdfFont Font { get; private set; }

        #endregion

        #region Serialization
        public static ChunkStyle CreateFromXElement(XElement element)
        {            
            ChunkStyle chunkStyle = new ChunkStyle();
            chunkStyle.PopulateFromXElement(element);

            return chunkStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            base.PopulateFromXElement(element);
            Font.PopulateFromXElement(element);
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            XElement fontElement = Font.GenerateXElement(xmlns + "Font");
            element.Add(fontElement.Attributes());

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            ChunkStyle chunkStyle = style as ChunkStyle;
            base.Merge(chunkStyle);

            Font.Merge(chunkStyle.Font);
        }

        #endregion
    }
}
