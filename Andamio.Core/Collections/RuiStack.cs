using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Andamio.Collections
{
    /// <summary>
    /// Represents a Last-in-first-out (LIFO) generic collection used to implement a Recently-Used-Items collection.
    /// </summary>
    /// <typeparam name="T">Generic type T of the elements held on this collection.</typeparam>
    [Serializable]
    public class RuiStack<T> : IEnumerable<T>, ICollection<T>
    {
        #region Constructors
        /// <summary>Default RUIStack Constructors.</summary>
        public RuiStack()
        {
            MaxItems = 20;
        }

        /// <summary>
        /// Constructs a RUIStack object and specifies a comparison method to provide the RUI behavior.
        /// </summary>
        /// <param name="comparisonEqual">Method to compare items in the collection.</param>
        public RuiStack(IEqualityComparer<T> comparer) : this()
        {
            Comparer = comparer;
        }

        /// <summary>
        /// Constructs a RUIStack object and specifies a comparison method to provide the RUI behavior.
        /// </summary>
        /// <param name="items">Items to populate the RUIStack.</param>
        /// <param name="comparisonEqual">Method to compare items in the collection.</param>
        /// <param name="maxItems">Max Item Count allowed.</param>
        public RuiStack(IEnumerable<T> items, IEqualityComparer<T> comparer, int maxItems)
            : this(comparer)
        {
            if (maxItems <= 0)
            { throw new ArgumentException("maxItems"); }

            MaxItems = maxItems;
            AddRange(items);
        }
        #endregion

        #region Items
        private readonly LinkedList<T> _ruiList = new LinkedList<T>();

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return _ruiList.Count; }
        }

        /// <summary>
        /// Gets an array of Items currently in RUIStack.
        /// </summary>
        public T[] GetItems()
        {
            return _ruiList.ToArray();
        }

        /// <summary>
        /// Gets the Max amount of items allowed in the RUIStack.
        /// </summary>
        public int MaxItems { get; set; }

        /// <summary>
        /// Gets the element at the specified index. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index. </returns>
        public object this[int index]
        {
            get
            {
                if (Count == 0)
                { throw new InvalidOperationException("RUIStack is empty."); }
                if (index >= Count)
                { throw new IndexOutOfRangeException(); }

                LinkedListNode<T> node = _ruiList.First;
                for (int i = 0; i <= index; i++)
                {
                    node = node.Next;
                }
                return node.Value;
            }
        }

        #endregion

        #region Comparison
        public IEqualityComparer<T> Comparer { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// Removes and returns the object at the top of the RUIStack.
        /// </summary>
        /// <returns>The Object at the top of the RUIStack.</returns>
        public T Pop()
        {
            if (Count <= 0)
            { throw new InvalidOperationException("RUIStack is empty."); }

            T item = _ruiList.First.Value;
            _ruiList.RemoveFirst();
            return item;
        }

        /// <summary>
        /// Returns the object at the top of the RUIStack without removing it. 
        /// </summary>
        /// <returns>The Object at the top of the RUIStack.</returns>        
        public T Peek()
        {
            if (Count <= 0)
            { throw new InvalidOperationException("RUIStack is empty."); }

            return _ruiList.First.Value;
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items != null && items.Any())
            {
                foreach (T item in items.Take(MaxItems))
                {
                    Add(item);
                }
            }
        }

        #endregion

        #region ICollection<T> Members
        /// <summary>
        /// Adds an item to the RUIStack. 
        /// </summary>
        /// <param name="item">The object to add to the RUIStack.</param>
        public void Add(T newItem)
        {
            if (Comparer != null)
            {
                T item = _ruiList.SingleOrDefault(match => Comparer.Equals(match, newItem));
                if (item != null)
                { _ruiList.Remove(item); }
            }
            else
            {
                if (_ruiList.Contains(newItem))
                { _ruiList.Remove(newItem); }
            }

            _ruiList.AddFirst(newItem);

            if (Count > MaxItems)
            {
                _ruiList.RemoveLast();
            }
        }

        /// <summary>
        /// Determines whether the RUIStack contains a specific value. 
        /// </summary>
        /// <param name="item">The object to locate in the RUIStack.</param>
        /// <returns>true if item is found in the RUIStack; otherwise, false. </returns>
        public bool Contains(T item)
        {
            if (Comparer != null)
            {
                return _ruiList.Contains(item, Comparer);
            }
            else
            {
                return _ruiList.Contains(item);
            }
        }

        /// <summary>
        /// Copies the elements of the RUIStack to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">Destination one-dimensional array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _ruiList.CopyTo(array, arrayIndex);            
        }

        /// <summary>
        /// Gets a value indicating whether the ICollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes specified item from the RUIStack.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Return true of item was removed; false otherwise.</returns>
        public bool Remove(T item)
        {
            if (_ruiList.Count > 0)
            {
                _ruiList.Remove(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all objects from the RUIStack.
        /// </summary>
        public void Clear()
        {
            if (_ruiList.Count > 0)
            { 
                _ruiList.Clear(); 
            }
        }

        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (T iter in _ruiList)
            { 
                yield return iter; 
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T iter in _ruiList)
            { 
                yield return iter; 
            }
        }
        #endregion
    }
}
