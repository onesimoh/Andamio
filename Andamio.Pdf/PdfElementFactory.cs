using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Andamio.Pdf
{
    internal static class PdfElementFactory
    {
        public static bool IsValidPdfElement(XElement element)
        {
            string localName = element.Name.LocalName;
            if (!localName.ParseEnum<PdfElementKind>(PdfElementKind.Unknown).IsUnknown())
            {
                return true;
            }

            return false;
        }

        public static IPdfElement CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            if (element.Name.Equals(xmlns + PdfElementKind.Table.DisplayName()))
            {
                return Table.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Paragraph.DisplayName()))
            {
                return Paragraph.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Phrase.DisplayName()))
            {
                return Phrase.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Chunk.DisplayName()))
            {
                return Chunk.CreateFromXElement(element);
            }
            else if (Header.PdfHeaderRegEx.IsMatch(element.Name.LocalName))
            {
                return Header.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Image.DisplayName()))
            {
                return PdfImage.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Panel.DisplayName()))
            {
                return PdfPanel.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Signature.DisplayName()))
            {
                return SignaturePanel.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Separator.DisplayName()))
            {
                return Separator.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.NewLine.DisplayName()))
            {
                return NewLine.CreateFromXElement(element);
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Type Not Supported.", element.Name.LocalName));
            }
        }
    }
}
