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

        class SelfClosingTestableRunnable : IRunnable
        {
            private ThreadRunner runner;
            private bool running;
            private bool shouldStop;
            private AutoResetEvent startedEvent = new AutoResetEvent(false);

            public SelfClosingTestableRunnable(ThreadRunner runner)
            {
                this.runner = runner;
            }

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
                while (!shouldStop)
                {
                    runner.Stop();
                }
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

        /// <summary>
        /// A thread calling ThreadRunner.Stop() might end up calling Thread.Join() for the instance representing the thread itself.
        /// In that case it would deadlock, since Thread.Join() blocks the calling thread.This checks that if it happens,
        /// it doesn't deadlock.
        /// </summary>
        [TestMethod]
        public void ThreadSelfStoppingShouldNotDeadlock()
        {
            ThreadRunner runner = new ThreadRunner();
            SelfClosingTestableRunnable runnable = new SelfClosingTestableRunnable(runner);
            runner.Start(runnable);
            Assert.IsTrue(runner.Thread.Join(2000));
            runner.Thread.Interrupt();
        }
    }
}
