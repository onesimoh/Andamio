using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.CodeDom;
using System.Reflection;
using System.Diagnostics;

using Andamio;
using Andamio.Threading;
using Andamio.Serialization;
using Andamio.Data.Serialization;

namespace Andamio.Data.Entities
{
    #region EntityCollectionEventArgs
    public sealed class EntityCollectionEventArgs<EntityType> : EventArgs
        where EntityType : EntityBase
    {
        internal EntityCollectionEventArgs()
        {
        }
        internal EntityCollectionEventArgs(EntityType item)
        {
            Item = item;
        }

        public EntityType Item { get; internal set; }
    }

    #endregion



    #region Populate Entity Delegate
    public delegate IEnumerable<EntityType> PopulateMethodDelegate<EntityType>() where EntityType : EntityBase;

    #endregion


    [Serializable]
    [CollectionDataContract]
    [DebuggerDisplay("Count = {Count}")]
    public class EntityCollectionBase<EntityType> : IList<EntityType>, ICollection<EntityType>, IEnumerable<EntityType>, IList, INotifyCollectionChanged
        where EntityType : EntityBase
    {
        #region Public Events
        public event EventHandler Loaded;
        protected void OnLoaded(EventArgs eventArgs)
        {
            if (Loaded != null)
            { Loaded(this, eventArgs); }
        }

        public event EventHandler DataSync;
        protected void OnDataSync(EventArgs eventArgs)
        {
            if (DataSync != null)
            { DataSync.SafeEventInvoke(this, eventArgs); }
        }

        public event EventHandler<EntityCollectionEventArgs<EntityType>> ItemInserted;
        protected void OnItemInserted(EntityCollectionEventArgs<EntityType> eventArgs)
        {
            if (IsSerializing) return;                 

            if (ItemInserted != null)
            { ItemInserted(this, eventArgs); }
        }

        public event EventHandler<EntityCollectionEventArgs<EntityType>> ItemRemoved;
        protected void OnItemRemoved(EntityCollectionEventArgs<EntityType> eventArgs)
        {
            if (ItemRemoved != null)
            { ItemRemoved(this, eventArgs); }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            if (CollectionChanged != null)
            { CollectionChanged.SafeEventInvoke(this, eventArgs); }
        }

        #endregion

        #region Constructors
        public EntityCollectionBase()
            : base()
        {
            _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnListCollectionChanged);
        }

        public EntityCollectionBase(IEnumerable<EntityType> items)
            : this()
        {
            AddRange(items);
        }

        public EntityCollectionBase(EntityCollectionBase<EntityType> entityCollection)
            : this()
        {
            AddRange(entityCollection);
            DeletedItems = entityCollection.DeletedItems;
            IsLoadedOrAssigned = entityCollection.IsLoadedOrAssigned;
        }

        #endregion

        #region Items
        protected readonly ObservableCollection<EntityType> _items = new ObservableCollection<EntityType>();
        protected readonly List<EntityType> _deletedList = new List<EntityType>();
        public IList<EntityType> DeletedItems
        {
            get
            {
                return _deletedList;
            }
            internal set
            {
                IEnumerable<EntityType> deleted = value;
                if (deleted != null && deleted.Any())
                {
                    _deletedList.Clear();
                    _deletedList.AddRange(deleted);
                }
            }
        }

        public bool HasItems
        {
            get { return _items.Any(); }
        }
        public bool HasDeletedItems
        {
            get { return _deletedList.Any(); }
        }

        #endregion

        #region Modified
        public bool IsLoadedOrAssigned { get; set; }
        public bool IsContentModified
        {
            get { return _items.Any(match => match.IsModified); }
        }

        #endregion

        #region Sync Data
        public PopulateMethodDelegate<EntityType> PopulateMethodHandler { get; set; }
        
        public EntityCollectionBase<EntityType> SyncData(IEqualityComparer<EntityType> comparer)
        {
            return SyncData(comparer, PopulateMethodHandler);
        }

        public EntityCollectionBase<EntityType> SyncData(IEqualityComparer<EntityType> comparer, PopulateMethodDelegate<EntityType> dataSyncMethod)
        {
            if (dataSyncMethod == null)
            { throw new InvalidOperationException("Entity Collection cannot be synchronized as a method handler to populate the data was not provided."); }

            _items.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnListCollectionChanged);

