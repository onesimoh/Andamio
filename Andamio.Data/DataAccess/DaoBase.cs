using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Configuration;
using System.Security.Principal;
using System.Linq.Expressions;

using Andamio;
using Andamio.Data;
using Andamio.Data.Entities;
using Andamio.Data.Exceptions;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Base class for all Data Access Objects.
    /// </summary>
    public abstract class DaoBase : IDisposable
    {
        #region Constructors, Destructor
        /// <summary>
        /// Class destructor for finalization code, this method will run only if Dispose method is called by GC. It gives the base 
        /// class and derived classes the opportunity to run any clean up code.
        /// </summary>
        ~DaoBase()
        {
            // Although a situation in which Cleanup() executes twice should not occur since
            // GC.SuppressFinalize() is being called from Dispose(), a check is made to only
            // run Cleanup() if object has not been Disposed earlier.
            if (!IsDisposed)
            { Cleanup(); }
        }
        #endregion

        #region Exceptions
        /// <summary>
        /// Encapsulates the supplied Exception into inner exception of a DataAccessException.
        /// </summary>
        /// <param name="message">A message to associate with exception to throw.</param>
        /// <param name="e">The exception to throw.</param>
        protected void ThrowDataAccessException(string message, Exception e)
        {
            if (e is DataAccessException)
            { throw e; }
            throw new DataAccessException(message, e);
        }

        /// <summary>
        /// Encapsulates the supplied Exception into inner exception of a DataAccessException.
        /// </summary>
        /// <param name="e">The exception to throw.</param>
        protected virtual void ThrowDataAccessException(Exception e)
        {
            ThrowDataAccessException(e.Message, e);
        }

        #endregion

        #region Settings
        /// <summary>
        /// Returns a DataProvider specific for that database to access.
        /// </summary>
        /// <returns>The DataProvider for the Database.</returns>
        public virtual DbConnectionStringSettings ConnectionStringSettings { get; set; }

        /// <summary>
        ///  Gets or sets the timeout value, in seconds, for all object context operations.
        ///   A null value indicates that the default value of the underlying provider will be used.
        /// </summary>
        public virtual Nullable<int> CommandTimeout { get; set; }

        #endregion

        #region IDisposable Members, Cleanup()
        private bool _disposed = false;
        private readonly object _disposeSyncLock = new object();

        /// <summary>
        /// Gets a bool value which determines if Disposed method has been called by either GC or client code.
        /// </summary>
        public bool IsDisposed
        {
            get { lock (_disposeSyncLock) { return _disposed; } }
        }

        /// <summary>
        /// This method may be called by the client or ultimately will be executed by the GC.
        /// </summary>
        /// <remarks>
        /// Calling Dispose requests that the object is taken off Finalization queue which prevents Finalization code from executing twice.
        /// </remarks>
        public void Dispose()
        {
            if (_disposed)
            { return; }

            lock (_disposeSyncLock)
            {
                if (!_disposed)
                {
                    // Request object is taken off Finalization Queue
                    // this prevents Finalization code from executing twice
                    GC.SuppressFinalize(this);

                    Cleanup();
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// Inheritors will implement this method if any clean up is necessary, this method will run once when the class is disposed 
        /// either by the client or GC.
        /// </summary>
        protected virtual void Cleanup()
        {
        }
        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="EntityType"></typeparam>
    public abstract class DaoBase<EntityType> : DaoBase
        where EntityType : EntityBase, new()
    {
        #region Update\Insert
        /// <summary>
        /// Updates or Insert the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Insert or Update.</param>
        public virtual void Upsert(EntityType entity)
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
        public abstract void Insert(EntityType entity);

        /// <summary>
        /// Inserts the specified Entities in Bulk in a Transactional Operation.
        /// </summary>
        /// <param name="entities">The Entities to insert in the Data Source.</param>
        public abstract void BulkUpsert(IEnumerable<EntityType> entities);

        /// <summary>
        /// Updates the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Update.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to update an Entity that does not exist in the Data Source.
        /// </exception>        
        public abstract void Update(EntityType entity);

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified Entity from the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to delete from the Data Source.</param>
        public abstract void Delete(EntityType entity);

        #endregion

        #region Retrieval
        /// <summary>
        /// Retrieves all Entities in the Data Source.
        /// </summary>
        public abstract List<EntityType> All();

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public abstract List<EntityType> Many(Expression<Func<EntityType, bool>> predicate);

        /// <summary>
        /// Retrieves a single Entity from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public abstract EntityType Single(Expression<Func<EntityType, bool>> predicate);

        /// <summary>
        /// Retrieves an Entity from the Data Source for the specified Primary Key.
        /// </summary>
        /// <param name="primaryKey">The Primary Key that indentifies the Entity.</param>
        public abstract EntityType WithKey<EntityKey>(EntityKey primaryKey)
            where EntityKey : struct, IComparable<EntityKey>;

        #endregion

        #region Search
        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public abstract List<EntityType> Search<SortKey>(Expression<Func<EntityType, bool>> predicate
            , Expression<Func<EntityType, SortKey>> sort
            , SearchPageSettings pageSettings = null);

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public abstract List<EntityType> Search<SortKey>(DaoQuery<EntityType> query
            , Expression<Func<EntityType, SortKey>> sort
            , SearchPageSettings pageSettings = null);

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public abstract DaoQuery<EntityType> Query(Expression<Func<EntityType, bool>> predicate);

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        public abstract DaoQuery<EntityType> Query();

        #endregion

        #region Entity
        public DaoEntity<EntityType> Entity(EntityType entity)
        {
            return new DaoEntity<EntityType>(this, entity);
        }

        #endregion

        #region Sync
        internal abstract void SyncManyToMany<CollectionType>(EntityType entity
            , Expression<Func<EntityType, ICollection<CollectionType>>> navigationProperty
            , bool cascadeDelete = true)
            where CollectionType : EntityBase, new();

        public virtual void SyncMany(EntityCollectionBase<EntityType> collection)
        {
            if (collection == null) return;
            collection.DeletedItems.ForEach(deletedEntity => DAO.For<EntityType>().Delete(deletedEntity));
            collection.ForEach(entity => DAO.For<EntityType>().Upsert(entity));
            collection.Reset();
        }

        #endregion

        #region Related
        internal abstract List<RelatedType> GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
            where RelatedType : EntityBase;

        internal abstract List<RelatedType> GetRelated<RelatedType, TProperty>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty
            , Expression<Func<RelatedType, TProperty>> include)
            where RelatedType : EntityBase;

        internal abstract RelatedType GetRelated<RelatedType>(EntityType entity
            , Expression<Func<EntityType, RelatedType>> navigationProperty)
            where RelatedType : EntityBase;

        internal abstract void Load<RelatedType>(EntityType entity
            , Expression<Func<EntityType, ICollection<RelatedType>>> navigationProperty)
            where RelatedType : EntityBase;

        #endregion
    }
}
