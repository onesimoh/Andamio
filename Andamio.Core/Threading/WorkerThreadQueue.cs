using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;

namespace Andamio.Threading
{
    #region State Enum
    /// <summary>
    /// States of the WorkerThreadQueue.
    /// </summary>
    public enum WorkerThreadState
    {
        /// <summary>
        /// State of the WorkerThreadQueue when Killed.
        /// </summary>
        None,

        /// <summary>
        /// Orginal State for the WorkerThreadQueue when instatiated but has not been started.
        /// </summary>
        Idle,

        /// <summary>
        /// State of the WorkerThreadQueue when Paused.
        /// </summary>
        Paused,

        /// <summary>
        /// State of the WorkerThreadQueue when a request to Stop processing items has been sent.
        /// </summary>
        PendingStop,

        /// <summary>
        /// State of the WorkerThreadQueue when Stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// State of the WorkerThreadQueue when items are being processed.
        /// </summary>
        Processing
    }

    public static class WorkerThreadStateExtensions
    {
        public static bool IsDefined(this WorkerThreadState state)
        {
            return state != WorkerThreadState.None;
        }

        public static bool IsIdle(this WorkerThreadState state)
        {
            return state == WorkerThreadState.Idle;
        }

        public static bool IsPaused(this WorkerThreadState state)
        {
            return state == WorkerThreadState.Paused;
        }

        public static bool IsPendingStop(this WorkerThreadState state)
        {
            return state == WorkerThreadState.PendingStop;
        }

        public static bool IsStopped(this WorkerThreadState state)
        {
            return state == WorkerThreadState.Stopped;
        }

        public static bool IsProcessing(this WorkerThreadState state)
        {
            return state == WorkerThreadState.Processing;
        }

        public static bool IsAlive(this WorkerThreadState state)
        {
            return state.IsProcessing() || state.IsPaused();
        }
    }

    #endregion


