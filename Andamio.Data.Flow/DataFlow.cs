using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Andamio;
using Andamio.Data.Flow.Media;
using Andamio.Data.Flow.Media.Readers;
using Andamio.Data.Access;

namespace Andamio.Data.Flow
{
    public static class DataFlow
    {
        #region Excel
        /*
        public static ExcelMediaConfiguration FromExcel(string path)
        {            
            return new ExcelMediaConfiguration(path);
        }
        */

        #endregion

        #region CSV
        public static CSVMediaConfiguration FromCSV(string path)
        {
            return new CSVMediaConfiguration(path);
        }

        #endregion

        #region Fixed Length
        public static FixedLengthMediaConfiguration FromFixedLength(string path)
        {
            return new FixedLengthMediaConfiguration(path);
        }        
        
        #endregion

        #region Database
        public static DatabaseMediaConfiguration FromDatabase(DbConnectionSettings connectionSettings)
        {
            return new DatabaseMediaConfiguration(connectionSettings);
        }

        public static DatabaseMediaConfiguration FromDatabase(string connectionString)
        {
            return new DatabaseMediaConfiguration(connectionString);
        }

        public static DatabaseMediaConfiguration FromDatabase(ConnectionStringSettings connectionSettings)
        {
            return new DatabaseMediaConfiguration(connectionSettings);
        }

        #endregion
    }
}
