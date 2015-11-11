using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Base class for entities that require unique identity.
    /// </summary>
    /// <typeparam name="T">The type for key values (typically Int32 or Guid).</typeparam>
    [Serializable]
    [DataContract(IsReference = true)]    
    public abstract class SimpleKeyEntity<T> : EntityBase, IEquatable<SimpleKeyEntity<T>> 
        where T : struct
    {
        #region Protected Fields
        /// <summary>
        /// Unique ID field.
        /// </summary>
        protected T? _id = null;

        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SimpleKeyEntity()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the unique ID for Entity.
        /// </summary>
        [DataMember]
        public virtual T ID
        {
            get { return _id.HasValue ? _id.Value : default(T); }
            set
            {
                if (!_id.Equals(value))
                {
                    _id = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ID"));
                }
            }
        }

        #endregion

        #region Match
        /// <summary>
        /// Checks if a entity is a match.
        /// </summary>
        /// <param name="entity">Instance of SimpleKeyEntity to match against this instance.</param>
        /// <returns>true if ID value for both instances is equal; false otherwise.</returns>
        public virtual bool MatchKey(SimpleKeyEntity<T> entity)
        {
            if (entity != null)
            { return this.ID.Equals(entity.ID); }
            return false;
        }
        
        /// <summary>
        /// Determines equality between two objects of type SimpleKeyEntity.
        /// </summary>
        /// <param name="value">The Object to compare with theis instance object.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            SimpleKeyEntity<T> entity = value as SimpleKeyEntity<T>;
            if (entity != null)
            { 
                return Equals(entity);
            }
            return false;
        }
        
        /// <summary>
        /// Provides value equality between two objects of the same type.
        /// </summary>
        /// <param name="objValue">The Object to compare with the current Object.</param>
        /// <returns>true if the specified Object is equal to the current Object; otherwise, false.</returns>
        public bool Equals(SimpleKeyEntity<T> entity)
        {
            return this.MatchKey(entity);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        #endregion
    }
}
