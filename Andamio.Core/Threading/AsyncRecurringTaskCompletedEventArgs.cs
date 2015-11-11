using System;
using System.Collections.Generic;
using System.Text;

namespace Andamio.Threading
{
    /// <summary>
    /// Represents the method that will handle the Completed event of the AsyncRecurringTask class.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A AsyncRecurringTaskCompletedEventArgs that contains the event data.</param>
    public delegate void AsyncRecurringTaskCompletedEventHandler(object sender, AsyncRecurringTaskCompletedEventArgs e);

    /// <summary>
    /// Provides Data for Completed event of the AsyncRecurringTask class.
    /// </summary>
    public class AsyncRecurringTaskCompletedEventArgs : EventArgs
    {
        private AsyncRecurringTaskCompletedEventArgs() : base()
        { }

        /// <summary>
        /// Initializes a new instance of the AsyncRecurringTaskCompletedEventArgs class. 
        /// </summary>
        /// <param name="result">Object that results from completing the Task.</param>
        /// <param name="lastUpdated">Time when Task ran sucessfully.</param>
        public AsyncRecurringTaskCompletedEventArgs(object result, DateTime lastUpdated) : this()
        {
            Result = result;
            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Gets the object that results from completing the Task.
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// Gets the Time when Task ran sucessfully.
        /// </summary>
        public DateTime LastUpdated  { get; private set; }
    }
}
