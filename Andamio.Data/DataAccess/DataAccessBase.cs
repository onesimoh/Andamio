using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public abstract class DataAccessBase : DaoBase
    {
        #region Execute methods
        /// <summary>
        /// Executes the command using the CommandBehavior.Default behavior. 
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>An DbDataReader object.</returns>
        protected virtual DbDataReader ExecuteReader(DbCommand command)
        {
            DbDataReader reader = null;

            try
            {
                reader = ExecuteReader(command, CommandBehavior.Default);
            }
            catch (Exception e)
            {
                ThrowDataAccessException(e);
            }

            return reader;
        }

        /// <summary>
        /// Executes the command using the specified CommandBehavior value.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <param name="behavior">One of the CommandBehavior values.</param>
        /// <returns>An DbDataReader object.</returns>
        protected virtual DbDataReader ExecuteReader(DbCommand command, CommandBehavior behavior)
        {
            DbDataReader reader = null;

            try
            {
                if (command.Connection.State == ConnectionState.Closed)
                { command.Connection.Open(); }

                reader = command.ExecuteReader(behavior);
            }
            catch (Exception e)
            {
                ThrowDataAccessException(e);
            }

            return reader;
        }

        /// <summary>
        /// Executes the command against a connection object.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The number of rows affected.</returns>
        protected virtual int ExecuteNonQuery(DbCommand command)
        {
            int affectedRows = 0;

            try
            {
                if (command.Connection.State == ConnectionState.Closed)
                { command.Connection.Open(); }

                affectedRows = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                ThrowDataAccessException(e);
            }

            return affectedRows;
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row in the result set returned by the query. 
        /// All other columns and rows are ignored.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>The first column of the first row in the result set.</returns>
        protected virtual object ExecuteScalar(DbCommand command)
        {
            object result = null;

            try
            {
                if (command.Connection.State == ConnectionState.Closed)
                { command.Connection.Open(); }

                result = command.ExecuteScalar();
            }
            catch (Exception e)
            {
                ThrowDataAccessException(e);
            }

            return result;
        }
        #endregion

        #region Field Methods
        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        protected virtual Guid GetGuid(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetGuid(ordinal);
        }

        /// <summary>
        /// Returns the Nullable GUID value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable GUID value of the specified field.</returns>
        protected virtual Guid? GetNullableGuid(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetGuid(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the Int32 value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Int32 value of the specified field.</returns>        
        protected virtual int GetInt32(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetInt32(ordinal);
        }

        /// <summary>
        /// Returns the Nullable Int32 value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable Int32 value of the specified field.</returns>        
        protected virtual int? GetNullableInt32(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetInt32(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the Int16 value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Int16 value of the specified field.</returns>                
        protected virtual int GetInt16(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetInt16(ordinal);
        }

        /// <summary>
        /// Returns the Nullable Int16 value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable Int16 value of the specified field.</returns>        
        protected virtual int? GetNullableInt16(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetInt16(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the Decimal value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Decimal value of the specified field.</returns>                
        protected virtual decimal GetDecimal(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetDecimal(ordinal);
        }

        /// <summary>
        /// Returns the Nullable Decimal value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable Decimal value of the specified field.</returns>         
        protected virtual decimal? GetNullableDecimal(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetDecimal(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the Boolean value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Boolean value of the specified field.</returns>                        
        protected virtual bool GetBoolean(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetBoolean(ordinal);
        }

        /// <summary>
        /// Returns the Nullable Boolean value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable Boolean value of the specified field.</returns>        
        protected virtual bool? GetNullableBoolean(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetBoolean(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the DateTime value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The DateTime value of the specified field.</returns>                
        protected virtual DateTime GetDateTime(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetDateTime(ordinal);
        }

        /// <summary>
        /// Returns the Nullable DateTime value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable DateTime value of the specified field.</returns>        
        protected virtual DateTime? GetNullableDateTime(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetDateTime(ordinal); }
            return null;
        }

        /// <summary>
        /// Returns the String value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The String value of the specified field.</returns>        
        protected virtual string GetString(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            string stringValue = !dataRecord.IsDBNull(ordinal) ? dataRecord.GetString(ordinal) : null;
            return stringValue;
        }

        /// <summary>
        /// Returns the Char value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Char value of the specified field.</returns>        
        protected virtual char GetChar(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.GetChar(ordinal);
        }

        /// <summary>
        /// Returns the Nullable Char value of the specified field.
        /// </summary>
        /// <remarks>
        /// If the specified field is DBNull null is retuned instead. This method may be use
        /// to map to a table field that allows nulls.
        /// </remarks>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Nullable Char value of the specified field.</returns>        
        protected virtual char? GetNullableChar(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetChar(ordinal); }
            return null;
        }

        protected bool IsDbNull(IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.IsDBNull(ordinal);
        }

        #endregion

        #region Populate Methods
        /// <summary>
        /// Represents a method that populates an entity class with the field values of the data records.
        /// </summary>
        /// <typeparam name="E">Type of the entity to populate.</typeparam>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <returns>The populated entity.</returns>
        protected delegate E PopulateEntityDelegate<E>(IDataRecord dataRecord) where E : EntityBase;

        /// <summary>
        /// Populates an entity class.
        /// </summary>
        /// <typeparam name="E">Type of the entity to populate.</typeparam>
        /// <param name="dataReader">The reader containing the data to populate the entity with.</param>
        /// <param name="populateEntityDel">The method that populates the entity.</param>
        /// <returns>The populated entity.</returns>
        protected virtual E CreateEntity<E>(DbDataReader dataReader, PopulateEntityDelegate<E> populateEntityDel) where E : EntityBase
        {
            E entity = default(E);
            if (dataReader.Read())
            {
                entity = populateEntityDel((IDataRecord)dataReader);
                if (entity != null)
                {
                    entity.IsModified = false;
                    entity.IsReadFromDatabase = true;
                }
            }
            return entity;
        }

        /// <summary>
        /// Populates a list of the specified entity.
        /// </summary>
        /// <remarks>
        /// This method may be use to populate an entity list from the results obtained from executing a SQL SELECT Statement or
        /// stored procedure.
        /// </remarks>
        /// <typeparam name="E">Type of the entity to populate.</typeparam>
        /// <param name="dataReader">The reader containing the data to populate the entity list with.</param>
        /// <param name="populateEntityDel">The method that populates the entity.</param>
        /// <returns>The populated entity list.</returns>
        protected virtual List<E> CreateEntityList<E>(DbDataReader dataReader, PopulateEntityDelegate<E> populateEntityDel) where E : EntityBase
        {
            List<E> entityList = new List<E>();
            while (dataReader.Read())
            {
                E entity = populateEntityDel((IDataRecord)dataReader);

                if (entity != null)
                {
                    entity.IsModified = false;
                    entity.IsReadFromDatabase = true;

                    entityList.Add(entity);
                }
            }
            return entityList;
        }

        #endregion
    }
}
