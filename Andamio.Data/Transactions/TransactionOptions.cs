using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Andamio.Data.Transactions
{
    public class TransactionWrapperOptions
    {
        #region Constructors
        public TransactionWrapperOptions() 
            : this(TransactionScopeMode.Required, IsolationLevel.Serializable, TimeSpan.FromMinutes(1))
        {
        }
        
        public TransactionWrapperOptions(TransactionScopeMode mode) 
            : this(mode, IsolationLevel.Serializable, TimeSpan.FromMinutes(1))
        {
        }

        public TransactionWrapperOptions(IsolationLevel isolationLevel)
            : this(TransactionScopeMode.Required, isolationLevel, TimeSpan.FromMinutes(1))
        {
        }
        public TransactionWrapperOptions(TransactionScopeMode mode, IsolationLevel isolationLevel, TimeSpan timeout)
            : this(mode, isolationLevel, timeout, EnterpriseServicesInteropOption.None)
        {
        }

        public TransactionWrapperOptions(TransactionScopeMode mode
            , IsolationLevel isolationLevel
            , TimeSpan timeout
            , EnterpriseServicesInteropOption interopOption)
        {
            Mode = mode;
            IsolationLevel = isolationLevel;
            Timeout = timeout;
            InteropOption = interopOption;
        }
        
        public static TransactionOptions ForDebugging()
        {
            return ForDebugging(TransactionScopeMode.Required);
        }        
        public static TransactionOptions ForDebugging(TransactionScopeMode mode)
        {
            return new TransactionOptions() { Timeout = TimeSpan.Zero };
        }

        #endregion
        
        #region Properties
        public TransactionScopeMode Mode { get; set; } 
        public IsolationLevel IsolationLevel { get; set; }
        public TimeSpan Timeout { get; set; }
        public EnterpriseServicesInteropOption InteropOption { get; set; }
 
        #endregion
    }
}
