using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;

using Andamio;
using Andamio.Data;
using Andamio.Data.Entities;

namespace Andamio.Data
{
    /// <summary>
    /// Contains several Entity Framework Extension Methods.
    /// </summary>
    public static class EntityFrameworkExtensions
    {
        #region Object Context
        /// <summary>
        /// Creates a new ObjectSet instance while specifying Merge Options.
        /// </summary>
        /// <typeparam name="T">Entity type of the requested ObjectSet.</typeparam>
        /// <param name="context">The ObjectContext instance for which to create the ObjectSet.</param>
        /// <param name="mergeOption">Specifies how objects returned from a query are added to the object context.</param>
        public static ObjectSet<T> CreateObjectSet<T>(this ObjectContext context, MergeOption mergeOption = MergeOption.NoTracking)
            where T : class
        {
            ObjectSet<T> objectSet = context.CreateObjectSet<T>();
            objectSet.MergeOption = mergeOption;
            return objectSet;
        }

        public static ObjectStateManager GetObjStateManager(this DbContext dbContext)
        {
            ObjectStateManager objectStateManager = ((IObjectContextAdapter) dbContext).ObjectContext.ObjectStateManager;
            return objectStateManager;
        }

        #endregion

        #region Related Entities
        /// <summary>
        /// Gets all Related Entities for the specified Related End while specifying Merge Options.
        /// </summary>
        /// <typeparam name="T">The Type of the Related Entities.</typeparam>
        /// <param name="relatedEnd">Entity Framework Related End.</param>
        /// <param name="mergeOption">Specifies how objects returned from a query are added to the object context.</param>
        public static IEnumerable<T> GetRelatedEntities<T>(this IRelatedEnd relatedEnd, MergeOption mergeOption)
            where T : class
        {
            IEnumerable query = relatedEnd.CreateSourceQuery();
            ((ObjectQuery) query).MergeOption = mergeOption;
            return query.OfType<T>();
        }

        /// <summary>
        /// Gets all Related Entities for the specified Related End and configures No Tracking Merge Options.
        /// </summary>
        /// <typeparam name="T">The Type of the Related Entities.</typeparam>
        /// <param name="relatedEnd">Entity Framework Related End.</param>
        public static IEnumerable<T> GetRelatedEntitiesWithNoTracking<T>(this IRelatedEnd relatedEnd)
            where T : class
        {
            return relatedEnd.GetRelatedEntities<T>(MergeOption.NoTracking);
        }

        /// <summary>
        /// Returns the Related End for the specified target role in a relationship.
        /// </summary>
        /// <param name="entity">The Entity (With Relationships) for which to get the Related End.</param>
        /// <param name="relationshipName">Name of the relationship in which targetRoleName is defined.</param>
        /// <param name="targetRoleName">Target role to use to retrieve the other end of relationshipName.</param>
        public static IRelatedEnd GetRelatedEnd(this IEntityWithRelationships entity, string relationshipName, string targetRoleName)
        {
            return entity.RelationshipManager.GetRelatedEnd(relationshipName, targetRoleName);
        }

        /// <summary>
        /// Returns all Related Ends managed by the relationship manager for the specified Entity.
        /// </summary>
        /// <param name="entity">The Entity (With Relationships) for which to get all Related Ends.</param>
        public static IEnumerable<IRelatedEnd> GetAllRelatedEnds(this IEntityWithRelationships entity)
        {
            return entity.RelationshipManager.GetAllRelatedEnds();
        }

        /// <summary>
        /// Gets a Related Entities Collection for the specified Relationship Name and Target Role.
        /// </summary>
        /// <typeparam name="T">The Type of the Related Entities.</typeparam>
        /// <param name="entity">The Entity (With Relationships) for which to get the Related Entities.</param>
        /// <param name="relationshipName">Name of the relationship in which targetRoleName is defined.</param>
        /// <param name="targetRoleName">Target role to use to retrieve the other end of relationshipName.</param>
        public static List<T> GetRelatedCollection<T>(this IEntityWithRelationships entity, string relationshipName, string targetRoleName)
            where T : class
        {
            IRelatedEnd relatedEnd = entity.GetRelatedEnd(relationshipName, targetRoleName);
            return relatedEnd.GetRelatedEntitiesWithNoTracking<T>().ToList();
        }

        #endregion

