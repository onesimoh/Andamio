using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public abstract class StrongEntityDataAccessBase<EntityKey, EntityType> : DataAccessBase
        where EntityType : StrongEntityBase<EntityKey>, new()
        where EntityKey : struct
    {
        #region Public Virtual Methods - Data
        public virtual void Upsert(EntityType entity)
        {
            if (!entity.IsModified)
            { return; }

            if (!entity.IsReadFromDatabase)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public virtual void Insert(EntityType entity)
        {
            if (entity.IsReadFromDatabase)
            { throw new InvalidOperationException(); }

            using (DataConnectionProvider dataProvider = new DataConnectionProvider(this.ConnectionStringSettings))
            {
                using (DbCommand command = CreateInsertCommand(dataProvider, entity))
                {
                    ExecuteNonQuery(command);
                }
            }
        }

        public virtual void Update(EntityType entity)
        {
            if (!entity.IsReadFromDatabase)
            { throw new InvalidOperationException(); }

            if (!entity.IsModified)
                return;

            using (DataConnectionProvider dataProvider = new DataConnectionProvider(this.ConnectionStringSettings))
            {
                using (DbCommand command = CreateUpdateCommand(dataProvider, entity))
                {
                    ExecuteNonQuery(command);
                }
            }
        }

        public virtual void Delete(EntityType entity)
        {
            if (!entity.IsReadFromDatabase)
            { return; }

            using (DataConnectionProvider dataProvider = new DataConnectionProvider(this.ConnectionStringSettings))
            {
                using (DbCommand command = CreateDeleteCommand(dataProvider, entity.ID))
                {
                    ExecuteNonQuery(command);
                }
            }
        }

        public virtual EntityType GetForPrimaryKey(EntityKey primaryKey)
        {
            using (DataConnectionProvider dataProvider = new DataConnectionProvider(this.ConnectionStringSettings))
            {
                using (DbCommand command = CreateGetForPrimaryKeyCommand(dataProvider, primaryKey))
                {
                    using (DbDataReader reader = ExecuteReader(command))
                    {
                        return CreateEntity<EntityType>(reader, PopulateEntity);
                    }
                }
            }
        }

        #endregion

        #region Protected Abstract Methods - Entity
        protected abstract EntityType PopulateEntity(IDataRecord dataRecord);

        #endregion

        #region Protected Abstract Methods - Command
        protected abstract DbCommand CreateDeleteCommand(DataConnectionProvider dataProvider, EntityKey primaryKey);

        protected abstract DbCommand CreateUpdateCommand(DataConnectionProvider dataProvider, EntityType entity);

        protected abstract DbCommand CreateInsertCommand(DataConnectionProvider dataProvider, EntityType entity);

        protected abstract DbCommand CreateGetForPrimaryKeyCommand(DataConnectionProvider dataProvider, EntityKey primaryKey);

        #endregion
    }
}
