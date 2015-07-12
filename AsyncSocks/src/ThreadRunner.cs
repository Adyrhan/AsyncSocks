using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSocks
{
    public class ThreadRunner : IThreadRunner
    {
        private IRunnable runnable;
        private Thread thread;

        public ThreadRunner(IRunnable runnable)
        {
            this.runnable = runnable;
        }

        public bool IsRunning()
        {
            return runnable.IsRunning;
        }

        public Thread Thread
        {
            get { return thread; }
        }

        public IRunnable Runnable { get { return runnable; } }

        public void Start()
        {
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(runnable.Run);
                thread.Start();
                runnable.WaitStarted();
            }
        }

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
