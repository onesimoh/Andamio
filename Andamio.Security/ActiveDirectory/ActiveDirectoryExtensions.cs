using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

using Andamio;

namespace Andamio.Security.ActiveDirectory
{
    public static class ActiveDirectoryExtensions
    {
        public static KeyValuePair<string, object>[] ToKeyValuePairs(this ResultPropertyCollection properties)
        {
            if (properties == null)
            { throw new ArgumentNullException("properties"); }

            List<KeyValuePair<string, object>> values = new List<KeyValuePair<string, object>>();
            foreach (string propertyName in properties.PropertyNames)
            {
                ResultPropertyValueCollection propertyValues = properties[propertyName];
                if (propertyValues.Any())
                {
                    var propertyValue = (propertyValues.Count == 1) ? propertyValues.First() : propertyValues;
                    values.Add(new KeyValuePair<string, object>(propertyName.ToLower(), propertyValue));
                }
            }

            return values.ToArray();
        }

        public static KeyValuePair<string, object>[] ToKeyValuePairs(this PropertyCollection properties)
        {
            if (properties == null)
            { throw new ArgumentNullException("properties"); }

            List<KeyValuePair<string, object>> values = new List<KeyValuePair<string, object>>();
            foreach (PropertyValueCollection propertyValues in properties)
            {
                if (propertyValues.Any())
                {
                    var propertyValue = (propertyValues.Count == 1) ? propertyValues.First() : propertyValues;
                    values.Add(new KeyValuePair<string, object>(propertyValues.PropertyName.ToLower(), propertyValue));
                }
            }

            return values.ToArray();
        }

        public static bool Any(this SearchResultCollection searchResultCollection)
        {
            return searchResultCollection != null && searchResultCollection.Count > 0;
        }

        public static bool Any(this PropertyValueCollection propertyValueCollection)
        {
            return propertyValueCollection != null && propertyValueCollection.Count > 0;
        }

        public static SearchResult First(this SearchResultCollection searchResultCollection)
        {
            return searchResultCollection[0];
        }

        public static object First(this PropertyValueCollection propertyValueCollection)
        {
            return propertyValueCollection[0];
        }

        public static bool Any(this ResultPropertyValueCollection resultPropertyValueCollection)
        {
            return resultPropertyValueCollection != null && resultPropertyValueCollection.Count > 0;
        }

        public static T First<T>(this ResultPropertyValueCollection resultPropertyValueCollection)
        {
            return (T) resultPropertyValueCollection[0];
        }

        public static object First(this ResultPropertyValueCollection resultPropertyValueCollection)
        {
            return resultPropertyValueCollection[0];
        }
    }
}
