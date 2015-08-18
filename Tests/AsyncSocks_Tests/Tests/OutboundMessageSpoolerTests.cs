using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Text;
using System.Collections.Concurrent;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class OutboundMessageSpoolerTests
    {
        private OutboundMessageSpooler spooler;
        private Mock<IOutboundMessageSpoolerRunnable> runnableMock;
        private BlockingCollection<OutboundMessage> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            runnableMock = new Mock<IOutboundMessageSpoolerRunnable>();
            queue = new BlockingCollection<OutboundMessage>(new ConcurrentQueue<OutboundMessage>());
            spooler = new OutboundMessageSpooler(runnableMock.Object, queue);
        }

        [TestMethod]
        public void CreateShouldReturnANewInstance()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var spooler2 = OutboundMessageSpooler.Create(tcpClientMock.Object);
            Assert.IsTrue(spooler2 != null && spooler2 is OutboundMessageSpooler);
        }

        [TestMethod]
        public void InstanceShouldBehaveLikeAThreadRunner()
        {
            Assert.IsTrue(spooler is ThreadRunner);
        }

        [TestMethod]
        public void EnqueueShouldAddMessageToQueue()
        {
            byte[] messageBytes = Encoding.ASCII.GetBytes("This is a test message");
            var message = new OutboundMessage(messageBytes, null);
            spooler.Enqueue(message);

            Assert.AreEqual(message, queue.Take());
        }
        
    }
}
