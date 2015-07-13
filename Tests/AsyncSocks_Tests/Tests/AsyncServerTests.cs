using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Collections.Generic;
using System.Net;
using Moq;
using System.Text;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncServerTests
    {
        private AsyncServer server;
        private Mock<IConnectionManager> connectionManagerMock;
        private Mock<IClientConnectionAgent> clientConnectionAgentMock;
        private Mock<ITcpListener> tcpListenerMock;
        
        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            connectionManagerMock = new Mock<IConnectionManager>();
            clientConnectionAgentMock = new Mock<IClientConnectionAgent>();
            server = new AsyncServer(clientConnectionAgentMock.Object, connectionManagerMock.Object, tcpListenerMock.Object);
        }
        
        [TestMethod]
        public void StartShouldStartClientConnectionAgentToListenForNewConnections()
        {
            clientConnectionAgentMock.Setup(x => x.Start()).Verifiable();
            server.Start();
            clientConnectionAgentMock.Verify();
        }

        [TestMethod]
        public void StopShouldStopClientConnectionAgentAndCloseAllConnectionsInConnectionManager()
        {
            clientConnectionAgentMock.Setup(x => x.Stop()).Verifiable();
            connectionManagerMock.Setup(x => x.CloseAllConnetions()).Verifiable();
            server.Stop();
            clientConnectionAgentMock.Verify();
            connectionManagerMock.Verify();
        }

        [TestMethod]
        public void OnClientMessageReceivedCallbackShouldBeCalledWhenConnectionManagerFiresTheEvent()
        {
            var peerConnectionMock = new Mock<IPeerConnection>();
            var messageReceivedEvent = new AutoResetEvent(false);

            NewClientMessageDelegate newMessage = delegate(IPeerConnection sender, byte[] message)
            {
                messageReceivedEvent.Set();
            };

            server.OnNewMessageReceived += newMessage;

            connectionManagerMock.Raise(
                x => x.OnNewClientMessageReceived += null, 
                peerConnectionMock.Object, 
                Encoding.ASCII.GetBytes("Test message!")
            );

            Assert.IsTrue(messageReceivedEvent.WaitOne(2000));

        }
    }
}
