using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class TableCellStyle : PdfElementStyle
    {
        #region Constructors
        public TableCellStyle() : base()
        {
        }

        public static readonly TableCellStyle Default = new TableCellStyle()
        {
            BackgroundColor = Color.White,
            BorderColor = Color.Black,
            Padding = 2f,
            Leading = 10f,
            BorderWidth = 0.5f,
            HorizontalAlignment = TableCellAlign.Left,
            VerticalAlignment = TableCellAlign.Top,
        };
        #endregion

        #region Properties
        public Color? BackgroundColor { get; set; }
        public Color? BorderColor { get; set; }
        public float? Padding { get; set; }
        public float? Leading { get; set; }
        public float? BorderWidth { get; set; }
        public TableCellAlign? HorizontalAlignment { get; set; }
        public TableCellAlign? VerticalAlignment { get; set; }
        public bool? NoWrap { get; set; }

        #endregion

        #region Serialization
        public static TableCellStyle CreateFromXElement(XElement element)
        {
            TableCellStyle cellStyle = new TableCellStyle();
            cellStyle.PopulateFromXElement(element);

            return cellStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            base.PopulateFromXElement(element);

            BackgroundColor = element.Attribute("backgroundColor").NullableColor();
            BorderColor = element.Attribute("borderColor").NullableColor();
            Padding = element.Attribute("padding").NullableFloat();
            Leading = element.Attribute("leading").NullableFloat();
            BorderWidth = element.Attribute("borderWidth").NullableFloat();
            HorizontalAlignment = element.Attribute("horizontalAlign").NullableEnum<TableCellAlign>();
            VerticalAlignment = element.Attribute("verticalAlign").NullableEnum<TableCellAlign>();
            NoWrap = element.Attribute("nowrap").NullableBool();
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

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
            if (HorizontalAlignment.HasValue)
            {
                element.Add(new XAttribute("horizontalAlign", HorizontalAlignment));
            }
            if (VerticalAlignment.HasValue)
            {
                element.Add(new XAttribute("bottomIndent", VerticalAlignment));
            }
            if (NoWrap.HasValue)
            {
                element.Add(new XAttribute("nowrap", NoWrap));
            }

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            TableCellStyle cellStyle = style as TableCellStyle;
            base.Merge(cellStyle);

            if (cellStyle.BackgroundColor.HasValue)
            {
                BackgroundColor = cellStyle.BackgroundColor;
            }
            if (cellStyle.BorderColor.HasValue)
            {
                BorderColor = cellStyle.BorderColor;
            }
            if (cellStyle.Padding.HasValue)
            {
                Padding = cellStyle.Padding;
            }
            if (cellStyle.Leading.HasValue)
            {
                Leading = cellStyle.Leading;
            }
            if (cellStyle.BorderWidth.HasValue)
            {
                BorderWidth = cellStyle.BorderWidth;
            }
            if (cellStyle.HorizontalAlignment.HasValue)
            {
                HorizontalAlignment = cellStyle.HorizontalAlignment;
            }
            if (cellStyle.VerticalAlignment.HasValue)
            {
                VerticalAlignment = cellStyle.VerticalAlignment;
            }
            if (cellStyle.NoWrap.HasValue)
            {
                NoWrap = cellStyle.NoWrap;
            }
        }

        #endregion
    }
}
