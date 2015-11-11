using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;

using Andamio;

namespace Andamio.Threading
{
    #region State
    /// <summary>
    /// States of the WorkItem.
    /// </summary>
    public enum WorkItemState
    {
        /// <summary>
        /// Original State of the WorkItem before is processed.
        /// </summary>
        None,

        /// <summary>
        /// Original State of the WorkItem when being processed.
        /// </summary>
        Processing,

        /// <summary>
        /// Original State of the WorkItem after being processed.
        /// </summary>
        Completed,

        /// <summary>
        /// State of the WorkItem after completion with error.
        /// </summary>
        Error
    }

    public static class WorkItemStateExtensions
    {
        public static bool IsDefined(this WorkItemState state)
        {
            return state != WorkItemState.None;
        }

        public static bool IsProcessing(this WorkItemState state)
        {
            return state == WorkItemState.Processing;
        }

        public static bool IsCompleted(this WorkItemState state)
        {
            return state == WorkItemState.Completed;
        }

        public static bool IsError(this WorkItemState state)
        {
            return state == WorkItemState.Error;
        }
    
    }

    #endregion


    /// <summary>
    /// Represents a delegate and args to execute.
    /// </summary>
    public class WorkItem : IAsyncResult
    {
        #region Public Events
        /// <summary>
        /// Occurs when an exception is thrown while processing the WorkItem.
        /// </summary>
        public event ItemEventHandler<Exception> Error;
        protected void OnError(ItemEventArgs<Exception> eventArgs)
        {
            if (Error != null)
            { 
                Error.SafeEventInvoke(this, eventArgs); 
            }
        }

        /// <summary>
        /// Occurs when Work is completed.
        /// </summary>
        public event EventHandler Completed;
        protected void OnCompleted(EventArgs eventArgs)
        {
            if (Completed != null)
            { 
                Completed.SafeEventInvoke(this, eventArgs); 
            }
        }

        #endregion

        #region Private, Internal
        private Delegate _method = null;
        private object[] _methodArgs = null;
        internal int Attempts = 1;
        
        #endregion

        #region Constructor
        private WorkItem()
        {
            ReturnValue = null;
        }

        /// <summary>
        /// Constructs a WorkItem object.
        /// </summary>
        /// <param name="asyncState">Object that qualifies or contains information about WorkItem.</param>
        /// <param name="method">Method to use to create the new WorkItem.</param>
        /// <param name="methodArgs">Method Arguments to use to create the new WorkItem.</param>
        public WorkItem(object asyncState, Delegate method, params object[] methodArgs)
            : this()
        {
            _method = method;
            _methodArgs = methodArgs;
            AsyncState = asyncState;            
        }

        /// <summary>
        /// Constructs a WorkItem object.
        /// </summary>
        /// <param name="asyncState">Object that qualifies or contains information about WorkItem.</param>
        /// <param name="method">Method to use to create the new WorkItem.</param>
        public WorkItem(Delegate method) : this(null, method, null)
        {
        }

        /// <summary>
        /// Constructs a WorkItem object.
        /// </summary>
        /// <param name="method">Method to use to create the new WorkItem.</param>
        /// <param name="methodArgs">Method Arguments to use to create the new WorkItem.</param>
        public WorkItem(Delegate method, object[] methodArgs)
            : this(null, method, methodArgs)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        public WorkItem(ThreadStart start) : this()
        {
            _method = start;
        }

        #endregion

        #region AsyncResult
        /// <summary>
        /// Gets the object that qualifies or contains information about WorkItem.
        /// </summary>
        object IAsyncResult.AsyncState
        {
            get { return AsyncState; }
        }

        /// <summary>
        /// Gets a WaitHandle that is used to wait for an asynchronous operation to complete. 
        /// </summary>
        WaitHandle IAsyncResult.AsyncWaitHandle
        {
            get { return AsyncWaitHandle; }
        }

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        bool IAsyncResult.CompletedSynchronously
        {
            get { return CompletedSynchronously; }
        }

        /// <summary>
        /// Gets an indication whether the WorkItem operation has completed.
        /// </summary>
        bool IAsyncResult.IsCompleted
        {
            get { return IsCompleted; }
        }

        /// <summary>
        /// Gets the object that qualifies or contains information about WorkItem.
        /// </summary>
        public object AsyncState { get; set; }

        /// <summary>
        /// Gets a WaitHandle that is used to wait for an asynchronous operation to complete. 
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get { return _manualResetEvent; }
        }

        /// <summary>
        /// Gets an indication of whether the asynchronous operation completed synchronously.
        /// </summary>
        public bool CompletedSynchronously { get; private set; }

        /// <summary>
        /// Gets an indication whether the WorkItem operation has completed.
        /// </summary>        
        public bool IsCompleted
        {
            get { return CurrentState.IsCompleted() || CurrentState.IsError(); }
        }

