using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class OutboundMessageSpoolerTests
    {
        private OutboundMessageSpooler spooler;
        private Mock<IOutboundMessageSpoolerRunnable> runnableMock;

        [TestInitialize]
        public void BeforeEach()
        {
            runnableMock = new Mock<IOutboundMessageSpoolerRunnable>();
            var runnable = runnableMock.Object;
            spooler = new OutboundMessageSpooler(runnable);
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


        
    }
}
