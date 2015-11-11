using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio
{
    /// <summary>
    /// Privides functionality to release resources.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        #region Constructors, Destructor
        /// <summary>
        /// Disposable Desctructor.
        /// </summary>
        /// <remarks>
        /// This destructor will run only if Dispose method is not called.
        /// </remarks>
        ~Disposable()
        {
            // Though a situation in which Cleanup() executes twice should not occur since
            // GC.SuppressFinalize() is being called from Dispose(), a check is made to only
            // run Cleanup() if object has not been Disposed earlier.
            if (!IsDisposed)
            { 
                Cleanup();
                lock (_syncLock)
                {
                    _disposed = true;
                }
            }
        }
        #endregion

        #region Disposed
        private bool _disposed = false;
        private object _syncLock = new object();
        /// <summary>
        /// Gets an indication as to whether the object has been Disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                lock (_syncLock)
                {
                    return _disposed;
                }
            }
        }
        #endregion

        #region IDisposable Members, Cleanup()
        /// <summary>
        /// When implement by deriving class it defines a task that will release all resources associated with this object.
        /// </summary>
        protected abstract void Cleanup();

        /// <summary>
        /// It releases the resources associated with this object and sets IsDisposed property to true.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            { return; }

            lock (_syncLock)
            {
                if (!_disposed)
                {
                    // Request object is taken off Finalization Queue
                    // this allows Finalization code from executing twice
                    GC.SuppressFinalize(this);

                    Cleanup();
                    _disposed = true;
                }
            }
        }
        #endregion
    }
}
