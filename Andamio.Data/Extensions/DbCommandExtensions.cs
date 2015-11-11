using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace Andamio.Data
{
    public static class DbCommandExtensions
    {
        #region Guid
        /// <summary>
        /// Creates a DbType.Guid Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Guid Parameter object.</returns>
        public static DbParameter AddGuidParameter(this DbCommand command, string name, Guid value)
        {
            return AddInputParameter<Guid>(command, name, value, DbType.Guid);
        }

        /// <summary>
        /// Creates a DbType.Guid Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Guid Parameter object.</returns>
        public static DbParameter AddNullableGuidParameter(this DbCommand command, string name, Guid? value)
        {
            return AddNullableInputParameter<Guid>(command, name, value, DbType.Guid);
        }

        #endregion

        #region Integer
        /// <summary>
        /// Creates a DbType.Int32 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int32 Parameter object.</returns>                
        public static DbParameter AddInt32Parameter(this DbCommand command, string name, Int32 value)
        {
            return AddInputParameter<Int32>(command, name, value, DbType.Int32);
        }

        /// <summary>
        /// Creates a DbType.Int32 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int32 Parameter object.</returns>                
        public static DbParameter AddNullableInt32Parameter(this DbCommand command, string name, Int32? value)
        {
            return AddNullableInputParameter<Int32>(command, name, value, DbType.Int32);
        }

        /// <summary>
        /// Creates a DbType.Int16 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int16 Parameter object.</returns>        
        public static DbParameter AddInt16Parameter(this DbCommand command, string name, Int16 value)
        {
            return AddInputParameter<Int16>(command, name, value, DbType.Int16);
        }

        /// <summary>
        /// Creates a DbType.Int16 Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Int16 Parameter object.</returns>        
        public static DbParameter AddNullableInt16Parameter(this DbCommand command, string name, Int16? value)
        {
            return AddNullableInputParameter<Int16>(command, name, value, DbType.Int16);
        }

        #endregion

        #region Decimal
        /// <summary>
        /// Creates a DbType.Decimal Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Decimal Parameter object.</returns>        
        public static DbParameter AddDecimalParameter(this DbCommand command, string name, decimal value)
        {
            return AddInputParameter<Decimal>(command, name, value, DbType.Decimal);
        }

        /// <summary>
        /// Creates a DbType.Decimal Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Decimal Parameter object.</returns>        
        public static DbParameter AddNullableDecimalParameter(this DbCommand command, string name, decimal? value)
        {
            return AddNullableInputParameter<Decimal>(command, name, value, DbType.Decimal);
        }

        #endregion

        #region String
        /// <summary>
        /// Creates a DbType.String Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.String Parameter object.</returns>                        
        public static DbParameter AddStringParameter(this DbCommand command, string name, string value)
        {
            return AddInputParameter<String>(command, name, value, DbType.String);
        }

        #endregion

        #region DateTime
        /// <summary>
        /// Creates a DbType.DateTime Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.DateTime Parameter object.</returns>        
        public static DbParameter AddNullableDateTimeParameter(this DbCommand command, string name, DateTime value)
        {
            return AddNullableInputParameter<DateTime>(command, name, value, DbType.DateTime);
        }

        /// <summary>
        /// Creates a DbType.DateTime Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.DateTime Parameter object.</returns>        
        public static DbParameter AddDateTimeParameter(this DbCommand command, string name, DateTime value)
        {
            return AddInputParameter<DateTime>(command, name, value, DbType.DateTime);
        }

        #endregion

        #region Boolean
        /// <summary>
        /// Creates a DbType.Boolean Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Boolean Parameter object.</returns>         
        public static DbParameter AddBooleanParameter(this DbCommand command, string name, bool value)
        {
            return AddInputParameter<Boolean>(command, name, value, DbType.Boolean);
        }

        /// <summary>
        /// Creates a DbType.Boolean Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Boolean Parameter object.</returns>         
        public static DbParameter AddNullableBooleanParameter(this DbCommand command, string name, bool? value)
        {
            return AddNullableInputParameter<Boolean>(command, name, value, DbType.Boolean);
        }

        #endregion

        #region Binary
        /// <summary>
        /// Creates a DbType.Binary Parameter object.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <returns>A new DbType.Binary Parameter object.</returns>
        public static DbParameter AddBinaryParameter(this DbCommand command, string name, Byte[] value)
        {
            return AddInputParameter<Byte[]>(command, name, value, DbType.Binary);
        }

        #endregion

        #region Parameter
        /// <summary>
        /// Creates a Parameter object with specified name, value, and type.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <returns>A new Parameter object.</returns>
        public static DbParameter AddParameter(this DbCommand command, string name, object value, DbType dbType, ParameterDirection direction)
        {
            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = dbType;
            parameter.Direction = direction;

            command.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// Creates a Parameter object with specified name, value, type, and direction.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <param name="direction">The direction of the Parameter.</param>
        /// <returns>A new Parameter object.</returns>
        public static DbParameter AddInputParameter<T>(this DbCommand command, string name, T value, DbType dbType)
        {
            return AddParameter<T>(command, name, value, dbType, ParameterDirection.Input);
        }

        /// <summary>
        /// Creates a Parameter object with specified name, value, type, and direction.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="value">The value of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <param name="direction">The direction of the Parameter.</param>
        /// <returns>A new Parameter object.</returns>
        public static DbParameter AddNullableInputParameter<T>(this DbCommand command, string name, T? value, DbType dbType)
            where T : struct
        {
            object paramValue = null;

            if (value.HasValue)
                paramValue = value.Value;
            else
                paramValue = DBNull.Value;

            return AddParameter(command, name, paramValue, dbType, ParameterDirection.Input);
        }

        public static DbParameter AddParameter<T>(this DbCommand command, string name, T value, DbType dbType, ParameterDirection direction)
        {
            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = dbType;
            parameter.Direction = ParameterDirection.Input;

            command.Parameters.Add(parameter);

            return parameter;
        }

        /// <summary>
        /// Creates an output Parameter object with specified name, and value.
        /// </summary>
        /// <param name="name">The name of the Parameter.</param>
        /// <param name="dbType">The type of the Parameter.</param>
        /// <returns>A new output Parameter object.</returns>
        public static DbParameter CreateOutParameter(this DbCommand command, string name, DbType dbType)
        {
            DbParameter parameter = command.CreateParameter();

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
        public static DbParameter CreateReturnParameter(this DbCommand command, string name, DbType dbType)
        {
            DbParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.DbType = dbType;
            parameter.Direction = ParameterDirection.ReturnValue;

            return parameter;
        }

        public static DbParameter AddUdtParameter(this DbCommand command, string name, string udtTypeName, object value)
        {
            SqlParameter sqlParam = new SqlParameter(name, SqlDbType.Udt) { Value = value, UdtTypeName =  udtTypeName};
            command.Parameters.Add(sqlParam);
            return sqlParam;
        }

        #endregion
    }
}
