using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public class TableStyle : PdfElementStyle
    {
        #region Constructors
        public TableStyle() : base()
        {
        }

        public static readonly TableStyle Default = new TableStyle()
        {
            HorizontalAlignment = PdfHorizontalAlignment.Center,
            SpacingBefore = 0f,
            SpacingAfter = 0f,
            LockedWidth = false,
        };

        #endregion

        #region Properties
        public PdfHorizontalAlignment? HorizontalAlignment { get; set; }
        public float? SpacingBefore { get; set; }
        public float? SpacingAfter { get; set; }
        public bool? LockedWidth { get; set; }
        public float? WidthPercentage { get; set; }
        public float[] Widths { get; set; }

        #endregion

        #region Serialization
        public static TableStyle CreateFromXElement(XElement element)
        {
            TableStyle tableStyle = new TableStyle();
            tableStyle.PopulateFromXElement(element);

            return tableStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            base.PopulateFromXElement(element);
            
            XNamespace xmlns = element.Name.Namespace;

            HorizontalAlignment = element.Attribute("horizontalAlignment").NullableEnum<PdfHorizontalAlignment>();
            SpacingBefore = element.Attribute("spacingBefore").NullableFloat();
            SpacingAfter = element.Attribute("spacingAfter").NullableFloat();
            LockedWidth = element.Attribute("lockedWidth").NullableBool();
            WidthPercentage = element.Attribute("widthPercentage").NullableFloat();

            var widths = element.Attribute("widths").Optional();
            if (!widths.IsNullOrBlank())
            {
                Widths = widths.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(w => float.Parse(w)).ToArray();
            }
        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            if (HorizontalAlignment.HasValue)
            {
                element.Add(new XAttribute("horizontalAlignment", HorizontalAlignment));
            }
            if (SpacingBefore.HasValue)
            {
                element.Add(new XAttribute("spacingBefore", SpacingBefore));
            }
            if (SpacingAfter.HasValue)
            {
                element.Add(new XAttribute("spacingAfter", SpacingAfter));
            }
            if (LockedWidth.HasValue)
            {
                element.Add(new XAttribute("lockedWidth", LockedWidth));
            }
            if (Widths != null && Widths.Any())
            {
                element.Add(new XAttribute("widths", Widths.Select(w => w.ToString()).JoinStrings(",")));
            }
            if (WidthPercentage.HasValue)
            {
                element.Add(new XAttribute("widthPercentage", WidthPercentage));
            }

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            TableStyle tableStyle = style as TableStyle;
            base.Merge(tableStyle);

            if (tableStyle.HorizontalAlignment.HasValue)
            {
                HorizontalAlignment = tableStyle.HorizontalAlignment;
            }
            if (tableStyle.SpacingBefore.HasValue)
            {
                SpacingBefore = tableStyle.SpacingBefore;
            }
            if (tableStyle.SpacingAfter.HasValue)
            {
                SpacingAfter = tableStyle.SpacingAfter;
            }
            if (tableStyle.LockedWidth.HasValue)
            {
                LockedWidth = tableStyle.LockedWidth;
            }
            if (tableStyle.Widths != null && tableStyle.Widths.Any())
            {
                Widths = tableStyle.Widths;
            }
            if (tableStyle.WidthPercentage.HasValue)
            {
                WidthPercentage = tableStyle.WidthPercentage;
            }
        }

        #endregion
    }
}
