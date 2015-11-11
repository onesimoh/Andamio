using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Andamio.Pdf
{
    public class PdfPanelStyle : PdfElementStyle
    {
        #region Constructors
        public PdfPanelStyle() : base()
        {
        }

        public static readonly PdfPanelStyle Default = new PdfPanelStyle()
        {
            HorizontalAlignment = PdfHorizontalAlignment.Left,
            BackgroundColor = Color.White,
            BorderColor = Color.Transparent,
            Padding = 0f,
            Leading = 0f,
            BorderWidth = 0f,
            TopIndent = 0f,
            BottomIndent = 0f,
            Width = PdfUnit.Blank,
        };

        #endregion

        #region Properties
        public PdfHorizontalAlignment? HorizontalAlignment { get; set; }
        public Color? BackgroundColor { get; set; }
        public Color? BorderColor { get; set; }
        public float? Padding { get; set; }
        public float? Leading { get; set; }
        public float? BorderWidth { get; set; }
        public float? TopIndent { get; set; }
        public float? BottomIndent { get; set; }
        public PdfUnit? Width { get; set; }

        #endregion

        #region Serialization
        public static PdfPanelStyle CreateFromXElement(XElement element)
        {
            PdfPanelStyle panelStyle = new PdfPanelStyle();
            panelStyle.PopulateFromXElement(element);

            return panelStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            base.PopulateFromXElement(element);

            XNamespace xmlns = element.Name.Namespace;

            HorizontalAlignment = element.Attribute("horizontalAlignment").NullableEnum<PdfHorizontalAlignment>();
            BackgroundColor = element.Attribute("backgroundColor").NullableColor();
            BorderColor = element.Attribute("borderColor").NullableColor();
            Padding = element.Attribute("padding").NullableFloat();
            Leading = element.Attribute("leading").NullableFloat();
            BorderWidth = element.Attribute("borderWidth").NullableFloat();
            TopIndent = element.Attribute("topIndent").NullableFloat();
            BottomIndent = element.Attribute("bottomIndent").NullableFloat();
            Width = PdfUnit.Parse(element.Attribute("width").Optional());                    
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            if (HorizontalAlignment.HasValue)
            {
                element.Add(new XAttribute("horizontalAlignment", HorizontalAlignment));
            }
            if (BackgroundColor.HasValue)
            {
                element.Add(new XAttribute("backgroundColor", BackgroundColor));
            }
            if (BorderColor.HasValue)
            {
                element.Add(new XAttribute("borderColor", BorderColor));
            }
            if (Padding.HasValue)
            {
                element.Add(new XAttribute("padding", Padding));
            }
            if (Leading.HasValue)
            {
                element.Add(new XAttribute("leading", Leading));
            }
            if (BorderWidth.HasValue)
            {
                element.Add(new XAttribute("borderWidth", BorderWidth));
            }
            if (TopIndent.HasValue)
            {
                element.Add(new XAttribute("topIndent", TopIndent));
            }
            if (BottomIndent.HasValue)
            {
                element.Add(new XAttribute("bottomIndent", BottomIndent));
            }
            if (Width.HasValue && !Width.Value.IsBlank)
            {
                element.Add(new XAttribute("width", Width));
            }

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            PdfPanelStyle panelStyle = style as PdfPanelStyle;
            base.Merge(panelStyle);

            if (panelStyle.HorizontalAlignment.HasValue)
            {
                HorizontalAlignment = panelStyle.HorizontalAlignment;
            }
            if (panelStyle.BackgroundColor.HasValue)
            {
                BackgroundColor = panelStyle.BackgroundColor;
            }
            if (panelStyle.BorderColor.HasValue)
            {
                BorderColor = panelStyle.BorderColor;
            }
            if (panelStyle.Padding.HasValue)
            {
                Padding = panelStyle.Padding;
            }
            if (panelStyle.Leading.HasValue)
            {
                Leading = panelStyle.Leading;
            }
            if (panelStyle.BorderWidth.HasValue)
            {
                BorderWidth = panelStyle.BorderWidth;
            }
            if (panelStyle.TopIndent.HasValue)
            {
                TopIndent = panelStyle.TopIndent;
            }
            if (panelStyle.BottomIndent.HasValue)
            {
                BottomIndent = panelStyle.BottomIndent;
            }
            if (panelStyle.Width.HasValue && !panelStyle.Width.Value.IsBlank)
            {
                Width = panelStyle.Width;
            }
        }

        #endregion
    }
}
