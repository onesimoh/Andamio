using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.CodeDom;
using System.Reflection;
using System.Collections.ObjectModel;

using Andamio.Data.Entities;

namespace Andamio.Data.Serialization
{
    /// <summary>
    /// This is the surrogated version of the EntityCollection type that will be used for its 
    /// serialization/deserialization.
    /// </summary>
    [DataContract]
    public class EntityCollectionSurrogated<EntityType, EntityKey>
        where EntityType : SimpleKeyEntity<EntityKey>
        where EntityKey : struct, IComparable<EntityKey>
    {
        #region Constructors
        public EntityCollectionSurrogated()
        {
        }
        public EntityCollectionSurrogated(EntityCollection<EntityType, EntityKey> entityCollection)
        {
            IsLoadedOrAssigned = entityCollection.IsLoadedOrAssigned;
            Items = entityCollection.ToArray();
            DeletedItems = entityCollection.DeletedItems.ToArray();
        }

        #endregion

        #region Properties
        [DataMember]
        public bool IsLoadedOrAssigned { get; set; }

        [DataMember]
        public EntityType[] Items { get; set; }
        
        [DataMember]
        public EntityType[] DeletedItems { get; set; }

        #endregion

        #region Conversion
        public static implicit operator EntityCollection<EntityType, EntityKey>(EntityCollectionSurrogated<EntityType, EntityKey> entityCollectionSurrogated)
        {
            return new EntityCollection<EntityType,EntityKey>(entityCollectionSurrogated.Items)
            {
                IsLoadedOrAssigned = entityCollectionSurrogated.IsLoadedOrAssigned,
                DeletedItems = entityCollectionSurrogated.DeletedItems,
            };
        }

        public virtual EntityCollection<EntityType, EntityKey> ToEntityCollection()
        {
            return (EntityCollection<EntityType, EntityKey>) this;
        }

        #endregion
    }
}
