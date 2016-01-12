using System;
using System.Collections.Generic;
using System.Linq;
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
    public class Dao<EntityType> : DaoBase, IDao<EntityType>
        where EntityType : EntityBase, new()
    {
        #region Constructors
        public Dao()
        {
            if (DAO.Configuration.DataAdapter == null)
            { throw new InvalidOperationException("A Default Data Adapter has not been provided."); }
            DataAdapter = DAO.Configuration.DataAdapter;
        }

        public Dao(IDataAdapter dataAdapter)
        {
            if (dataAdapter == null) throw new ArgumentNullException("dataAdapter");
            DataAdapter = dataAdapter;
        }

        #endregion

        #region Properties
        public IDataAdapter DataAdapter { get; set; }

        #endregion

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
        public virtual void Insert(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is already in the database.");
            OnUpdateOrInsertEntity(entity);
            DataAdapter.Insert(entity);
        }

        /// <summary>
        /// Updates the specified Entity in the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to Update.</param>
        /// <exception cref="System.InvalidOperationException">
        /// When attempting to update an Entity that does not exist in the Data Source.
        /// </exception>        
        public virtual void Update(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (!entity.IsReadFromDatabase) throw new InvalidOperationException("Entity is not in the database.");
            OnUpdateOrInsertEntity(entity);
            DataAdapter.Update(entity);
        }

        /// <summary>
        /// Executes after current values have been updated or Entity has been added to State Manager, 
        /// and before Changes are saved to the Data Source.
        /// </summary>
        /// <param name="context">The ObjectContext instance managing the Update Operation.</param>
        /// <param name="entity">The Entity being updated.</param>
        protected virtual void OnUpdateOrInsertEntity(EntityType entity)
        {
        }

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified Entity from the Data Source.
        /// </summary>
        /// <param name="entity">The Entity to delete from the Data Source.</param>
        public virtual void Delete(EntityType entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            DataAdapter.Delete(entity);
        }

        #endregion

        #region Retrieval
        /// <summary>
        /// Retrieves all Entities in the Data Source.
        /// </summary>
        public virtual List<EntityType> All()
        {
            return DataAdapter.Query<EntityType>().ToList();
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public virtual List<EntityType> Many(Expression<Func<EntityType, bool>> predicate)
        {
            return DataAdapter.Query<EntityType>().Where(predicate).ToList();
        }

        /// <summary>
        /// Retrieves a single Entity from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public virtual EntityType Single(Expression<Func<EntityType, bool>> predicate)
        {
            return DataAdapter.Query<EntityType>().Where(predicate).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves an Entity from the Data Source for the specified Primary Key.
        /// </summary>
        /// <param name="primaryKey">The Primary Key that indentifies the Entity.</param>
        public virtual EntityType WithKey<EntityKey>(EntityKey primaryKey)
            where EntityKey : struct, IComparable<EntityKey>
        {
            return DataAdapter.WithKey<EntityType, EntityKey>(primaryKey);
        }

        #endregion

        #region Search
        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public virtual List<EntityType> Search<SortKey>(Expression<Func<EntityType, bool>> predicate
            , Expression<Func<EntityType, SortKey>> sort = null
            , SearchPageSettings pageSettings = null)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");

            pageSettings = pageSettings ?? new SearchPageSettings();
            var query = DataAdapter.Query<EntityType>().Where(predicate);

            if (sort != null)
            { query = query.OrderBy(sort); }

            pageSettings.Calculate(query);
            return query.Skip(pageSettings.Skip).Take(pageSettings.PageSize).ToList();
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public virtual List<EntityType> Search<SortKey>(DaoQuery<EntityType> query
            , Expression<Func<EntityType, SortKey>> sort = null
            , SearchPageSettings pageSettings = null)
        {
            if (query == null) throw new ArgumentNullException("query");
            return Search((Expression<Func<EntityType, bool>> ) query, sort, pageSettings);
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate function to filter the Entities in the Data Source.</param>
        public virtual DaoQuery<EntityType> Query(Expression<Func<EntityType, bool>> predicate)
        {
            DaoQuery<EntityType> query = new DaoQuery<EntityType>(predicate);
            return query;
        }

        /// <summary>
        /// Retrieves all Entities from the Data Source for the specified predicate.
        /// </summary>
        public virtual DaoQuery<EntityType> Query()
        {
            DaoQuery<EntityType> query = new DaoQuery<EntityType>();
            return query;
        }

        #endregion

        #region Entity
        public DaoEntity<EntityType> Entity(EntityType entity)
        {
            return new DaoEntity<EntityType>(DataAdapter, entity);
        }

        #endregion
    }
}
