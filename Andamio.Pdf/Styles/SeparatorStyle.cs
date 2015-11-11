using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class SeparatorStyle : PdfElementStyle
    {
        #region Constructors
        public SeparatorStyle() : base()
        {
        }

        public static readonly SeparatorStyle Default = new SeparatorStyle()
        {
            Width = 0.5f,
            BorderColor = Color.Black,
        };

        #endregion

        #region Properties
        public float? Width { get; set; }
        public Color? BorderColor { get; set; }

        #endregion

        #region Serialization
        public static SeparatorStyle CreateFromXElement(XElement element)
        {
            SeparatorStyle separatorStyle = new SeparatorStyle();
            separatorStyle.PopulateFromXElement(element);

            return separatorStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            base.PopulateFromXElement(element);
            XNamespace xmlns = element.Name.Namespace;

            BorderColor = element.Attribute("color").NullableColor();
            Width = element.Attribute("width").NullableFloat();
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            if (BorderColor.HasValue)
            {
                element.Add(new XAttribute("color", BorderColor));
            }
            if (Width.HasValue)
            {
                element.Add(new XAttribute("width", Width));
            }

            return element;
        }

        #endregion
    }
}