        #region Sync Entity Relationships
        /// <summary>
        /// Synchronizes specified Entities for the Related End based on Entity ID.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entities to Synchronize.</typeparam>
        /// <typeparam name="EntityKey">The Entity Key Type (ex. Guid, Int32, etc...)</typeparam>
        /// <param name="relatedEnd">The Related End for which to synchronize the Entities.</param>
        /// <param name="entities">The Entities to Synchronize.</param>
        public static void SyncRelatedEntities<EntityType, EntityKey>(this IRelatedEnd relatedEnd
            , EntityCollection<EntityType, EntityKey> entities)
            where EntityType : SimpleKeyEntity<EntityKey>
            where EntityKey : struct, IComparable<EntityKey>
        {
            EntityIdComparer<EntityType, EntityKey> comparer = new EntityIdComparer<EntityType, EntityKey>();
            relatedEnd.SyncRelatedEntities(entities, comparer);
        }

        /// <summary>
        /// Synchronizes specified Entities for the Related End based the specified Equality Comparer.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entities to Synchronize.</typeparam>
        /// <param name="relatedEnd">The Related End for which to synchronize the Entities.</param>
        /// <param name="entities">The Entities to Synchronize.</param>
        /// <param name="comparer">The IEqualityComparer to use for the Synchronization.</param>
        public static void SyncRelatedEntities<EntityType>(this IRelatedEnd relatedEnd
            , EntityCollectionBase<EntityType> entities
            , IEqualityComparer<EntityType> comparer)
            where EntityType : EntityBase
        {
            if (relatedEnd == null) throw new ArgumentNullException("relatedEnd");
            if (entities == null) return;
            
            // Add related Entities to Related End instance.
            var relatedEntities = relatedEnd.CreateQuery<EntityType>().ToArray();            
            foreach (EntityType entity in entities)
            {
                if (!relatedEntities.Contains(entity, comparer))
                {
                    relatedEnd.Add(entity);
                }
            }

            // Delete all removed Entities from Related End.
            if (entities.DeletedItems.Any())
            {
                var deletedItems = relatedEntities.Where(match => entities.DeletedItems.Contains(match, comparer)).ToArray();
                if (deletedItems.Any())
                {
                    relatedEnd.Load(MergeOption.OverwriteChanges);
                    deletedItems.ForEach(deletedItem => relatedEnd.Remove(deletedItem));
                }
            }
        }

        /// <summary>
        /// Adds the specified Entity to the Related End based on the provided Equality Comparer.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entity to add to the relationship.</typeparam>
        /// <param name="relatedEnd">The Related End for which to add the Entity.</param>
        /// <param name="entity">The Entity to Add to the Related End.</param>
        /// <param name="comparer">The comparer to test whether the specified Entity is already in the Relationship.</param>
        public static void AddRelatedEntity<EntityType>(this IRelatedEnd relatedEnd
            , EntityType entity
            , IEqualityComparer<EntityType> comparer)
            where EntityType : EntityBase
        {
            if (relatedEnd == null) throw new ArgumentNullException("relatedEnd");
            if (entity == null) throw new ArgumentNullException("entity");

            var relatedEntities = relatedEnd.GetRelatedEntitiesWithNoTracking<EntityType>();
            if (!relatedEntities.Contains(entity, comparer))
            {
                relatedEnd.Add(entity);
            }
        }

        /// <summary>
        /// Removes the specified Entity to the Related End based on the provided Equality Comparer.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entity to Remove from the relationship.</typeparam>
        /// <param name="relatedEnd">The Related End for which to Remove the Entity.</param>
        /// <param name="entity">The Entity to Remove from the Related End.</param>
        /// <param name="comparer">The comparer to test whether the specified Entity is in the Relationship.</param>
        public static void RemoveRelatedEntity<EntityType>(this IRelatedEnd relatedEnd
            , EntityType entity
            , IEqualityComparer<EntityType> comparer)
            where EntityType : EntityBase
        {
            if (relatedEnd == null) throw new ArgumentNullException("relatedEnd");
            if (entity == null) throw new ArgumentNullException("entity");

            var relatedEntities = relatedEnd.GetRelatedEntitiesWithNoTracking<EntityType>();
            if (relatedEntities.Contains(entity, comparer))
            {
                relatedEnd.Load();
                relatedEnd.Remove(entity);
            }
        }

        /// <summary>
        /// Adds the specified Entity to the Related End.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entity to add to the relationship.</typeparam>
        /// <typeparam name="EntityKey">The Entity Key Type.</typeparam>
        /// <param name="relatedEnd">The Related End for which to add the Entity.</param>
        /// <param name="entity">The Entity to Add to the Related End.</param>
        public static void AddRelatedEntity<EntityType, EntityKey>(this IRelatedEnd relatedEnd, EntityType entity)
            where EntityType : SimpleKeyEntity<EntityKey>
            where EntityKey : struct, IComparable<EntityKey>
        {
            EntityIdComparer<EntityType, EntityKey> comparer = new EntityIdComparer<EntityType, EntityKey>();
            relatedEnd.AddRelatedEntity(entity, comparer);
        }

