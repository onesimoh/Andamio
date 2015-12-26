using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Andamio.Data.Entities;

using Andamio;
using Andamio.Collections;

namespace Andamio.Data.Access
{
    public sealed class EntityRelationshipRepository
    {        
        #region Constructors
        public EntityRelationshipRepository()
        {
            Entities = new CollectionBase<EntityBase>();
            Entities.ItemsInserted += OnEntitiesInserted;
            Entities.ItemsRemoved += OnEntitiesRemoved;

            Associations = new CollectionBase<EntityAssociation>();
            Associations.ItemsInserted += OnAssociationsInserted;
            Associations.ItemsRemoved += OnAssociationsRemoved;
        }

        #endregion

        #region Entities
        public CollectionBase<EntityBase> Entities { get; private set;  }        
        void OnEntitiesInserted(object sender, ItemEventArgs<IEnumerable<EntityBase>> e)
        {
        
        }

        private void OnEntitiesRemoved(object sender, ItemEventArgs<IEnumerable<EntityBase>> e)
        {

        }

        public EntityRelationship<EntityType> Entity<EntityType>(EntityType entity) 
            where EntityType : EntityBase, new()
        {
            return new EntityRelationship<EntityType>(this, entity);
        }

        #endregion

        #region Related
        public IEnumerable<RelatedType> Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression) relationship.Body).Member.Name;
            var related = Associations.Where(association => association.Entity.Equals(entity) && association.Relationship.Equals(relationshipName))
                .Select(association => association.RelatedEntity)
                .OfType<RelatedType>();
            return related;
        }

        public RelatedType Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression) relationship.Body).Member.Name;
            var related = Associations.Where(association => association.Entity.Equals(entity) && association.Relationship.Equals(relationshipName))
                .OfType<RelatedType>()
                .SingleOrDefault();
            return related;
        }

        #endregion

        #region Associations
        public CollectionBase<EntityAssociation> Associations { get; private set; }
        void OnAssociationsInserted(object sender, ItemEventArgs<IEnumerable<EntityAssociation>> e)
        {

        }

        void OnAssociationsRemoved(object sender, ItemEventArgs<IEnumerable<EntityAssociation>> e)
        {
        
        }

        internal EntityAssociation AddAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship) 
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression) relationship.Body).Member.Name;
            return AddAssociation(entity, relatedEntity, relationshipName);
        }

        internal EntityAssociation AddAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression)relationship.Body).Member.Name;
            return AddAssociation(entity, relatedEntity, relationshipName);
        }

        private EntityAssociation AddAssociation(EntityBase entity, EntityBase relatedEntity, string relationship)
        {
            var association = new EntityAssociation(entity, relatedEntity, relationship);
            Associations.Add(association);
            return association;
        }

        internal bool HasAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {            
            string relationshipName = ((MemberExpression)relationship.Body).Member.Name;
            return HasAssociation(entity, relatedEntity, relationshipName);
        }

        internal bool HasAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression)relationship.Body).Member.Name;
            return HasAssociation(entity, relatedEntity, relationshipName);
        }

        private bool HasAssociation(EntityBase entity, EntityBase relatedEntity, string relationship)
        {
            return Associations.Any(association => association.Entity.Equals(entity)
                && association.RelatedEntity.Equals(relatedEntity)
                && association.Relationship.Equals(relationship));
        }

        internal void RemoveAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression) relationship.Body).Member.Name;
            RemoveAssociation(entity, relatedEntity, relationshipName);
        }

        internal void RemoveAssociation<EntityType, RelatedType>(EntityType entity
            , RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase
        {
            string relationshipName = ((MemberExpression) relationship.Body).Member.Name;
            RemoveAssociation(entity, relatedEntity, relationshipName);
        }

        private void RemoveAssociation(EntityBase entity, EntityBase relatedEntity, string relationship)
        {
            Associations.RemoveAll(association => association.Entity.Equals(entity)
                && association.RelatedEntity.Equals(relatedEntity)
                && association.Relationship.Equals(relationship));
        }

        #endregion
    }



    public sealed class EntityRelationship<EntityType>
         where EntityType : EntityBase, new()
    {
        #region Constructors
        private EntityRelationship()
        {
        }

        internal EntityRelationship(EntityRelationshipRepository repository, EntityType entity)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (entity == null) throw new ArgumentNullException("entity");

            Repository = repository;
            Entity = entity;
        }

        #endregion

        #region Properties
        private EntityRelationshipRepository Repository { get; set; }
        private EntityType Entity { get; set; }

        #endregion

        #region Related
        public IEnumerable<RelatedType> Related<RelatedType>(Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.Related(Entity, relationship);
        }

        public RelatedType Related<RelatedType>(Expression<Func<EntityType, RelatedType>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.Related(Entity, relationship);
        }

        #endregion

        #region Associations
        public EntityAssociation AddAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.AddAssociation(Entity, relatedEntity, relationship);
        }

        public EntityAssociation AddAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.AddAssociation(Entity, relatedEntity, relationship);
        }

        public bool HasAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.HasAssociation(Entity, relatedEntity, relationship);
        }

        public bool HasAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where RelatedType : EntityBase
        {
            return Repository.HasAssociation(Entity, relatedEntity, relationship);
        }

        public void RemoveAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, ICollection<RelatedType>>> relationship)
            where RelatedType : EntityBase
        {
            Repository.RemoveAssociation(Entity, relatedEntity, relationship);
        }

        public void RemoveAssociation<RelatedType>(RelatedType relatedEntity
            , Expression<Func<EntityType, RelatedType>> relationship)
            where RelatedType : EntityBase
        {
            Repository.RemoveAssociation(Entity, relatedEntity, relationship);
        }

        #endregion
    }



    public sealed class EntityAssociation
    {
        #region Constructors
        private EntityAssociation()
        {
        }

        internal EntityAssociation(EntityBase entity, EntityBase relatedEntity, string relationship)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (relatedEntity == null) throw new ArgumentNullException("relatedEntity");
            if (relationship == null) throw new ArgumentNullException("relationship");

            Entity = entity;
            RelatedEntity = relatedEntity;
            Relationship = relationship;
        }

        #endregion

        #region Properties
        public EntityBase Entity { get; private set; }
        public EntityBase RelatedEntity { get; private set; }
        public string Relationship { get; private set; }

        #endregion
    }
}
