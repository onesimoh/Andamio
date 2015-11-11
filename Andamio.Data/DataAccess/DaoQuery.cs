using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Reflection;
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
        internal readonly List<Expression<Func<EntityType, bool>>> Queries = new List<Expression<Func<EntityType, bool>>>();
        internal readonly List<Func<EntityType, bool>> Predicates = new List<Func<EntityType, bool>>();

        internal DaoQuery()
        {
        }

        internal DaoQuery(Expression<Func<EntityType, bool>> predicate)
            : this()
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            Queries.Add(predicate);
        }

        public DaoQuery<EntityType> Where(Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            Queries.Add(predicate);
            return this;
        }

        public DaoQuery<EntityType> WhereIf(bool condition, Expression<Func<EntityType, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (condition)
            {
                Queries.Add(predicate);
            }
            return this;
        }

        public DaoQuery<EntityType> FilterIf(bool condition, Func<EntityType, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (condition)
            {
                Predicates.Add(predicate);
            }
            return this;
        }
    }
}
