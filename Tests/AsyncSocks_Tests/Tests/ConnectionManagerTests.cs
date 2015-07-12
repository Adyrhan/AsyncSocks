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
        private Dictionary<IPEndPoint, IPeerConnection> dict;
        private Mock<IPeerConnectionFactory> connectionFactoryMock;

        [TestInitialize]
        public void BeforeEach()
        {
            dictMock = new Mock<Dictionary<IPEndPoint, PeerConnection>>();
            dict = new Dictionary<IPEndPoint, IPeerConnection>();
            connectionFactoryMock = new Mock<IPeerConnectionFactory>();
            connManager = new ConnectionManager(dict, connectionFactoryMock.Object);
        }
        
        [TestMethod]
        public void AddShouldCreateANewPeerConnectionFromTcpClientAndAddsItToItsList()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var socketMock = new Mock<ISocket>();
            var peerConnectionMock = new Mock<IPeerConnection>();
            
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);

            connectionFactoryMock.Setup(
                x => x.Create(
                    It.IsAny<IInboundMessageSpooler>(), 
                    It.IsAny<IOutboundMessageSpooler>(), 
                    tcpClientMock.Object
                )
            ).Returns(peerConnectionMock.Object).Verifiable();

            tcpClientMock.Setup(x => x.Client).Returns(socketMock.Object).Verifiable();
            socketMock.Setup(x => x.RemoteEndPoint).Returns(endPoint).Verifiable();

            peerConnectionMock.Setup(x => x.StartSpoolers()).Verifiable();
            peerConnectionMock.Setup(x => x.RemoteEndPoint).Returns(endPoint);

            connManager.Add(tcpClientMock.Object);

            tcpClientMock.Verify();
            socketMock.Verify();
            connectionFactoryMock.Verify();
            peerConnectionMock.Verify();

            Assert.AreEqual(endPoint, dict[endPoint].RemoteEndPoint); 
        }

    }
}
