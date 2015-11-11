using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Andamio.Configuration
{
    /// <summary>
    /// Represents a generic configuration element containing a collection of child elements. 
    /// </summary>
    /// <typeparam name="T">Generic Type T of the configuration element.</typeparam>
    public abstract class ConfigurationElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> 
        where T : ConfigurationElement, IConfigKeyElement, new()
    {
        #region ConfigurationElementCollection Overrides
        /// <summary>
        /// Creates a new ConfigurationElement.
        /// </summary>
        /// <returns>A new ConfigurationElement.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }
        
        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The ConfigurationElement to return the key for.</param>
        /// <returns>An Object that acts as the key for the specified ConfigurationElement.</returns>
        protected override object GetElementKey( ConfigurationElement element )
        {
            return ((T) element).Key;
        }
        #endregion   
             
        #region IEnumerable<T> Members
		/// <summary>
		/// Enumerates the services in the collection.
		/// </summary>
        public new IEnumerator<T> GetEnumerator()
        {
            for( int i = 0; i < base.Count; i++ )
                yield return (T) base.BaseGet(i);
        }
        #endregion        
    }
}
