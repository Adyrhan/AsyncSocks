﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Collections.Concurrent;
using Moq;
using System.Text;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class MessagePollerRunnableTests
    {
        private IMessagePollerRunnable<byte[]> runnable;
        private BlockingCollection<ReadResult<byte[]>> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            queue = new BlockingCollection<ReadResult<byte[]>>();
            runnable = new MessagePollerRunnable<byte[]>(queue);
        }

        [TestMethod]
        public void PollShouldGetAMessageFromQueueAndRaiseOnNewMessageReceivedEvent()
        {
            var connectionMock = new Mock<IAsyncClient<byte[]>>();
            var originalMessage = Encoding.ASCII.GetBytes("This is a message");
            var callbackCalledEvent = new AutoResetEvent(false);

            var callback = new NewMessageReceived<byte[]>((object sender, NewMessageReceivedEventArgs<byte[]> e) =>
            {
                Assert.IsNull(e.Sender);
                Assert.IsNotNull(sender);
                Assert.AreEqual(originalMessage, e.Message);
                callbackCalledEvent.Set();
            });

            runnable.OnNewMessageReceived += callback;

            queue.Add(new ReadResult<byte[]>(originalMessage));

            runnable.Poll();

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }

        [TestMethod]
        public void RunShouldCallPool()
        {
            var runner = new ThreadRunner(runnable);
            var connectionMock = new Mock<IAsyncClient<byte[]>>();
            var originalMessage = Encoding.ASCII.GetBytes("This is a message");
            var callbackCalledEvent = new AutoResetEvent(false);

            IAsyncClient<byte[]> receivedSender = null;
            byte[] receivedMessage = null;

            var callback = new NewMessageReceived<byte[]>((object sender, NewMessageReceivedEventArgs<byte[]> e) =>
            {
                receivedMessage = e.Message;
                receivedSender = e.Sender;

                callbackCalledEvent.Set();
                //Assert.AreEqual(connectionMock.Object, e.Sender);
                //Assert.AreEqual(originalMessage, e.Message);
            });
            
            runnable.OnNewMessageReceived += callback;

            queue.Add(new ReadResult<byte[]>(originalMessage));

            Assert.IsFalse(runnable.IsRunning);

            runner.Start();

            Assert.IsTrue(runnable.IsRunning);
            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));

            Assert.IsNull(receivedSender);
            Assert.AreEqual(originalMessage, receivedMessage);

            runner.Stop();
        }

        [TestMethod]
        public void ShouldFireOnErrorEventWheneverMessageObjectContainsError()
        {
            Exception receivedError = null;
            var expectedError = new Exception("Fake test error");
            runnable.OnReadError += (object sender, ReadErrorEventArgs e) =>
            {
                receivedError = e.Error;
            };
            queue.Add(new ReadResult<byte[]>(expectedError));
            runnable.Poll();
            Assert.AreSame(expectedError, receivedError);
        }
    }
}
