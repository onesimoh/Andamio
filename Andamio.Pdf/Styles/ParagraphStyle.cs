using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class ParagraphStyle : PdfElementStyle
    {
        #region Constructors
        public ParagraphStyle() : base()
        {
            Font = new PdfFont();
        }

        public static readonly ParagraphStyle Default = new ParagraphStyle() 
        {
            IndentationLeft = 0f,
            IndentationRight = 0f,
            Leading = 0f,
            SpacingAfter = 0f,
            SpacingBefore = 0f,
            Font = PdfFont.Default ,
        };

        #endregion

        #region Properties
        public float? IndentationLeft { get; set; }
        public float? IndentationRight { get; set; }
        public float? Leading { get; set; }
        public float? SpacingAfter { get; set; }
        public float? SpacingBefore { get; set; }

        public PdfFont Font { get; private set; }

        #endregion

        #region Serialization
        public static ParagraphStyle CreateFromXElement(XElement element)
        {
            ParagraphStyle paragraphStyle = new ParagraphStyle();
            paragraphStyle.PopulateFromXElement(element);            

            return paragraphStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {            
            XNamespace xmlns = element.Name.Namespace;
            base.PopulateFromXElement(element);
            Font.PopulateFromXElement(element);

            IndentationLeft = element.Attribute("indentationLeft").NullableFloat();
            IndentationRight = element.Attribute("indentationRight").NullableFloat();
            Leading = element.Attribute("leading").NullableFloat();
            SpacingBefore = element.Attribute("spacingBefore").NullableFloat();
            SpacingAfter = element.Attribute("spacingAfter").NullableFloat();            
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            XElement fontElement = Font.GenerateXElement(xmlns + "Font");
            element.Add(fontElement.Attributes());

            if (IndentationLeft.HasValue)
            {
                element.Add(new XAttribute("indentationLeft", IndentationLeft));
            }
            if (IndentationRight.HasValue)
            {
                element.Add(new XAttribute("indentationRight", IndentationRight));
            }            
            if (Leading.HasValue)
            {
                element.Add(new XAttribute("leading", Leading));
            }
            if (SpacingBefore.HasValue)
            {
                element.Add(new XAttribute("spacingBefore", SpacingBefore));
            }
            if (SpacingAfter.HasValue)
            {
                element.Add(new XAttribute("spacingAfter", SpacingAfter));
            }

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            ParagraphStyle paragraphStyle = style as ParagraphStyle;
            base.Merge(paragraphStyle);

            Font.Merge(paragraphStyle.Font);

            if (!paragraphStyle.IndentationLeft.HasValue)
            {
                IndentationLeft = paragraphStyle.IndentationLeft;
            }
            if (paragraphStyle.IndentationRight.HasValue)
            {
                IndentationRight = paragraphStyle.IndentationRight;
            }
            if (paragraphStyle.Leading.HasValue)
            {
                Leading = paragraphStyle.Leading;
            }
            if (paragraphStyle.SpacingBefore.HasValue)
            {
                SpacingBefore = paragraphStyle.SpacingBefore;
            }
            if (paragraphStyle.SpacingAfter.HasValue)
            {
                SpacingAfter = paragraphStyle.SpacingAfter;
            }
        }

        #endregion
    }
}
