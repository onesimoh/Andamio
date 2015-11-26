﻿using System;
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
            DaoEntityConfiguration<EntityType> entityConfig = Configuration.Resolve<EntityType>();
            return entityConfig.Dao;
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
        #region Constructors
        public DaoConfiguration()
        {
            EF = new EntityFrameworkConfiguration();
        }

        #endregion

        #region Factory
        private readonly object _syncLock = new object();
        private readonly Dictionary<Type, DaoEntityConfiguration> _mappings = new Dictionary<Type, DaoEntityConfiguration>();

        public DaoEntityConfiguration<EntityType> For<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration<EntityType> entityConfig = new DaoEntityConfiguration<EntityType>();
            _mappings.Add(typeof(EntityType), entityConfig);
            return entityConfig;
        }

        internal DaoEntityConfiguration<EntityType> Resolve<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration entityConfig;
            if (!_mappings.ContainsKey(typeof(EntityType)))
            {
                lock (_syncLock)
                {
                    if (!_mappings.TryGetValue(typeof(EntityType), out entityConfig))
                    {
                        Assembly asm = Assembly.GetAssembly(typeof(EntityType));
                        Type daoType = asm.GetTypes().Where(type => type.DerivesFromType(typeof(DaoBase<>)))
                            .Where(match => match.GetImplementedType(typeof(DaoBase<>)).GetGenericArguments().First().Equals(typeof(EntityType)))
                            .SingleOrDefault();

                        DaoBase<EntityType> dao = (DaoBase<EntityType>) Activator.CreateInstance(daoType);
                        dao.ConnectionStringSettings = ConnectionStringSettings;
                        dao.CommandTimeout = CommandTimeout;

                        entityConfig = new DaoEntityConfiguration<EntityType>(dao);
                        _mappings.Add(typeof(EntityType), entityConfig);
                    }
                }
            }
            else
            {
                entityConfig = _mappings[typeof(EntityType)];
            }

            return (DaoEntityConfiguration<EntityType>) entityConfig;
        }

        #endregion

        #region Properties
        /// Returns a DataProvider specific for that database to access.
        /// </summary>
        /// <returns>The DataProvider for the Database.</returns>
        public DbConnectionStringSettings ConnectionStringSettings { get; set; }

        /// <summary>
        ///  Gets or sets the timeout value, in seconds, for all object context operations.
        ///   A null value indicates that the default value of the underlying provider will be used.
        /// </summary>
        public Nullable<int> CommandTimeout { get; set; }

        #endregion

        #region EF
        public EntityFrameworkConfiguration EF { get; private set; }

        #endregion
    }


    public abstract class DaoEntityConfiguration { }
    public class DaoEntityConfiguration<EntityType> : DaoEntityConfiguration
        where EntityType : EntityBase, new()
    {
        #region Constructors
        internal DaoEntityConfiguration()
        {

        }

        internal DaoEntityConfiguration(DaoBase<EntityType> dao)
        {
            BindTo(dao);
        }

        #endregion

        #region Bindings
        internal DaoBase<EntityType> Dao { get; private set; }
        public DaoEntityConfiguration<EntityType> BindTo<DaoType>()
            where DaoType : DaoBase<EntityType>, new()
        {
            Dao = Activator.CreateInstance<DaoType>();
            return this;
        }

        public DaoEntityConfiguration<EntityType> BindTo<DaoType>(DbConnectionStringSettings connectionStringSettings, Nullable<int> commandTimeout = null)
            where DaoType : DaoBase<EntityType>, new()
        {
            if (connectionStringSettings == null) throw new ArgumentNullException("connectionStringSettings");
            DaoBase dao = BindTo<DaoType>().Dao;
            dao.ConnectionStringSettings = connectionStringSettings;
            dao.CommandTimeout = commandTimeout;
            return this;
        }

        public DaoEntityConfiguration<EntityType> BindTo(DaoBase<EntityType> dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            Dao = dao;
            return this;
        }

        #endregion

        #region Stubs
        public DaoEntityConfiguration<EntityType> UseFake(FakeDAO<EntityType> dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            Dao = dao;
            return this;
        }

        public DaoEntityConfiguration<EntityType> UseFake<DaoType>()
            where DaoType : FakeDAO<EntityType>, new()
        {
            Dao = Activator.CreateInstance<DaoType>();
            return this;
        }

        public DaoEntityConfiguration<EntityType> UseFake<DaoType>(IEnumerable<EntityType> entities)
            where DaoType : FakeDAO<EntityType>, new()
        {
            var args = new object[] { entities };
            Dao = (DaoType) Activator.CreateInstance(typeof(DaoType), args);
            return this;
        }

        public DaoEntityConfiguration<EntityType> UseFake()
        {
            return UseFake<FakeDAO<EntityType>>();
        }

        public DaoEntityConfiguration<EntityType> UseFake(IEnumerable<EntityType> entities)
        {
            return UseFake<FakeDAO<EntityType>>(entities);
        }

        #endregion
    }


    public class EntityFrameworkConfiguration
    {
        #region Register
        public void RegisterDbContext<T>()
            where T : DbContext
        {
            RegisterDbContext<T>(() => Activator.CreateInstance<T>());
        }

        public void RegisterDbContext<T>(Func<T> contextResolver)
            where T : DbContext
        {
            if (contextResolver == null) throw new ArgumentNullException("contextResolver");
            _ContextResolver = contextResolver;
        }

        #endregion

        #region Resolve
        private Func<DbContext> _ContextResolver;
        public DbContext ResolveDbContext()
        {
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

                return (DbContext) Activator.CreateInstance(allDbContexts.First());
            }
            else
            {
                return _ContextResolver();
            }
        }

        #endregion
    }

}
