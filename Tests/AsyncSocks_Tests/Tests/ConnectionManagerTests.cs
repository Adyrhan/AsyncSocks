using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class ConnectionManagerTests
    {
        private IConnectionManager connManager;
        private Dictionary<IPEndPoint, IPeerConnection> dict;
        private Mock<IPeerConnectionFactory> connectionFactoryMock;
        private Mock<IMessagePoller> messagePollerMock;

        [TestInitialize]
        public void BeforeEach()
        {
            messagePollerMock = new Mock<IMessagePoller>();
            dict = new Dictionary<IPEndPoint, IPeerConnection>();
            connectionFactoryMock = new Mock<IPeerConnectionFactory>();
            connManager = new ConnectionManager(dict, connectionFactoryMock.Object, messagePollerMock.Object);
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

        [TestMethod]
        public void CloseAllConnectionsShouldCloseConnections()
        {
            var connections = new List<ClientSocketConnectionFixture>();
            for(int i = 0; i < 10; i++)
            {
                var client = new Mock<ITcpClient>();
                var socket = new Mock<ISocket>();
                var conn = new Mock<IPeerConnection>();

                client.Setup(x => x.Client).Returns(socket.Object);
                socket.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80." + i.ToString()), 80));
                
                conn.Setup(x => x.Close()).Verifiable();

                connectionFactoryMock.Setup(
                    x => x.Create(
                        It.IsAny<IInboundMessageSpooler>(),
                        It.IsAny<IOutboundMessageSpooler>(),
                        client.Object
                    )
                ).Returns(conn.Object);

                
                ClientSocketConnectionFixture fixture;
                fixture.client = client;
                fixture.connection = conn;
                fixture.socket = socket;

                connections.Add(fixture);

                connManager.Add(client.Object);
            }
            connManager.CloseAllConnetions();

            foreach(ClientSocketConnectionFixture conn in connections)
            {
                conn.client.Verify();
                conn.socket.Verify();
                conn.connection.Verify();
            }
        }

        [TestMethod]
        public void OnNewClientMessageReceivedCallbacksShouldBeCalledWhenEventIsFiredByAPeerConnectionInstance()
        {
            var callbackCalledEvent = new AutoResetEvent(true);
            var tcpClientMock = new Mock<ITcpClient>();
            var socketMock = new Mock<ISocket>();
            var peerConnectionMock = new Mock<IPeerConnection>();
            
            connectionFactoryMock.Setup(
                x => x.Create(
                    It.IsAny<IInboundMessageSpooler>(), 
                    It.IsAny<IOutboundMessageSpooler>(), 
                    tcpClientMock.Object
                )
            ).Returns(peerConnectionMock.Object).Verifiable();

            tcpClientMock.Setup(x => x.Client).Returns(socketMock.Object);
            socketMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80));

            connManager.Add(tcpClientMock.Object);

            NewClientMessageDelegate callback = delegate(IPeerConnection sender, byte[] message)
            {
                callbackCalledEvent.Set();
            };

            connManager.OnNewClientMessageReceived += callback;

            messagePollerMock.Raise(x => x.OnNewClientMessageReceived += null, peerConnectionMock.Object, Encoding.ASCII.GetBytes("This is a test!"));

            bool called = callbackCalledEvent.WaitOne(2000);

            connectionFactoryMock.Verify();
            Assert.IsTrue(called);
        }
    }

    public struct ClientSocketConnectionFixture
    {
        public Mock<ITcpClient> client;
        public Mock<ISocket> socket;
        public Mock<IPeerConnection> connection;
    }
}
