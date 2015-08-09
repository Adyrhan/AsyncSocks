using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Net.Sockets;
using Moq;
using System.Threading;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class ClientConnectionAgentRunnableTest
    {
        private Mock<ITcpListener> tcpListenerMock;
        private IClientConnectionAgentRunnable agent;
        private Mock<IAsyncClientFactory> connectionFactoryMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            connectionFactoryMock = new Mock<IAsyncClientFactory>();
            agent = new ClientConnectionAgentRunnable(tcpListenerMock.Object, connectionFactoryMock.Object);
        }
        
        [TestMethod]
        public void ShouldImplementIRunnable()
        {
            Assert.IsTrue(agent is IRunnable);
        }
        
        [TestMethod]
        public void ShouldCallDelegateWhenNewClientConnects()
        {
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
            Mock<IAsyncClient> peerConnectionMock = new Mock<IAsyncClient>();
            Mock<NewClientConnected> newClientCallbackMock = new Mock<NewClientConnected>();

            var ev = new NewClientConnectedEventArgs(peerConnectionMock.Object);

            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object).Verifiable();
            connectionFactoryMock.Setup(x => x.Create(tcpClientMock.Object)).Returns(peerConnectionMock.Object).Verifiable();
            //newClientCallbackMock.Setup(x => x(It.IsAny<object>(), It.IsAny<NewClientConnectedEventArgs>())).Verifiable();

            object senderParam = null;
            NewClientConnectedEventArgs eParam = null;

            agent.OnNewClientConnection += delegate (object sender, NewClientConnectedEventArgs e)
            {
                senderParam = sender;
                eParam = e;
            };

            agent.AcceptClientConnection();

            tcpListenerMock.Verify();
            //newClientCallbackMock.Verify();
            connectionFactoryMock.Verify();

            Assert.AreEqual(agent, senderParam);
            Assert.AreEqual(ev.Client, eParam.Client);
        }

        [TestMethod]
        public void RunShouldCallAcceptClientConnection()
        {
            Mock<IAsyncClient> peerConnectionMock = new Mock<IAsyncClient>();
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
            Mock<NewClientConnected> newClientCallbackMock = new Mock<NewClientConnected>();
            
            ThreadRunner runner = new ThreadRunner(agent);
            AutoResetEvent AcceptClientConnectionWasCalled = new AutoResetEvent(false);

            tcpListenerMock.Setup(x => x.Start()).Verifiable();
            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object).Verifiable();
            connectionFactoryMock.Setup(x => x.Create(tcpClientMock.Object)).Returns(peerConnectionMock.Object).Verifiable();
            //newClientCallbackMock.Setup(x => x(peerConnectionMock.Object)).Callback(() => AcceptClientConnectionWasCalled.Set());
            tcpListenerMock.Setup(x => x.Stop()).Verifiable();
            agent.OnNewClientConnection += delegate (object sender, NewClientConnectedEventArgs e)
            {
                AcceptClientConnectionWasCalled.Set();
            };
            
            runner.Start();

            AcceptClientConnectionWasCalled.WaitOne(2000);

            runner.Stop();

            tcpListenerMock.Verify();
            //newClientCallbackMock.Verify();
        }
    }
}
