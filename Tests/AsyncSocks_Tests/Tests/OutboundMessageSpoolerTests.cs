using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class OutboundMessageSpoolerTests
    {
        [TestMethod]
        public void CreateShouldReturnANewInstance()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            OutboundMessageSpooler spooler = OutboundMessageSpooler.Create(tcpClientMock.Object);
            Assert.IsTrue(spooler != null && spooler is OutboundMessageSpooler);
        }

        [TestMethod]
        public void InstanceShouldBehaveLikeAThreadRunner() 
        {
            var tcpClientMock = new Mock<ITcpClient>();
            OutboundMessageSpooler spooler = OutboundMessageSpooler.Create(tcpClientMock.Object);
            Assert.IsTrue(spooler is ThreadRunner);
        }

    }
}