        /// <summary>
        /// Removes the specified Entity from the Related End.
        /// </summary>
        /// <typeparam name="EntityType">The Type of Entity to remove from the relationship.</typeparam>
        /// <typeparam name="EntityKey">The Entity Key Type.</typeparam>
        /// <param name="relatedEnd">The Related End for which to remove the Entity.</param>
        /// <param name="entity">The Entity to remove from the Related End.</param>
        public static void RemoveRelatedEntity<EntityType, EntityKey>(this IRelatedEnd relatedEnd, EntityType entity)
            where EntityType : SimpleKeyEntity<EntityKey>
            where EntityKey : struct, IComparable<EntityKey>
        {
            EntityIdComparer<EntityType, EntityKey> comparer = new EntityIdComparer<EntityType, EntityKey>();
            relatedEnd.RemoveRelatedEntity(entity, comparer);
        }

        /// <summary>
        /// Attaches the specified Entities to the ObjectSet.
        /// </summary>
        /// <typeparam name="EntityType">The Entity Type to attach to the ObjectSet.</typeparam>
        /// <typeparam name="EntityKey">The Entity Key Type for the Entity to attache to the ObjectSet.</typeparam>
        /// <param name="objectSet">The ObjectSet to which attach the Entity.</param>
        /// <param name="entities">The Entities to attach to the ObjectSet.</param>
        public static EntityCollection<EntityType, EntityKey> AttachEntities<EntityType, EntityKey>(this ObjectSet<EntityType> objectSet
            , EntityCollection<EntityType, EntityKey> entities)
            where EntityType : SimpleKeyEntity<EntityKey>
            where EntityKey : struct, IComparable<EntityKey>
        {
            if (objectSet == null) throw new ArgumentNullException("objectSet");

            EntityCollection<EntityType, EntityKey> attachedEntities = new EntityCollection<EntityType, EntityKey>();

            if (entities != null)
            {
                if (entities.Any())
                {
                    for (int i = 0; i < entities.Count; i++)
                    {
                        EntityType entity = entities[i];
                        objectSet.SafeAttach(ref entity);
                        attachedEntities.Add(entity);
                    }
                }

                if (entities.DeletedItems.Any())
                {
                    EntityType[] deletedEntities = entities.DeletedItems.ToArray();
                    for (int i = 0; i < deletedEntities.Length; i++)
                    {
                        EntityType deletedEntity = deletedEntities[i];
                        objectSet.SafeAttach(ref deletedEntity);
                        attachedEntities.DeletedItems.Add(deletedEntity);
                    }
 
                    entities.DeletedItems.ForEach(deletedEntity => objectSet.SafeAttach(ref deletedEntity)); 
                }
            }

            return attachedEntities;
        }

        /// <summary>
        /// Verifies whether specified Entity is already attached to the ObjectSet, if it is, it applies the new values;
        /// otherwise it attaches the Entity to the ObjectSet.
        /// </summary>
        /// <typeparam name="EntityType">The Entity Type to attach to the ObjectSet.</typeparam>
        /// <param name="objectSet">The ObjectSet to which attach the Entity.</param>
        /// <param name="entity">The Entity to attach to the ObjectSet.</param>
        public static void SafeAttach<EntityType>(this ObjectSet<EntityType> objectSet, ref EntityType entity)
            where EntityType : EntityBase
        {
            ObjectContext context = objectSet.Context;
            ObjectStateManager objectStateManager = context.ObjectStateManager;

            ObjectStateEntry objectStateEntry;
            object entityWithKey;
            if ((entity is IEntityWithKey) && (context.TryGetObjectByKey(((IEntityWithKey) entity).EntityKey, out entityWithKey)
                && objectStateManager.TryGetObjectStateEntry(((IEntityWithKey) entity).EntityKey, out objectStateEntry)))
            {
                objectStateEntry.ApplyCurrentValues(entity);
                entity = (EntityType) objectStateManager.GetObjectStateEntry(((IEntityWithKey)entity).EntityKey).Entity;
            }
            else
            {
                objectSet.Attach(entity);
            }
        }

