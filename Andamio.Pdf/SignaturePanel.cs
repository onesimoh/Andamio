using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using System.Security.Principal;

using Andamio;
using Andamio.Security;
using Andamio.Security.ActiveDirectory;

using iTextSharp.text;
using iText = iTextSharp.text;
using iTextPdf = iTextSharp.text.pdf;
using iTextDraw = iTextSharp.text.pdf.draw;

using FrameworkSecurity = Andamio.Security;
using AD = Andamio.Security.ActiveDirectory;

namespace Andamio.Pdf
{
    public class SignaturePanel : PdfElement
    {
        #region Constructors
        public SignaturePanel() : base()
        {
            Style = new SignaturePanelStyle();
        }

        public SignaturePanel(string signature, DateTime date, string user) : this()
        {
            if (signature == null)
            { throw new ArgumentNullException("signature"); }
            if (user == null)
            { throw new ArgumentNullException("user"); }

            Signature = signature;
            Date = date.Date;
            User = user;
        }

        #endregion

        #region Properties
        public string Signature { get; set; }
        public DateTime Date { get; set; }
        public string User { get; set; }

        #endregion

        #region Style
        public SignaturePanelStyle Style { get; private set; }

        #endregion

        #region Serialization
        internal static SignaturePanel CreateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;

            string signature = element.Value.Trim(new char[] { '\t', '\n', '\r' });
            DateTime date = element.Attribute("date").DateTime();
            string user = element.Attribute("user").Value;
            SignaturePanel signaturePanel = new SignaturePanel(signature, date, user);            
            signaturePanel.Style.PopulateFromXElement(element);

            return signaturePanel;
        }

        public XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name);

            return element;
        }

        #endregion

        #region Pdf Element
        public override iTextSharp.text.IElement GeneratePdfElement()
        {
            SignaturePanelStyle style = (Manifest != null) ? Manifest.Styles.GetMergedFromConfiguration(Style) : Style;

            Phrase phrase = new Phrase();

            Chunk signature = new Chunk(Signature);
            signature.Style.Font.Apply(style.Font);
            phrase.Content.Add(signature);
            phrase.Content.Add(new NewLine());
            
            Separator separator = new Separator();
            separator.Style.BorderColor = style.BorderColor;
            separator.Style.Width = style.BorderWidth;
            phrase.Content.Add(separator);

            GenericIdentity identity = new GenericIdentity(User);

            AD.ActiveDirectory activeDirectory = new AD.ActiveDirectory();
            UserDescriptor adUser = activeDirectory.GetUser(identity.GetUserName());
            string fullName = adUser.DisplayName;

            phrase.Content.Add(new NewLine());
            phrase.Content.Add(new Chunk(String.Format("{0} ({1:MMMM dd, yyyy})", fullName, Date)));

            PdfPanel panel = new PdfPanel(phrase);
            panel.Style.BackgroundColor = style.BackgroundColor;
            panel.Style.BorderColor = style.BorderColor;
            panel.Style.Padding = style.Padding;
            panel.Style.BorderWidth = style.BorderWidth;
            panel.Style.Width = style.Width;

            return panel.GeneratePdfElement();
        }

        #endregion
    }
}
