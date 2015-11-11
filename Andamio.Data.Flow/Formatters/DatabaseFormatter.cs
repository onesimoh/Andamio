using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Data.Entity;
using System.Data.SqlClient;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media;
using Andamio.Data.Flow.Media.Readers;
using Andamio.Data.Access;

namespace Andamio.Data.Flow.Formatters
{
    #region DatabaseFormatterType
    public enum DatabaseFormatterType
    {
        Undefined = 0,
        Table = 1,
        SqlQuery = 2,
    }

    public static class DatabaseFormatterTypeExtensions
    {
        public static bool IsDefined(this DatabaseFormatterType type)
        {
            return type != DatabaseFormatterType.Undefined;
        }
        public static bool IsSqlTable(this DatabaseFormatterType type)
        {
            return type == DatabaseFormatterType.Table;
        }
        public static bool IsSqlQuery(this DatabaseFormatterType type)
        {
            return type == DatabaseFormatterType.SqlQuery;
        }
    }

    #endregion


    public class DatabaseFormatterSettings
    {
        #region Constructors
        public DatabaseFormatterSettings()
        {
        }

        #endregion

        #region Properties
        public string TableName { get; set; }
        public string TableSchema { get; set; }
        public string SqlQuery { get; set; }
        public DatabaseFormatterType FormatterType { get; set; } 
        public string ConnectionString { get; set; }
        public string ConnectionProvider { get; set; }
        public string PreEvent { get; set; }
        public string PostEvent { get; set; }
        public bool AlwaysTruncate { get; set; }

        #endregion
    }


    public class DatabaseFormatter : Formatter
    {
        #region Constructors
        private DatabaseFormatter() : base()
        {
        }

        internal DatabaseFormatter(MediaConfiguration mediaConfiguration)
            : base(mediaConfiguration)
        {
            Settings = new DatabaseFormatterSettings();
            Columns = new DatabaseColumnCollection();
            Columns.ItemsInserted += OnColumnsInserted;     
        }

        #endregion

        #region Settings
        public DatabaseFormatterSettings Settings { get; private set; }

        #endregion

        #region Connection
        public DatabaseFormatter Connection(DbConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");
            Settings.ConnectionString = connectionStringSettings.ConnectionString;
            Settings.ConnectionProvider = connectionStringSettings.ProviderName;
            return this;
        }

        public DatabaseFormatter Connection(string connectionString)
        {
            if (connectionString.IsNullOrBlank()) throw new ArgumentNullException("connectionString");
            Settings.ConnectionString = connectionString;
            return this;
        }

        public DatabaseFormatter Connection(ConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");
            Settings.ConnectionString = connectionStringSettings.ConnectionString;
            Settings.ConnectionProvider = connectionStringSettings.ProviderName;
            return this;
        }

        #endregion

        #region Events
        public DatabaseFormatter PreEvent(string @event)
        {
            if (!@event.IsNullOrBlank())
            { Settings.PreEvent = @event; }
            return this;
        }

        public DatabaseFormatter PostEvent(string @event)
        {
            if (!@event.IsNullOrBlank())
            { Settings.PostEvent = @event; }
            return this;
        }

        public DatabaseFormatter Truncate()
        {
            Settings.AlwaysTruncate = true;
            return this;
        }

        public DatabaseFormatter Truncate(bool truncate)
        {
            Settings.AlwaysTruncate = truncate;
            return this;
        }

        public DatabaseFormatter NotTruncate()
        {
            Settings.AlwaysTruncate = false;
            return this;
        }

        #endregion

        #region Table
        public DatabaseFormatter Table(string tableName, string schemaName = null)
        {
            if (tableName.IsNullOrBlank()) throw new ArgumentNullException("tableName");
            Settings.SqlQuery = null;
            Settings.TableName = tableName;
            Settings.TableSchema = schemaName;
            Settings.FormatterType = DatabaseFormatterType.Table;
            return this;  
        }

        public string Table()
        {
            return !Settings.TableSchema.IsNullOrBlank() ? String.Format("{0}.{1}", Settings.TableSchema, Settings.TableName) : Settings.TableName;
        }

        #endregion

        #region Query
        public DatabaseFormatter Query(string sqlQuery)
        {
            if (sqlQuery.IsNullOrBlank()) throw new ArgumentNullException("sqlQuery");
            Settings.TableName = null;
            Settings.TableSchema = null;            
            Settings.SqlQuery = sqlQuery;
            Settings.FormatterType = DatabaseFormatterType.SqlQuery;
            return this;
        }

        #endregion

        #region Columns
        private readonly List<string> Excluded = new List<string>();
        public DatabaseColumnCollection Columns { get; private set; }
        public DatabaseColumn Column(string columnName)
        {
            DatabaseColumn columnConfig = new DatabaseColumn(columnName);
            Columns.Add(columnConfig);
            return columnConfig;
        }

