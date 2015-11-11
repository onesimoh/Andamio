﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Andamio
{
    public sealed class SearchPageSettings
    {
        public SearchPageSettings()
        {
            PageIndex = 1;
            PageSize = Int16.MaxValue;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; private set; }
        public int IndexStart { get; private set; }
        public int IndexEnd { get; private set; }
        public int PageCount { get; private set; }
        public int Skip
        {
            get
            {
                return ((PageIndex - 1) * PageSize);
            }
        }

        public void Calculate<T>(IQueryable<T> query)
        {
            Calculate(query.Count());
        }
        public void Calculate<T>(IEnumerable<T> query)
        {
            Calculate(query.Count());
        }

        public void Calculate(DataTable dataTable)
        {
            Calculate(dataTable.Rows.Count);
        }

        private void Calculate(int totalCount)
        {
            TotalCount = totalCount;
            IndexStart = (PageIndex - 1) * PageSize + 1;
            IndexEnd = (PageIndex) * PageSize;
            if (IndexEnd > totalCount)
            { IndexEnd = totalCount; }
            PageCount = (IndexEnd - IndexStart) + 1;
        }
    }
}
