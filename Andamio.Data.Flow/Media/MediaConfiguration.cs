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
using Andamio.Data.Access;

namespace Andamio.Data.Flow.Media
{
    public abstract class MediaConfiguration
    {
        #region Constructors
        protected MediaConfiguration()
        {
            Log = new Log();
            Log.AttachDebugger();
        }

        protected MediaConfiguration(ImportReader reader, Log log)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (log == null) throw new ArgumentNullException("log");
            Reader = reader;
            Log = log;
        }

        #endregion

        #region Log
        public Log Log { get; set; }

        #endregion

        #region Reader
        public ImportReader Reader { get; set; }

        private void ValidateReader()
        {
            if (Reader == null) throw new InvalidOperationException("Reader has not been provided.");
            if (Log == null) throw new InvalidOperationException("Log has not been provided.");
            Reader.Log = Log;
        }

        #endregion

        #region Grid
        public GridFormatter ToGrid()
        {
            ValidateReader();
            return new GridFormatter(this);
        }

        #endregion

        #region Database
        public DatabaseFormatter ToDatabase()
        {
            ValidateReader();
            return new DatabaseFormatter(this);
        }

        public DatabaseFormatter ToDatabase(DbConnectionSettings connectionSettings)
        {
            DatabaseFormatter databaseConfiguration = ToDatabase();
            databaseConfiguration.Connection(connectionSettings);
            return databaseConfiguration;
        }

        public DatabaseFormatter ToDatabase(string connectionString)
        {
            DatabaseFormatter databaseConfiguration = ToDatabase();
            databaseConfiguration.Connection(connectionString);
            return databaseConfiguration;
        }

        public DatabaseFormatter ToDatabase(System.Configuration.ConnectionStringSettings connectionStringSettings)
        {
            DatabaseFormatter databaseConfiguration = ToDatabase();
            databaseConfiguration.Connection(connectionStringSettings);
            return databaseConfiguration;
        }

        #endregion

        #region Entity
        public EntityFormatter<T> ToEntity<T>()
        {
            ValidateReader();
            return new EntityFormatter<T>(this);
        }

        #endregion
    }
}
