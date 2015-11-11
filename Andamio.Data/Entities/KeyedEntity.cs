using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andamio.Data
{ 
    /// <summary>
    /// Defines an entity containing a key.
    /// </summary>
    public interface IKeyedEntity
    {
        /// <summary>
        /// Gets or sets the System.Data.Entity.Core.EntityKey for instances of entity
        /// types that implement this interface.
        /// </summary>
        KeyedEntity EntityKey { get; }
    }


    /// <summary>
    /// An identifier for an entity.
    /// </summary>
    [DebuggerDisplay("{FullyQualifiedName}: {KeyValue}")]
    public class KeyedEntity : IEquatable<KeyedEntity>
    {
        #region Constructor
        /// <summary>Default Constructor</summary>
        private KeyedEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Data.Entity.Core.EntityKey class
        /// with an entity set name and specific entity key pair.
        /// </summary>
        /// <param name="fullyQualifiedName">Fully Qualified Entity Name, defines a unique name for the entity.</param>
        /// <param name="keyValue">Entity key value.</param>
        public KeyedEntity(string fullyQualifiedName, object keyValue) : this()
        {
            if (fullyQualifiedName.IsNullOrBlank()) throw new ArgumentNullException("fullyQualifiedName");
            if (keyValue == null) throw new ArgumentNullException("keyValue");

            FullyQualifiedName = fullyQualifiedName;
            KeyValue = keyValue;
        }
        
        #endregion

        #region Properties
        /// <summary>
        /// Fully Qualified Entity Name, defines a unique name for the entity.
        /// </summary>
        public string FullyQualifiedName { get; private set; }

        /// <summary>
        /// Entity key value.
        /// </summary>
        public object KeyValue { get; private set; }

        #endregion

        #region Operator Override
        /// <summary>
        /// Compares two KeyedEntity objects.
        /// </summary>
        /// <param name="left">A KeyedEntity to compare.</param>
        /// <param name="right">A KeyedEntity to compare.</param>
        /// <returns>true if KeyedEntities are NOT equal; otherwise, false.</returns>
        public static bool operator !=(KeyedEntity left, KeyedEntity right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Compares two KeyedEntity objects.
        /// </summary>
        /// <param name="left">A KeyedEntity to compare.</param>
        /// <param name="right">A KeyedEntity to compare.</param>
        /// <returns>true if KeyedEntities are equal; otherwise, false.</returns>
        public static bool operator ==(KeyedEntity left, KeyedEntity right)
        {
            if (Object.ReferenceEquals(left, right)) return true;
            return left.Equals(right);
        }

        public bool Equals(KeyedEntity keyedEntity)
        {
            if (Object.ReferenceEquals(keyedEntity, null)) return false;
            return (this.FullyQualifiedName == keyedEntity.FullyQualifiedName) && (this.KeyValue.Equals(keyedEntity.KeyValue));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is KeyedEntity)
            { return this.Equals((KeyedEntity)obj); }
            return false;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", FullyQualifiedName, KeyValue);
        }
        #endregion
    }
}
