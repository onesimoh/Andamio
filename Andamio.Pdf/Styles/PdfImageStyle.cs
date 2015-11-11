using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

namespace Andamio.Pdf
{
    public enum PdfImageType
    {
        File,
        Resource,
    }

    public static class PdfImageTypeExtensions
    {
        public static bool IsFile(this PdfImageType type)
        {
            return type == PdfImageType.File;
        }
        public static bool IsResource(this PdfImageType type)
        {
            return type == PdfImageType.Resource;
        }
    }

    public class PdfImageStyle : PdfElementStyle
    {
        #region Constructors
        public PdfImageStyle() : base()
        {
        }

        public static readonly PdfImageStyle Default = new PdfImageStyle();
        #endregion

        #region Properties
        public string Source { get; set; }
        public PdfImageType? Type { get; set; }

        #endregion

        #region Serialization
        public static PdfImageStyle CreateFromXElement(XElement element)
        {
            PdfImageStyle imageStyle = new PdfImageStyle();
            imageStyle.PopulateFromXElement(element);

            return imageStyle;
        }

        public override void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            base.PopulateFromXElement(element);

            Source = element.Attribute("source").Optional();
            Type = element.Attribute("type").NullableEnum<PdfImageType>();

        }

        public override XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = base.GenerateXElement(name);

            if (!Source.IsNullOrBlank())
            {
                element.Add(new XAttribute("source", Source));
            }

            if (Type.HasValue)
            {
                element.Add(new XAttribute("type", Type));
            }

            return element;
        }

        #endregion

        #region Merge
        public override void Merge(PdfElementStyle style)
        {
            PdfImageStyle imageStyle = style as PdfImageStyle;
            base.Merge(imageStyle);

            if (!imageStyle.Source.IsNullOrBlank())
            {
                Source = imageStyle.Source;
            }

            if (imageStyle.Type.HasValue)
            {
                Type = imageStyle.Type;
            }
        }

        #endregion
    }
}
