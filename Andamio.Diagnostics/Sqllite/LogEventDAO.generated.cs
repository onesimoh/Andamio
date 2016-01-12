




/*
 *  EventLogEntriesDAO Data Access Object
 *
 *	Edm Entity: AppAdminDataModel.EventLogEntry
 *  MD5 Hash: EAAA144830A928FD108AB53FEA195D60
 *  
 *  This is a GENERATED file.
 *  Modifications should be made to the configuration of Entity Code Generator Tool.
 *
 */

using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

using Andamio;
using Andamio.Configuration;
using Andamio.Data;
using Andamio.Data.Access;
using Andamio.Data.Entities;
using Andamio.Data.Transactions;
using Andamio.Data.Serialization;

namespace Andamio.Diagnostics.Sqllite
{
    public class LogEventMappings : EntityTypeConfiguration<LogEvent>
    {
        public LogEventMappings()
        {
            ToTable("LogEvents");

            Ignore(e => e.IsModified);
            Ignore(e => e.IsReadFromDatabase);
            Ignore(e => e.IsSerializing);
        }
    }

    public partial class LogEventDAO : Dao<LogEvent>
    {
        #region Constructors
        public LogEventDAO() : base()
        {
        }

        #endregion
	}
}