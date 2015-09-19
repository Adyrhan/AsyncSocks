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
        private IAsyncServer server;
        private Mock<IConnectionManager> connectionManagerMock;
        private Mock<IClientConnectionAgent> clientConnectionAgentMock;
        private Mock<ITcpListener> tcpListenerMock;
        private ClientConfig clientConfig;
        
        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            connectionManagerMock = new Mock<IConnectionManager>();
            clientConnectionAgentMock = new Mock<IClientConnectionAgent>();
            clientConfig = new ClientConfig(10 * 1024 * 1024);
            server = new AsyncServer(clientConnectionAgentMock.Object, connectionManagerMock.Object, tcpListenerMock.Object, clientConfig);
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
            connectionManagerMock.Setup(x => x.CloseAllConnections()).Verifiable();
            server.Stop();
            clientConnectionAgentMock.Verify();
            connectionManagerMock.Verify();
        }

        [TestMethod]
        public void OnClientMessageReceivedCallbackShouldBeCalledWhenConnectionManagerFiresTheEvent()
        {
            var peerConnectionMock = new Mock<IAsyncClient>();
            var messageReceivedEvent = new AutoResetEvent(false);

            NewMessageReceived newMessage = (object sender, NewMessageReceivedEventArgs e) => messageReceivedEvent.Set();

            server.OnNewMessageReceived += newMessage;

            var ev = new NewMessageReceivedEventArgs(peerConnectionMock.Object, Encoding.ASCII.GetBytes("Test message!"));

            connectionManagerMock.Raise(x => x.OnNewMessageReceived += null, ev);

            Assert.IsTrue(messageReceivedEvent.WaitOne(2000));

        }

        [TestMethod]
        public void OnNewClientConnectedShouldFireUpWhenClientConnectionAgentDoes()
        {
            var callbackCalledEvent = new AutoResetEvent(false);
            var peerConnectionMock = new Mock<IAsyncClient>();

            IAsyncClient peerConnectionArgument = null;

            var callback = new NewClientConnected((object sender, NewClientConnectedEventArgs e) =>
            {
                peerConnectionArgument = e.Client;
                callbackCalledEvent.Set();

            });

            server.OnNewClientConnected += callback;

            connectionManagerMock.Setup(x => x.Add(peerConnectionMock.Object)).Verifiable();

            var ev = new NewClientConnectedEventArgs(peerConnectionMock.Object);

            clientConnectionAgentMock.Raise(x => x.OnNewClientConnection += null, ev);
            

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000), "Delegate not called");
            Assert.AreEqual(peerConnectionMock.Object, peerConnectionArgument);

            connectionManagerMock.Verify();
        }

        [TestMethod]
        public void StaticFactoryCreateShouldCreateInstance()
        {
            var instance = AsyncServer.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80));

            Assert.IsTrue(instance != null && instance is AsyncServer);
        }

        [TestMethod]
        public void OnPeerDisconnectedEventShouldFireWhenConnectionManagerDoesIt()
        {
            AutoResetEvent callbackCalledEvent = new AutoResetEvent(false);

            IAsyncClient peerArgument = null;
            server.OnPeerDisconnected += new PeerDisconnected((object sender, PeerDisconnectedEventArgs e) =>
            {
                peerArgument = e.Peer;
                callbackCalledEvent.Set();
            });

            var peerConnectionMock = new Mock<IAsyncClient>();

            var ev = new PeerDisconnectedEventArgs(peerConnectionMock.Object);

            connectionManagerMock.Raise(x => x.OnPeerDisconnected += null, ev);

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
            Assert.AreEqual(peerConnectionMock.Object, peerArgument, "PeerConnection instance is not the same as expected");
        }

        [TestMethod]
        public void ClientConfigPropertyGetsClientConfigObject()
        {
            Assert.AreEqual(clientConfig, server.ClientConfig);
        }

        //[TestMethod]
        //public void ClientConfigPropertyCanOnlyBeSetBeforeCallingStartMethod()
        //{
        //    var clientConfig = new ClientConfig(10 * 1024 * 1024);
        //    var clientConfig2 = new ClientConfig(10 * 1024 * 1024);

        //    server.ClientConfig = clientConfig;
        //    Assert.AreEqual(clientConfig, server.ClientConfig);

        //    server.Start();

        //    server.ClientConfig = clientConfig2;
        //    Assert.AreEqual(clientConfig, server.ClientConfig);

        //}

        //[TestMethod]
        //public void ClientConfigAlwaysReturnAnInstance()
        //{
        //    Assert.IsNotNull(server.ClientConfig);
        //}
    }
}
