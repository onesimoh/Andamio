using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Andamio;
using Andamio.Collections;
using Andamio.Diagnostics;
using Andamio.Data.Flow.Media;
using Andamio.Data.Flow.Media.Readers;

namespace Andamio.Data.Flow.Formatters
{
    public class EntityFormatter<EntityT> : Formatter
    {
        #region Constructors
        private EntityFormatter()
        {
        }

        internal EntityFormatter(MediaConfiguration mediaConfiguration)
            : base(mediaConfiguration)
        {
            Columns = new EntityColumnCollection<EntityT>();
            Columns.ItemsInserted += OnColumnsInserted;        
        }

        #endregion

        #region Columns
        private readonly List<string> Excluded = new List<string>();
        public EntityColumnCollection<EntityT> Columns { get; private set; }
        public EntityColumn<EntityT> Column(string columnName)
        {
            var columnConfig = new EntityColumn<EntityT>(columnName);
            Columns.Add(columnConfig);
            return columnConfig;
        }

        void OnColumnsInserted(object sender, ItemEventArgs<IEnumerable<EntityColumn<EntityT>>> e)
        {
            var columns = e.Item;
            if (columns != null && columns.Any())
            { columns.ForEach(c =>  c.Parent = this); }
        }

        public EntityFormatter<EntityT> Exclude(string columnName)
        {
            if (columnName.IsNullOrBlank()) throw new ArgumentNullException("columnName");
            Excluded.Add(columnName);
            return this;
        }

        #endregion

        #region Collection
        public virtual CollectionBase<EntityT> ToCollection() 
        {
            Log.Info(String.Format("Populating Entity Collection of Type '{0}' with imported data...", typeof(EntityT)));

            DataGrid dataGrid = Import();

            DataGridColumns excludedColumns = new DataGridColumns();
            System.Reflection.PropertyInfo[] entityProperties = typeof(EntityT).GetProperties();
            dataGrid.Columns.ForEach(delegate(DataGridColumn column)
            {
                if (!entityProperties.Any(property => property.Name.Equals(column.BindingName)))
                {
                    excludedColumns.Add(column);
                    Log.Warning(String.Format("A Property mapping to Column '{0}' does not exist for Type '{1}'. Values corresponding to this property will not be set."
                        , column.BindingName
                        , typeof(EntityT)));
                }
            });
            
            CollectionBase<EntityT> collection = new CollectionBase<EntityT>();
            dataGrid.Rows.ForEach(delegate(DataGridRow row) 
            {
                EntityT entity = Activator.CreateInstance<EntityT>();
                collection.Add(entity);
                row.Cells.Where(c => !excludedColumns.Contains(c.Column)).ForEach(delegate(DataGridCell cell)
                {                  
                    try
                    {
                        entity.Property(cell.Column.BindingName).Value = cell.OriginalValue;
                    }
                    catch (Exception exception)
                    {
                        Log.Error(String.Format("An error occurred while setting Property '{0}' corresponding to Column '{1}'."
                            , cell.Column.BindingName
                            , cell.Column.ColumnName)
                            , exception);
                    }
                });                                                                            
            });

            Log.Info(String.Format("Done populating Entity Collection. Total rows imported: {0}.", collection.Count));

            return collection;
        }

        #endregion

        #region Mappings
        public virtual EntityFormatter<EntityT> Mappings(FileInfo file)
        {
            ReadMappings(file);
            return this;
        }

        public virtual EntityFormatter<EntityT> Mappings(XmlReader xmlReader)
        {
            ReadMappings(xmlReader);
            return this;
        }

        public virtual EntityFormatter<EntityT> Mappings(string xml)
        {
            ReadMappings(xml);
            return this;
        }

        public virtual EntityFormatter<EntityT> Mappings(XDocument document)
        {
            ReadMappings(document);
            return this;
        }

        public virtual EntityFormatter<EntityT> Mappings(XElement element)
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
                EntityColumn<EntityT> column = Columns.New();
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
