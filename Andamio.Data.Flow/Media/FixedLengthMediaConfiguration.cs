using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Formatters;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Media
{
    public class FixedLengthMediaConfiguration : MediaConfiguration
    {
        #region Constructors
        private FixedLengthMediaConfiguration() : base()
        {
            Settings = new FileReaderSettings();
            Reader = ReaderFactory.FixedLength(Settings);
        }

        internal FixedLengthMediaConfiguration(string path) : this()
        {
            if (path.IsNullOrBlank()) throw new ArgumentNullException("path");
            Settings.FilePath = path;
        }

        #endregion

        #region Settings
        public FileReaderSettings Settings { get; private set; }

        #endregion

        #region Rows
        public FixedLengthMediaConfiguration Take(int rowCount)
        {
            Settings.RowCount = rowCount;
            return this;
        }

        public FixedLengthMediaConfiguration Skip(int rowCount)
        {
            if (rowCount < 0) throw new ArgumentException("Row Count must be zero or greater.");
            Settings.SkipCount = rowCount;
            return this;
        }

        #endregion

        #region Log
        public FixedLengthMediaConfiguration WithLog(Log log)
        {
            if (log == null) throw new ArgumentNullException("log");
            Log = log;
            return this;
        }

        #endregion
    }
}