            try
            {
                // PopulateMethodHandler should execute a method that populates the collection
                // (for example from a database store).
                var synchedDataItems = dataSyncMethod();

                // Process deleted items, get items from the database that are also contained in the deleted list
                // all other items in deleted list are ignored as thet were already deleted, or never committed.
                if (_deletedList.Any())
                {
                    _deletedList.Clear();

                    var deletedItems = synchedDataItems.Where(match => _deletedList.Contains(match, comparer));
                    if (deletedItems.Any())
                    {
                        _deletedList.AddRange(deletedItems);
                    }
                }

                if (_items.Any())
                {
                    // Sync data in collection: the idea is to preserve any changes currently made, ex. those
                    // items that have been modified and also sync down items in the database
                    foreach (EntityType synchedDataItem in synchedDataItems)
                    {
                        if (_deletedList.Contains(synchedDataItem, comparer))
                        { continue; }

                        EntityType dataItem = _items.FirstOrDefault(match => comparer.Equals(match, synchedDataItem));
                        if (dataItem != null)
                        {
                            int index = _items.IndexOf(dataItem);
                            _items[index] = synchedDataItem;
                            NotifyCollectionItemChanged(synchedDataItem, dataItem, index);
                        }
                        else
                        {
                            _items.Add(synchedDataItem);
                            NotifyCollectionItemAdded(synchedDataItem);
                        }
                    }
                }
                else
                {
                    foreach (var synchedDataItem in synchedDataItems)
                    {
                        _items.Add(synchedDataItem);
                    }
                    NotifyCollectionRefreshed();
                }

                // Raise DataSync event to signal Sync operation has completed.
                OnDataSync(EventArgs.Empty);

                IsLoadedOrAssigned = true;

                return this;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnListCollectionChanged);
            }
        }

        public EntityCollectionBase<EntityType> Load()
        {
            _items.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnListCollectionChanged);

            try
            {
                // Remove all items and deleted Items as a Force Load is taking place.
                // The Collection will be loaded anew from the database,
                _items.Clear();
                _deletedList.Clear();

                // PopulateMethodHandler should execute a method that populates the collection
                // (for example from a database store).
                if (PopulateMethodHandler != null)
                {
                    var dataItems = PopulateMethodHandler();
                    if (dataItems != null)
                    {
                        dataItems.ForEach(dataItem => _items.Add(dataItem));
                    }
                }
                else
                {
                    // Raise Loaded event to signal Load operation has completed.
                    OnLoaded(EventArgs.Empty);
                }
                
                NotifyCollectionRefreshed();
                
                IsLoadedOrAssigned = true;

                return this;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnListCollectionChanged);
            }
        }

        public void RefreshData(IEqualityComparer<EntityType> comparer)
        {
            if (IsLoadedOrAssigned)
            {
                List<EntityType> items = new List<EntityType>(_items);
                SyncData(comparer);

                this.RemoveAll(match => !items.Contains(match, comparer));
                AddRange(this.Where(match => !items.Contains(match, comparer)));
            }
            else
            {
                Load();
                DeleteAll();
            }
        }

        void OnListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsLoadedOrAssigned = true;
            OnCollectionChanged(e);
        }

        #endregion

        #region Merge
        public void Merge(EntityType item, IEqualityComparer<EntityType> comparer)
        {
            Merge(new EntityType[] { item }, comparer);
        }

        public void Merge(IEnumerable<EntityType> mergingItems, IEqualityComparer<EntityType> comparer)
        {
            _items.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnListCollectionChanged);

            try
            {
                foreach (EntityType mergingItem in mergingItems)
                {
                    if (_deletedList.Contains(mergingItem, comparer))
                    { continue; }

                    EntityType dataItem = _items.FirstOrDefault(match => comparer.Equals(match, mergingItem));
                    if (dataItem != null)
                    {
                        int index = _items.IndexOf(dataItem);
                        _items[index] = mergingItem;
                        NotifyCollectionItemChanged(mergingItem, dataItem, index);
                    }
                    else
                    {
                        _items.Add(mergingItem);
                        NotifyCollectionItemAdded(mergingItem);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _items.CollectionChanged += new NotifyCollectionChangedEventHandler(OnListCollectionChanged);
            }
        }

        #endregion

        #region Data Modified Notify
        internal void NotifyCollectionRefreshed()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        internal void NotifyCollectionItemAdded(EntityType newItem)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem));
        }

        internal void NotifyCollectionItemChanged(EntityType newItem, EntityType oldItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem));
        }

        #endregion

        #region Methods
        public void AddRange(IEnumerable<EntityType> items)
        {
            if (items != null && items.Any())
            {
                foreach (EntityType item in items)
                {
                    Add(item);
                }
            }
        }

        public void Reset()
        {
            IsLoadedOrAssigned = HasItems;
            _deletedList.Clear();
        }

        public void AssignCollection(EntityCollectionBase<EntityType> collection)
        {
            Clear();
            if (collection != null)
            {
                AddRange(collection);

                IsLoadedOrAssigned = collection.IsLoadedOrAssigned;
                DeletedItems = collection.DeletedItems;
            }
        }

        public bool Remove(EntityType item, IEqualityComparer<EntityType> comparer)
        {
            var items = _items.Where(match => comparer.Equals(match, item));
            if (items.Any())
            {
                RemoveAll(items);
                return true;
            }
            return false;
        }

        public void Move(EntityType item, int index)
        {
            if (item == null) throw new ArgumentNullException("item");
            if (index < 0) throw new ArgumentOutOfRangeException("index");

            int oldIndex = IndexOf(item);
            if (oldIndex >= 0)
            { RemoveAt(oldIndex); }

            Insert(index, item);
        }

        #endregion

        #region IList<EntityType> Members
        public int IndexOf(EntityType item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, EntityType item)
        {
            _items.Insert(index, item);
            OnItemInserted(new EntityCollectionEventArgs<EntityType>(item));
        }

        public void RemoveAt(int index)
        {
            EntityType item = _items[index];
            if (item != null)
            {
                _deletedList.Add(item);
                _items.RemoveAt(index);

                OnItemRemoved(new EntityCollectionEventArgs<EntityType>(item));
            }
        }

        public EntityType this[int index]
        {
            get { return _items[index]; }
            set { _items[index] = value; }
        }

        #endregion

        #region ICollection<EntityType> Members
        public void Add(EntityType item)
        {
            _items.Add(item);
            OnItemInserted(new EntityCollectionEventArgs<EntityType>(item));
        }

        public void DeleteAll()
        {
            List<EntityType> allItems = new List<EntityType>(_items);
            _deletedList.Clear();
            RemoveAll(allItems);
        }

        public void Clear()
        {
            _items.Clear();
            _deletedList.Clear();
        }

        public bool Contains(EntityType item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(EntityType[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EntityType item)
        {
            if (_items.Remove(item))
            {
                if (!_deletedList.Contains(item))
                { _deletedList.Add(item); }
                OnItemRemoved(new EntityCollectionEventArgs<EntityType>(item));

                return true;
            }
            return false;
        }

        public void Remove(Func<EntityType, bool> predicate)
        {
            var items = _items.Where(predicate).ToArray();
            items.ForEach(item => Remove(item));
        }

        public void RemoveAll(IEnumerable<EntityType> items)
        {
            if (items != null && items.Any())
            {
                foreach (EntityType item in items)
                {
                    Remove(item);
                }
            }
        }

        #endregion

        #region IEnumerable<EntityType> Members
        public IEnumerator<EntityType> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IList Members
        int IList.Add(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            EntityType entity = (EntityType)value;

            Add(entity);

            return IndexOf(entity);
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            return Contains((EntityType)value);
        }

        int IList.IndexOf(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            return IndexOf((EntityType)value);
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            Insert(index, (EntityType)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            Remove((EntityType)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (!(value is EntityType))
                { throw new InvalidCastException(); }

                _items[index] = (EntityType)value;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(_items.ToArray(), 0, array, index, _items.Count);
        }

        int ICollection.Count
        {
            get { return _items.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        private readonly object _syncObj = new object();
        object ICollection.SyncRoot
        {
            get { return _syncObj; }
        }

        #endregion

        #region Serialization
        /// <summary>
        /// True if entity is currently being Serialized; false otherwise.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public bool IsSerializing { get; set; }

        [OnSerializing]
        void OnSerializing(StreamingContext context)
        {
            IsSerializing = true;
            OnSerializing();
        }

        [OnSerialized]
        void OnSerialized(StreamingContext context)
        {
            IsSerializing = false;
        }

        [OnDeserializing]
        void OnDeserializing(StreamingContext context)
        {
            IsSerializing = true;
            OnDeserializing();
        }

        [OnDeserialized]
        void OnDeserialized(StreamingContext context)
        {
            IsSerializing = false;
        }

        /// <summary>
        /// Executes before entity deserialization.
        /// </summary>
        public virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Executes before entity serialization.
        /// </summary>
        public virtual void OnSerializing()
        {
        }

        #endregion 
    }
}
