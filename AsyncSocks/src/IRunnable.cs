using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AsyncSocks
{
    /// <summary>
    /// Interface for the implementation of a Runnable object.
    /// This objects represent a task that runs forever, until stopping is requested.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        /// Holds the code to run for the task
        /// </summary>
        void Run();
        /// <summary>
        /// Sets the runnable into stopping state.
        /// </summary>
        void Stop();
        /// <summary>
        /// Returns true if the runnable is being run by a thread.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// This method blocks until the runnable is considered ready and running.
        /// </summary>
        /// <returns>True if started, false otherwise.</returns>
        bool WaitStarted(); // Should block until the thread has started
    }
}
