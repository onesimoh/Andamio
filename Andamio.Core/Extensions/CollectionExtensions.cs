using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Andamio
{
    public static class CollectionExtensions
    {
        #region ObservableCollection
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                foreach (T item in items)
                { 
                    collection.Add(item); 
                }
            }
        }

        #endregion

        #region ICollection
        /// <summary>
        /// Converts a set of objects into a delimited string value.
        /// </summary>
        /// <param name="values">The collection of values to convert.</param>
        /// <param name="separator">The separator to include between values.</param>
        /// <returns>A string representation of the given values joined using a separator.</returns>
        public static string Join(this IEnumerable values, string separator)
        {
            if (values == null)
            { return String.Empty; }

            List<string> strings = new List<string>();
            foreach (Object value in values)
            {
                if (value == null)
                { strings.Add("null"); }
                else
                { strings.Add(value.ToString()); }
            }
            if (strings.Count == 0)
                return String.Empty;

            return String.Join(separator, strings.ToArray());
        }
        #endregion

        #region IDictionary
        public static void AddRange(this IDictionary dictionary, IEnumerable<KeyValuePair> elements)
        {
            if (elements != null && elements.Any())
            {
                foreach (KeyValuePair element in elements)
                {
                    dictionary.Add(element.Key, element.Value);
                }
            }
        }

        public static void AddRange<TKey, TValue>(this IDictionary dictionary, IEnumerable<KeyValuePair<TKey, TValue>> elements)
        {
            if (elements != null && elements.Any())
            {
                foreach (KeyValuePair<TKey, TValue> element in elements)
                {
                    dictionary.Add(element.Key, element.Value);
                }
            }
        }

        /// <summary>
        /// Retrieves an element from a dictionary.
        /// </summary>
        /// <typeparam name="T">The type of the element to retrieve.</typeparam>
        /// <param name="bag">The dictionary to query.</param>
        /// <param name="key">The name of the element to retrieve.</param>
        /// <param name="defaultValue">The value to return if key is not present in the dictionary.</param>
        /// <returns>The requested element, or if no element exists with the specified name, then the
        /// default value is returned.</returns>
        public static T GetValue<T>(this IDictionary bag, string key, T defaultValue = default(T))
        {
            object value = bag[key];
            if (value == null)
            { value = defaultValue; }
            return (T)value;
        }

        public static string StringValue(this IDictionary bag, string key)
        {
            return bag.Contains(key) ? Convert.ToString(bag[key]) : String.Empty;
        }

        public static T Parse<T>(this IDictionary bag, string key, T defaultValue = default(T))
            where T : struct
        {
            return bag.StringValue(key).Parse<T>(defaultValue);
        }

        /// <summary>
        /// Inserts an element in a dictionary.  If an element already exists with the given name,
        /// then the value is overwritten.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="bag">The dictionary to modify.</param>
        /// <param name="key">The name of the element to insert or overwrite.</param>
        /// <param name="value">The element to insert.</param>
        public static void SetValue<T>(this IDictionary bag, string key, T value)
        {
            bag[key] = value;
        }

        /// <summary>
        /// Retrieves the de-serialized value from the dictionary for the specified key. 
        /// </summary>
        /// <remarks>
        /// If no value is available for the specified key, the default is returned.
        /// </remarks>
        /// <typeparam name="T">The type of the element to retrieve.</typeparam>
        /// <param name="bag">The dictionary to query.</param>
        /// <param name="key">The name of the element to retrieve.</param>
        /// <param name="defaultValue">The value to return if key is not present in the dictionary.</param>
        /// <returns>
        /// The requested element, or if no element exists with the specified name, then the default value is returned.
        /// </returns>
        public static T GetSerializedValue<T>(this IDictionary bag, string key, T defaultValue)
            where T : class
        {
            byte[] serializedData = bag[key] as byte[];
            if (serializedData != null)
            {
                IFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream(serializedData))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return (T)formatter.Deserialize(stream);
                }
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Serializes the specified value in Binary Format and stores the serialized copy in the Dictionary with the specified key.
        /// </summary>
        /// <remarks>
        /// If an element already exists with the given name, then the value is overwritten.
        /// </summary>
        /// </remarks>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="bag">The dictionary to modify.</param>
        /// <param name="key">The name of the element to insert or overwrite.</param>
        /// <param name="value">The element to insert.</param>
        public static void SetSerializedValue<T>(this IDictionary bag, string key, T value)
            where T : class
        {
            if (value != null)
            {
                IFormatter formatter = new BinaryFormatter();
                using (MemoryStream stream = new MemoryStream())
                {
                    formatter.Serialize(stream, value);
                    stream.Seek(0, SeekOrigin.Begin);
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        byte[] buffer = new byte[reader.BaseStream.Length];
                        bag[key] = reader.ReadBytes(buffer.Length);
                    }
                }
            }
            else
            {
                bag[key] = null;
            }
        }

        /// <summary>
        /// Converts a IDictionary into an array of KeyValuePair objects.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>An array of KeyValuePair objects.</returns>
        public static KeyValuePair[] ToKeyValuePairs(this IDictionary dictionary)
        {
            if (dictionary == null)
            { throw new ArgumentNullException("dictionary"); }

            List<KeyValuePair> values = new List<KeyValuePair>();
            foreach (var key in dictionary.Keys)
            {
                values.Add(new KeyValuePair(key.ToString(), dictionary[key].ToString()));
            }

            return values.ToArray();
        }

        /// <summary>
        /// Converts a IDictionary into an array of KeyValuePair objects.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>An array of KeyValuePair objects.</returns>
        public static KeyValuePair<TKey, TValue>[] ToKeyValuePairs<TKey, TValue>(this IDictionary dictionary)
        {
            if (dictionary == null)
            { throw new ArgumentNullException("dictionary"); }

            List<KeyValuePair<TKey, TValue>> values = new List<KeyValuePair<TKey, TValue>>();
            foreach (var key in dictionary.Keys)
            {
                values.Add(new KeyValuePair<TKey, TValue>((TKey) key, (TValue) dictionary[key]));
            }

            return values.ToArray();
        }
        #endregion

        #region NameValueCollection
        /// <summary>
        /// Converts a NameValueCollection into an array of KeyValuePair objects.
        /// </summary>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>An array of KeyValuePair objects.</returns>
        public static KeyValuePair[] ConvertToKeyValuePairs(this NameValueCollection collection, bool sortByKey = true)
        {
            if (collection == null)
            { throw new ArgumentNullException("collection"); }

            List<KeyValuePair> values = new List<KeyValuePair>();
            foreach (string key in collection)
            {
                values.Add(new KeyValuePair(key, collection[key]));
            }
            return (sortByKey) ? values.OrderBy(kvp => kvp.Key).ToArray() : values.ToArray();
        }

        /// <summary>
        /// Converts a NameValueCollection into Dictionary.
        /// </summary>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>An Dictionary.</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (collection != null && collection.Any())
            {
                foreach (string key in collection)
                {
                    dictionary.Add(key, collection[key]);
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Gets a KeyValuePair instance using the provided key.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="key">The key element</param>
        /// <returns>A KeyValuePair instance.</returns>
        public static KeyValuePair ToKeyValuePair(this NameValueCollection collection, string key)
        {
            return new KeyValuePair(key, collection[key]);
        }

        public static bool Any(this NameValueCollection collection)
        {
            return (collection.Count > 0);
        }

        public static void AddRange(this NameValueCollection collection, IEnumerable<KeyValuePair> items)
        {
            if (collection == null)
            { throw new ArgumentNullException("collection"); }

            if (items != null && items.Any())
            {
                items.ForEach(kvp => collection.Add(kvp.Key, kvp.Value));
            }
        }

        public static NameValueCollection Expand(this NameValueCollection collection)
        {
            NameValueCollection expandedNameValueCollection = new NameValueCollection();
            foreach (string key in collection)
            {
                string[] expandedCriteria = collection[key].Split(",");
                if (expandedCriteria != null && expandedCriteria.Any())
                {
                    expandedNameValueCollection.AddRange(expandedCriteria.Select(c => new KeyValuePair(key, c)));
                }
            }

            return expandedNameValueCollection;
        }

        public static NameValueCollection Collapse(this NameValueCollection collection)
        {
            Dictionary<string, string> collapsedItems = new Dictionary<string, string>();
            foreach (string key in collection)
            {
                string value;
                if (collapsedItems.TryGetValue(key, out value))
                {
                    collapsedItems[key] = value + "," + collection[key];
                }
                else
                {
                    collapsedItems.Add(key, collection[key]);
                }
            }

            NameValueCollection collapsedNameValueCollection = new NameValueCollection();
            collapsedNameValueCollection.AddRange(collapsedItems.ToKeyValuePairs());
            return collapsedNameValueCollection;
        }

        #endregion

        #region CaptureCollection
        public static string[] GetValues(this CaptureCollection captureCollection)
        {
            if (captureCollection == null)
            { throw new ArgumentNullException("captureCollection"); }

            int i = 0;
            string[] values = new string[captureCollection.Count];
            foreach (Capture capture in captureCollection)
            {
                values[i++] = capture.Value;
            }

            return values;
        }

        #endregion

        #region LinkedList
        public static IEnumerable<LinkedListNode<T>> GetUpcoming<T>(this LinkedListNode<T> node)
        {
            LinkedListNode<T> currentNode = node.Next;
            while (currentNode != null)
            {
                yield return currentNode;
                currentNode = currentNode.Next;
            }
        }

        public static IEnumerable<LinkedListNode<T>> GetUpcoming<T>(this LinkedListNode<T> node, Predicate<LinkedListNode<T>> predicate)
        {
            LinkedListNode<T> currentNode = node.Next;
            while (currentNode != null)
            {
                if (predicate(currentNode))
                {
                    yield return currentNode;
                }
                currentNode = currentNode.Next;
            }
        }

        public static IEnumerable<LinkedListNode<T>> GetPreceding<T>(this LinkedListNode<T> node)
        {
            LinkedListNode<T> currentNode = node.Previous;
            while (currentNode != null)
            {
                yield return currentNode;
                currentNode = currentNode.Previous;
            }
        }

        public static IEnumerable<LinkedListNode<T>> GetPreceding<T>(this LinkedListNode<T> node, Predicate<LinkedListNode<T>> predicate)
        {
            LinkedListNode<T> currentNode = node.Previous;
            while (currentNode != null)
            {
                if (predicate.Invoke(currentNode))
                {
                    yield return currentNode;
                }
                currentNode = currentNode.Previous;
            }
        }

        #endregion

        #region Matches
        public static bool Any(this MatchCollection matches)
        {
            return matches != null && matches.Count > 0;
        }

        //public static bool ToKeyValuePairs<KeyType, ValueType>(this MatchCollection matches
        //    , Func<Match, KeyType> keyExpression
        //    , Func<Match, ValueType> valueExpression)
        //{
        //    Dictionary<KeyType, ValueType> allMatches = new Dictionary<KeyType, ValueType>();

        //}

        #endregion
    }
}
