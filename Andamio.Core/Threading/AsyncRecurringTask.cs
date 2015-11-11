using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Andamio.Threading
{
    /// <summary>
    /// Executes a Recurring operation asynchronously on a separate thread.
    /// </summary>
    /// <remarks>
    /// The <b>AsyncRecurringTask</b> abstract class allows you to run an operation asynchronously on a separate thread when a fast 
    /// and responsive UI is of the essence.<br /><br />
    /// 
    /// To make use of this class an inheritor must implement <b>AsyncRecurringTask.RecurringTask()</b> method, which will execute 
    /// recurrently at a interval specified through the class constructor, a time out time is also specified to abort execution 
    /// if it exceeds such time.<br /><br />
    /// 
    /// A series of events are available to subscriber to react to the state of the Task, for example, the <b>Completed</b> event signals 
    /// whenever operation execution finishes, <b>Updating</b> event indicates that the operation is currently running, you may make use 
    /// of this event to display an animated icon or a simple text as an indicator to the user that the Task is running. <b>Error</b> event, 
    /// on the other hand, may be used to alert the user an error occurred while the operation ran.
    /// </remarks>
    /// <example>
    /// The example below illustrates how to implement <b>AsyncRecurringTask</b> to create a recurring task that checks periodically for 
    /// new updates, assuming the application was deployed using Click Once.
    /// <code lang="cs">
    /// public class VersionUpdateTask : AsyncRecurringTask
    /// {
    ///     private bool _intervalChanged = false;
    ///
    ///     public VersionUpdateTask( int interval ) : base( interval )
    ///     {
    ///     }
    ///    
    ///     public VersionUpdateTask( int interval, int timeout ) : base( interval, timeout )
    ///     {
    ///     }
    ///
    ///     public override object RecurringTask()
    ///     {                        
    ///         bool newVersionAvailable = false;
    ///         
    ///         // Confirm you are running through ClickOnce
    ///         if( ApplicationDeployment.IsNetworkDeployed )
    ///         {
    ///             ApplicationDeployment currentDeploy = ApplicationDeployment.CurrentDeployment;
    ///             
    ///             // Check for available update on the server
    ///             if( currentDeploy.CheckForUpdate() )
    ///                 newVersionAvailable = true;
    ///         }
    ///        
    ///         return newVersionAvailable;
    ///     }
    ///    
    ///     public override void RecurringTaskCompleted( object state )
    ///     {
    ///         bool updateAvailable = Convert.ToBoolean( state );
    ///        
    ///         if( updateAvailable &amp;&amp; !_intervalChanged )
    ///         {
    ///               _intervalChanged = true;
    ///               // Set New Timer Time Interval to 1 hour.
    ///               Change( 3600 );   
    ///         }
    ///     }
    ///
    ///     protected override void OnError( Exception exception )
    ///     {
    ///         base.OnError( exception );
    ///        
    ///         string message = String.Format( "{0} Async Task Exception.", this.GetType() );
    ///         System.Diagnostics.Trace.WriteLine( message );
    ///     }                
    /// }   
    /// </code>
    /// </example>
    public abstract class AsyncRecurringTask : Disposable
    {
        #region State
        /// <summary>
        /// Enumeration of the different States supported by this class.
        /// </summary>
        public enum TaskState
        {
            /// <summary>
            /// Initial state before Start() method is called.
            /// </summary>
            Idle = 0,

            /// <summary>
            /// Asynchronous operation is currently in progress.
            /// </summary>            
            Processing = 1,

            /// <summary>
            /// Start() method has been called, but Asynchronous operation is currently NOT in progress.
            /// </summary>
            Running = 2,

            /// <summary>
            /// Asynchronous task is stopped. Timer is disposed.
            /// </summary>
            Stopped = 3,

            /// <summary>
            /// Asynchronous task is Paused. Timer continues running but asynchronous operation is not executed. 
            /// </summary>          
            Paused = 4
        }
        #endregion

        #region Delegates
        private delegate bool RecurringTaskHandler(out object state);
        private delegate void RecurringTaskCompletedHandler(object state, DateTime timeStamp);

        #endregion

        #region Public Events
        /// <summary>
        /// Occurs when the Asynchronous operation has normally completed.
        /// </summary>
        public event AsyncRecurringTaskCompletedEventHandler Completed;

        /// <summary>
        /// Occurs when the Asynchronous operation is in progress.
        /// </summary>
        public event EventHandler Updating;

        /// <summary>
        /// Occurs when the Asynchronous Task has been Stopped by the user.
        /// </summary>
        public event EventHandler Stopped;

        ///// <summary>
        ///// Occurs when the Asynchronous Task has been Paused by the user.
        ///// </summary>
        //public event EventHandler Paused;

        /// <summary>
        /// Occurs when the asynchronous operation throws an exception, subscribers could react to this event to Log or visually
        /// report to the user the error.
        /// </summary>
        public event ItemEventHandler<Exception> Error;

        #endregion

        #region Private Fields
        /// <summary>
        /// Represents one second as 1000 miliseconds.
        /// </summary>
        protected const int ONE_SECOND = 1000;

        private Timer _timer = null;

        private object _syncLock = new object();
        private object _statusLock = new object();

        private TaskState _state = TaskState.Idle;

        #endregion

        #region Constructors, Destructor
        /// <summary>
        /// Private Default Constructor.
        /// </summary>
        private AsyncRecurringTask()
        { }

        /// <summary>
        /// Creates a new instance of the class and sets the interval value in seconds at which the asynchronous operation will execute. 
        /// </summary>
        /// <param name="interval">Int value that assigns, in seconds, the interval at which the asynchronous operation will execute.</param>
        public AsyncRecurringTask(int interval)
            : this()
        {
            this.Interval = interval;
        }

        /// <summary>
        /// Creates a new instance of the class and sets the interval and time-out value in seconds at which the asynchronous operation will execute and aborted.
        /// </summary>
        /// <param name="interval">Int value that assigns, in seconds, the interval at which the asynchronous operation will execute.</param>
        /// <param name="timeout">Int value that assigns, in seconds, the time-out period at which the asynchronous operation is aborted.</param>
        public AsyncRecurringTask(int interval, int timeout)
            : this()
        {
            this.Interval = interval;
            this.Timeout = timeout;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the time interval in seconds at which the asynchronous operation executes.
        /// </summary>        
        public int Interval { get; private set; }

        /// <summary>
        /// Gets the time interval in seconds at which the asynchronous operation is aborted.
        /// </summary>
        public int Timeout { get; private set; }

        /// <summary>
        /// Gets the Last Time for which the asynchronous operation ran successfully.
        /// </summary>
        public DateTime? LastUpdated { get; private set; }

        /// <summary>
        /// Determines if current state of the Task is <b>TaskState.Processing</b>. 
        /// </summary>
        public bool IsProcessing
        {
            get { return this.State == TaskState.Processing; }
        }

        /// <summary>
        /// Indicates whether Current State is Paused.
        /// </summary>
        public bool IsPaused
        {
            get { return _state == TaskState.Paused; }
        }

        /// <summary>
        /// Gets the current state of the Task.
        /// </summary>
        public TaskState State
        {
            get { lock (_syncLock) { return _state; } }
            private set { lock (_syncLock) { _state = value; } }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Commences execution of the Asynchronous Task.
        /// </summary>
        public virtual void Start()
        {
            if (this.State == TaskState.Idle || this.State == TaskState.Stopped)
            {
                int timeInterval = this.Interval * ONE_SECOND;
                _timer = new Timer(OnTimer, null, 0, timeInterval);
            }

            this._state = TaskState.Running;
        }

        /// <summary>
        /// Stops execution of Asynchronous Task.
        /// </summary>        
        public virtual void Stop()
        {
            if ((_state == TaskState.Paused || _state == TaskState.Running || _state == TaskState.Processing) && !this.IsDisposed)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                _state = TaskState.Stopped;
                Stopped.SafeEventInvoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Pauses execution of Asynchronous Task.
        /// </summary>
        public virtual void Pause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Runs the specified operation synchronously in the same thread as the caller.
        /// </summary>
        /// <remarks>
        /// If the operation produces a result it is returned; otherwise, null (Or Nothing) is returned. Executing this method causes 
        /// <b>Updating</b> event to be raised. Subscribes may react to this event to notify the user the operation is running.
        /// <br /><br />
        /// On completion, <b>Completed</b> event is raised, unless operation fails due to an exception being thrown, in which 
        /// case <b>Completed</b> won’t be raised and LastUpdated property will not update.
        /// </remarks>
        /// <returns>Returns result produced by running the operation, or null (Nothing) if one is not produced.</returns>
        public virtual object RunSync()
        {
            object result = null;

            if (_state != TaskState.Processing)
            {
                lock (_syncLock)
                {
                    if (_state != TaskState.Processing)
                    {
                        TaskState oldState = _state;
                        _state = TaskState.Processing;

                        if (OnRecurringTask(out result))
                        {
                            this.LastUpdated = DateTime.Now;
                            OnRecurringTaskCompleted(result, this.LastUpdated.Value);
                        }

                        _state = oldState;
                    }
                }
            }

            return result;
        }

        #endregion

        #region Private, Protected
        private void OnTimer(object state)
        {
            TraceMessage(String.Format("OnTimer() - Thread Id: {0}.", Thread.CurrentThread.ManagedThreadId));

            if (_state == TaskState.Processing)
            {
                TraceMessage(String.Format("OnTimer() - Still Processing, Thread Id: {0}.", Thread.CurrentThread.ManagedThreadId));
                return;
            }

            lock (_syncLock)
            {
                if (_state != TaskState.Processing)
                {
                    TaskState oldState = _state;
                    _state = TaskState.Processing;

                    RecurringTaskHandler recurringTaskHandler = new RecurringTaskHandler(OnRecurringTask);

                    object result;

                    IAsyncResult asyncResult = recurringTaskHandler.BeginInvoke(out result, null, null);

                    int timeOut = this.Timeout * ONE_SECOND;

                    // Wait until timout expires, or operation completes.
                    if (asyncResult.AsyncWaitHandle.WaitOne(timeOut, false))
                    {

                        bool success = recurringTaskHandler.EndInvoke(out result, asyncResult);
                        this.LastUpdated = DateTime.Now;
                        TraceMessage(String.Format("Last Updated: {0}, Thread Id: {1}", this.LastUpdated, Thread.CurrentThread.ManagedThreadId));

                        if (success)
                        {
                            OnRecurringTaskCompleted(result, this.LastUpdated.Value);
                        }
                    }
                    else
                    {
                        TraceMessage(String.Format("RecurringTaskCompleted() - Timeout, Thread Id: {0}.", Thread.CurrentThread.ManagedThreadId));
                    }

                    // Is case the Stop method was called an a last timer tick was in effect
                    if (_state == TaskState.Processing)
                    {
                        _state = oldState;
                    }
                }
            }
        }

        /// <summary>
        /// This method executes on a separate thread the specified asynchronous operation.
        /// </summary>
        /// <remarks>
        /// If the operation produces a result you may assign this value to the output parameter <b>state</b>. Executing this method causes 
        /// <b>Updating</b> event to be raised. Subscribes may react to this event to notify the user the asynchronous operation is running.
        /// <br /><br />
        /// If execution fails due to an exception, false is returned and <b>Error</b> event is raised; otherwise true is returned. 
        /// Inheritors may decide to override <b>OnError</b> to attain additional control over default error handling mechanism.
        /// <para>
        /// <b>Note to inheritors:</b> it is important that you refrain from overriding the default behavior of this method. This method is 
        /// intrinsic part of the functionality of this class and altering its behavior may lead to undesired results.
        /// </para>
        /// </remarks>
        /// <param name="state">Contains result if operation produces one; otherwise null (Or Nothing) is returned.</param>
        /// <returns>true if no exception were generated while executing the specified operation; otherwise, false.</returns>
        protected virtual bool OnRecurringTask(out object state)
        {
            state = null;
            bool success = true;

            try
            {
                Updating.SafeEventInvoke(this, EventArgs.Empty);
                state = RecurringTask();
            }
            catch (Exception ex)
            {
                OnError(ex);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// This method runs after the specified operation has been processed, it executes <b>RecurringTaskCompleted</b> method and supplies 
        /// the result produced by the specified operation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method raises <b>Completed</b> event and it supplies <b>AsyncRecurringTaskCompletedEventArgs</b> event argument containing 
        /// the result produced by running the specified operation and a timestamp value that indicates that last successful execution. 
        /// </para>
        /// <para>
        /// If execution fails due to an exception, false is returned and <b>Error</b> event is raised; otherwise true is returned. 
        /// Inheritors may decide to override <b>OnError</b> to attain additional control over default error handling mechanism.
        /// </para>
        /// <para>
        /// <b>Note to inheritors:</b> it is important that you refrain from overriding the default behavior of this method. This method is 
        /// intrinsic part of the functionality of this class and altering its behavior may lead to undesired results.
        /// </para>
        /// </remarks>        
        /// <param name="state">Contains result if operation produces one; otherwise null (Or Nothing) is returned.</param>
        /// <param name="timeStamp"></param>
        protected virtual void OnRecurringTaskCompleted(object state, DateTime timeStamp)
        {
            try
            {
                RecurringTaskCompleted(state);

                DateTime lastUpdated = this.LastUpdated ?? DateTime.MinValue;
                if (timeStamp >= lastUpdated)
                {
                    Completed.SafeEventInvoke(this, new AsyncRecurringTaskCompletedEventArgs(state, lastUpdated));
                }
                else
                {
                    TraceMessage(String.Format("Unrecent Update: {0}", timeStamp));
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Changes the interval at which the recurring task is called and timeout.
        /// </summary>
        /// <param name="interval">The time interval, in seconds, at which the recurring task is called.</param>
        /// <param name="timeout">Int value that assigns, in seconds, the time-out period at which the asynchronous operation is aborted.</param>
        protected virtual void ChangeTimeInterval(int interval, int timeout)
        {
            this.Interval = interval;
            this.Timeout = timeout;

            ChangeTimeInterval();
        }

        /// <summary>
        /// Changes the interval at which the recurring task is called.
        /// </summary>
        /// <param name="interval">The time interval at which the recurring task is called.</param>
        protected virtual void ChangeTimeInterval(int interval)
        {
            this.Interval = interval;

            ChangeTimeInterval();
        }

        private void ChangeTimeInterval()
        {
            if (!this.IsDisposed && !(_state == TaskState.Stopped || _state == TaskState.Idle) && _timer != null)
            {
                int timeInterval = this.Interval * ONE_SECOND;
                _timer.Change(timeInterval, timeInterval);
            }
        }

        /// <summary>
        /// Raises Error event with specified Exception.
        /// </summary>
        /// <param name="exception">Exception used to raise Error event.</param>
        protected virtual void OnError(Exception exception)
        {
            TraceMessage(exception.Message);

            // Raise Error Event
            Error.SafeEventInvoke(this, new ItemEventArgs<Exception>(exception));
        }

        /// <summary>
        /// Writes a message to configured System Diagnostics Listeners.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected void TraceMessage(string message)
        {
            string trcMessage = String.Format("{0} RecurringTask().\n {1}.", this.GetType(), message);
            System.Diagnostics.Trace.WriteLine(trcMessage);
        }
        #endregion

        #region Disposable Members
        /// <summary>
        /// Inheritors will implement this method if any clean up is necessary, this method will run once when the class is disposed 
        /// either by the client or GC.
        /// </summary>
        protected override void Cleanup()
        {

        }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Defines the task to execute on a recurring basis.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>Note to inheritors:</b> Override this method to provide your own functionality. 
        /// </para>
        /// </remarks>
        /// <returns>Returns the result produced by running recurring task.</returns>
        public abstract object RecurringTask();

        /// <summary>
        /// Defines a method that will be called on successful completion of the recurring task.
        /// </summary>
        /// <remarks>
        /// This method may be used to provide additional wrap-up functionality, for example logging results to a file, or raising an event.
        /// <para>
        /// <b>Note to inheritors:</b> Override this method to provide your own functionality.
        /// </para>
        /// </remarks>
        /// <param name="state">Contains result if operation produces one; otherwise null (Or Nothing) is returned.</param>
        public abstract void RecurringTaskCompleted(object state);

        #endregion
    }
}
