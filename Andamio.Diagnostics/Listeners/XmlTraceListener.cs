using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

using Andamio;
using Andamio.Xml;
using Andamio.Security;
using Andamio.Diagnostics.Configuration;

namespace Andamio.Diagnostics.Listeners
{
    #region XmlTraceListenerSettings
    /// <summary>
    /// Defines the settings for the Xml Trace Listener.
    /// </summary>
    public sealed class XmlTraceListenerSettings : TraceListenerSettings
    {
        /// <summary>Default Constructor</summary>
        public XmlTraceListenerSettings() : base()
        {
            MaxSizeInMB = 0;
            Mode = FileLogMode.Continuous;
            FilePath = "Log.xml";
        }

        /// <summary>
        /// Gets or Sets the Max Size in MB for a rolling file.
        /// </summary>
        public int MaxSizeInMB { get; set; }

        /// <summary>
        /// Defines whether the Log file is Continuous or Rolling.
        /// </summary>
        public FileLogMode Mode { get; set; }

        /// <summary>
        /// Gets or Sets the File Path for the Log File.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or Sets the optiona Xslt Transformation File to to parse the Xml Log into a more huma readable format.
        /// </summary>
        public string TransformationFile { get; set; }

        /// <summary>
        /// Creates the Xml listener from the current Settings, if a Listener Type is not provided then an instance of XmlTraceListener is returned.
        /// </summary>
        /// <returns>An Xml Log Recorder Listener instance.</returns>
        public override ILogRecorder CreateListener()
        {
            ILogRecorder listener = base.CreateListener();
            if (listener == null)
            { listener = new XmlTraceListener(this); }
            return listener;
        }
    }

    #endregion


    /// <summary>
    /// Directs tracing or debugging output as XML.
    /// </summary>
    public class XmlTraceListener : TraceListenerBase
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        private XmlTraceListener() : base()
        {
        }

        /// <summary>
        /// Creates a new instace of the XmlTraceListener class.
        /// </summary>
        /// <param name="settings">The settings for the Xml Trace Listener instance.</param>
        public XmlTraceListener(XmlTraceListenerSettings settings) : this()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        /// <summary>
        /// Gets the settings for the Xml Trace Listener.
        /// </summary>
        public XmlTraceListenerSettings Settings { get; private set; }

        #endregion

        #region Document
        /// <summary>
        /// Get EOD Offset based on the LogFooter from where to begin writing.
        /// </summary>
        protected virtual long EndOfDocumentOffset
        {
            get { return -1 * ((new UTF8Encoding()).GetByteCount(XmlFileLogEndTag) + 2); }
        }

        #endregion

        #region Trace
        protected readonly string XmlFileHeader = "<?xml version=\"1.0\"?>";
        protected readonly string XsltStylesheet = "<?xml-stylesheet type=\"text/xsl\" href=\"{0}\" ?>";
        protected readonly string XmlFileLogTag = "<Log>";
        protected readonly string XmlFileLogEndTag = "</Log>";

        StreamWriter _writer;
        /// <summary>
        /// Executes before the Log Entry is logged.
        /// </summary>
        protected override void PreTrace()
        {
            FileStream fileStream = File.Open(Settings.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            _writer = new StreamWriter(fileStream);

            if ((Settings.Mode == FileLogMode.Rolling) && (_writer.BaseStream.Length >= BYTE * Settings.MaxSizeInMB))
            {
                fileStream.Flush();               

                _writer.Flush();
                _writer.Close();

                DiagnosisConfigSection diagnosisConfig = DiagnosisConfigSection.FromConfig();
                
                string traceFileName = String.Format("{0}_{1}{2}"
                    , Path.GetFileNameWithoutExtension(diagnosisConfig.XmlLogSettings.FilePath)
                    , DateTime.Now.ToString("yyyyMMdd-HHmmss")
                    , Path.GetExtension(Settings.FilePath));

                Settings.FilePath = Path.Combine(Path.GetDirectoryName(Settings.FilePath), traceFileName);
                fileStream = File.Open(Settings.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                _writer = new StreamWriter(fileStream);
            }

            if (_writer.BaseStream.Length == 0)
            {
                _writer.WriteLine(XmlFileHeader);
                if (!Settings.TransformationFile.IsNullOrBlank())
                { _writer.WriteLine(String.Format(XsltStylesheet, Settings.TransformationFile)); }
                _writer.WriteLine(XmlFileLogTag);
            }
            else
            {
                _writer.BaseStream.Seek(EndOfDocumentOffset, SeekOrigin.End);
            }                       
        }

        /// <summary>
        /// Executes after the Log Entry is logged.
        /// </summary>
        protected override void PostTrace()
        {
            if (_writer != null)
            {
                _writer.WriteLine(XmlFileLogEndTag);
                _writer.Flush();
                _writer.Close();
            }
        }

        /// <summary>
        /// Logs the specified Log Entry.
        /// </summary>
        /// <param name="logEntry">The Log Entry to write to the Log.</param>
        protected override void WriteLogEntry(LogEntry logEntry)
        {
            XmlFragmentWriter xmlFragmentWriter = new XmlFragmentWriter(_writer) { Formatting = Formatting.Indented };
            XElement logEntryElem = logEntry.GenerateXml("LogEntry");            
            logEntryElem.WriteTo(xmlFragmentWriter);
        }

        #endregion
    }
}
