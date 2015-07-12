using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    public interface IRunnable
    {
        void Run();
        void Stop();
        bool IsRunning { get; } 
        bool WaitStarted(); // Should block until the thread has started
    }
}
