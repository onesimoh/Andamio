using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Data.Caching
{
    #region CacheType
    public enum CacheType
    {
        None = 0,
        Absolute = 1,
        Sliding = 2
    }

    public static class CacheTypeExtensions
    {
        public static bool IsDefined(this CacheType cacheType)
        {
            return cacheType != CacheType.None;
        }
        public static bool IsAbsolute(this CacheType cacheType)
        {
            return cacheType == CacheType.Absolute;
        }
        public static bool IsSliding(this CacheType cacheType)
        {
            return cacheType == CacheType.Sliding;
        }    
    }
    
    #endregion

    public sealed class CachedItem
    {
        #region Constructors
        private CachedItem()
        { 
        }

        public CachedItem(object value) : this()
        {
            CacheType = CacheType.None;
            Expiration = DateTime.MaxValue;
            Increment = TimeSpan.Zero;
            Value = value;

        }

        public CachedItem(object value, CacheType cacheType, TimeSpan increment)
        {
            Value = value;
            CacheType = cacheType;
            Increment = increment;
            if (cacheType.IsDefined())
            { Expiration = DateTime.Now.Add(increment); }
        }

        #endregion

        #region Value
        public object Value { get; set; }
        
        #endregion

        #region CacheType
        public CacheType CacheType { get; private set; }

        #endregion

        #region Expiration
        public TimeSpan Increment { get; private set; }
        public DateTime Expiration { get; private set; }
        public bool IsExpired
        {
            get { return CacheType.IsDefined() && Expiration < DateTime.Now; }
        }

        public void IncrementTime()
        {
            Expiration = DateTime.Now.Add(Increment);
        }

        #endregion
    }
}
