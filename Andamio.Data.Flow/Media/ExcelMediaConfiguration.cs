using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media.Readers;
using Andamio.Data.Flow.Formatters;

namespace Andamio.Data.Flow.Media
{
    public class ExcelMediaConfiguration : MediaConfiguration
    {
        #region Constructors
        private ExcelMediaConfiguration() : base()
        {
            Settings = new ExcelReaderSettings();
            Reader = ReaderFactory.Excel(Settings);
        }

        internal ExcelMediaConfiguration(string path) : this()
        {
            if (path.IsNullOrBlank()) throw new ArgumentNullException("path");
            Settings.FilePath = path;
        }

        internal ExcelMediaConfiguration(string path, params string[] sheets) : this(path)
        {
            if (sheets != null && sheets.Any())
            {
                Settings.Worksheet = sheets.JoinStrings(",");
            }
        }

        #endregion

        #region Settings
        public ExcelReaderSettings Settings { get; private set; }

        #endregion

        #region Sheet
        public ExcelMediaConfiguration Sheet(string worksheet)
        {
            if (worksheet.IsNullOrBlank()) throw new ArgumentNullException("worksheet");
            Settings.Worksheet = worksheet;
            return this;
        }

        #endregion

        #region Protection
        public ExcelMediaConfiguration Password(string password)
        {
            if (password.IsNullOrBlank()) throw new ArgumentNullException("password");
            Settings.Password = password;
            return this;
        }

        #endregion

        #region Rows
        public ExcelMediaConfiguration Take(int rowCount)
        {
            Settings.RowCount = rowCount;
            return this;
        }

        public ExcelMediaConfiguration Skip(int rowCount)
        {
            if (rowCount < 0) throw new ArgumentException("Row Count must be zero or greater.");
            Settings.SkipCount = rowCount;
            return this;
        }

        #endregion

        #region Log
        public ExcelMediaConfiguration WithLog(Log log)
        {
            if (log == null) throw new ArgumentNullException("log");
            Log = log;
            return this;
        }

        #endregion
    }
}
