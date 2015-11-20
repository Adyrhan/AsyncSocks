using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AsyncSocks
{
    public interface IThreadRunner
    {
        bool IsRunning();
        Thread Thread { get; }
        void Start();
        void Start(IRunnable runnable);
        void Stop();
        IRunnable Runnable { get; }
    }
}
