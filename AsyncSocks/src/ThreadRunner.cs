using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSocks
{
    /// <summary>
    /// Used to stop and start threads that run IRunnable objects.
    /// </summary>
    public class ThreadRunner : IThreadRunner
    {
        private IRunnable runnable;
        private Thread thread;

        /// <summary>
        /// The runnable run by this instance.
        /// </summary>
        public IRunnable Runnable { get { return runnable; } }

        /// <summary>
        /// Name of the thread. For debugging purposes.
        /// </summary>
        public string ThreadName { get; set; }

        public ThreadRunner(IRunnable runnable)
        {
            this.runnable = runnable;
        }

        /// <summary>
        /// Indicates that the thread is running.
        /// </summary>
        /// <returns>True if running, false otherwise.</returns>
        public bool IsRunning()
        {
            return runnable.IsRunning;
        }

        /// <summary>
        /// Thread object used by this instance.
        /// </summary>
        public Thread Thread
        {
            get { return thread; }
        }
        
        /// <summary>
        /// Starts the thread. Only returns when the runnable reaches the started state.
        /// </summary>
        public virtual void Start()
        {
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(runnable.Run);
                thread.Name = ThreadName;
                thread.Start();
                runnable.WaitStarted();
            }
        }

        /// <summary>
        /// Stops the thread and waits until it ends running.
        /// </summary>
        public void Stop()
        {
            if (thread != null && thread.IsAlive)
            {
                runnable.Stop();
                thread.Interrupt();
                thread.Join();
            }
        }

    }
}
