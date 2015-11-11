using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Defines methods to compare two Entities
    /// </summary>
    /// <typeparam name="EntityType">The Type of Entity to compare.</typeparam>
    /// <typeparam name="EntityKey">The Type of the ID field that identifies the Entity.</typeparam>
    public class EntityIdComparer<EntityType, EntityKey> : IEqualityComparer<EntityType>
        where EntityType : SimpleKeyEntity<EntityKey>
        where EntityKey : struct, IComparable<EntityKey>
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public EntityIdComparer()
        {
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first Entity to Compare.</param>
        /// <param name="y">The second Entity to Compare</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(EntityType x, EntityType y)
        {
            return x.ID.CompareTo(y.ID) == 0;
        }

        /// <summary>
        /// Returns a hash code for the specified Entity.
        /// </summary>
        /// <param name="obj">The Entity for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(EntityType entity)
        {
            return entity.ID.GetHashCode();
        }
    }
}
