using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Validation;

using Andamio;
using Andamio.Data;
using Andamio.Security;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public partial class FakeDAO<EntityType> : DaoBase<EntityType>
        where EntityType : EntityBase, new()
    {
        #region Internals
        private static readonly EntityRelationshipRepository _EntityRelationshipRepository = new EntityRelationshipRepository();
        void OnEntitiesInserted(object sender, EntityCollectionEventArgs<EntityType> e)
        {
            EntityType entity = e.Item;
            if (entity != null)
            {
                entity.IsModified = false;
                entity.IsReadFromDatabase = true;
            }
        }
        
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public FakeDAO()
        {
            _Internal.ItemInserted += OnEntitiesInserted;
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public FakeDAO(IEnumerable<EntityType> entities) : this()
        {
            _EntityRelationshipRepository.Add(entities);
        }

        #endregion

        #region Update\Insert
        /// <summary>
        /// Inserts the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Insert.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to insert an Entity that already exists in the Data Source.
        /// </exception>
        public override void Insert(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (entity.IsReadFromDatabase || _EntityRelationshipRepository.Contains(entity)) throw new InvalidOperationException("Entity already exists in the data storage.");

            _EntityRelationshipRepository.Add(entity);
        }

        /// <summary>
        /// Inserts the specified Entities in Bulk in a Transactional Operation.
        /// </summary>
        /// <param name="entities">The Entities to insert in the Data Source.</param>
        public override void BulkUpsert(IEnumerable<EntityType> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");
            entities.ForEach(e => Upsert(e));
        }

        /// <summary>
        /// Updates the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Update.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to update an Entity that does not exist in the Data Source.
        /// </exception>        
        public override void Update(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!entity.IsReadFromDatabase || !_EntityRelationshipRepository.Contains(entity)) throw new InvalidOperationException("Entity does not exists in the data storage.");

            entity.IsModified = false;
            entity.IsReadFromDatabase = true;
        }

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified Entity from the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to delete from the Data Source.</param>
        public override void Delete(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (_EntityRelationshipRepository.Contains(entity))
            {
                _EntityRelationshipRepository.Remove(entity);
            }
        }

        #endregion

        #region Retrieval
        /// <summary>
        /// Retrieves all Entities in the Data Source.
        /// </summary>
        public override List<EntityType> All()
        {
            return _EntityRelationshipRepository.OfType<EntityType>().ToList();
        }

        /// <summary>
        /// Retrieves all Entities in the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override List<EntityType> Many(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return _EntityRelationshipRepository.OfType<EntityType>().Where(predicate.Compile()).ToList();
        }

        /// <summary>
        /// Retrieves a single Entity from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override EntityType Single(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            return _EntityRelationshipRepository.OfType<EntityType>().SingleOrDefault(predicate.Compile());
        }

        /// <summary>
        /// Retrieves an Entity from the Data Source for the specified Primary Key.
        /// </summary>
        /// <param name="primaryKey">The Primary Key that indentifies the Entity.</param>
        public override EntityType WithKey<EntityKey>(EntityKey primaryKey)
        {
            throw new InvalidOperationException();
        }

        #endregion

        #region Search
        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override List<EntityType> Search<SortKey>(Expression<Func<EntityType, bool>> predicate
            , Expression<Func<EntityType, SortKey>> sort
            , SearchPageSettings pageSettings = null)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (sort == null) throw new ArgumentNullException("sort");

            pageSettings = pageSettings ?? new SearchPageSettings();

            var searchResults = _EntityRelationshipRepository.OfType<EntityType>().Where(predicate.Compile()).OrderBy(sort.Compile());
            pageSettings.Calculate(searchResults);

            return searchResults.Skip(pageSettings.Skip).Take(pageSettings.PageSize).ToList();
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override List<EntityType> Search<SortKey>(DaoQuery<EntityType> query
            , Expression<Func<EntityType, SortKey>> sort
            , SearchPageSettings pageSettings = null)
        {
            if (query == null) throw new ArgumentNullException("query");
            if (sort == null) throw new ArgumentNullException("sort");

            pageSettings = pageSettings ?? new SearchPageSettings();

            IEnumerable<EntityType> searchResults = _EntityRelationshipRepository.OfType<EntityType>();
            query.Queries.ForEach(predicate => searchResults = searchResults.Where(predicate.Compile()));
            query.Predicates.ForEach(predicate => searchResults = searchResults.Where(predicate));

            pageSettings.Calculate(searchResults);

            return searchResults.Skip(pageSettings.Skip).Take(pageSettings.PageSize).ToList();
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override DaoQuery<EntityType> Query(Expression<Func<EntityType, bool>> predicate)
        {
            DaoQuery<EntityType> query = new DaoQuery<EntityType>(predicate);
            return query;
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        public override DaoQuery<EntityType> Query()
        {
            DaoQuery<EntityType> query = new DaoQuery<EntityType>();
            return query;
        }

        #endregion

        #region Sync
        internal override void SyncManyToMany<CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> navigationProperty
            , bool cascadeDelete = false)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (navigationProperty == null) throw new ArgumentNullException("navigationProperty");

            string propertyName = ((MemberExpression)navigationProperty.Body).Member.Name;
            var collection = entity.Property(propertyName).Value as EntityCollectionBase<CollectionType>;

            if (collection == null)
            {
                throw new InvalidCastException();
            }

            var relatedEntities = _EntityRelationshipRepository.RelatedEntities(entity, navigationProperty);

            foreach (CollectionType childEntity in collection)
            {
                if (childEntity.IsModified)
                {
                    DAO.For<CollectionType>().Upsert(childEntity);
                }

                if (!relatedEntities.Contains(childEntity))
                {
                    _EntityRelationshipRepository.AddAssociation(entity, childEntity, navigationProperty);
                } 
            }

            if (collection.DeletedItems.Any())
            {
                foreach (CollectionType deletedEntity in collection.DeletedItems)
                {
                    if (relatedEntities.Contains(deletedEntity))
                    {
                        _EntityRelationshipRepository.RemoveAssociation(entity, deletedEntity, navigationProperty);
                    }
                    if (cascadeDelete)
                    {
                        DAO.For<CollectionType>().Delete(deletedEntity);
                    }
                }
            }

            collection.Reset();
        }

        internal override void SyncMany<CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> navigationProperty)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (navigationProperty == null) throw new ArgumentNullException("navigationProperty");

            string propertyName = ((MemberExpression)navigationProperty.Body).Member.Name;
            var collection = entity.Property(propertyName).Value as EntityCollectionBase<CollectionType>;

            if (collection == null)
            {
                throw new InvalidCastException();
            }

            if (collection.DeletedItems.Any())
            {
                foreach (CollectionType deletedEntity in collection.DeletedItems)
                {
                    if (relatedEntities.Contains(deletedEntity))
                    {
                        _EntityRelationshipRepository.RemoveAssociation(entity, deletedEntity, navigationProperty);
                    }
                    
                    DAO.For<CollectionType>().Delete(deletedEntity);
                }
            }

            foreach (CollectionType childEntity in collection)
            {
                if (childEntity.IsModified)
                {
                    DAO.For<CollectionType>().Upsert(childEntity);
                }

                if (!relatedEntities.Contains(childEntity))
                {
                    _EntityRelationshipRepository.AddAssociation(entity, childEntity, navigationProperty);
                }
            }

            collection.Reset();
        }

        #endregion

        #region Related
        internal override List<RelatedType> GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (navigationProperty == null) throw new ArgumentNullException("navigationProperty");

            var relatedEntities = _EntityRelationshipRepository.RelatedEntities(entity, navigationProperty);

            string propertyName = ((MemberExpression) navigationProperty.Body).Member.Name;            

            var relatedEntities = entity.Property(propertyName).Value as ICollection<RelatedType>;
            return relatedEntities.ToList();
        }

        internal override List<RelatedType> GetRelated<RelatedType, TProperty>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty
            , Expression<Func<RelatedType, TProperty>> include)
        {
            return GetRelated<RelatedType>(entity, navigationProperty);
        }

        internal override RelatedType GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> navigationProperty)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (navigationProperty == null) throw new ArgumentNullException("navigationProperty");

            RelatedType relatedEntity = _EntityRelationshipRepository.RelatedEntity(entity, navigationProperty);
            return relatedEntity;
        }

        internal override void Load<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
