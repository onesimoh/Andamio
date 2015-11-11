using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

using Andamio;
using Andamio.Data;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public class DaoEntity<EntityType>
        where EntityType : EntityBase, new()
    {
        #region Constructors
        private DaoEntity()
        {
        }

        internal DaoEntity(DaoBase<EntityType> dao, EntityType entity)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            if (entity == null) throw new ArgumentNullException("entity");

            Dao = dao;
            Entity = entity;
        }

        #endregion

        #region Properties
        private DaoBase<EntityType> Dao { get; set; }
        private EntityType Entity { get; set; }

        #endregion

        #region Related
        public virtual List<RelatedType> GetRelated<RelatedType>(Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
            where RelatedType : EntityBase, new()
        {
            return Dao.GetRelated(Entity, navigationProperty);
        }

        public virtual List<RelatedType> GetRelated<RelatedType, TProperty>(Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty
            , Expression<Func<RelatedType, TProperty>> include)
            where RelatedType : EntityBase, new()
        {
            return Dao.GetRelated(Entity, navigationProperty, include);
        }

        public virtual RelatedType GetRelated<RelatedType>(Expression<Func<EntityType, RelatedType>> navigationProperty)
            where RelatedType : EntityBase
        {
            return Dao.GetRelated(Entity, navigationProperty);
        }

        public virtual void Load<RelatedType>(Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
            where RelatedType : EntityBase
        {
            Dao.Load(Entity, navigationProperty);
        }

        #endregion

        #region Sync
        public virtual void SyncManyToMany<CollectionType>(Expression<Func<EntityType, ICollection<CollectionType>>> navigationProperty
            , bool cascadeDelete = true)
            where CollectionType : EntityBase, new()
        {
            Dao.SyncManyToMany(Entity, navigationProperty, cascadeDelete);
        }

        #endregion
    }
}
