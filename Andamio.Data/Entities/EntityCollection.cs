using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.CodeDom;
using System.Reflection;
using System.Diagnostics;

using Andamio.Threading;
using Andamio.Serialization;
using Andamio.Data.Serialization;

namespace Andamio.Data.Entities
{
    /// <summary>
    /// Represents an Entity Collection.
    /// </summary>
    /// <typeparam name="EntityType">The Entity Type of the Collection.</typeparam>
    /// <typeparam name="EntityKey">The Entity Key Type of the Collection.</typeparam>
    [Serializable]
    [CollectionDataContract]
    [DebuggerDisplay("Count = {Count}")]
    public class EntityCollection<EntityType, EntityKey> : EntityCollectionBase<EntityType>
        where EntityType : SimpleKeyEntity<EntityKey>
        where EntityKey : struct, IComparable<EntityKey>
    {
        #region Constructors
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public EntityCollection() : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public EntityCollection(IEnumerable<EntityType> items) : base(items)
        {
        }

        public EntityCollection(EntityCollectionBase<EntityType> entityCollection) : base(entityCollection)
        {

        }

        public EntityCollection(EntityCollectionSurrogated<EntityType, EntityKey> entityCollectionSurrogated)
            : this((EntityCollection<EntityType, EntityKey>) entityCollectionSurrogated)
        {
        }

        #endregion

        #region Items
        /// <summary>
        /// Returns the Entity for the specified Identifier.
        /// </summary>
        /// <param name="id"></param>
        public EntityType this[EntityKey id]
        {
            get
            {
                return _items.FirstOrDefault(match => match.ID.CompareTo(id) == 0);
            }
        }

        #endregion

        #region Sync Data
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EntityCollectionBase<EntityType> SyncData()
        {
            IEqualityComparer<EntityType> comparer = new EntityIdComparer<EntityType, EntityKey>();
            return SyncData(comparer);
        }

        public EntityCollectionBase<EntityType> SyncData(PopulateMethodDelegate<EntityType> dataSyncMethod)
        {
            IEqualityComparer<EntityType> comparer = new EntityIdComparer<EntityType, EntityKey>();
            return SyncData(comparer, dataSyncMethod);
        }


        public void RefreshData()
        {
            IEqualityComparer<EntityType> comparer = new EntityIdComparer<EntityType, EntityKey>();
            RefreshData(comparer);
        }

        #endregion

        #region Merge
        /// <summary>
        /// Merges the specified Entity within the Collection.
        /// </summary>
        /// <param name="entity">The Entity to merge in the Collection.</param>
        public void Merge(EntityType entity)
        {
            Merge(new EntityType[] { entity });
        }

        /// <summary>
        /// Merges the specified Entities within the Collection.
        /// </summary>
        /// <param name="entities">The Entities to merge in the Collection.</param>
        public void Merge(IEnumerable<EntityType> entities)
        {
            IEqualityComparer<EntityType> comparer = new EntityIdComparer<EntityType, EntityKey>();
            Merge(entities, comparer);
        }

        #endregion
    }
}
