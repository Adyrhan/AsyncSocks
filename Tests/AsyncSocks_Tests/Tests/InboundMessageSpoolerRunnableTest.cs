﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using AsyncSocks_Tests.Helpers;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class InboundMessageSpoolerRunnableTest
    {

        private InboundMessageSpoolerRunnable spooler;
        private BlockingCollection<NetworkMessage> queue;
        private Mock<INetworkMessageReader> readerMock;

        [TestInitialize]
        public void BeforeEach()
        {
            readerMock = new Mock<INetworkMessageReader>();
            queue = new BlockingCollection<NetworkMessage>(new ConcurrentQueue<NetworkMessage>());
            spooler = new InboundMessageSpoolerRunnable(readerMock.Object, queue);
        }

        [TestMethod]
        public void SpoolShouldAddMessageInQueue()
        {
            string messageString = "This is a test message";

            readerMock.Setup(x => x.Read()).Returns(Encoding.ASCII.GetBytes(messageString));

            spooler.Spool();

            string storedMessage = Encoding.ASCII.GetString(queue.Take().Message);

            Assert.AreEqual(messageString, storedMessage);
        }

        [TestMethod]
        public void ShouldImplementIRunnableAndIInboundMessageSpoolerRunnable()
        {
            Assert.IsTrue(spooler is IRunnable && spooler is IInboundMessageSpoolerRunnable);
        }

        [TestMethod]
        public void StopShouldStopSpooler()
        {
            ThreadRunner runner = new ThreadRunner(spooler);
            AsyncStoppingThreadRunner asyncRunner = new AsyncStoppingThreadRunner(runner);
            runner.Start();
            asyncRunner.Stop();
            runner.Thread.Join(2000);
            Assert.IsFalse(runner.Thread.IsAlive);
        }

        [TestMethod]
        public void RunShouldCallSpool()
        {
            AutoResetEvent spoolCalledEvent = new AutoResetEvent(false);
            readerMock.Setup(x => x.Read()).Returns(new byte[] { 0 }).Callback(() => spoolCalledEvent.Set());
            
            ThreadRunner runner = new ThreadRunner(spooler);
            runner.Start();
            Assert.IsTrue(spoolCalledEvent.WaitOne(2000));
            runner.Stop();

        }
    }
}
