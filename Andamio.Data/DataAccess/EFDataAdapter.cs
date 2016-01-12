using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Reflection;

using Andamio;
using Andamio.Security;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Entity Framework Data Storage.
    /// </summary>
    public class EFDataAdapter : IDataAdapter
    {
        #region Constructor
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public EFDataAdapter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public EFDataAdapter(Func<DbContext> contextResolver)
        {
            if (contextResolver == null) throw new ArgumentNullException("contextResolver");
            _ContextResolver = contextResolver;
        }

        #endregion

        #region Settings
        /// <summary>
        ///  Gets or sets the timeout value, in seconds, for all object context operations.
        ///   A null value indicates that the default value of the underlying provider will be used.
        /// </summary>
        public virtual Nullable<int> CommandTimeout { get; set; }

        #endregion

        #region Update\Insert
        /// <summary>
        /// Inserts the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Insert.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to insert an Entity that already exists in the Data Source.
        /// </exception>
        public virtual void Insert<EntityType>(EntityType entity)
            where EntityType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is already in the database.");

            using (var context = InstantiateDbContext())
            {
                try
                {
                    DbEntityEntry<EntityType> entityEntry = context.Entry<EntityType>(entity);
                    entityEntry.State = EntityState.Added;

                    OnUpsertEntity(context, entity);

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
        }

        /// <summary>
        /// Updates the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Update.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to update an Entity that does not exist in the Data Source.
        /// </exception>        
        public virtual void Update<EntityType>(EntityType entity)
            where EntityType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is not in the database.");

            if (entity.IsModified)
            {
                using (var context = InstantiateDbContext())
                {
                    DbEntityEntry<EntityType> entityEntry = context.Entry<EntityType>(entity);

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
        }

        /// <summary>
        /// Executes after current values have been updated or Entity has been added to State Manager, 
        /// and before Changes are saved to the Data Source.
        /// </summary>
        /// <param name="context">The ObjectContext instance managing the Update Operation.</param>
        /// <param name="entity">The Entity being updated.</param>
        protected virtual void OnUpsertEntity<EntityType>(DbContext context, EntityType entity)
            where EntityType : EntityBase, new()
        {
        }

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified Entity from the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to delete from the Data Source.</param>
        public virtual void Delete<EntityType>(EntityType entity)
            where EntityType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (entity.IsReadFromDatabase)
            {
                using (var context = InstantiateDbContext())
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
        public virtual IQueryable<EntityType> Query<EntityType>()
            where EntityType : EntityBase, new()
        {
            var context = InstantiateDbContext();
            return context.Set<EntityType>().AsNoTracking<EntityType>();
        }

        /// <summary>
        /// Retrieves an Entity from the Data Source for the specified Primary Key.
        /// </summary>
        /// <param name="primaryKey">The Primary Key that indentifies the Entity.</param>
        public virtual EntityType WithKey<EntityType, EntityKey>(EntityKey primaryKey)
            where EntityType : EntityBase, new()
            where EntityKey : struct, IComparable<EntityKey>
        {
            using (var context = InstantiateDbContext())
            {
                EntityType entity = context.Set<EntityType>().Find(primaryKey);
                if (entity != null)
                {
                    OnPopulateEntity(context, entity);
                }

                return entity;
            }
        }

        /// <summary>
        /// Executes after Entity is Retrieved from the Data Source.
        /// Use this to populate values that do not originate from the Data Source.
        /// </summary>
        /// <param name="context">The ObjectContext instance managing the Operation.</param>
        /// <param name="entity">The Entity to populate.</param>
        protected virtual void OnPopulateEntity<EntityType>(DbContext context, EntityType entity)
            where EntityType : EntityBase, new()
        {
        }

        #endregion

        #region Sync
        public virtual void SyncManyToMany<EntityType, CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> property
            , bool cascadeDelete = false)
            where EntityType : EntityBase, new()
            where CollectionType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (property == null) throw new ArgumentNullException("property");

            string propertyName = ((MemberExpression) property.Body).Member.Name;

            using (var context = InstantiateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<CollectionType>(property).Query();
                var relatedEntities = query.ToList();

                ObjectStateManager objectStateManager = context.GetObjStateManager();

                var collection = (EntityCollectionBase<CollectionType>) property.Compile().Invoke(entity);

                foreach (CollectionType childEntity in collection)
                {
                    if (childEntity.IsModified)
                    {
                        DAO.For<CollectionType>().Upsert(childEntity);
                    }

                    if (!relatedEntities.Any(match => match.Equals(childEntity)))
                    {
                        objectStateManager.ChangeRelationshipState(entity, childEntity, propertyName, EntityState.Added);
                    }
                }

                if (collection.DeletedItems.Any())
                {
                    foreach (CollectionType deletedEntity in collection.DeletedItems)
                    {
                        var deleted = context.Set<CollectionType>().SafeAttach(deletedEntity);
                        if (relatedEntities.Any(match => match.Equals(deleted)))
                        {
                            objectStateManager.ChangeRelationshipState(entity, deleted, propertyName, EntityState.Deleted);
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
        public virtual IEnumerable<RelatedType> Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> property)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (property == null) throw new ArgumentNullException("property");

            using (var context = InstantiateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<RelatedType>(property).Query().AsNoTracking();
                return query.ToList();
            }
        }

        public virtual IEnumerable<RelatedType> Related<EntityType, RelatedType, TProperty>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> property
            , Expression<Func<RelatedType, TProperty>> include)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (property == null) throw new ArgumentNullException("property");

            using (var context = InstantiateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Collection<RelatedType>(property).Query()
                    .Include(include)
                    .AsNoTracking();
                return query.ToList();
            }
        }

        public virtual RelatedType Related<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> property)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (property == null) throw new ArgumentNullException("property");

            using (var context = InstantiateDbContext())
            {
                context.Set<EntityType>().Attach(entity);
                var query = context.Entry<EntityType>(entity).Reference<RelatedType>(property).Query().AsNoTracking();
                return query.SingleOrDefault();
            }
        }

        public virtual void Load<EntityType, RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> property)
            where EntityType : EntityBase, new()
            where RelatedType : EntityBase, new()
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (property == null) throw new ArgumentNullException("property");

            using (var context = InstantiateDbContext())
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
                    context.Entry<EntityType>(entity).Collection<RelatedType>(property).Load();
                }

                context.Entry<EntityType>(entity).State = EntityState.Detached;

                context.Dispose();
            }
        }

        #endregion

        #region DbContext
        private readonly Func<DbContext> _ContextResolver;

        /// <summary>
        /// Creates the Object Context instance for the Connection String.
        /// </summary>
        protected virtual DbContext InstantiateDbContext()
        {
            DbContext dbContext;
            if (_ContextResolver == null)
            {
                Assembly asm = Assembly.GetEntryAssembly();
                var allDbContexts = asm.GetTypes().Where(type => type.DerivesFromType<DbContext>());
                if (!allDbContexts.Any())
                {
                    throw new InvalidOperationException();
                }
                if (allDbContexts.Count() > 1)
                {
                    throw new InvalidOperationException();
                }

                dbContext = (DbContext) Activator.CreateInstance(allDbContexts.First());
            }
            else
            {
                dbContext = _ContextResolver();
            }

            if (dbContext == null)
            {
                throw new Exception("A DbContext could not be resolved!");
            }

            if (dbContext.Database.Connection.ConnectionString.IsNullOrBlank())
            {
                throw new InvalidOperationException("A valid Connection String has not been configured.");
            }

            ObjectContext objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
            objectContext.ObjectMaterialized += new ObjectMaterializedEventHandler(Context_ObjectMaterialized);
            objectContext.SavingChanges += new EventHandler(Context_SavingChanges);
            objectContext.CommandTimeout = CommandTimeout;

            return dbContext;
        }

        void Context_SavingChanges(object sender, EventArgs e)
        {
            ObjectContext objectContext = (ObjectContext)sender;

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
                    {
                        changedEntity.ChangeState(entityState);
                    }
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
                EntityBase entity = (EntityBase) e.Entity;
                entity.IsReadFromDatabase = true;
                entity.IsModified = false;

                DbContext dbContext = sender as DbContext;
                //if (entity is EntityType)
                //{
                //    OnPopulateEntity(dbContext, (EntityType) entity);
                //}
            }
        }

        #endregion
    }
}
