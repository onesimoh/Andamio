using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public class DaoEntity<EntityType>
        where EntityType : EntityBase, new()
    {
        #region Constructors
        protected DaoEntity()
        {
        }

        public DaoEntity(IDataAdapter dataAdapter, EntityType entity)
        {
            if (dataAdapter == null) throw new ArgumentNullException("dataAdapter");
            if (entity == null) throw new ArgumentNullException("entity");

            DataAdapter = dataAdapter;
            Entity = entity;
        }

        #endregion

        #region Properties
        public IDataAdapter DataAdapter { get; private set; }
        public EntityType Entity { get; private set; }

        #endregion

        #region Related
        public virtual IEnumerable<RelatedType> Related<RelatedType>(Expression<Func<EntityType, ICollection<RelatedType>>> property)
            where RelatedType : EntityBase, new()
        {
            if (property == null) throw new ArgumentNullException("property");
            return DataAdapter.Related(Entity, property);
        }

        public virtual IEnumerable<RelatedType> Related<RelatedType, TProperty>(Expression<Func<EntityType, ICollection<RelatedType>>> property
            , Expression<Func<RelatedType, TProperty>> include)
            where RelatedType : EntityBase, new()
        {
            if (property == null) throw new ArgumentNullException("property");
            return DataAdapter.Related(Entity, property, include);
        }

        public virtual RelatedType Related<RelatedType>(Expression<Func<EntityType, RelatedType>> property)
            where RelatedType : EntityBase, new()
        {
            if (property == null) throw new ArgumentNullException("property");
            return DataAdapter.Related(Entity, property);
        }

        #endregion

        #region Sync
        public virtual void SyncManyToMany<CollectionType>(Expression<Func<EntityType
            , ICollection<CollectionType>>> property
            , bool cascadeDelete = true)
            where CollectionType : EntityBase, new()
        {
            if (property == null) throw new ArgumentNullException("property");
            DataAdapter.SyncManyToMany(Entity, property, cascadeDelete);
        }

        public virtual void SyncMany<CollectionType>(Expression<Func<EntityType
            , ICollection<CollectionType>>> property)
            where CollectionType : EntityBase, new()
        {
            string propertyName = ((MemberExpression) property.Body).Member.Name;
            var collection = (EntityCollectionBase<CollectionType>) property.Compile().Invoke(Entity);

            if (collection == null) return;
            collection.DeletedItems.ForEach(deletedEntity => DAO.For<CollectionType>().Delete(deletedEntity));
            collection.ForEach(entity => DAO.For<CollectionType>().Upsert(entity));
            collection.Reset();
        }

        #endregion
    }
}
