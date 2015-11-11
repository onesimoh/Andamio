using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.ComponentModel;

namespace Andamio.Threading
{
    public static class ThreadingExtensions
    {
        /// <summary>
        /// Executes a delegate and provided arguments while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate method to Invoke.</param>
        /// <param name="args">Arguments to pass into the method to Invoke.</param>
        public static void SafeInvoke(this Delegate del, params object[] args)
        {
            if (del != null)
            {
                DispatcherObject dispatcherObj = del.Target as DispatcherObject;
                if (dispatcherObj != null && !dispatcherObj.CheckAccess())
                {
                    dispatcherObj.Dispatcher.Invoke(del, args);
                    return;
                }

                ISynchronizeInvoke synchronizer = del.Target as ISynchronizeInvoke;
                if (synchronizer != null && synchronizer.InvokeRequired)
                {
                    synchronizer.Invoke(del, args);
                    return;
                }

                //Not requiring thread afinity or invoke is not required
                del.DynamicInvoke(args);
            }
        }

        public static void SafeInvoke(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            eventHandler.SafeInvoke(new object[] { sender, eventArgs });            
        }

        public static void SafeEventInvoke(this EventHandler eventHandler, object sender, EventArgs eventArgs)
        {
            if( eventHandler == null )
            { return; }                        

            foreach (EventHandler sink in eventHandler.GetInvocationList().OfType<EventHandler>())
            {
                sink.SafeInvoke(sender, eventArgs);                
            }
        }

        public static void SafeEventInvoke(this Delegate del, object sender, EventArgs eventArgs)
        {
            if( del == null )
            { return; }                        

            foreach (Delegate sink in del.GetInvocationList())
            {
                sink.SafeInvoke(new object[] { sender, eventArgs });                
            }
        }
    }
}
