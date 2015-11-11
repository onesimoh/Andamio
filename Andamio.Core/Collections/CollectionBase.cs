using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.ObjectModel;

using Andamio.Threading;

namespace Andamio.Collections
{
    [Serializable]
    public class CollectionBase<EntityType> : IList<EntityType>, ICollection<EntityType>, IEnumerable<EntityType>, IList, INotifyCollectionChanged
    {
        #region Public Events
        public event EventHandler Loaded;
        protected void OnLoaded(EventArgs eventArgs)
        {
            if (Loaded != null)
            { Loaded(this, eventArgs); }
        }

        public event EventHandler<ItemEventArgs<IEnumerable<EntityType>>> ItemsInserted;
        protected virtual void OnItemsInserted(ItemEventArgs<IEnumerable<EntityType>> eventArgs)
        {
            if (ItemsInserted != null)
            { ItemsInserted(this, eventArgs); }
        }

        public event EventHandler<ItemEventArgs<IEnumerable<EntityType>>> ItemsRemoved;
        protected virtual void OnItemsRemoved(ItemEventArgs<IEnumerable<EntityType>> eventArgs)
        {
            if (ItemsRemoved != null)
            { ItemsRemoved(this, eventArgs); }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            if (CollectionChanged != null)
            { CollectionChanged.SafeEventInvoke(this, eventArgs); }
        }

        #endregion

        #region Constructors
        public CollectionBase() : base()
        {
            _collection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        public CollectionBase(IEnumerable<EntityType> items) : this()
        {
            AddRange(items);
        }

        #endregion

        #region Items
        public bool IsLoadedOrAssigned { get; set; }
        private readonly ObservableCollection<EntityType> _collection = new ObservableCollection<EntityType>();
        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = new List<EntityType>(e.NewItems.Cast<EntityType>());
                    OnItemsInserted(new ItemEventArgs<IEnumerable<EntityType>>(newItems));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedItems = new List<EntityType>(e.OldItems.Cast<EntityType>());
                    OnItemsRemoved(new ItemEventArgs<IEnumerable<EntityType>>(removedItems));
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                default:
                    break;
            }

            OnCollectionChanged(e);
        }

        public CollectionBase<EntityType> Load()
        {
            _collection.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnCollectionChanged);

            try
            {
                // Remove all items and deleted Items as a Force Load is taking place.
                // The Collection will be loaded anew.
                _collection.Clear();

                // Raise Loaded event to signal Load operation has completed.
                OnLoaded(EventArgs.Empty);

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                IsLoadedOrAssigned = true;

                return this;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                _collection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }
        }

        #endregion

        #region Add/Remove
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

        public bool Remove(EntityType item, IEqualityComparer<EntityType> comparer)
        {
            var items = _collection.Where(match => comparer.Equals(match, item));
            if (items.Any())
            {
                RemoveAll(items);
                return true;
            }
            return false;
        }

        #endregion

        #region IList<EntityType>
        public int IndexOf(EntityType item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, EntityType item)
        {
            _collection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            EntityType item = _collection[index];
            if (item != null)
            {
                _collection.RemoveAt(index);
            }
        }

        public EntityType this[int index]
        {
            get { return _collection[index]; }
            set { _collection[index] = value; }
        }

        #endregion

        #region ICollection<EntityType>
        public void Add(EntityType item)
        {
            _collection.Add(item);
        }

        public void Clear()
        {
            _collection.Clear();
        }

        public bool Contains(EntityType item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(EntityType [] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(EntityType item)
        {
            if (_collection.Remove(item))
            {
                return true;
            }
            return false;
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

        #region IEnumerable<EntityType>
        public IEnumerator<EntityType> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IList
        int IList.Add(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            EntityType entity = (EntityType) value;
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

            return Contains((EntityType) value);
        }

        int IList.IndexOf(object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            return IndexOf((EntityType) value);
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is EntityType))
            { throw new InvalidCastException(); }

            Insert(index, (EntityType) value);
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

            Remove((EntityType) value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return _collection[index]; }
            set 
            { 
                if (!(value is EntityType))
                { 
                    throw new InvalidCastException(); 
                }
                _collection[index] = (EntityType) value; 
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(_collection.ToArray(), 0, array, index, _collection.Count);
        }

        int ICollection.Count
        {
            get { return _collection.Count; }
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
    }
}
