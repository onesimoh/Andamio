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
    public static class DAO
    {
        #region Factory
        public static DaoBase<EntityType> For<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration entityConfig = Configuration[typeof(EntityType)];
            return (DaoBase<EntityType>) entityConfig.Dao;
        }

        #endregion

        #region Configuration
        private static readonly DaoConfiguration _configuration = new DaoConfiguration();
        public static DaoConfiguration Configuration
        {
            get { return _configuration; }
        }

        #endregion
    }


    public class DaoConfiguration
    {
        #region Factory
        private readonly object _syncLock = new object();
        private readonly Dictionary<Type, DaoEntityConfiguration> _mappings = new Dictionary<Type, DaoEntityConfiguration>();
        public DaoEntityConfiguration For<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration entityConfig = new DaoEntityConfiguration();
            _mappings.Add(typeof(EntityType), entityConfig);
            return entityConfig;
        }

        internal DaoEntityConfiguration this[Type entityType]
        {
            get
            {
                DaoEntityConfiguration entityConfig;
                if (!_mappings.ContainsKey(entityType))
                {
                    lock (_syncLock)
                    {
                        if (!_mappings.TryGetValue(entityType, out entityConfig))
                        {
                            Assembly asm = Assembly.GetAssembly(entityType);
                            Type daoType = asm.GetTypes().Where(type => type.DerivesFromType(typeof(DaoBase<>)))
                                .Where(match => match.GetImplementedType(typeof(DaoBase<>)).GetGenericArguments().First().Equals(entityType))
                                .SingleOrDefault();

                            DaoBase dao = (DaoBase)Activator.CreateInstance(daoType);

                            entityConfig = new DaoEntityConfiguration();
                            entityConfig.BindTo(dao);
                            _mappings.Add(entityType, entityConfig);
                        }
                    }
                }
                else
                {
                    entityConfig = _mappings[entityType];
                }

                return entityConfig;
            }
        }

        #endregion

        #region Properties
        public DbConnectionStringSettings ConnectionStringSettings { get; set; }

        #endregion
    }


    public class DaoEntityConfiguration
    {
        #region Bindings
        internal DaoBase Dao { get; private set; }
        public DaoEntityConfiguration BindTo<DaoType>()
            where DaoType : DaoBase, new()
        {
            Dao = Activator.CreateInstance<DaoType>();
            return this;
        }

        public DaoEntityConfiguration BindTo<DaoType>(DbConnectionStringSettings connectionStringSettings)
            where DaoType : DaoBase, new()
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");

            BindTo<DaoType>().Dao.ConnectionStringSettings = connectionStringSettings;
            return this;
        }

        public DaoEntityConfiguration BindTo(DaoBase dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            Dao = dao;
            return this;
        }

        #endregion
    }
}
