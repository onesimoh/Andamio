using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public interface IDataAdapter	    
    {
        #region CRUD
        void Insert<EntityType>(EntityType entity)
            where EntityType : EntityBase, new();

	    void Update<EntityType>(EntityType entity)
            where EntityType : EntityBase, new();

	    void Delete<EntityType>(EntityType entity)
            where EntityType : EntityBase, new();

        IQueryable<EntityType> Query<EntityType>()
            where EntityType : EntityBase, new();

        EntityType WithKey<EntityType, EntityKey>(EntityKey primaryKey)
            where EntityType : EntityBase, new()
            where EntityKey : struct, IComparable<EntityKey>;

        #endregion

        #region Related
        IEnumerable<RelatedType> Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> property)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new();

        IEnumerable<RelatedType> Related<EntityType, RelatedType, TProperty>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> property
            , Expression<Func<RelatedType, TProperty>> include)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new();

        RelatedType Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> property)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new();

        #endregion

        #region Sync
        void SyncManyToMany<EntityType, CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> property
            , bool cascadeDelete = false)
            where EntityType : EntityBase, new()
            where CollectionType : EntityBase, new();

        #endregion
    }
}
