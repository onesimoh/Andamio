using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Andamio;
using Andamio.Data.Entities;

namespace Andamio.Data.Access
{
    public static class DAO
    {
        #region Factory
        public static Dao<EntityType> For<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration<EntityType> entityConfig = Configuration.Resolve<EntityType>();
            return entityConfig.Dao;
        }

        public static DaoEntity<EntityType> For<EntityType>(EntityType entity)
            where EntityType : EntityBase, new()
        {
            return DAO.For<EntityType>().Entity(entity);
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
        }

        #endregion

        #region Factory
        private readonly object _syncLock = new object();
        private readonly Dictionary<Type, DaoEntityConfiguration> _mappings = new Dictionary<Type, DaoEntityConfiguration>();

        public DaoEntityConfiguration<EntityType> For<EntityType>()
            where EntityType : EntityBase, new()
        {
            DaoEntityConfiguration entityConfig;
            if (!_mappings.ContainsKey(typeof(EntityType)))
            {
                lock (_syncLock)
                {
                    if (!_mappings.TryGetValue(typeof(EntityType), out entityConfig))
                    {
                        var dao = new Dao<EntityType>(DAO.Configuration.DataAdapter);
                        entityConfig = new DaoEntityConfiguration<EntityType>();
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
                        Type daoType = asm.GetTypes().Where(type => type.DerivesFromType(typeof(Dao<>)))
                            .Where(match => match.GetImplementedType(typeof(Dao<>)).GetGenericArguments().First().Equals(typeof(EntityType)))
                            .SingleOrDefault();

                        if (daoType == null)
                        { daoType = typeof(Dao<EntityType>); }

                        Dao<EntityType> dao = (Dao<EntityType>) Activator.CreateInstance(daoType);
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
        public DbConnectionSettings ConnectionSettings { get; set; }

        /// <summary>
        ///  Gets or sets the timeout value, in seconds, for all object context operations.
        ///   A null value indicates that the default value of the underlying provider will be used.
        /// </summary>
        public Nullable<int> CommandTimeout { get; set; }

        #endregion

        #region Data Storage
        public IDataAdapter DataAdapter { get; private set; }

        public void ConfigureDataAdapter<DataAdapter>(params object[] args)
            where DataAdapter : IDataAdapter
        {
            IDataAdapter dataAdapter = (args != null)
                ? (DataAdapter) Activator.CreateInstance(typeof(DataAdapter), args)
                : Activator.CreateInstance<DataAdapter>();
            ConfigureDataAdapter(dataAdapter);
        }

        public void ConfigureDataAdapter(IDataAdapter dataAdapter)
        {
            if (dataAdapter == null) throw new ArgumentNullException("dataAdapter");
            this.DataAdapter = dataAdapter;
        }

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

        internal DaoEntityConfiguration(Dao<EntityType> dao)
        {
            BindTo(dao);
        }

        #endregion

        #region Bindings
        internal Dao<EntityType> Dao { get; private set; }
        //internal IDataAdapter DataAdapter { get; private set; }
        public DaoEntityConfiguration<EntityType> BindTo<DaoType>(IDataAdapter dataAdapter = null)
            where DaoType : Dao<EntityType>, new()
        {
            DaoType dao = Activator.CreateInstance<DaoType>();            
            if (dataAdapter != null)
            { dao.DataAdapter = dataAdapter; }
            return BindTo(dao);
        }

        public DaoEntityConfiguration<EntityType> BindTo(Dao<EntityType> dao)
        {
            if (dao == null) throw new ArgumentNullException("dao");
            Dao = dao;
            return this;
        }

        /*
        public DaoEntityConfiguration<EntityType> WithDataAdapter(IDataAdapter dataAdapter)
        {
            if (dataAdapter == null) throw new ArgumentNullException("dataAdapter");
            DataAdapter = dataAdapter;
            return this;
        }
        */

        #endregion
    }
}