        /// <summary>
        /// Gets the return value of the executing delegate method.
        /// </summary>
        public object ReturnValue { get; private set; }

        #endregion

        #region State
        private WorkItemState _currentState = WorkItemState.None;
        private object _syncLock = new object();

        /// <summary>
        /// Returns current state of the WorkItem.
        /// </summary>
        public WorkItemState CurrentState
        {
            get
            {
                lock (_syncLock)
                {
                    return _currentState;
                }
            }
            internal set
            {
                lock (_syncLock)
                {
                    _currentState = value;
                }
            }
        }

        #endregion

        #region Run
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        /// <summary>
        /// Runs the specified delegate with provided parameters.
        /// </summary>
        /// <remarks>
        /// In case an error occurs or the operation times-out this method will raise the Error event. On successful completion
        /// the State of the WorkItem is set to State.Completed and IAsyncResult.AsyncWaitHandle property will is signaled to
        /// indicate completion.
        /// </remarks>        
        internal virtual object Run(TimeSpan timeout)
        {
            AsyncFire asyncFire = delegate(Delegate del, object[] args)
            {
                try
                {
                    ReturnValue = del.DynamicInvoke(args);
                }
                catch (Exception e)
                {
                    CurrentState = WorkItemState.Error;

                    // Signal method completed execution.
                    _manualResetEvent.Set();

                    // Because the method to run is executed dynamicaly using reflection any exception that occur
                    // will create a reflection exception, inner exception contains the real exception we need to 
                    // propagate back. 
                    Exception exception = e.InnerException != null ? e.InnerException : e;
                    OnError(new ItemEventArgs<Exception>(exception));
                }
            };

            this.CurrentState = WorkItemState.Processing;

            IAsyncResult asyncResult = asyncFire.BeginInvoke(_method, _methodArgs, null, null);
            if (asyncResult.AsyncWaitHandle.WaitOne(timeout, false))
            {
                asyncFire.EndInvoke(asyncResult);

                CurrentState = WorkItemState.Completed;

                // Signal method completed execution.
                _manualResetEvent.Set();

                OnCompleted(EventArgs.Empty);
            }
            else
            {
                CurrentState = WorkItemState.Error;

                // Signal method completed execution.
                _manualResetEvent.Set();

                OnError(new ItemEventArgs<Exception>(new TimeoutException()));
            }

            CompletedSynchronously = true;
            return ReturnValue;
        }

        /// <summary>
        /// Runs the specified delegate with provided parameters.
        /// </summary>
        /// <remarks>
        /// In case an error occurs or the operation times-out this method will raise the Error event. On successful completion
        /// the State of the WorkItem is set to State.Completed and IAsyncResult.AsyncWaitHandle property will is signaled to
        /// indicate completion.
        /// </remarks>   
        public void Run()
        {
            // TimeSpan that represents -1 milliseconds to wait indefinitely.
            Run(new TimeSpan(0, 0, 0, 0, -1));
        }

        public virtual IAsyncResult RunAsync()
        {
            CompletedSynchronously = false;
            AsyncFire asyncFire = delegate(Delegate del, object[] args)
            {
                try
                {
                    ReturnValue = del.DynamicInvoke(args);
                    CurrentState = WorkItemState.Completed;

                    OnCompleted(EventArgs.Empty);
                }
                catch (Exception e)
                {
                    CurrentState = WorkItemState.Error;

                    // Signal method completed execution.
                    _manualResetEvent.Set();

                    // Because the method to run is executed dynamicaly using reflection any exception that occurs
                    // will create a reflection exception, inner exception contains the real exception we need to 
                    // propagate back. 
                    Exception ex = e.InnerException != null ? e.InnerException : e;
                    OnError(new ItemEventArgs<Exception>(ex));
                }
            };

            AsyncCallback callback = delegate(IAsyncResult result)
            {
                AsyncResult asyncResult = (AsyncResult)result;
                AsyncFire asyncFireResult = (AsyncFire)asyncResult.AsyncDelegate;

                // Signal method completed execution.
                _manualResetEvent.Set();

                asyncFireResult.EndInvoke(result);
            };

            CurrentState = WorkItemState.Processing;

            IAsyncResult async = asyncFire.BeginInvoke(_method, _methodArgs, callback, null);
            return (IAsyncResult) this;
        }

        #endregion

        #region Destroy
        public void Destroy()
        {
            IAsyncResult asyncResult = (IAsyncResult) this;

            try
            {
                if (!asyncResult.AsyncWaitHandle.SafeWaitHandle.IsClosed)
                {
                    asyncResult.AsyncWaitHandle.SafeWaitHandle.Close();
                    asyncResult.AsyncWaitHandle.Close();

                    asyncResult.AsyncWaitHandle.Dispose();
                    asyncResult.AsyncWaitHandle.SafeWaitHandle.Dispose();
                }
            }
            finally
            {
            }
        }

        #endregion
    }
}
