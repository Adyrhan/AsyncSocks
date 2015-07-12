using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Collections.Generic;
using System.Net;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class ConnectionManagerTests
    {
        private ConnectionManager connManager;
        private Mock<Dictionary<IPEndPoint, PeerConnection>> dictMock;
        private Dictionary<IPEndPoint, PeerConnection> dict;

        [TestInitialize]
        public void BeforeEach()
        {
            dictMock = new Mock<Dictionary<IPEndPoint, PeerConnection>>();
            dict = new Dictionary<IPEndPoint, PeerConnection>();
            connManager = new ConnectionManager(dict);
        }
        
        [TestMethod]
        public void AddCreatesANewPeerConnectionAndAddsItToItsList()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var socketMock = new Mock<ISocket>();
            
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);
            tcpClientMock.Setup(x => x.Client).Returns(socketMock.Object).Verifiable();
            socketMock.Setup(x => x.RemoteEndPoint).Returns(endPoint).Verifiable();

            connManager.Add(tcpClientMock.Object);

            tcpClientMock.Verify();
            socketMock.Verify();
            Assert.AreEqual(endPoint, dict[endPoint].RemoteEndPoint); 
        }
    }
}
