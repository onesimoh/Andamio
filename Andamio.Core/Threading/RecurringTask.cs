using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Andamio.Threading
{
    public static class RecurringTaskExtensions
    {
        public static RecurringTask AsRecurring(this Action action, TimeSpan interval, TimeSpan? timeout = null)
        {
            RecurringTask recurring = RecurringTask.New(action, interval, timeout);
            return recurring;
        }
    }


    public class RecurringTask : Disposable
    {
        #region Constructors
        private RecurringTask()
        {
        }

        public RecurringTask(Action methodInvocation, TimeSpan interval, TimeSpan? timeout = null)
        {
            Interval = interval;
            Timeout = timeout;
            MethodInvocation = methodInvocation;
        }

        public static RecurringTask New(Action action, TimeSpan interval, TimeSpan? timeout = null)
        {
            return new RecurringTask(action, interval, timeout);
        }

        #endregion

        #region Properties
        public TimeSpan Interval { get; private set; }
        public TimeSpan? Timeout { get; private set; }
        public Action MethodInvocation { get; private set; }

        #endregion

        #region State
        Timer _timer;
        object _syncLock = new object();

        public RecurringTask Start()
        {
            _timer = new Timer(state =>
            {
                if (!Task.Run(MethodInvocation).Wait(Timeout ?? TimeSpan.FromMilliseconds(-1)))
                {

                }
            }, null, TimeSpan.Zero, Interval);
            
            return this;
        }

        public RecurringTask Stop()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }

            return this;
        }

        #endregion

        #region Dispose
        protected override void Cleanup()
        {
            Stop();
        }

        #endregion
    }
}
