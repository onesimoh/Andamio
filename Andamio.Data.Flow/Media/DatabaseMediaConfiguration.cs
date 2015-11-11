using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Formatters;
using Andamio.Data.Flow.Media.Readers;
using Andamio.Data.Access;

namespace Andamio.Data.Flow.Media
{
    public class DatabaseMediaConfiguration : MediaConfiguration
    {
        #region Constructors
        private DatabaseMediaConfiguration() : base()
        {
            Settings = new DatabaseReaderSettings();
            Reader = ReaderFactory.Database(Settings);
        }

        public DatabaseMediaConfiguration(DbConnectionStringSettings connectionStringSettings) : this()
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");
            Settings.ConnectionString = connectionStringSettings.ConnectionString;
            Settings.ConnectionProvider = connectionStringSettings.ProviderName;
        }

        public DatabaseMediaConfiguration(string connectionString) : this()
        {
            if (connectionString.IsNullOrBlank()) throw new ArgumentNullException("connectionString");
            Settings.ConnectionString = connectionString;
        }

        public DatabaseMediaConfiguration(ConnectionStringSettings connectionStringSettings) : this()
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");
            Settings.ConnectionString = connectionStringSettings.ConnectionString;
            Settings.ConnectionProvider = connectionStringSettings.ProviderName;
        }

        #endregion

        #region Settings
        public DatabaseReaderSettings Settings { get; private set; }

        #endregion

        #region Table
        public DatabaseMediaConfiguration Table(string tableName)
        {
            if (tableName.IsNullOrBlank()) throw new ArgumentNullException("tableName");
            //Settings.TableName = tableName;
            return this;
        }

        public DatabaseMediaConfiguration Table(string tableName, string schemaName)
        {
            if (tableName.IsNullOrBlank()) throw new ArgumentNullException("tableName");
            if (schemaName.IsNullOrBlank()) throw new ArgumentNullException("schemaName");
            //Settings.TableName = tableName;
            //Settings.TableSchema = schemaName;
            return this;
        }

        #endregion

        #region Sql Query
        public DatabaseMediaConfiguration Query(string sqlQuery)
        {
            if (sqlQuery.IsNullOrBlank()) throw new ArgumentNullException("sqlQuery");
            Settings.SqlQuery = sqlQuery;
            return this;
        }

        #endregion

        #region Log
        public DatabaseMediaConfiguration WithLog(Log log)
        {
            if (log == null) throw new ArgumentNullException("log");
            Log = log;
            return this;
        }

        #endregion
    }
}
