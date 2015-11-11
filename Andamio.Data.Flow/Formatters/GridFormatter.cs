using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Data;
using Andamio.Data.Flow.Media;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Formatters
{
    public class GridFormatter : Formatter
    {
        #region Constructors
        private GridFormatter()
        {
        }

        internal GridFormatter(MediaConfiguration mediaConfiguration)
            : base(mediaConfiguration)
        {
            Columns = new GridColumnCollection();
            Columns.ItemsInserted += OnColumnsInserted; 
        }

        #endregion

        #region Columns
        private readonly List<string> Excluded = new List<string>();
        public GridColumnCollection Columns { get; private set; }
        public GridColumn Column(string columnName)
        {
            GridColumn columnConfig = new GridColumn(columnName);
            Columns.Add(columnConfig);
            return columnConfig;
        }

        void OnColumnsInserted(object sender, ItemEventArgs<IEnumerable<GridColumn>> e)
        {
            var columns = e.Item;
            if (columns != null && columns.Any())
            { columns.ForEach(c => c.Parent = this); }
        }

        public GridFormatter Exclude(string columnName)
        {
            if (columnName.IsNullOrBlank()) throw new ArgumentNullException("columnName");
            Excluded.Add(columnName);
            return this;
        }

        #endregion

        #region Mappings
        public virtual GridFormatter Mappings(FileInfo file)
        {
            ReadMappings(file);
            return this;
        }

        public virtual GridFormatter Mappings(XmlReader xmlReader)
        {
            ReadMappings(xmlReader);
            return this;
        }

        public virtual GridFormatter Mappings(string xml)
        {
            ReadMappings(xml);
            return this;
        }

        public virtual GridFormatter Mappings(XDocument document)
        {
            ReadMappings(document);
            return this;
        }

        public virtual GridFormatter Mappings(XElement element)
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
                GridColumn column = Columns.New();
                column.FromElement(columnNode);
            });
        }

        #endregion

        #region Import
        public override DataGrid Import()
        {
            DataGrid dataGrid = new DataGrid();
            Columns.Where(match => !Excluded.Contains(match.ColumnName)).ForEach(c => dataGrid.Columns.Add(c));
            dataGrid.Read(MediaConfiguration.Reader);
            return dataGrid;
        }

        #endregion
    }
}
