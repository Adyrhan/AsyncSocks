using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Collections.Concurrent;
using Moq;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class MessagePollerTests
    {
        private IMessagePoller poller;
        private BlockingCollection<byte[]> queue;
        private Mock<IMessagePollerRunnable> runnable;

        [TestInitialize]
        public void BeforeEach()
        {
            queue = new BlockingCollection<byte[]>();
            runnable = new Mock<IMessagePollerRunnable>();
            poller = new MessagePoller(runnable.Object);
        }


        [TestMethod]
        public void OnNewClientMessageReceivedFiresUpWhenTheRunnableOneDoes()
        {
            var callbackCalledEvent = new AutoResetEvent(false);

            NewMessageReceived callback = delegate(IAsyncClient sender, byte[] message)
            {
                callbackCalledEvent.Set();

            };

            poller.OnNewClientMessageReceived += callback;

            runnable.Raise(x => x.OnNewMessageReceived += null, null, null);

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }
    }
}
