using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Andamio;

using iText = iTextSharp.text;

namespace Andamio.Pdf
{
    public class PdfManifestStyles : PdfElementStyleCollection
    {
        #region Constructors
        public PdfManifestStyles()
        {
            EmbeddedFonts = new EmbeddedPdfFonts();
        }

        #endregion

        #region Fonts
        public EmbeddedPdfFonts EmbeddedFonts { get; private set; }

        #endregion

        #region Styles
        private readonly Dictionary<String, PdfElementStyle> ConfiguredStyles = new Dictionary<String, PdfElementStyle>();
        public T GetMergedFromConfiguration<T>(T style) where T : PdfElementStyle 
        {
            if (style == null)
            { throw new ArgumentNullException("style"); }

            string keyName = style.GetFullyQualifiedKeyName();            
            if (!style.KeyName.IsNullOrBlank() && !ConfiguredStyles.ContainsKey(keyName))
            {
                throw new ApplicationException(String.Format("Invalid Style Key Name '{0}' of Type '{1}' Not Found."
                    , style.KeyName
                    , typeof(T)));                
            }

            PdfElementStyle configStyle;
            if (ConfiguredStyles.TryGetValue(keyName, out configStyle))
            {
                if (!(configStyle is T))
                {
                    throw new InvalidOperationException(String.Format("Invalid Style of Type '{0}' does not match specified Type '{1}'."
                        , configStyle.GetType()
                        , typeof(T)));
                }

                return Merge<T>((T) configStyle, style);
            }
            
            return style;
        }

        public static T Merge<T>(params T[] styles) where T : PdfElementStyle
        {
            if (styles == null)
            { throw new ArgumentNullException("styles"); }

            T mergedStyle = Activator.CreateInstance<T>();
            styles.Where(style => style != null).ForEach(style => mergedStyle.Merge(style));

            return mergedStyle;
        }

        #endregion

        #region Serialization
        public static PdfManifestStyles CreateFromXElement(XElement element)
        {
            PdfManifestStyles manifestStyles = new PdfManifestStyles();
            manifestStyles.PopulateFromXElement(element);

            return manifestStyles;
        }

        public void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            // Fonts
            XElement fontsElement;
            if (element.TryGetElement(xmlns + "Fonts", out fontsElement))
            {
                var embeddedFonts = fontsElement.Elements(xmlns + "register");
                EmbeddedFonts.AddRange(embeddedFonts.Select(embeddedFont => EmbeddedPdfFont.CreateFromXElement(embeddedFont)));
            }

            // Styles
            foreach (XElement elementStyle in element.Elements())
            {
                if (!PdfElementStyleFactory.IsValidPdfElement(elementStyle))
                { continue; }

                PdfElementStyle style = PdfElementStyleFactory.CreateFromXElement(elementStyle);
                Add(style);
                
                string keyName = style.GetFullyQualifiedKeyName();
                ConfiguredStyles.Add(keyName, style);              
            }
        }

        public XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);



            return element;
        }

        
        #endregion
    }
}
