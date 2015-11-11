using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Configuration;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Creates a Provider to work against the specified database.
    /// </summary>
    /// <remarks>
    /// DataProvider class provides a generic programming data access model that does not depend on a specific data provider.
    /// </remarks>
    public sealed class DataConnectionProvider : IDisposable
    {
        #region Private Fields
        private readonly string _connectionString;

        private bool _disposed = false;
        private object _syncLock = new object();
        #endregion

        #region Constructors, Destructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        private DataConnectionProvider()
        {
        }

        /// <summary>
        /// Creates a new DataProvider object using specified connection string.
        /// </summary>
        /// <param name="connectionStringName">The name of the connection string to access the database.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Exception raised of connection string cannot be found.</exception>
        public DataConnectionProvider(string connectionStringName)
        {
            if (String.IsNullOrEmpty(connectionStringName))
            { throw new ArgumentException("Invalid connection string name.", "connectionStringName"); }

            //get the connection string from the config file
            ConnectionStringSettings connStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];

            if (connStringSettings == null)
            { throw new ConfigurationErrorsException(String.Format("Could not find connection string settings for connection named '{0}'.", connectionStringName)); }

            // Get Connection String from config.
            _connectionString = connStringSettings.ConnectionString;

            // Gets instance of Data Provider Factory based on provider type.
            ProviderFactory = DbProviderFactories.GetFactory(connStringSettings.ProviderName);

            // Create Database Connection for Provider.            
            Connection = ProviderFactory.CreateConnection();
            Connection.ConnectionString = _connectionString;
        }

        public DataConnectionProvider(string connectionString, string providerName)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Invalid connection string.", "connectionString");

            if (String.IsNullOrEmpty(providerName))
                throw new ArgumentException("Invalid provider Name.", "providerName");

            _connectionString = connectionString;

            // Gets instance of Data Provider Factory based on provider type.
            ProviderFactory = DbProviderFactories.GetFactory(providerName);

            // Create Database Connection for Provider.
            Connection = this.ProviderFactory.CreateConnection();
            Connection.ConnectionString = _connectionString;
        }

        public DataConnectionProvider(DbConnectionStringSettings connectionStringSettings)
        {
            if (connectionStringSettings == null)
                throw new ArgumentNullException("connectionStringSettings");

            _connectionString = connectionStringSettings.ConnectionString;

            // Gets instance of Data Provider Factory based on provider type.
            ProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);

            // Create Database Connection for Provider.
            Connection = this.ProviderFactory.CreateConnection();
            Connection.ConnectionString = _connectionString;
        }

        /// <summary>
        /// Class destructor for finalization code, this method will run only if Dispose method is called by GC. It gives the base 
        /// class and derived classes the opportunity to run any clean up code.
        /// </summary>
        ~DataConnectionProvider()
        {
            // Though a situation in which Cleanup() executes twice should not occur since
            // GC.SuppressFinalize() is being called from Dispose(), a check is made to only
            // run Cleanup() if object has not been Disposed earlier.
            if (!IsDisposed)
                Cleanup();
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the DbProviderFactory for the database.
        /// </summary>
        public DbProviderFactory ProviderFactory { get; internal set; }

        /// <summary>
        /// Gets the connection string for the Data Provider.
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Gets the Connection associated with the connection string.
        /// </summary>
        public DbConnection Connection { get; internal set; }

        /// <summary>
        /// Gets a bool value which determines if Disposed method has been called by either GC or client code.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (_syncLock)
                {
                    return _disposed;
                }
            }
        }

        #endregion

        #region Public Methods - DbCommand
        /// <summary>
        /// Creates a DbCommand object associated with the current connection.
        /// </summary>
        /// <param name="commandType">Type of command to create.</param>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <returns>A DbCommand object associated with the current connection.</returns>
        public DbCommand CreateCommand(CommandType commandType, string commandText)
        {
            DbCommand command = this.ProviderFactory.CreateCommand();

            command.Connection = this.Connection;
            command.CommandText = commandText;
            command.CommandType = commandType;

            return command;
        }

        /// <summary>
        /// Creates a DbCommand object that will execute a stored procedure.
        /// </summary>
        /// <param name="commandText">The text command to run against the database.</param>
        /// <returns>A stored procedure DbCommand object.</returns>
        public DbCommand CreateStoredProcedureCommand(string commandText)
        {
            return CreateCommand(CommandType.StoredProcedure, commandText);
        }

        public DbCommand CreateTextCommand(string commandText)
        {
            return CreateCommand(CommandType.Text, commandText);
        }

        #endregion

        #region Public Methods - Parameters
        /// <summary>
        /// Creates a DbType.Guid Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Guid Parameter object.</returns>
        public DbParameter CreateGuidParameter(string name, Guid value)
        {
            return CreateParameter(name, value, DbType.Guid, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.Int32 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int32 Parameter object.</returns>                
        public DbParameter CreateInt32Parameter(string name, Int32 value)
        {
            return CreateParameter(name, value, DbType.Int32, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.Int16 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int16 Parameter object.</returns>        
        public DbParameter CreateInt16Parameter(string name, Int16 value)
        {
            return CreateParameter(name, value, DbType.Int16, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.Decimal Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Decimal Parameter object.</returns>        
        public DbParameter CreateDecimalParameter(string name, decimal value)
        {
            return CreateParameter(name, value, DbType.Decimal, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.String Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.String Parameter object.</returns>                        
        public DbParameter CreateStringParameter(string name, string value)
        {
            return CreateParameter(name, value, DbType.String, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.DateTime Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.DateTime Parameter object.</returns>        
        public DbParameter CreateDateTimeParameter(string name, DateTime value)
        {
            return CreateParameter(name, value, DbType.DateTime, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a DbType.Boolean Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Boolean Parameter object.</returns>         
        public DbParameter CreateBooleanParameter(string name, bool value)
        {
            return CreateParameter(name, value, DbType.Boolean, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a Parameter object with specified name, value, and type.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <returns>A new Parameter object.</returns>
        public DbParameter CreateParameter(string name, object value, DbType dbType)
        {
            return CreateParameter(name, value, dbType, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a Parameter object with specified name, value, type, and direction.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <param name="direction">The direction of the Parameter.</param>
        /// <returns>A new Parameter object.</returns>
        public DbParameter CreateParameter(string name, object value, DbType dbType, ParameterDirection direction)
        {
            DbParameter parameter = this.ProviderFactory.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = (value == null) ? DBNull.Value : value;
            parameter.DbType = dbType;
            parameter.Direction = direction;

            return parameter;
        }

        public DbParameter CreateParameter<T>(string name, T value)
        {
            return CreateParameter<T>(name, value, ParameterDirection.Input);
        }

        public DbParameter CreateParameter<T>(string name, T value, ParameterDirection direction)
        {
            DbType dbType = ClrTypeToDbType(typeof(T));
            return CreateParameter(name, value, dbType, direction);
        }

        /// <summary>
        /// Creates an output Parameter object with specified name, and value.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <returns>A new output Parameter object.</returns>
        public DbParameter CreateOutParameter(string name, DbType dbType)
        {
            DbParameter parameter = this.ProviderFactory.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Direction = ParameterDirection.Output;

            return parameter;
        }

        /// <summary>
        /// Creates an Return Parameter object with specified name, and value.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <returns>A new Return Parameter object.</returns>
        public DbParameter CreateReturnParameter(string name, DbType dbType)
        {
            DbParameter parameter = this.ProviderFactory.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Direction = ParameterDirection.ReturnValue;

            return parameter;
        }

        #endregion

        #region Private Methods
        private DbType ClrTypeToDbType(Type type)
        {
            DbType dbType = EnumDisplayAttribute.ParseEnum<DbType>(type.Name, DbType.Object);
            return dbType;
        }
        #endregion

        #region IDisposable Members, Cleanup()
        public void Dispose()
        {
            if (_disposed)
                return;

            lock (_syncLock)
            {
                if (!_disposed)
                {
                    // Request object is taken off Finalization Queue
                    // this prevents Finalization code from executing twice
                    GC.SuppressFinalize(this);

                    Cleanup();
                    _disposed = true;
                }
            }
        }

        private void Cleanup()
        {
            if (this.Connection != null && this.Connection.State != ConnectionState.Closed)
            {
                this.Connection.Close();
                this.Connection.Dispose();
            }
        }

        #endregion
    }
}
