using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.SQLite;

using Andamio;
using Andamio.Diagnostics;
using Andamio.Diagnostics.Configuration;

namespace Andamio.Diagnostics.Sqllite
{
    public class SqliteDataContext : DbContext
    {
        static SQLiteConnection Connection(string filePath)
        {
            if (filePath.IsNullOrBlank()) throw new ArgumentNullException("filePath");
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder() { DataSource = filePath };
            SQLiteConnection connection = new SQLiteConnection(builder.ConnectionString, true);
            return connection;
        }

        public SqliteDataContext(string filePath)
            : base(Connection(filePath), true)
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer<SqliteDataContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add<LogEvent>(new LogEventMappings());
        }

        public DbSet<LogEvent> LogEvents { get; set; }
    }
}
