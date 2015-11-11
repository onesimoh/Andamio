using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Andamio
{
    public static class DataExtensions
    {
        public static void AddRange(this DataColumnCollection dataColumnCollection, IEnumerable<DataColumn> dataColumns)
        {
            if (dataColumns != null && dataColumns.Any())
            {
                dataColumnCollection.AddRange(dataColumns.ToArray());
            }
        }

        public static void AddRange(this DataColumnCollection dataColumnCollection, DataColumnCollection dataColumns)
        {
            if (dataColumns != null && dataColumns.Any())
            {
                dataColumnCollection.AddRange(dataColumns.ToArray<DataColumn>());
            }
        }

        public static bool Any(this InternalDataCollectionBase dataCollectionBase)
        {
            return (dataCollectionBase != null && dataCollectionBase.Count > 0);
        }

        public static T[] ToArray<T>(this InternalDataCollectionBase dataCollectionBase)
        {
            T[] items = {};
            if (dataCollectionBase != null && dataCollectionBase.Any())
            {
                items = new T[dataCollectionBase.Count];
                dataCollectionBase.CopyTo(items, 0);
            }

            return items;
        }

        public static IEnumerable<DataColumn> Skip(this DataColumnCollection dataColumnCollection, int count)
        {
            return dataColumnCollection.ToArray<DataColumn>().Skip(count);
        }

        public static IEnumerable<DataRow> Skip(this DataRowCollection dataRowCollection, int count)
        {
            return dataRowCollection.ToArray<DataRow>().Skip(count);
        }

        public static IEnumerable<DataRow> Take(this DataRowCollection dataRowCollection, int count)
        {
            return dataRowCollection.ToArray<DataRow>().Take(count);
        }

        public static void ImportRows(this DataTable dataTable, IEnumerable<DataRow> dataRows)
        {
            if (dataRows != null && dataRows.Any())
            {
                dataRows.ForEach(dataRow => dataTable.ImportRow(dataRow));
            }
        }
    }
}
