using System;
using System.Linq.Expressions;

using Andamio;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public class DaoQuery<EntityType>
        where EntityType : EntityBase, new()
    {
        #region Constructor
        public DaoQuery()
        {
        }

        public DaoQuery(Expression<Func<EntityType, bool>> predicate)
            : this()
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            _Query = predicate;
        }

        #endregion

        #region Conversion
        public static implicit operator Expression<Func<EntityType, bool>>(DaoQuery<EntityType> query)
        {
            return query._Query;
        }

        #endregion

        #region Query
        Expression<Func<EntityType, bool>> _Query;
        public DaoQuery<EntityType> Where(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (_Query != null)
            {
                _Query = _Query.AndAlso(predicate);
            }
            else
            {
                _Query = predicate;
            }
            
            return this;
        }

        public DaoQuery<EntityType> WhereIf(bool condition, Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (condition)
            {
                Where(predicate);
            }
            return this;
        }

        #endregion
    }
}
