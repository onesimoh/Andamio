using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Formatters
{
    public abstract class Formatter
    {
        #region Constructors
        protected Formatter()
        {
        }

        protected Formatter(MediaConfiguration mediaConfiguration) : this()
        {
            if (mediaConfiguration == null) throw new ArgumentNullException("mediaConfiguration");
            MediaConfiguration = mediaConfiguration;
        }

        #endregion

        #region Media
        public MediaConfiguration MediaConfiguration { get; private set; }
        public Log Log
        {
            get { return MediaConfiguration.Log; }
        }

        #endregion

        #region Mappings
        public virtual void ReadMappings(FileInfo file)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (!file.Exists) throw new FileNotFoundException(String.Format("Specified file '{0}' Not found.", file.Name), file.Name);
            string xmlConfig = file.Content();
            ReadMappings(xmlConfig);
        }

        public virtual void ReadMappings(XmlReader xmlReader)
        {
            if (xmlReader == null) throw new ArgumentNullException("xmlReader");
            XDocument document = XDocument.Load(xmlReader);
            ReadMappings(document);
        }

        public virtual void ReadMappings(string xml)
        {
            if (xml.IsNullOrBlank()) throw new ArgumentNullException("xml");
            try
            {
                XDocument document = XDocument.Parse(xml);
                ReadMappings(document);
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Field mappings does not conform to valid Xml.", exception);
            }
        }

        public virtual void ReadMappings(XDocument document)
        {
            if (document == null) throw new ArgumentNullException("document");
            ReadMappings(document.Root);
        }

        public abstract void ReadMappings(XElement element);

        #endregion

        #region Import
        public abstract DataGrid Import();

        #endregion
    }
}
