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
        private BlockingCollection<byte[]> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            runnableMock = new Mock<IOutboundMessageSpoolerRunnable>();
            var runnable = runnableMock.Object;
            queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            spooler = new OutboundMessageSpooler(runnable, queue);
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
            
            spooler.Enqueue(messageBytes);

            Assert.AreEqual(messageBytes, queue.Take());
        }
        
    }
}
