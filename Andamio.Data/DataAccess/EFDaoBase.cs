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
using Andamio.Data.Transactions;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Base class for Entity Andamio Data Access Objects (DAO).
    /// </summary>
    /// <typeparam name="EntityType">The Entity Type managed by this DAO.</typeparam>
    /// <typeparam name="ObjectContextType">The Object Context to use with this DAO.</typeparam>
    public class EFDaoBase<EntityType, DbContextType> : DaoBase<EntityType>
        where EntityType : EntityBase, new()
        where DbContextType : DbContext
    {
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public EFDaoBase()
        {
        }

        #endregion

        #region Update\Insert
        /// <summary>
        /// Updates or Insert the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Insert or Update.</param>
        public override void Upsert(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (!entity.IsReadFromDatabase)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

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
            if (entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is already in the database.");

            using (DbContextType context = CreateDbContext())
            {
                Insert(entity, context);
            }
        }

        /// <summary>
        /// Inserts the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Insert.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to insert an Entity that already exists in the Data Source.
        /// </exception>
        public virtual void Insert(EntityType entity, DbContextType context)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (context == null) throw new ArgumentNullException("context");
            if (entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is already in the database.");

            try
            {
                OnInsertEntity(context, entity);
                ObjectContext objectContext = ((IObjectContextAdapter) context).ObjectContext;
                objectContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Inserts the specified Entities in Bulk in a Transactional Operation.
        /// </summary>
        /// <param name="entities">The Entities to insert in the Data Source.</param>
        public override void BulkUpsert(IEnumerable<EntityType> entities)
        {
            if (entities == null) throw new ArgumentNullException("entities");

            using (TransactionWrapper transaction = TransactionWrapper.Create())
            {
                using (DbContextType context = CreateDbContext())
                {
                    entities.ForEach(delegate(EntityType entity)
                    {
                        if (!entity.IsReadFromDatabase)
                        {
                            Insert(entity, context);
                        }
                        else
                        {
                            Update(entity, context);
                        }
                    });
                    transaction.Complete();
                }
            }
        }

        private void OnInsertEntity(DbContextType context, EntityType entity)
        {
            if (!entity.IsReadFromDatabase)
            {
                DbEntityEntry<EntityType> entityEntry = context.Entry<EntityType>(entity);
                entityEntry.State = EntityState.Added;

                OnUpsertEntity(context, entity);
            }
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
            if (!entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is not in the database.");

            if (entity.IsModified)
            {
                using (DbContextType dbContext = CreateDbContext())
                {
                    Update(entity, dbContext);
                }
            }
        }

        /// <summary>
        /// Updates the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Update.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to update an Entity that does not exist in the Data Source.
        /// </exception>        
        public virtual void Update(EntityType entity, DbContextType context)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (context == null) throw new ArgumentNullException("context");
            if (!entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is not in the database.");

            if (entity.IsModified)
            {
                DbEntityEntry<EntityType> entityEntry = context.Entry<EntityType>(entity);

                /*
                var localEntries = dbContext.Set<EntityType>().Local;
                ObjectStateManager objectStateManager = dbContext.GetObjStateManager();
                var trackedEntities =  dbContext.ChangeTracker.Entries<EntityType>();
                //var obj = objectStateManager.GetObjectStateEntry(entity);

                DbEntityEntry<EntityType> trackedEntity;
                if ((entity is IKeyedEntity) && dbContext.ChangeTracker.TryFindTrackedEntity<EntityType>((IKeyedEntity)entity, out trackedEntity))
                {
                    trackedEntity.CurrentValues.SetValues(entity);
                }
                else
                {
                    //entityEntry.State = EntityState.Unchanged;
                }
                */

                try
                {
                    entityEntry.State = EntityState.Modified;
                    OnUpsertEntity(context, entity);
                    context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Executes after current values have been updated or Entity has been added to State Manager, 
        /// and before Changes are saved to the Data Source.
        /// </summary>
        /// <param name="context">The ObjectContext instance managing the Update Operation.</param>
        /// <param name="entity">The Entity being updated.</param>
        protected virtual void OnUpsertEntity(DbContextType context, EntityType entity)
        {
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

            if (entity.IsReadFromDatabase)
            {
                using (var context = CreateDbContext())
                {
                    DbEntityEntry<EntityType> entityEntry = context.Entry<EntityType>(entity);
                    entityEntry.State = EntityState.Deleted;
                    context.SaveChanges();
                }
            }
        }

        #endregion

        #region Retrieval
        /// <summary>
        /// Retrieves all Entities in the Data Source.
        /// </summary>
        public override List<EntityType> All()
        {
            using (var context = CreateDbContext())
            {
                return context.Set<EntityType>().AsNoTracking<EntityType>().ToList();
            }
        }

        /// <summary>
        /// Retrieves all Entities in the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override List<EntityType> Many(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            using (var context = CreateDbContext())
            {
                return context.Set<EntityType>().Where(predicate).AsNoTracking<EntityType>().ToList();
            }
        }

        /// <summary>
        /// Retrieves a single Entity from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public override EntityType Single(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            using (var context = CreateDbContext())
            {
                return context.Set<EntityType>().Where(predicate).AsNoTracking<EntityType>().FirstOrDefault();
            }
        }

        /// <summary>
        /// Retrieves an Entity from the Data Source for the specified Primary Key.
        /// </summary>
        /// <param name="primaryKey">The Primary Key that indentifies the Entity.</param>
        public override EntityType WithKey<EntityKey>(EntityKey primaryKey)
        {
            using (var context = CreateDbContext())
            {
                EntityType entity = context.Set<EntityType>().Find(primaryKey);
                if (entity != null)
                {
                    OnPopulateEntity(context, entity);
                }

                return entity;
            }
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

            pageSettings = pageSettings ?? new SearchPageSettings();

            using (var context = CreateDbContext())
            {
                var query = context.Set<EntityType>().Where(predicate);
                if (sort != null)
                {
                    query = query.OrderBy(sort);
                }
                pageSettings.Calculate(query);

                return query.Skip(pageSettings.Skip).Take(pageSettings.PageSize).AsNoTracking().ToList();
            }
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

            pageSettings = pageSettings ?? new SearchPageSettings();

            using (var context = CreateDbContext())
            {
                IQueryable<EntityType> queryable = context.Set<EntityType>();
                query.Queries.ForEach(predicate => queryable = queryable.Where(predicate));
                if (sort != null)
                { 
                    queryable = queryable.OrderBy(sort); 
                }

                if (query.Predicates.Any())
                {
                    IEnumerable<EntityType> enumerator = queryable;
                    query.Predicates.ForEach(predicate => enumerator = enumerator.Where(predicate));

                    pageSettings.Calculate(enumerator);
                    return enumerator.Skip(pageSettings.Skip).Take(pageSettings.PageSize).ToList();
                }
                else
                {
                    pageSettings.Calculate(queryable);
                    return queryable.Skip(pageSettings.Skip).Take(pageSettings.PageSize).AsNoTracking().ToList();
                }
            }
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

        /// <summary>
        /// Executes after Entity is Retrieved from the Data Source.
        /// Use this to populate values that do not originate from the Data Source.
        /// </summary>
        /// <param name="context">The ObjectContext instance managing the Operation.</param>
        /// <param name="entity">The Entity to populate.</param>
        protected virtual void OnPopulateEntity(DbContextType context, EntityType entity)
        {
        }

        #endregion

        #region Sync
        internal override void SyncManyToMany<CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> navigationProperty
            , bool cascadeDelete = false)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (navigationProperty == null) throw new ArgumentNullException("navigationProperty");

            string navigationPropertyName = ((MemberExpression)navigationProperty.Body).Member.Name;

            using (var context = CreateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<CollectionType>(navigationProperty).Query();
                var relatedEntities = query.ToList();

                ObjectStateManager objectStateManager = context.GetObjStateManager();

                var collection = (EntityCollectionBase<CollectionType>) navigationProperty.Compile().Invoke(entity);
                foreach (CollectionType childEntity in collection)
                {
                    if (childEntity.IsModified)
                    {
                        DAO.For<CollectionType>().Upsert(childEntity);                        
                    }

                    if (!relatedEntities.Any(match => match.Equals(childEntity)))
                    {
                        objectStateManager.ChangeRelationshipState(entity, childEntity, navigationPropertyName, EntityState.Added);                    
                    }
                }

                context.SaveChanges();

                if (collection.DeletedItems.Any())
                {
                    foreach (CollectionType deletedEntity in collection.DeletedItems)
                    {
                        var deleted = context.Set<CollectionType>().SafeAttach(deletedEntity);
                        if (relatedEntities.Any(match => match.Equals(deleted)))
                        {
                            objectStateManager.ChangeRelationshipState(entity, deleted, navigationPropertyName, EntityState.Deleted);
                        }
                    }

                    context.SaveChanges();
                }

                if (cascadeDelete && collection.DeletedItems.Any())
                {
                    collection.DeletedItems.ForEach(deletedItem => DAO.For<CollectionType>().Delete(deletedItem));
                    context.SaveChanges();
                }                
            }
        }

        #endregion

        #region Related
        internal override List<RelatedType> GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            using (var context = CreateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<RelatedType>(navigationProperty).Query().AsNoTracking();
                return query.ToList();
            }
        }

        internal override List<RelatedType> GetRelated<RelatedType, TProperty>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty
            , Expression<Func<RelatedType, TProperty>> include)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            using (var context = CreateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<RelatedType>(navigationProperty).Query()
                    .Include(include)                    
                    .AsNoTracking();
                return query.ToList();
            }
        }

        internal override RelatedType GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> navigationProperty)
        {
            using (var context = CreateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Reference<RelatedType>(navigationProperty).Query().AsNoTracking();
                return query.SingleOrDefault();
            }
        }

        internal override void Load<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
        {
            using (var context = CreateDbContext())
            {
                var entries = context.ChangeTracker.Entries();

                try
                {
                    context.Set<EntityType>().SafeAttach(entity);
                }
                catch
                {
                    context.Entry<EntityType>(entity).CurrentValues.SetValues(entity);
                }
                finally
                {
                    context.Entry<EntityType>(entity).Collection<RelatedType>(navigationProperty).Load();
                }

                context.Entry<EntityType>(entity).State = EntityState.Detached;

                context.Dispose();
            }
        }

        #endregion

        #region DbContext
        /// <summary>
        /// Creates the Object Context instance for the Connection String.
        /// </summary>
        protected virtual DbContextType CreateDbContext()
        {
            var dbContext = Activator.CreateInstance<DbContextType>();
            if (ConnectionStringSettings != null)
            {
                dbContext.Database.Connection.ConnectionString = ConnectionStringSettings.ConnectionString;
            }

            ObjectContext objectContext = ((IObjectContextAdapter) dbContext).ObjectContext;
            objectContext.ObjectMaterialized += new ObjectMaterializedEventHandler(Context_ObjectMaterialized);
            objectContext.SavingChanges += new EventHandler(Context_SavingChanges);
            objectContext.CommandTimeout = 120;

            return dbContext;
        }

        void Context_SavingChanges(object sender, EventArgs e)
        {
            ObjectContext objectContext = (ObjectContext) sender;

            var changedEntities = objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified);
            if (!changedEntities.Any())
            { return; }

            string identityName = UserIdentity.GetIdentityName();

            foreach (ObjectStateEntry changedEntity in changedEntities)
            {
                if (changedEntity.IsRelationship)
                {
                    continue;
                }

                EntityBase entity = changedEntity.Entity as EntityBase;                
                if (entity != null && !entity.IsModified)
                {
                    changedEntity.ChangeState(EntityState.Unchanged);
                    continue;
                }

                if (entity != null)
                {
                    EntityState entityState = entity.IsReadFromDatabase ? EntityState.Modified : EntityState.Added;
                    if (changedEntity.State != entityState)
                    { changedEntity.ChangeState(entityState); }
                }

                if (changedEntity.Entity is IAuditable)
                {
                    IAuditable auditableEntity = (IAuditable) changedEntity.Entity;
                    if (changedEntity.State == EntityState.Added)
                    {
                        DateTime dt = DateTime.UtcNow;
                        auditableEntity.CreatedBy = identityName;
                        auditableEntity.CreatedDateTime = dt;
                        auditableEntity.UpdatedBy = identityName;
                        auditableEntity.UpdatedDateTime = dt;
                    }
                    else if (changedEntity.State == EntityState.Modified)
                    {
                        auditableEntity.UpdatedBy = identityName;
                        auditableEntity.UpdatedDateTime = DateTime.UtcNow;
                    }
                    else
                    { }
                }
                
                if (entity != null)
                {
                    entity.IsModified = false;
                    entity.IsReadFromDatabase = true;
                }
            }
        }

        void Context_ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            if (e.Entity is EntityBase)
            {
                EntityBase entity = (EntityBase)e.Entity;
                entity.IsReadFromDatabase = true;
                entity.IsModified = false;

                DbContextType dbContext = sender as DbContextType;
                if (entity is EntityType)
                {
                    OnPopulateEntity(dbContext, (EntityType)entity);
                }
            }
        }

        #endregion
    }
}
