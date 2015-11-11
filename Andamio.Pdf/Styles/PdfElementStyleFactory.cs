using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Andamio.Pdf
{
    internal static class PdfElementStyleFactory
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

        public static PdfElementStyle CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            if (element.Name.Equals(xmlns + PdfElementKind.Table.DisplayName()))
            {
                return TableStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.TableCell.DisplayName()))
            {
                return TableCellStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Paragraph.DisplayName()))
            {
                return ParagraphStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Phrase.DisplayName()))
            {
                return PhraseStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Chunk.DisplayName()))
            {
                return ChunkStyle.CreateFromXElement(element);
            }
            else if (Header.PdfHeaderRegEx.IsMatch(element.Name.LocalName))
            {
                return HeaderStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Image.DisplayName()))
            {
                return PdfImageStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Panel.DisplayName()))
            {
                return PdfPanelStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Signature.DisplayName()))
            {
                return SignaturePanelStyle.CreateFromXElement(element);
            }
            else if (element.Name.Equals(xmlns + PdfElementKind.Separator.DisplayName()))
            {
                return SeparatorStyle.CreateFromXElement(element);
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Type Not Supported.", element.Name));
            }
        }

        public static XElement GenerateXElement(PdfElementStyle style, XNamespace xmlNamespace = null)
        {
            XNamespace xmlns = xmlNamespace ?? XNamespace.None;

            if (style is TableStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Table.DisplayName());
            }
            else if (style is TableCellStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.TableCell.DisplayName());
            }
            else if (style is ParagraphStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Paragraph.DisplayName());
            }
            else if (style is PhraseStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Phrase.DisplayName());
            }
            else if (style is ChunkStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Chunk.DisplayName());
            }
            else if (style is HeaderStyle)
            {
                return ((HeaderStyle) style).GenerateXElement(xmlns);
            }
            else if (style is PdfImageStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Image.DisplayName());
            }
            else if (style is PdfPanelStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Panel.DisplayName());
            }
            else if (style is SignaturePanelStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Signature.DisplayName());
            }
            else if (style is SeparatorStyle)
            {
                return style.GenerateXElement(xmlns + PdfElementKind.Separator.DisplayName());
            }
            else
            {
                throw new NotSupportedException(String.Format("'{0}' Type Not Supported.", style.GetType().Name));
            }
        }

    }
}
