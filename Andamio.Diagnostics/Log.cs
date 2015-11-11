using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Andamio;
using Andamio.Security;
using Andamio.Serialization;
using Andamio.Diagnostics.Listeners;
using Andamio.Diagnostics.Configuration;
using Andamio.Diagnostics.Sqllite;

namespace Andamio.Diagnostics
{
    /// <summary>
    /// Provide Tracing/Logging capabilities.
    /// </summary>
    public partial class Log
    {
        #region Constructors
        /// <summary>Default Constructor</summary>
        public Log()
        {
            Entries = new LogEntries();
        }

        /// <summary>
        /// Initializes a new Logger with the specified settings.
        /// </summary>
        /// <param name="settings">The seetings to initialize the logger.</param>
        public Log(LoggerSettings settings) : this()
        {
            InitializeListeners(settings);
        }

        /// <summary>
        /// Initializes a new Logger with the specified log entries.
        /// </summary>
        /// <param name="logEntries">The entries to initialize the logger.</param>
        public Log(IEnumerable<LogEntry> logEntries) : this()
        {
            Entries.AddRange(logEntries);
        }

        /// <summary>
        /// Initializes a new Logger with the specified log entries.
        /// </summary>
        /// <param name="logEntries">The serialized entries to initialize the logger.</param>
        public Log(BinaryContent logEntries) : this()
        {
            if (logEntries == null) throw new ArgumentNullException("logEntries");
            LoadBinary(logEntries);
        }

        /// <summary>
        /// Gets the Logger from the Configuration.
        /// </summary>
        /// <returns>An instance of the Logger from configuration.</returns>
        public static Log FromConfig()
        {
            Log logger = new Log();
            logger.InitializeListenersFromConfig();
            return logger;
        }

        #endregion

        #region Conversion
        public static explicit operator BinaryContent(Log log)
        {
            if (log == null) throw new ArgumentNullException("log");

            using (MemoryStream stream = new MemoryStream())
            {
                log.WriteTo(stream);
                stream.Seek(0, SeekOrigin.Begin);
                stream.Position = 0;

                BinaryContent binaryContent = new BinaryContent(stream);
                return binaryContent;
            }
        }

        #endregion

        #region Entries
        /// <summary>
        /// Gets the Log Entries in the Logger.
        /// </summary>
        public LogEntries Entries { get; private set; }

        /// <summary>
        /// Contextual log, important messages and traces.
        /// </summary>
        /// <returns>Log Entries</returns>
        public LogEntries Contextual()
        {
            int index = 0;
            LogEntries logEntries = new LogEntries();
            while (logEntries.Count < 200)
            {
                if (index >= Entries.Count) break;
                LogEntry logEntry = Entries[index++];

                switch (logEntry.EventType)
                {
                    case LogEventType.Information:
                    case LogEventType.Warning:
                        logEntries.Add(logEntry);
                        break;
                    case LogEventType.Error:
                    case LogEventType.Critical:
                        logEntries.AddRange(Entries.Skip(index - 10).Take(10).Where(match => !logEntries.Contains(match)));
                        logEntries.AddRange(Entries.Skip(index).Take(10));
                        index += 10;
                        break;
                    case LogEventType.Trace:
                    default:
                        continue;
                }
            }

            return logEntries;
        }

        public LogEntries Chronoligical()
        {
            LogEntries logEntries = new LogEntries();
            logEntries.AddRange(this.Entries.OrderByDescending(logEntry => logEntry.TimeStamp));
            return logEntries;
        }

        #endregion

        #region Write
        /// <summary>
        /// Writes trace information to the Log.
        /// </summary>
        /// <param name="logEntry">A log entry to log.</param>
        public void Write(LogEntry logEntry)
        {
            if (logEntry == null) throw new ArgumentNullException("logEntry");
            Listeners.ForEach(listener => listener.Trace(logEntry));
            Entries.Add(logEntry);
        }

        #endregion

        #region Listeners
        public void AttachDebugger()
        {
            if (!Listeners.Any(l => l.GetType().Equals(typeof(DebugTraceListener))))
            {
                Listeners.Add(new DebugTraceListener());
            }
        }

        public void AttachConsole()
        {
            if (!Listeners.Any(l => l.GetType().Equals(typeof(Andamio.Diagnostics.Listeners.ConsoleTraceListener))))
            {
                Listeners.Add(new Andamio.Diagnostics.Listeners.ConsoleTraceListener());
            }
        }

        /// <summary>
        /// Gets the Collection of Configured Listeners.
        /// </summary>
        private object _syncLock = new object();
        private readonly List<ILogRecorder> _logRecorders = new List<ILogRecorder>();
        public List<ILogRecorder> Listeners
        {
            get
            {
                lock (_syncLock)
                {
                    return _logRecorders;
                }
            }
        }

        private void InitializeListenersFromConfig()
        {
            lock (_syncLock)
            {
                LoggerSettings logSettings = LoggerSettings.FromConfiguration();
                InitializeListeners(logSettings);
            }
        }

