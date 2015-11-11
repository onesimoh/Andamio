using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Data.Entity;

using Andamio;
using Andamio.Data;
using Andamio.Data.Entities;
using Andamio.Data.Transactions;

namespace Andamio.Data.Access
{
    /// <summary>
    /// Base class for Entity Framework Data Access Objects (DAO) that managed Simple Key Entities.
    /// </summary>
    /// <typeparam name="EntityType">The Entity Type managed by this DAO.</typeparam>
    /// <typeparam name="EntityKey">The Entity Key Type that Identifies the Entities managed by this DAO.</typeparam>
    /// <typeparam name="ObjectContextType">The Object Context to use with this DAO.</typeparam>
    public abstract class EFSimpleKeyDaoBase<EntityType, EntityKey, DbContextType> : EFDaoBase<EntityType, DbContextType>
        where EntityType : SimpleKeyEntity<EntityKey>, new()
        where DbContextType : DbContext
        where EntityKey : struct, IComparable<EntityKey>
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        protected EFSimpleKeyDaoBase()
            : base()
        {
        }

        #endregion

        #region Retrieval
        /// <summary>
        /// Retrieves all Entities with the specified IDs
        /// </summary>
        /// <param name="entityIDs">The IDs of the entities to retrieve.</param>
        public virtual List<EntityType> GetMany(params EntityKey[] entityIDs)
        {
            if (entityIDs == null) throw new ArgumentNullException("entityIDs");

            using (DbContextType context = CreateDbContext())
            {
                return context.Set<EntityType>().Where(match => entityIDs.Contains(match.ID))
                    .AsNoTracking<EntityType>().ToList();
            }
        }

        #endregion

        #region Delete
        public virtual void Delete(EntityKey primaryKey)
        {
            EntityType entity = new EntityType() { ID = primaryKey };
            base.Delete(entity);
        }

        #endregion
    }
}
