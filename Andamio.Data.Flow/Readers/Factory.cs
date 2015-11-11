using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Andamio;
using Andamio.Data;
using Andamio.Diagnostics;

namespace Andamio.Data.Flow.Media.Readers
{
    internal static class ReaderFactory
    {
        #region Excel
        /*
        public static ExcelReader Excel(ExcelReaderSettings settings, Log logger)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (logger == null) throw new ArgumentNullException("logger");
            return new ExcelReader(settings, logger);
        }

        public static ExcelReader Excel(ExcelReaderSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new ExcelReader(settings);
        }
        */

        #endregion

        #region CSV
        public static CSVReader CSV(FileReaderSettings settings, Log logger)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (logger == null) throw new ArgumentNullException("logger");
            return new CSVReader(settings, logger);
        }

        public static CSVReader CSV(FileReaderSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new CSVReader(settings);
        }

        #endregion

        #region FixedLength
        public static FixedLengthReader FixedLength(FileReaderSettings settings, Log logger)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (logger == null) throw new ArgumentNullException("logger");
            return new FixedLengthReader(settings, logger);
        }

        public static FixedLengthReader FixedLength(FileReaderSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new FixedLengthReader(settings);
        }

        #endregion

        #region Database
        public static SqlQueryReader Database(DatabaseReaderSettings settings, Log logger)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (logger == null) throw new ArgumentNullException("logger");
            return new SqlQueryReader(settings, logger);
        }

        public static SqlQueryReader Database(DatabaseReaderSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            return new SqlQueryReader(settings);
        }

        #endregion

        #region Create
        public static T Create<T>(ReaderSettings settings, Log logger) where T : ImportReader
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (logger == null) throw new ArgumentNullException("logger");
            object[] args = { settings, logger };
            return Activator.CreateInstance(typeof(T), args) as T;
        }

        public static T Create<T>(ReaderSettings settings) where T : ImportReader
        {
            if (settings == null) throw new ArgumentNullException("settings");
            object[] args = { settings };
            return Activator.CreateInstance(typeof(T), args) as T;
        }

        #endregion
    }
}
