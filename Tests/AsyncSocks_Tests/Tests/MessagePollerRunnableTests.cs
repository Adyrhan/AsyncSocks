using System;
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
        private IMessagePollerRunnable runnable;
        private BlockingCollection<NetworkMessage> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            queue = new BlockingCollection<NetworkMessage>();
            runnable = new MessagePollerRunnable(queue);
        }

        [TestMethod]
        public void PollShouldGetAMessageFromQueueAndRaiseOnNewMessageReceivedEvent()
        {
            var connectionMock = new Mock<IAsyncClient>();
            var originalMessage = Encoding.ASCII.GetBytes("This is a message");
            var callbackCalledEvent = new AutoResetEvent(false);

            var callback = new NewClientMessageReceived(delegate(IAsyncClient sender, byte[] message)
            {
                callbackCalledEvent.Set();
                Assert.AreEqual(connectionMock.Object, sender);
                Assert.AreEqual(originalMessage, message);
            });

            runnable.OnNewMessageReceived += callback;

            queue.Add(new NetworkMessage(connectionMock.Object, originalMessage));

            runnable.Poll();

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }

        [TestMethod]
        public void RunShouldCallPool()
        {
            var runner = new ThreadRunner(runnable);
            var connectionMock = new Mock<IAsyncClient>();
            var originalMessage = Encoding.ASCII.GetBytes("This is a message");
            var callbackCalledEvent = new AutoResetEvent(false);

            var callback = new NewClientMessageReceived(delegate(IAsyncClient sender, byte[] message)
            {
                callbackCalledEvent.Set();
                Assert.AreEqual(connectionMock.Object, sender);
                Assert.AreEqual(originalMessage, message);
            });
            
            runnable.OnNewMessageReceived += callback;

            queue.Add(new NetworkMessage(connectionMock.Object, originalMessage));

            Assert.IsFalse(runnable.IsRunning);

            runner.Start();

            Assert.IsTrue(runnable.IsRunning);
            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }
    }
}
