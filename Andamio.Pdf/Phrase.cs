using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Andamio.Pdf
{
    public class Phrase : PdfContainerElement
    {
        #region Constructors
        public Phrase() : base()
        {
            Style = new PhraseStyle();
        }

        public Phrase(string text) : this()
        {
            if (!text.IsNullOrBlank())
            { Content.Add(new Chunk(text)); }
        }

        public Phrase(Chunk chunk) : this()
        {
            if (chunk != null)
            { Content.Add(chunk); }
        }

        public Phrase(IEnumerable<Chunk> elements) : this()
        {
            Content.AddRange(elements);
        }

        #endregion

        #region Style
        public PhraseStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static Phrase CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            
            Phrase phrase;
            if (element.Elements().Any())
            {
                phrase = new Phrase();
                phrase.Content.AddRange(element.Elements().Select(e => PdfElementFactory.CreateFromXElement(e)));
            }
            else
            {
                phrase = new Phrase(element.Value);
            }

            phrase.Style.PopulateFromXElement(element);

            return phrase;
        }

        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            return element;
        }

        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            iTextSharp.text.Phrase phrase = new iTextSharp.text.Phrase();
            if (Content.Any())
            { phrase.AddRange(Content.Select(e => e.GeneratePdfElement())); }

            return phrase;
        }

        #endregion
    }
}
