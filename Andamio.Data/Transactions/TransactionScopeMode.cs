using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Andamio.Data.Transactions
{
    public enum TransactionScopeMode
    {
        /// <summary>
        /// A transaction is required by the scope. It uses an ambient transaction if
        /// one already exists. Otherwise, it creates a new transaction before entering
        /// the scope. This is the default value.        
        /// </summary>
        Required = 0,
        
        /// <summary>
        /// A new transaction is always created for the scope.
        /// </summary>
        RequiresNew = 1,
        
        /// <summary>
        /// The ambient transaction context is suppressed when creating the scope. All
        /// operations within the scope are done without an ambient transaction context.
        /// </summary>
        Suppress = 2,
    }

    public static class TransactionScopeModeExtensions
    {
        public static TransactionScopeOption ToTransactionScopeOption(this TransactionScopeMode mode)
        {
            return (TransactionScopeOption) mode;
        }
    }
}
