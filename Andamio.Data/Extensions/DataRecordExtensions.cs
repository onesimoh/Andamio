using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Andamio.Data
{
    public static class DataRecordExtensions
    {
        #region Guids
        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        public static Guid GetGuid(this IDataRecord dataRecord, string fieldName)
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
        public static Guid? GetNullableGuid(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetGuid(ordinal); }
            return null;
        }

        #endregion

        #region Integer
        /// <summary>
        /// Returns the Int32 value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Int32 value of the specified field.</returns>        
        public static int GetInt32(this IDataRecord dataRecord, string fieldName)
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
        public static int? GetNullableInt32(this IDataRecord dataRecord, string fieldName)
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
        public static int GetInt16(this IDataRecord dataRecord, string fieldName)
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
        public static int? GetNullableInt16(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetInt16(ordinal); }
            return null;
        }

        #endregion

        #region Decimmal
        /// <summary>
        /// Returns the Decimal value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Decimal value of the specified field.</returns>                
        public static decimal GetDecimal(this IDataRecord dataRecord, string fieldName)
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
        public static decimal? GetNullableDecimal(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetDecimal(ordinal); }
            return null;
        }


        #endregion

        #region Boolean
        /// <summary>
        /// Returns the Boolean value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The Boolean value of the specified field.</returns>                        
        public static bool GetBoolean(this IDataRecord dataRecord, string fieldName)
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
        public static bool? GetNullableBoolean(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetBoolean(ordinal); }
            return null;
        }

        #endregion

        #region DateTime
        /// <summary>
        /// Returns the DateTime value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The DateTime value of the specified field.</returns>                
        public static DateTime GetDateTime(this IDataRecord dataRecord, string fieldName)
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
        public static DateTime? GetNullableDateTime(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetDateTime(ordinal); }
            return null;
        }

        #endregion

        #region String
        /// <summary>
        /// Returns the String value of the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The String value of the specified field.</returns>        
        public static string GetString(this IDataRecord dataRecord, string fieldName)
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
        public static char GetChar(this IDataRecord dataRecord, string fieldName)
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
        public static char? GetNullableChar(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            if (!dataRecord.IsDBNull(ordinal))
            { return dataRecord.GetChar(ordinal); }
            return null;
        }

        #endregion

        #region DbNull
        /// <summary>
        /// Return whether field is null.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public static bool IsDbNull(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            return dataRecord.IsDBNull(ordinal);
        }

        #endregion

        #region Binary
        /// <summary>
        /// Reads a stream of bytes from the specified field.
        /// </summary>
        /// <param name="dataRecord">The data record containing column information.</param>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>Returns the field value as an Array of Bytes.</returns>
        public static byte[] GetBinary(this IDataRecord dataRecord, string fieldName)
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);

            byte[] bytes = null;

            if (!dataRecord.IsDBNull(ordinal))
            {
                //get the number of bytes available
                long totalBytes = dataRecord.GetBytes(ordinal, 0, null, 0, 0);

                //read the bytes
                bytes = new byte[totalBytes];
                dataRecord.GetBytes(ordinal, 0, bytes, 0, bytes.Length);
            }

            return bytes;
        }

        #endregion

        #region Enums
        public static E GetEnum<E>(this IDataRecord dataRecord, string fieldName)
            where E : struct
        {
            int ordinal = dataRecord.GetOrdinal(fieldName);
            int intValue = dataRecord.GetInt32(ordinal);
            return (E) Enum.Parse(typeof(E), intValue.ToString());
        }

        #endregion
    }
}