        /// <summary>
        /// Verifies whether specified Entity is already attached to the ObjectSet, if it is, it applies the new values;
        /// otherwise it attaches the Entity to the ObjectSet.
        /// </summary>
        /// <typeparam name="EntityType">The Entity Type to attach to the ObjectSet.</typeparam>
        /// <param name="objectSet">The ObjectSet to which attach the Entity.</param>
        /// <param name="entity">The Entity to attach to the ObjectSet.</param>
        public static EntityType SafeAttach<EntityType>(this DbSet<EntityType> dbSet, EntityType entity)
            where EntityType : EntityBase
        {
            if (dbSet.Local.Contains(entity))
            {
                return dbSet.Local.FirstOrDefault(match => entity.Equals(match));
            }
            if (entity is IKeyedEntity)
            {
                KeyedEntity keyedEntity = ((IKeyedEntity) entity).EntityKey;
                var localKeyedEntities = dbSet.Local.OfType<IKeyedEntity>().ToArray();
                IKeyedEntity localEntity = localKeyedEntities.FirstOrDefault(match => match.EntityKey == keyedEntity);
                if (localEntity != null)
                {
                    return (EntityType) localEntity;
                }
            }

            return dbSet.Attach(entity);
        } 

        #endregion

        #region Tracking
        /// <summary>
        /// Configures No Tracking Options to all entities in the IQueryable Data Source.
        /// </summary>
        /// <typeparam name="T">The Entity Type for which to set No Tracking Merge Options.</typeparam>
        /// <param name="query">Thr IQueryable object containing the entities for which to set No Tracking Merge Options.</param>
        public static IQueryable<T> NoTracking<T>(this IQueryable<T> query)
            where T : class
        {
            ((ObjectQuery) query).MergeOption = MergeOption.NoTracking;
            return query;
        }

        public static DbEntityEntry<T> FindTrackedEntity<T>(this DbChangeTracker tracker, IEntityWithKey keyedEntity)
            where T : class
        {
            foreach (DbEntityEntry<T> entityEntry in tracker.Entries<T>())
            {
                if (!(entityEntry.Entity is IEntityWithKey))
                { continue; }

                if (((IEntityWithKey) entityEntry.Entity).EntityKey == keyedEntity.EntityKey)
                {
                    return entityEntry;
                }
            }

            return null;
        }

        public static DbEntityEntry<T> FindTrackedEntity<T>(this DbChangeTracker tracker, IKeyedEntity keyedEntity)
            where T : class
        {
            foreach (DbEntityEntry<T> entityEntry in tracker.Entries<T>())
            {
                if (!(entityEntry.Entity is IKeyedEntity))
                { continue; }

                if (((IKeyedEntity)entityEntry.Entity).EntityKey == keyedEntity.EntityKey)
                {
                    return entityEntry;
                }
            }

            return null;
        }

        public static bool TryFindTrackedEntity<T>(this DbChangeTracker tracker, IEntityWithKey keyedEntity, out DbEntityEntry<T> entityEntry)
            where T : class
        {
            entityEntry = tracker.FindTrackedEntity<T>(keyedEntity);
            return (entityEntry != null);
        }

        public static bool TryFindTrackedEntity<T>(this DbChangeTracker tracker, IKeyedEntity keyedEntity, out DbEntityEntry<T> entityEntry)
            where T : class
        {
            entityEntry = tracker.FindTrackedEntity<T>(keyedEntity);
            return (entityEntry != null);
        }

        #endregion

        #region Query
        /// <summary>
        /// Return a IQueryable that represents the Data in the Related End.
        /// </summary>
        /// <typeparam name="T">The Entity type in the Related End.</typeparam>
        /// <param name="relatedEnd">The Related End.</param>
        /// <param name="mergeOption">Specifies how objects returned from a query are added to the object context.</param>
        public static IQueryable<T> CreateQuery<T>(this IRelatedEnd relatedEnd, MergeOption mergeOption)
            where T : class
        {
            var query = relatedEnd.CreateSourceQuery().AsQueryable().OfType<T>();
            ((ObjectQuery) query).MergeOption = mergeOption;
            return query;
        }

        /// <summary>
        /// Return a IQueryable that represents the Data in the Related End.
        /// </summary>
        /// <typeparam name="T">The Entity type in the Related End.</typeparam>
        /// <param name="relatedEnd">The Related End.</param>
        public static IQueryable<T> CreateQuery<T>(this IRelatedEnd relatedEnd)
        {
            return relatedEnd.CreateSourceQuery().AsQueryable().OfType<T>();
        }

        #endregion

        #region Tracing
        /// <summary>
        /// Returns the Sql Command to execute against the Data Source.
        /// This is primarily used for debugging and troubleshooting.
        /// </summary>
        /// <typeparam name="T">The Entity Type that represents the items in the Data Source.</typeparam>
        /// <param name="query">The IQueryable that represents the Data Source.</param>
        public static string TraceString<T>(this IQueryable<T> query)
        {
            ObjectQuery objectQuery = query as ObjectQuery;
            if (objectQuery != null)
            {
                return objectQuery.ToTraceString();
            }

            return String.Empty;
        }

        #endregion
    }
}
