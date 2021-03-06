using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Andamio.Threading
{
    /// <summary>
    /// Creates and acquires a Write Lock.
    /// </summary>
    public class WriteLock : IDisposable
    {
        #region Private Fields
        private bool _disposed = false;
        private object _syncLock = new object();
        
        private ReaderWriterLock _rwLock = null;
        private bool _timedOut = false;
        #endregion        
        
        #region Constructors
        private WriteLock()
        {   }                        
        
        private WriteLock( ReaderWriterLock syncLock, TimeSpan timeout )
        {
            if( syncLock == null )
                throw new ArgumentNullException( "ReaderWriterLock syncLock" );

            _rwLock = syncLock;
            
            try
            {
                syncLock.AcquireWriterLock( timeout );
            }
            catch( ApplicationException )
            {
                _timedOut = true;
            }
        }
        
        /// <summary>
        /// ReadLock Desctructor.
        /// </summary>
        /// <remarks>
        /// This destructor will run only if Dispose method is not called.
        /// </remarks>        
        ~WriteLock()
        {
            // Though a situation in which Cleanup() executes twice should not occur since
            // GC.SuppressFinalize() is being called from Dispose(), a check is made to only
            // run Cleanup() if object has not been Disposed earlier.
            if( !IsDisposed )
                Cleanup();         
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// Indicates wheter object has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get{ lock( _syncLock ) { return _disposed; } }
        }        
        #endregion
        
        #region Public Methods
        /// <summary>
        /// Acquires a write lock on the resource.
        /// </summary>
        /// <param name="syncLock">ReaderWriterLock to acquire Write Lock on.</param>
        /// <returns>Returns an instance of the WriteLock.</returns>
        public static WriteLock Acquire( ReaderWriterLock syncLock )
        {
            return new WriteLock( syncLock, TimeSpan.Zero );
        }
        
        /// <summary>
        /// Acquires a write lock on the resource using a time-out.
        /// </summary>
        /// <param name="syncLock">ReaderWriterLock to acquire Write Lock on.</param>
        /// <param name="timeout">The TimeSpan specifying the time-out period.</param>
        /// <returns>Returns an instance of the WriteLock.</returns>
        public static WriteLock Acquire( ReaderWriterLock syncLock, TimeSpan timeout )
        {
            return new WriteLock( syncLock, timeout );
        }
        #endregion
        
        #region IDisposable Members, Cleanup()
        /// <summary>
        /// Disposes of the object and releases the lock.
        /// </summary>
        public void Dispose()
        {
            if( _disposed )
                return;
                
            lock( _syncLock )
            {
                if( !_disposed )
                {
                    // Request object is taken off Finalization Queue
                    // this allows Finalization code from executing twice
                    GC.SuppressFinalize( this );                    
                    
                    Cleanup();
                    
                    _disposed = true;
                }
            }        
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void Cleanup()
        {
            if( !_timedOut )
                _rwLock.ReleaseWriterLock();
        }
        #endregion    
    }
}
