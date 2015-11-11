using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Common;

using Andamio.Diagnostics;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Media.Readers
{
    #region Settings
    public class DatabaseReaderSettings : ReaderSettings
    {
        #region Constructors
        public DatabaseReaderSettings()
        {
            //TableSchema = "dbo";
        }

        #endregion

        #region Properties
        //public string TableName { get; set; }
        //public string TableSchema { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionProvider { get; set; }
        public string SqlQuery { get; set; }

        #endregion
    }

    #endregion


    public class SqlQueryReader : TabularImportReader
    {
        #region Constructors
        private SqlQueryReader() : base()
        {
        }

        public SqlQueryReader(DatabaseReaderSettings settings) : base()
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        public SqlQueryReader(DatabaseReaderSettings settings, Log log): base(log)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            Settings = settings;
        }

        #endregion

        #region Settings
        public DatabaseReaderSettings Settings { get; protected set; }

        #endregion

        #region Populate
        protected override DataGrid ReadRawData(DataGridColumns columns)
        {
            try
            {                
                // Data
                using (DbContext context = new DbContext(Settings.ConnectionString))
                {
                    using (DbCommand dbCommand = context.Database.Connection.CreateCommand())
                    {
                        context.Database.Connection.Open();
                        dbCommand.CommandText = Settings.SqlQuery;
                        dbCommand.CommandTimeout = 300;

                        DataGrid dataGrid = new DataGrid();
                        using (DbDataReader reader = dbCommand.ExecuteReader())
                        {
                            for (int field = 0; field < reader.FieldCount; field++)
                            {
                                string columnName = reader.GetName(field);
                                dataGrid.Columns.Add(new DataGridColumn(columnName)
                                {
                                    ColumnType = reader.GetFieldType(field),
                                    OriginalSourceIndex = reader.GetOrdinal(columnName)
                                });
                            }

                            while (reader.Read())
                            {
                                DataGridRow dataGridRow = dataGrid.Rows.New();
                                for (int field = 0; field < reader.FieldCount; field++)
                                {
                                    DataGridCell cell = new DataGridCell();
                                    cell.OriginalValue = !reader.IsDBNull(field) ? reader[field] : null;
                                    dataGridRow.Cells.Add(cell);
                                }
                            }

                            return dataGrid;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("An error occurred while executing the Sql Source Query.", e);
            }            
        }


        #endregion
    }
}
