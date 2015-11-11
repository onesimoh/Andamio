using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Diagnostics;

namespace Andamio.Data.Flow.Media.Readers
{
    public class CSVReader : TabularImportReader
    {
        #region Constructors
        protected CSVReader()
        {
        }

        public CSVReader(FileReaderSettings settings) : base()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        public CSVReader(FileReaderSettings settings, Log log) : base(log)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;        
        }

        #endregion

        #region Settings
        public FileReaderSettings Settings { get; protected set; }

        #endregion

        #region Populate
        protected override DataGrid ReadRawData(DataGridColumns columns)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
