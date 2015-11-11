using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Andamio.Data.Transactions
{
    /// <summary>
    /// Wrapper class for System.Transactions.TransactionScope object.
    /// </summary>
    public class TransactionWrapper : Disposable
    {
        #region Constructors
        private readonly TransactionScope _transactionScope;
        public TransactionWrapper() : this(new TransactionWrapperOptions())
        {
        }
        public TransactionWrapper(TransactionScopeMode mode)
            : this(new TransactionWrapperOptions() { Mode = mode })
        {
        }
        public TransactionWrapper(TransactionScopeMode mode, TimeSpan timeout)
            : this(new TransactionWrapperOptions() { Mode = mode, Timeout = timeout })
        {
        }
        public TransactionWrapper(TransactionWrapperOptions options)
        {
            TransactionOptions transactionOptions = new TransactionOptions()                
            {
                IsolationLevel = options.IsolationLevel,
                Timeout = options.Timeout,
            };
            _transactionScope = new TransactionScope(options.Mode.ToTransactionScopeOption(), transactionOptions, options.InteropOption);
        }

        #endregion

        #region Create
        public static TransactionWrapper NoLock()
        {
            return NoLock(TransactionScopeMode.Required);
        }

        public static TransactionWrapper NoLock(TransactionScopeMode mode)
        {
            TransactionWrapperOptions options = new TransactionWrapperOptions(mode) { IsolationLevel = IsolationLevel.ReadUncommitted };            
            return Create(options);
        }        
        
        public static TransactionWrapper Create()
        {
            return Create(TransactionScopeMode.Required);
        }
       
        public static TransactionWrapper Create(TransactionScopeMode mode)
        {
            return Create(new TransactionWrapperOptions(mode));
        }

        public static TransactionWrapper Create(TransactionWrapperOptions options)
        {              
            #if DEBUG
            options.Timeout = TimeSpan.Zero;
            #endif
            return new TransactionWrapper(options);
        }

        #endregion

        #region Completed
        //private static readonly object _isCompleSyncLock = new object();
        private bool _isCompleted = false;
        //public bool IsCompleted
        //{
        //    get { 
        //}
        
        /// <summary>
        /// Indicates that all operations within the scope are completed successfully.
        /// </summary>        
        public void Complete()
        {
            if (!_isCompleted)
            {
                _isCompleted = true;
                _transactionScope.Complete();
            }
        }
        
        #endregion
    
        #region Dispose
        protected override void Cleanup()
        {
            try
            {
                if (_transactionScope != null)
                { _transactionScope.Dispose(); }
            }
            catch
            {
                // Ignore exception
            }
        }

        #endregion
    }
}