        private void InitializeListeners(LoggerSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("logSession");

            lock (_syncLock)
            {
                _logRecorders.Clear();

                // Xml Logging
                if (settings.XmlTraceListener != null)
                {
                    ILogRecorder listener = settings.XmlTraceListener.CreateListener();
                    if (listener != null)
                    { _logRecorders.Add(listener); }
                }

                // Database Logging
                if (settings.SqliteTraceListener != null)
                {
                    ILogRecorder listener = settings.SqliteTraceListener.CreateListener();
                    if (listener != null)
                    { _logRecorders.Add(listener); }
                }

                // Console Output Logging
                if (settings.ConsoleListener != null)
                {
                    ILogRecorder listener = settings.ConsoleListener.CreateListener();
                    if (listener != null)
                    { _logRecorders.Add(listener); }
                }

                // Debug Outpur Logging
                if (settings.SqliteTraceListener != null)
                {
                    ILogRecorder listener = settings.DebugListener.CreateListener();
                    if (listener != null)
                    { _logRecorders.Add(listener); }
                }
            }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the content of the Log to Xml.
        /// </summary>
        /// <param name="name">The Xml Root Element Name.</param>
        /// <returns>An XElement root element containing the log entries.</returns>
        internal XElement GenerateXElement(XName name)
        {
            XNamespace xmlns = name.Namespace;
            XElement element = new XElement(name, Entries.Select(logEntry => logEntry.GenerateXml(xmlns + "LogEntry")));
            return element;
        }

        /// <summary>
        /// Populates the Log from an Xml Element containing the entries.
        /// </summary>
        /// <param name="element">The Xml Element containing the entries.</param>
        internal void PopulateFromXElement(XElement element)
        {
            XNamespace xmlns = element.Name.Namespace;
            var logEntries = element.Elements(xmlns + "LogEntry").Select(logEntryXElem => LogEntry.CreateFromXElement(logEntryXElem));
            Entries.AddRange(logEntries);
        }

        /// <summary>
        /// Loads the content of the Log from Xml.
        /// </summary>
        /// <param name="xmlReader">The XmlReader instance containing the data.</param>
        public void Load(XmlReader xmlReader)
        {
            XDocument document = XDocument.Load(xmlReader);
            XElement root = document.Root;
            PopulateFromXElement(root);
        }

        /// <summary>
        /// Loads the content of the Log from Xml.
        /// </summary>
        /// <param name="xmlReader">The XmlReader instance containing the data.</param>
        public void LoadXml(string filePath)
        {
            if (filePath.IsNullOrBlank()) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException(String.Format("Specified File '{0}' not found.", filePath), filePath);

            using (FileStream stream = File.OpenRead(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    Load(reader);
                }
            }
        }

        /// <summary>
        /// Loads the content of the Log from Sqlite.
        /// </summary>
        /// <param name="xmlReader">The XmlReader instance containing the data.</param>
        public void LoadSqlite(string filePath)
        {
            if (filePath.IsNullOrBlank()) throw new ArgumentNullException("filePath");
            if (!File.Exists(filePath)) throw new FileNotFoundException(String.Format("Specified File '{0}' not found.", filePath), filePath);

            using (SqliteDataContext context = new SqliteDataContext(filePath))
            {
                LogEvents logEvents = new LogEvents(context.LogEvents);
                Entries.AddRange(logEvents.Select(logEvent => (LogEntry) logEvent));
            }
        }

        /// <summary>
        /// Serializes the content of the Log to Xml.
        /// </summary>
        /// <returns>The XDocument Root Element containig the serialized entries.</returns>
        public XDocument ToXmlDocument()
        {
            XElement rootElement = GenerateXElement("Log");
            XDocument xml = new XDocument(rootElement);
            return xml;
        }

        /// <summary>
        /// Serializes and Writes the content of the Log to Xml.
        /// </summary>
        /// <param name="filePath">The Xml file path where the log will be serialized to.</param>
        public void ToXml(string filePath)
        {
            XDocument root = ToXmlDocument();
            root.Save(filePath, SaveOptions.None);
        }

        /// <summary>
        /// Loads the content of the Log from binary serialized entries.
        /// </summary>
        /// <param name="stream">The Stream containing the binary serialized entries.</param>
        public void LoadBinary(Stream stream)
        {
            if (stream == null)
            { throw new ArgumentNullException("stream"); }

            BinaryContent binaryContent = stream.Bytes();
            LoadBinary(binaryContent);
        }

        /// <summary>
        /// Loads the content of the Log from binary serialized entries.
        /// </summary>
        /// <param name="binaryContent">The entries serialized in binary format.</param>
        public void LoadBinary(BinaryContent binaryContent)
        {
            if (binaryContent == null)
            { throw new ArgumentNullException("binaryContent"); }

            GenericBinaryFormatter formatter = new GenericBinaryFormatter();
            using (MemoryStream stream = new MemoryStream(binaryContent))
            {
                while (stream.Position < binaryContent.Size)
                {
                    var logEntries = formatter.Deserialize<LogEntries>(stream);
                    Entries.AddRange(logEntries);
                }
            }
        }

        #endregion

        #region IO
        /// <summary>
        /// Writes the Content of the Log to the specified Stream in binary format.
        /// </summary>
        /// <param name="stream">The Stream where the content of the log will be written.</param>
        public void WriteTo(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            if (Entries.Any())
            {
                stream.Seek((int)stream.Position, SeekOrigin.Begin);
                stream.Position = (int)stream.Position;

                GenericBinaryFormatter formatter = new GenericBinaryFormatter();
                byte[] serialized = formatter.Serialize<LogEntries>(Entries);
                if (serialized != null && serialized.Any())
                {
                    stream.Write(serialized, 0, serialized.Length);
                }
            }
        }

        #endregion
    }
}