        void OnColumnsInserted(object sender, ItemEventArgs<IEnumerable<DatabaseColumn>> e)
        {
            var columns = e.Item;
            if (columns != null && columns.Any())
            { columns.ForEach(c => c.Parent = this); }
        }

        public DatabaseFormatter Exclude(string columnName)
        {
            if (columnName.IsNullOrBlank()) throw new ArgumentNullException("columnName");
            Excluded.Add(columnName);
            return this;
        }

        #endregion

        #region Mappings
        public virtual DatabaseFormatter Mappings(FileInfo file)
        {
            ReadMappings(file);
            return this;
        }

        public virtual DatabaseFormatter Mappings(XmlReader xmlReader)
        {
            ReadMappings(xmlReader);
            return this;
        }

        public virtual DatabaseFormatter Mappings(string xml)
        {
            ReadMappings(xml);
            return this;
        }

        public virtual DatabaseFormatter Mappings(XDocument document)
        {
            ReadMappings(document);
            return this;
        }

        public virtual DatabaseFormatter Mappings(XElement element)
        {
            ReadMappings(element);
            return this;
        }

        public override void ReadMappings(XElement element)
        {
            if (element == null) throw new ArgumentNullException("element");
            XNamespace xmlns = element.Name.Namespace;
            element.Elements(xmlns + "Column").ForEach(delegate(XElement columnNode)
            {
                DatabaseColumn column = Columns.New();
                column.FromElement(columnNode);
            });
        }

        #endregion

        #region Import
        public override DataGrid Import()
        {
            if (!Settings.FormatterType.IsDefined()) throw new InvalidOperationException("A Table Name or Sql Query was not specified.");

            DataGrid dataGrid = new DataGrid();
            Columns.Where(match => !Excluded.Contains(match.ColumnName)).ForEach(c => dataGrid.Columns.Add(c));
            dataGrid.Read(MediaConfiguration.Reader);

            Log.Info(String.Format("Importing {0} Rows to Database...", dataGrid.Rows.Count));

            string sqlQuery;
            if (Settings.FormatterType.IsSqlTable())
            {
                sqlQuery = String.Format("INSERT INTO {0} ({1}) VALUES ({2})"
                    , Table()
                    , Columns.Select(c => c.DbColumnName()).JoinStrings(",")
                    , Columns.Select(c => c.DbParamName()).JoinStrings(","));
            }
            else if (Settings.FormatterType.IsSqlQuery())
            {
                sqlQuery = Settings.SqlQuery;
            }
            else
            {
                throw new NotSupportedException(String.Format("Formatter Type '{0}' Is Not Supported.", Settings.FormatterType.DisplayName()));
            }

            using (DbContext context = new DbContext(Settings.ConnectionString))
            {
                context.Database.CommandTimeout = 300;
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {                    
                    if (Settings.FormatterType.IsSqlTable() && Settings.AlwaysTruncate)
                    {
                        Log.Info("Truncating Table...");
                        context.Database.ExecuteSqlCommand(String.Format("TRUNCATE TABLE {0}", Table()));
                        Log.Info("Done Truncating Table.");
                    }

                    if (!Settings.PreEvent.IsNullOrBlank())
                    {
                        Log.Info("Executing Pre-event...", new LogAttachment("Sql", Settings.PreEvent));
                        Log.Trace(Settings.PreEvent);
                        context.Database.ExecuteSqlCommand(Settings.PreEvent);
                        Log.Info("Done Executing Pre-event.");
                    }

                    foreach (DataGridRow row in dataGrid.Rows)
                    {
                        int rowIndex = row.Index() + 1;
                        SqlParameter[] sqlParams = null;
                        try
                        {                            
                            Log.Trace(String.Format("Importing Row '{0}' to Database.", rowIndex));
                            sqlParams = Columns.Select(c => c.DbParam(row[c.ColumnName])).ToArray();
                            context.Database.ExecuteSqlCommand(sqlQuery, sqlParams);
                        }
                        catch (Exception exception)
                        {
                            string error = String.Format("An error ocurred while importing Row '{0}' to Database.", rowIndex);

                            LogAttachments attachments = new LogAttachments();
                            attachments.Add(new LogAttachment("Sql", sqlQuery));
                            if (sqlParams != null)
                            { 
                                attachments.AddRange(sqlParams.Select(p => new LogAttachment(p.ParameterName, p.Value.FormatString()))); 
                            }

                            Log.Error(error, exception, attachments.ToArray());

                            throw new AbortException(error, exception);
                        }
                    }

                    if (!Settings.PostEvent.IsNullOrBlank())
                    {
                        Log.Info("Executing Post-event...");
                        Log.Trace(Settings.PostEvent);
                        context.Database.ExecuteSqlCommand(Settings.PostEvent);
                        Log.Info("Done Executing Post-event.");
                    }

                    transaction.Commit();
                }
            }

            Log.Info("Done importing to Database.");

            return dataGrid;
        }

        #endregion
    }
}