    /// <summary>
    /// Allows processing of Work Items in a separate Thread. It provides an easy and safe way to Start, Pause, Stop, and Kill this process.
    /// </summary>
    public class WorkerThreadQueue : Disposable, IEnumerable, ICollection
    {
        #region Events
        /// <summary>
        /// Occurs when WorkerThreadQueue is started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// Raises the Started event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>
        protected virtual void OnStarted(EventArgs args)
        {
            if (Started != null)
            {
                Started.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs when WorkerThreadQueue is Stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Raises the Stopped event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnStopped(EventArgs args)
        {
            if (Stopped != null)
            {
                Stopped.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs before WorkerThreadQueue is Stopped.
        /// </summary>        
        public event EventHandler Stopping;

        /// <summary>
        /// Raises the Stopping event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnStopping(EventArgs args)
        {
            if (Stopping != null)
            {
                Stopping.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs when WorkerThreadQueue is Killed.
        /// </summary>        
        public event EventHandler Killed;

        /// <summary>
        /// Raises the Killed event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnKilled(EventArgs args)
        {
            if (Killed != null)
            {
                Killed.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs after WorkerThreadQueue is processed.
        /// </summary>
        public event ItemEventHandler<WorkItem> WorkItemProcessed;

        /// <summary>
        /// Raises the WorkItemProcessed event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnWorkItemProcessed(ItemEventArgs<WorkItem> args)
        {
            if (WorkItemProcessed != null)
            {
                WorkItemProcessed.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs when WorkerThreadQueue has processed all pending items and is wating for additional items to process.
        /// </summary>
        public event EventHandler QueueEmpty;

        /// <summary>
        /// Raises the QueueEmpty event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnQueueEmpty(EventArgs args)
        {
            if (QueueEmpty != null)
            {
                QueueEmpty.SafeEventInvoke(this, args);
            }
        }

        /// <summary>
        /// Occurs when WorkItem(s) are added to WorkerThreadQueue for processing.
        /// </summary>
        public event EventHandler ItemsAdded;

        /// <summary>
        /// Raises the ItemsAdded event.
        /// </summary>
        /// <param name="args">The EventArgs object that contains the event data.</param>        
        protected virtual void OnItemsAdded(EventArgs args)
        {
            if (ItemsAdded != null)
            {
                ItemsAdded.SafeEventInvoke(this, args);
            }
        }

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
        #endregion

        #region Private
        private Queue<WorkItem> _workItemQueue = null;

        private readonly AutoResetEvent _workItemAutoResetEvent = new AutoResetEvent(true);
        private readonly ManualResetEvent _pauseManualResetEvent = new ManualResetEvent(true);

        #endregion

        #region Constructors
        /// <summary>
        /// Default WorkerThreadQueue Construtor.
        /// </summary>
        public WorkerThreadQueue(int throttle = 1, int attempts = 1)
        {
            if (throttle < 1) throw new ArgumentOutOfRangeException("throttle");
            if (attempts < 1) throw new ArgumentOutOfRangeException("attempts");

            RunInBackground = true;
            Throttle = throttle;
            Attempts = attempts;
            _workItemQueue = new Queue<WorkItem>();
        }

        /// <summary>
        /// Constructs a WorkerThreadQueue object and provides a list or WorkItems to process.
        /// </summary>
        /// <param name="workItems">List of WorkItems objects to process by WorkerThreadQueue.</param>
        public WorkerThreadQueue(List<WorkItem> workItems) : this()
        {
            QueueWorkItemRange(workItems);
        }

        #endregion

        #region IEnumerable
        /// <summary>
        /// Returns an enumerator that iterates through a collection. 
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            foreach (WorkItem workItemIter in _workItemQueue)
            {
                yield return workItemIter;
            }
        }
        #endregion

        #region ICollection
        /// <summary>
        /// Copies the Queue elements to an existing one-dimensional Array, starting at the specified array index. 
        /// </summary>
        /// <param name="array">One-dimensional array that is the destination of the items in WorkerThreadQueue.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(Array array, int index)
        {
            if (_workItemQueue == null || _workItemQueue.Count == 0)
                return;

            lock (this.SyncRoot)
            {
                WorkItem[] workItemArray = new WorkItem[_workItemQueue.Count];
                workItemArray.CopyTo(array, index);
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the ICollection is synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize the WorkerThreadQueue instance.
        /// </summary>
        public object SyncRoot
        {
            get { return ((ICollection)_workItemQueue).SyncRoot; }
        }

        /// <summary>
        /// Gets the number of elements contained in the WorkerThreadQueue.
        /// </summary>
        public int Count
        {
            get
            {
                if (_workItemQueue == null)
                { return 0; }

                lock (this.SyncRoot)
                {
                    return _workItemQueue.Count;
                }
            }
        }
        #endregion

        #region Items
        /// <summary>
        /// Returns an array of WorkItems currently in WorkerThreadQueue.
        /// </summary>
        public WorkItem[] Items
        {
            get
            {
                lock (this.SyncRoot)
                {
                    WorkItem[] workItemArray = new WorkItem[this.Count];
                    _workItemQueue.CopyTo(workItemArray, 0);

                    return workItemArray;
                }
            }
        }

        /// <summary>
        /// Returns a bool flag indicating whether WorkerThreadQueue is Empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return this.Count > 0 ? false : true; }
        }

        #endregion

        #region State
        private object _syncRoot = new object();
        private WorkerThreadState _currentState = WorkerThreadState.Idle;
        /// <summary>
        /// Represents the current state of the WorkerThreadQueue instance.
        /// </summary>
        public WorkerThreadState CurrentState
        {
            get
            {
                lock (_syncRoot)
                {
                    return _currentState;
                }
            }
            internal set
            {
                lock (_syncRoot)
                {
                    _currentState = value;
                }
            }
        }

        #endregion

        #region Queue
        /// <summary>
        /// Add a WorkItem to the WorkerThreadQueue.
        /// </summary>
        /// <param name="workItem">Items to add to WorkerThreadQueue.</param>
        public virtual void QueueWorkItem(WorkItem workItem)
        {
            if (workItem == null) throw new ArgumentNullException("workItem");

            lock (this.SyncRoot)
            {
                _workItemQueue.Enqueue(workItem);
            }

            // Signal that a new WorkItem has been added to the queue.
            _workItemAutoResetEvent.Set();

            OnItemsAdded(EventArgs.Empty);
        }

        /// <summary>
        /// Adds a list of items to the WorkerThreadQueue.
        /// </summary>
        /// <param name="workItems">List of WorkItems to add to the WorkerThreadQueue.</param>
        public virtual void QueueWorkItemRange(List<WorkItem> workItems)
        {
            if (workItems == null)
            { throw new ArgumentNullException("workItems"); }

            lock (this.SyncRoot)
            {
                foreach (WorkItem workItemIter in workItems)
                {
                    _workItemQueue.Enqueue(workItemIter);
                }
            }

            // Signal that a new WorkItem has been added to the queue.
            _workItemAutoResetEvent.Set();

            OnItemsAdded(EventArgs.Empty);
        }

        /// <summary>
        /// Created and Adds a WorkItem to the WorkerThreadQueue.
        /// </summary>
        /// <param name="method">Method to use to create the new WorkItem.</param>
        /// <param name="methodArgs">Method Arguments to use to create the new WorkItem.</param>
        /// <returns>WorkItem added to the WorkerThreadQueue.</returns>
        public virtual WorkItem QueueWorkItem(Delegate method, object[] methodArgs)
        {
            WorkItem workItem = new WorkItem(method, methodArgs);

            QueueWorkItem(workItem);

            return workItem;
        }

        /// <summary>
        /// Created and Adds a WorkItem to the WorkerThreadQueue.
        /// </summary>
        /// <param name="asyncState">Object that qualifies or contains information about WorkItem.</param>
        /// <param name="method">Method to use to create the new WorkItem.</param>
        /// <param name="methodArgs">Method Arguments to use to create the new WorkItem.</param>
        /// <returns>WorkItem added to the WorkerThreadQueue.</returns>
        public virtual WorkItem QueueWorkItem(object asyncState, Delegate method, object[] methodArgs)
        {
            WorkItem workItem = new WorkItem(asyncState, method, methodArgs);

            QueueWorkItem(workItem);

            return workItem;
        }

        #endregion

        #region Play
        public bool RunInBackground { get; set; }
        public int Throttle { get; set; }
        private int _attempts;
        public int Attempts
        {
            get { return _attempts; }
            set
            {
                if (value < 1) throw new ArgumentOutOfRangeException("Attempts must be greater than zero.");
                _attempts = value;
            }
        }

        private Thread _workItemThread = null;
        /// <summary>
        /// Begins processing items in the WorkerThreadQueue.
        /// </summary>                
        public virtual void Start()
        {
            if (CurrentState.IsProcessing())
            { return; }

            if (CurrentState.IsPaused())
            {
                CurrentState = WorkerThreadState.Processing;

                // Signal in case Pause was previously called
                _pauseManualResetEvent.Set();

                return;
            }

            CurrentState = WorkerThreadState.Processing;

            _workItemThread = new Thread(ProcessWorkItemQueue) { IsBackground = RunInBackground };
            _workItemThread.Start();

            OnStarted(EventArgs.Empty);
        }

        /// <summary>
        /// Pauses execution of the items in the WorkerThreadQueue.
        /// </summary>
        public void Pause()
        {
            if (CurrentState.IsProcessing())
            {
                // set to non-signaled state which will cause WaitOne() to block 
                // until a signal is sent to continue either by calling Start() again, Stop(), or Kill
                _pauseManualResetEvent.Reset();

                CurrentState = WorkerThreadState.Paused;
            }
        }

        /// <summary>
        /// Stops processing Items in the WorkerThreadQueue.
        /// </summary>
        public virtual void Stop()
        {
            if (CurrentState.IsAlive())
            {
                OnStopping(EventArgs.Empty);

                // Send request to stop iterating through Work Items
                CurrentState = WorkerThreadState.PendingStop;

                if (!IsDisposed)
                {
                    // Signal in case Thread is waiting on new WorkItem to arrive.
                    _workItemAutoResetEvent.Set();

                    // Signal in case Pause was previously called
                    _pauseManualResetEvent.Set();
                }
            }
        }

        /// <summary>
        /// Stops processing Items in the WorkerThreadQueue and kills the underlying thread.
        /// </summary>
        /// <remarks>This method blocks caller until WorkerThreadQueue Stops processing current item.</remarks>
        public virtual void Kill()
        {
            if (_workItemThread != null && _workItemThread.IsAlive)
            {
                // Stop processing items prior to killing the thread queue, 
                Stop();

                // We wait for the thread the finish, this operation blocks the caller
                // until it is safe to return
                _workItemThread.Join();
            }

            Reset();

            CurrentState = WorkerThreadState.None;

            OnKilled(EventArgs.Empty);
        }

        /// <summary>
        /// Clears all items in the WorkerThreadQueue and resets Total Process Count.
        /// </summary>
        public virtual void Reset()
        {
            lock (this.SyncRoot)
            {
                if (_workItemQueue != null && _workItemQueue.Count > 0)
                {
                    _workItemQueue.Clear();
                }
            }

            TotalProcessed = 0;
        }
        #endregion

        #region Process
        /// <summary>
        /// Returns a count of all items processed by the instance.
        /// </summary>       
        public int TotalProcessed { get; private set; }        

        private readonly AutoResetEvent _throttleAutoResetEvent = new AutoResetEvent(false);
        private int _activeCount = 0;

        private void ProcessWorkItemQueue()
        {
            while (!CurrentState.IsPendingStop())
            {
                while (!IsEmpty)
                {
                    // if a request to pause has been sent this will block until a signal is sent
                    // to either Resume execution or Stop
                    _pauseManualResetEvent.WaitOne();

                    // If a request to stop has been sent we stop iterating through the list
                    // and set IsAlive flag to false
                    if (CurrentState.IsPendingStop())
                    {
                        CurrentState = WorkerThreadState.Stopped;
                        OnStopped(EventArgs.Empty);
                        return;
                    }                    

                    WorkItem workItem = GetNext();
                    if (workItem == null) continue;

                    workItem.Completed += OnWorkItemCompleted;
                    workItem.Error += OnWorkItemError;
                    workItem.RunAsync();
                    Interlocked.Increment(ref _activeCount);

                    // If Throttle is reached wait for a cycle to open, wait until signaled
                    if (Throttle == 1 || _activeCount >= Throttle)
                    {
                        _throttleAutoResetEvent.WaitOne();
                    }
                }

                // Event that signal Queue is waiting for additonal items to process.
                OnQueueEmpty(EventArgs.Empty);

                // Check state of Stop Request in case one was made; otherwise we run the risk of waiting
                // indefinetely for the user to Stop the queue once more, notice that right after
                // the quue wil wait for a signal, thus we won exit the loop unless one is received.
                if (CurrentState.IsPendingStop())
                { break; }

                // Reset to non-signaled state to wait for a new signal indicating new items in the queue
                _workItemAutoResetEvent.Reset();

                // Signal that end of queue has been reached, thread hangs until a new item is added.
                _workItemAutoResetEvent.WaitOne();
            }

            CurrentState = WorkerThreadState.Stopped;
            OnStopped(EventArgs.Empty);
        }

        void OnWorkItemError(object sender, ItemEventArgs<Exception> e)
        {
            WorkItem workItem = (WorkItem) sender;
            if (workItem.Attempts < Attempts)
            {
                workItem.Attempts++;
                QueueWorkItem(workItem);
            }

            FinalizeWorkItem(workItem);
            OnError(e);
        }

        void OnWorkItemCompleted(object sender, EventArgs e)
        {
            WorkItem workItem = (WorkItem) sender;

            FinalizeWorkItem(workItem);
            OnWorkItemProcessed(new ItemEventArgs<WorkItem>(workItem));
        }

        void FinalizeWorkItem(WorkItem workItem)
        {
            TotalProcessed++;
            Interlocked.Decrement(ref _activeCount);

            // If available cycles, then signal to continue in case process is waiting.
            if (Throttle == 1 || _activeCount < Throttle)
            {
                _throttleAutoResetEvent.Set();
            }
        }

        private WorkItem GetNext()
        {
            if (IsEmpty)
            { return null; }

            WorkItem workItem = null;
            lock (this.SyncRoot)
            {
                if (_workItemQueue != null && _workItemQueue.Any())
                {
                    workItem = _workItemQueue.Dequeue();
                }
            }

            return workItem;
        }

        #endregion

        #region Disposable
        /// <summary>
        /// Releases utilized resources.
        /// </summary>
        protected override void Cleanup()
        {
            if (_workItemAutoResetEvent != null)
            {
                _workItemAutoResetEvent.Close();
            }
            if (_pauseManualResetEvent != null)
            {
                _pauseManualResetEvent.Close();
            }
        }
        #endregion
    }
}
