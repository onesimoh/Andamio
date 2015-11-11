using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class PhraseStyle : PdfElementStyle
    {
        #region Constructors
        public PhraseStyle() : base()
        {
            Font = new PdfFont();
        }

        public static readonly PhraseStyle Default = new PhraseStyle() { Font = PdfFont.Default };

        #endregion

        #region Properties
        public PdfFont Font { get; private set; }

        #endregion

        #region Serialization
        public static PhraseStyle CreateFromXElement(XElement element)
        {
            PhraseStyle phraseStyle = new PhraseStyle();
            phraseStyle.PopulateFromXElement(element);

            return phraseStyle;
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
            ParagraphStyle paragraphStyle = style as ParagraphStyle;
            base.Merge(paragraphStyle);
            Font.Merge(paragraphStyle.Font);
        }

        #endregion
    }
}
