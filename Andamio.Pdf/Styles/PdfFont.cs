using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class EmbeddedPdfFonts : CollectionBase<EmbeddedPdfFont>
    {
        #region Constructors
        public EmbeddedPdfFonts() : base()
        {
        }

        public EmbeddedPdfFonts(IEnumerable<EmbeddedPdfFont> embeddedFonts)
            : base(embeddedFonts)
        {
        }

        #endregion
    }


    public class EmbeddedPdfFont
    {
        #region Constructors
        private EmbeddedPdfFont() : base()
        {
        }

        public EmbeddedPdfFont(string path) : this()
        {
            if (path.IsNullOrBlank())
            { throw new ArgumentNullException("path"); }
            Path = path;
        }

        #endregion

        #region Properties
        public string Name { get; set; }
        public string Path { get; set; }
        public string Encoding { get; set; }

        public static readonly string DefaultEnconding = "Cp1252";
        
        #endregion

        #region Serialization
        public static EmbeddedPdfFont CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            return new EmbeddedPdfFont(element.Attribute("path").Value)
            {
                Name = element.Attribute("name").Value,
                Encoding = element.Attribute("encoding").Optional(),
            };
        }

        public XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            if (!Name.IsNullOrBlank())
            {
                element.Add(new XAttribute("name", Name));
            }

            element.Add(new XAttribute("path", Path));

            if (!Encoding.IsNullOrBlank())
            {
                element.Add(new XAttribute("encoding", Encoding));
            }

            return element;
        }

        #endregion
    }


    public class PdfFont : PdfElementStyle
    {
        #region Constructors
        public PdfFont() : base()
        {
        }

        public PdfFont(string name) : this()
        {
            if (name.IsNullOrBlank())
            { throw new ArgumentNullException("name"); }
            Name = name;
        }

        public PdfFont(string name, float size) : this(name)
        {
            Size = size;
        }

        public PdfFont(string name, float size, Color color) : this(name, size)
        {
            Color = color;
        }

        public PdfFont(string name, float size, Color color, FontStyle style) : this(name, size, color)
        {
            Style = style;
        }

        public static readonly PdfFont Default = new PdfFont()
        {
            Name = DefaultFont,
            Size = DefaultSize,
            Style = DefaultStyle,
            Color = System.Drawing.Color.Black,
        };
        #endregion

        #region Properties
        public string Name { get; set; }
        public float? Size { get; set; }        
        public FontStyle? Style { get; set; }
        public Color? Color { get; set; }
        private string ColorStringValue { get; set; }

        public static readonly float DefaultSize = iText.Font.DEFAULTSIZE;
        public static readonly FontStyle DefaultStyle = (FontStyle) iText.Font.NORMAL;
        public static readonly string DefaultFont = iText.Font.FontFamily.HELVETICA.ToString();

        #endregion

        #region Serialization
        public override void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            Name = element.Attribute("fontName").Optional();
            Size = element.Attribute("fontSize").NullableFloat();
            Style = element.Attribute("fontStyle").NullableEnum<FontStyle>();
            Color = element.Attribute("fontColor").NullableColor();
            ColorStringValue = element.Attribute("fontColor").Optional();
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement fontElement = new XElement(name);

            if (!Name.IsNullOrBlank())
            {
                fontElement.Add(new XAttribute("fontName", Name));
            }           
            if (Size.HasValue)
            {
                fontElement.Add(new XAttribute("fontSize", Size));
            }
            if (Style.HasValue)
            {
                fontElement.Add(new XAttribute("fontStyle", Style));
            }
            if (!ColorStringValue.IsNullOrBlank())
            {
                fontElement.Add(new XAttribute("fontColor", ColorStringValue));
            }

            return fontElement;

        }

        #endregion

        #region Merge
        public override void Apply(PdfElementStyle style)
        {
            if (style == null)
            { throw new ArgumentNullException("style"); }

            PdfFont font = style as PdfFont;
            if (font == null)
            { throw new ArgumentException(String.Format("Invalid Style Type '{0}'.", style.GetType().Name)); }

            Name = font.Name;
            Size = font.Size;
            Style = font.Style;
            Color = font.Color;
        }

        public override void Merge(PdfElementStyle style)
        {
            PdfFont fontStyle = style as PdfFont;
            base.Merge(fontStyle);

            if (!fontStyle.Name.IsNullOrBlank())
            {
                Name = fontStyle.Name;
            }            
            if (fontStyle.Size.HasValue)
            {
                Size = fontStyle.Size;
            }
            if (fontStyle.Style.HasValue)
            {
                Style = fontStyle.Style;
            }
            if (fontStyle.Color.HasValue)
            {
                Color = fontStyle.Color;
            }
            
        }
        #endregion

        #region Conversion
        public iText.Font ToiTextSharpFont()
        {
            string name = (!Name.IsNullOrBlank()) ? Name : DefaultFont;
            float size = (Size.HasValue) ? Size.Value : DefaultSize;
            FontStyle style = (Style.HasValue) ? Style.Value : DefaultStyle;
            Color color = (Color.HasValue) ? Color.Value : PdfFont.Default.Color.Value;

            return iText.FontFactory.GetFont(name, size, (int) style, new iText.BaseColor(color));
        }

        static public explicit operator iText.Font(PdfFont pdfFont)
        {
            return pdfFont.ToiTextSharpFont();
        }

        #endregion
    }
}
