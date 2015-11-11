using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Reflection;

using Capital.Threading;

namespace Capital.Data.Caching
{
    /// <summary>
    /// Implements Base class for cache related functionality. It provides a programmatic way to store arbitrary data in memory
    /// using key/value pairs and associate time restrictions to these items.<br />
    /// <list type="">
    ///     <listheader>
    ///         <term>Expiration Type</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Sliding expiration</term>
    ///         <description>
    ///             Specifies how long after an item was last accessed that it expires. For example, you can set an item to expire 
    ///             20 minutes after it was last accessed in the cache.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>Absolute expiration</term>
    ///         <description>
    ///             Specifies that an item expires at a set time, regardless of how often it is accessed.
    ///         </description>
    ///     </item>   
    /// </list>
    /// </summary>
    public abstract partial class CacheBase : Disposable
    {
        #region Private Fields
        /// <summary>
        /// Represents one second in miliseconds.
        /// </summary>
        protected const int ONE_SECOND = 1000;

        private object _syncLock = new object();

        private Dictionary<string, CachedItem> _cache;

        private System.Threading.Timer _timer;

        private readonly int _checkExpirePeriod;

        private ReaderWriterLock _rwSyncLock = new ReaderWriterLock();

        #endregion

        #region Constructors, Destructor
        /// <summary>
        /// Default class constructor. Creates a new instance and sets a default value of 15 mins for Absolute and Sliding time intervals. 
        /// A default value of 5 secs. intervals is set to test periodically for expired items. 
        /// </summary>
        public CacheBase()
            : this(new TimeSpan(0, 15, 0), new TimeSpan(0, 15, 0))
        {
        }

        /// <summary>
        /// Creates a new instance and sets the default values for Absolute and Sliding time intervals.
        /// A default value of 5 secs. intervals is set to test periodically for expired items. 
        /// </summary>
        /// <param name="defaultAbsoluteInterval">Specifies a Default Absolute Interval value, which is the time an object lasts in the cache.</param>
        /// <param name="defaultSlidingInterval">
        /// Specifies a Default Sliding Interval, which is the time from when an object is last accessed until it is removed.
        /// For example, if this value is equivalent of 20 mins. the object is expired and removed from the cache it has not been accessed 
        /// for the duration of this time.
        /// </param>        
        public CacheBase(TimeSpan defaultAbsoluteInterval, TimeSpan defaultSlidingInterval)
            : this(defaultAbsoluteInterval, defaultSlidingInterval, 5)
        {
        }

        /// <summary>
        /// Creates a new instance and sets the default values for Absolute, Sliding, and expiration time intervals.
        /// </summary>
        /// <param name="defaultAbsoluteInterval">Specifies a Default Absolute Interval value, which is the time an object lasts in the cache.</param>
        /// <param name="defaultSlidingInterval">
        /// Specifies a Default Sliding Interval, which is the time from when an object is last accessed until it is removed.
        /// For example, if this value is equivalent of 20 mins. the object is expired and removed from the cache it has not been accessed 
        /// for the duration of this time.
        /// </param>
        /// <param name="checkExpirePeriod">Time interval in seconds at which expired items are inspected and removed from the cache.</param>
        public CacheBase(TimeSpan defaultAbsoluteInterval, TimeSpan defaultSlidingInterval, int checkExpirePeriod)
        {
            this.DefaultAbsoluteInterval = defaultAbsoluteInterval;
            this.DefaultSlidingInterval = defaultSlidingInterval;

            _checkExpirePeriod = ONE_SECOND * checkExpirePeriod;
            InitializeCache();
        }

        /// <summary>
        /// Class destructor for finalization code, this method will run only if Dispose method is called by GC. It gives the base 
        /// class and derived classes the opportunity to run any clean up code.
        /// </summary>
        ~CacheBase()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the cache Default Absolute Interval value, which is the time an object lasts in the cache.
        /// </summary>
        public virtual TimeSpan DefaultAbsoluteInterval { get; private set; }

        /// <summary>
        /// Gets the cache Default Sliding Interval, which is the time from when an object is last accessed until it is removed.
        /// For example, if this value is equivalent of 20 mins. the object is expired and removed from the cache it has not been accessed 
        /// for the duration of this time.
        /// </summary>
        public virtual TimeSpan DefaultSlidingInterval { get; private set; }

        /// <summary>
        /// Gets the number of items contained in cache.
        /// </summary>
        public int Count
        {
            get
            {
                using (ReadLock.Acquire(_rwSyncLock))
                {
                    return _cache.Count;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets or sets the cache item at the specified key. 
        /// </summary>
        /// <remarks>You can use this property to retrieve the value of a specified cache item, or to add an item and a key for it to the cache.</remarks>
        /// <param name="key">A String object that represents the key for the cache item.</param>
        /// <returns>The specified cache item associated with suppiled key.</returns>
        public virtual object this[string key]
        {
            get
            {
                using (ReadLock.Acquire(_rwSyncLock))
                {
                    CachedItem item = _cache[key];
                    if (item.CacheType.IsSliding())
                    { 
                        item.IncrementTime(); 
                    }
                    return item.Value;
                }
            }
            set
            {
                using (WriteLock.Acquire(_rwSyncLock))
                {
                    CachedItem item = _cache[key];
                    item.Value = value;
                    if (item.CacheType.IsSliding())
                    { 
                        item.IncrementTime(); 
                    }
                }
            }
        }

        /// <summary>
        /// Adds the specified item to the Cache without any expiration restrictions.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="value">The item to be added to the cache.</param>
        public virtual void Add(string key, object value)
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                _cache.Add(key, new CachedItem(value));
            }
        }

        /// <summary>
        /// Adds the specified item to the Cache and assigns the Default Absolute Expiration policy to it.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="value">The item to be added to the cache.</param>       
        public virtual void AddAbsoluteExpiration(string key, object value)
        {
            AddAbsoluteExpiration(key, value, this.DefaultAbsoluteInterval);
        }

        /// <summary>
        /// Adds the specified item to the Cache and assigns the supplied Absolute Expiration policy to it.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="value">The item to be added to the cache.</param>
        /// <param name="interval">Default Absolute Interval value, which is the time an object lasts in the cache.</param>
        public virtual void AddAbsoluteExpiration(string key, object value, TimeSpan interval)
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                _cache.Add(key, new CachedItem(value, CacheType.Absolute, interval));
            }
        }

