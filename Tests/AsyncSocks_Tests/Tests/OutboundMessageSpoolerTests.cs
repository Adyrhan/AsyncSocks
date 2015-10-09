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
        private OutboundMessageSpooler<byte[]> spooler;
        private Mock<IOutboundMessageSpoolerRunnable> runnableMock;
        private BlockingCollection<OutboundMessage<byte[]>> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            runnableMock = new Mock<IOutboundMessageSpoolerRunnable>();
            queue = new BlockingCollection<OutboundMessage<byte[]>>(new ConcurrentQueue<OutboundMessage<byte[]>>());
            spooler = new OutboundMessageSpooler<byte[]>(runnableMock.Object, queue);
        }

        [TestMethod]
        public void CreateShouldReturnANewInstance()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var spooler2 = OutboundMessageSpooler<byte[]>.Create(tcpClientMock.Object);
            Assert.IsTrue(spooler2 != null && spooler2 is OutboundMessageSpooler<byte[]>);
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
            var message = new OutboundMessage<byte[]>(messageBytes, null);
            spooler.Enqueue(message);

            Assert.AreEqual(message, queue.Take());
        }
        
    }
}
