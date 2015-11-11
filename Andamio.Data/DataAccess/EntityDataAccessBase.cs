using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;

using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public abstract class EntityDataAccessBase<EntityKey, EntityType> : DataAccessBase
        where EntityType : SimpleKeyEntity<EntityKey>, new()
        where EntityKey : struct
    {
        #region Public Virtual Methods - Data
        public virtual void Upsert(ref EntityType entity)
        {
            if (!entity.IsReadFromDatabase)
            {
                EntityKey newEntityId;
                Insert(ref entity, out newEntityId);
            }
            else
            {
                Update(entity);
            }
        }

        public virtual void Insert(ref EntityType entity, out EntityKey newEntityId)
        {
            if (entity.IsReadFromDatabase)
            { throw new InvalidOperationException(); }

            using (DataConnectionProvider dataProvider = new DataConnectionProvider(this.ConnectionStringSettings))
            {
                string newIdOutParameterName;
                using (DbCommand command = CreateInsertCommand(dataProvider, entity, out newIdOutParameterName))
                {
                    ExecuteNonQuery(command);

                    // Populate new Entity from database
                    newEntityId = (EntityKey)command.Parameters[newIdOutParameterName].Value;
                    entity = GetForPrimaryKey(newEntityId);
                }
            }
        }

        public virtual void Update(EntityType entity)
        {
            if (!entity.IsReadFromDatabase)
            { throw new InvalidOperationException(); }

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
        protected abstract DbCommand CreateDeleteCommand(DataConnectionProvider dataProvider, EntityKey entity);

        protected abstract DbCommand CreateUpdateCommand(DataConnectionProvider dataProvider, EntityType entity);

        protected abstract DbCommand CreateInsertCommand(DataConnectionProvider dataProvider, EntityType entity, out string newIdOutParameterName);

        protected abstract DbCommand CreateGetForPrimaryKeyCommand(DataConnectionProvider dataProvider, EntityKey primaryKey);

        #endregion
    }
}