        /// <summary>
        /// Adds the specified item to the Cache and assigns the Default Sliding Expiration policy to it.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="value">The item to be added to the cache.</param>              
        public virtual void AddSlidingExpiration(string key, object value)
        {
            AddSlidingExpiration(key, value, this.DefaultAbsoluteInterval);
        }

        /// <summary>
        /// Adds the specified item to the Cache and assigns the supplied Sliding Expiration policy to it.
        /// </summary>
        /// <param name="key">The cache key used to reference the item.</param>
        /// <param name="value">The item to be added to the cache.</param>
        /// <param name="interval">
        /// Sliding Interval, which is the time from when an object is last accessed until it is removed.
        /// For example, if this value is equivalent of 20 mins. the object is expired and removed from the cache it has not been accessed 
        /// for the duration of this time.
        /// </param>
        public virtual void AddSlidingExpiration(string key, object value, TimeSpan interval)
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                _cache.Add(key, new CachedItem(value, CacheType.Sliding, interval));
            }
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">A String identifier for the cache item to remove.</param>
        public virtual void Remove(string key)
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                if (_cache.ContainsKey(key))
                { _cache.Remove(key); }
            }
        }

        /// <summary>
        /// Removes all items from the cache.
        /// </summary>
        public virtual void RemoveAll()
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Retrieves the specified item from the cache object.
        /// </summary>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <param name="value">Contains the item associated with the specified key, if found; otherwise null (Or Nothing) is returned.</param>
        /// <returns>true if an element with the specified key is found; otherwise, false.</returns>
        public virtual bool TryGetValue(string key, out object value)
        {
            using (ReadLock.Acquire(_rwSyncLock))
            {
                CachedItem cachedItem;
                if (_cache.TryGetValue(key, out cachedItem))
                {
                    if (cachedItem.CacheType.IsSliding())
                    { 
                        cachedItem.IncrementTime(); 
                    }

                    value = cachedItem.Value;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
        }
        public virtual bool TryGetValue<T>(string key, out T value)
        {           
            object objValue;
            if (TryGetValue(key, out objValue) && objValue.GetType().Equals(typeof(T)))
            { 
                value = (T) objValue;
                return true; 
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Retrieves the specified item from the cache object. 
        /// </summary>
        /// <typeparam name="T">Generic type of the object to retrieve.</typeparam>
        /// <param name="key">The identifier for the cache item to retrieve.</param>
        /// <returns>The retrieved cache item of specified type T; if the key is not found default value of generic type T is found.</returns>
        public virtual T GetValue<T>(string key)
        {
            object value;
            if (TryGetValue(key, out value) && value.GetType().Equals(typeof(T)))
            { return (T)value; }

            return default(T);
        }

        /// <summary>
        /// Determines whether the cache object contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if an element with the specified key is found; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            using (ReadLock.Acquire(_rwSyncLock))
            {
                return _cache.ContainsKey(key);
            }
        }

        /// <summary>
        /// Determines whether the cache object contains the specified value.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>true if an element with the specified value is found; otherwise, false.</returns>
        public bool ContainsValue(object value)
        {
            using (ReadLock.Acquire(_rwSyncLock))
            {
                foreach (KeyValuePair<string, CachedItem> kvpIter in _cache)
                {
                    CachedItem cachedItem = kvpIter.Value;
                    if (Object.ReferenceEquals(value, cachedItem.Value))
                    { return true; }
                }
            }
            return false;
        }


        #endregion

        #region Private
        private void InitializeCache()
        {
            _cache = new Dictionary<string, CachedItem>();

            TimerCallback callback = new TimerCallback(CheckExpiration);

            _timer = new System.Threading.Timer(callback, null, _checkExpirePeriod, _checkExpirePeriod);
        }

        private void CheckExpiration(object stateInfo)
        {
            using (WriteLock.Acquire(_rwSyncLock))
            {
                List<string> expirationList = new List<string>();

                foreach (KeyValuePair<string, CachedItem> kvpIter in _cache)
                {
                    CachedItem cachedItem = kvpIter.Value;

                    // Only inspect Absolute/Sliding Time objects which could potentially expire
                    // Add to expiration list of expired
                    if (cachedItem.IsExpired)
                        expirationList.Add(kvpIter.Key);
                }

                // Remove expire items   
                foreach (string keyIter in expirationList)
                    _cache.Remove(keyIter);
            }
        }
        #endregion

        #region Disposable Members
        /// <summary>
        /// Inheritors will implement this method if any clean up is necessary, this method will run once when the class is disposed 
        /// either by the client or GC.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        protected override void Cleanup()
        {
            if (_timer != null)
                _timer.Dispose();

            RemoveAll();
        }

        #endregion
    }
}
