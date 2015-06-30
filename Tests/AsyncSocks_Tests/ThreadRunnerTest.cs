using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Threading;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class ThreadRunnerTest
    {
        class TestableRunnable : IRunnable
        {
            private bool running;
            private bool shouldStop;
            private AutoResetEvent startedEvent = new AutoResetEvent(false);

            public bool IsRunning
            {
                get
                {
                    return running;
                }
            }

            public void Run()
            {
                running = true;
                startedEvent.Set();
                while (!shouldStop) {}
                running = false;
            }

            public void Stop()
            {
                shouldStop = true;
            }

            public bool WaitStarted()
            {
                return startedEvent.WaitOne();
            }
        }
           
        [TestMethod]
        public void StartAndStopShouldStartAndStopThread()
        {
            ThreadRunner runner = new ThreadRunner(new TestableRunnable());
            Assert.IsFalse(runner.IsRunning());
            runner.Start();
            Assert.IsTrue(runner.IsRunning());
            runner.Stop();
            Assert.IsFalse(runner.IsRunning());
        }


    }
}
