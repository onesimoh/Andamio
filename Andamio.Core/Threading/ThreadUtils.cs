using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Andamio.Threading
{
    /// <summary>
    /// Provides helper methods to assist Invoking Delegates.
    /// </summary>
    /// <remarks>
    /// DEPRECATED: Use ThreadExtensions which also has support for WPF dispatcher object calls.
    /// </remarks>
    [Obsolete]
    public static class ThreadUtils
    {
        /// <summary>
        /// Executes a delegate and provided arguments while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate method to Invoke.</param>
        /// <param name="args">Arguments to pass into the method to Invoke.</param>
        public static void InvokeDelegate(Delegate del, params object[] args)
        {
            if (del != null)
            {
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

        /// <summary>
        /// Executes a parameter-less delegate while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate method to Invoke.</param>
        public static void InvokeDelegate(Delegate del)
        {
            InvokeDelegate(del, new object[] { });
        }

        /// <summary>
        /// Executes a delegate of type GenericEventHandler while providing thread afinity.
        /// </summary>
        /// <param name="eventHandler">Delegate method to Invoke.</param>
        public static void Fire(GenericEventHandler eventHandler)
        {
            SafeEventInvoke(eventHandler);
        }

        /// <summary>
        /// Invokes each delegate in the Invocation List while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate method to Invoke.</param>
        public static void SafeEventInvoke(Delegate del)
        {
            if (del != null)
            {                
                foreach (Delegate sink in del.GetInvocationList())
                {
                    try
                    {
                        InvokeDelegate(sink, new object[] { });
                    }
                    catch
                    { }
                }
            }
        }

        /// <summary>
        /// Invokes each delegate in the Invocation List while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate method to Invoke.</param>
        /// <param name="args">Arguments to pass into the method to Invoke.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void SafeEventInvoke(Delegate del, params object[] args)
        {
            if (del == null)
            { return; }            

            foreach (Delegate sink in del.GetInvocationList())
            {
                try
                {
                    InvokeDelegate(sink, args);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("UnsafeFire Error [{0}] - {1}.", e.GetType(), e.Message));
                }
            }
        }

        /// <summary>
        /// Invokes each delegate async in the Invocation List Asynchronously while providing thread afinity.
        /// </summary>
        /// <param name="del">Delegate to Invoke.</param>
        /// <param name="args">Arguments to pass into the method to Invoke.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InvokeDelegateAsync(Delegate del, params object[] args)
        {
            if (del == null)
            { return; }

            AsyncCallback cleanUp = delegate(IAsyncResult asyncResult)
            {
                asyncResult.AsyncWaitHandle.Close();
            };

            AsyncFire asyncFire = new AsyncFire(InvokeDelegate);

            foreach (Delegate sink in del.GetInvocationList())
            {
                asyncFire.BeginInvoke(sink, args, cleanUp, null);
            }
        }
    }
}
