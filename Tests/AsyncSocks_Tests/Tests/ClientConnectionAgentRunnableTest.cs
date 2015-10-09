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
        private IClientConnectionAgentRunnable<byte[]> agent;
        private Mock<IAsyncClientFactory<byte[]>> connectionFactoryMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            connectionFactoryMock = new Mock<IAsyncClientFactory<byte[]>>();
            agent = new ClientConnectionAgentRunnable<byte[]>(tcpListenerMock.Object, connectionFactoryMock.Object);
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
            Mock<IAsyncClient<byte[]>> peerConnectionMock = new Mock<IAsyncClient<byte[]>>();
            Mock<NewClientConnected<byte[]>> newClientCallbackMock = new Mock<NewClientConnected<byte[]>>();

            var ev = new NewClientConnectedEventArgs<byte[]>(peerConnectionMock.Object);

            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object).Verifiable();
            connectionFactoryMock.Setup(x => x.Create(tcpClientMock.Object)).Returns(peerConnectionMock.Object).Verifiable();
            //newClientCallbackMock.Setup(x => x(It.IsAny<object>(), It.IsAny<NewClientConnectedEventArgs>())).Verifiable();

            object senderParam = null;
            NewClientConnectedEventArgs<byte[]> eParam = null;

            agent.OnNewClientConnection += (object sender, NewClientConnectedEventArgs<byte[]> e) =>
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
            Mock<IAsyncClient <byte[]>> peerConnectionMock = new Mock<IAsyncClient<byte[]>> ();
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
            Mock<NewClientConnected<byte[]>> newClientCallbackMock = new Mock<NewClientConnected<byte[]>>();
            
            ThreadRunner runner = new ThreadRunner(agent);
            AutoResetEvent AcceptClientConnectionWasCalled = new AutoResetEvent(false);

            tcpListenerMock.Setup(x => x.Start()).Verifiable();
            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object).Verifiable();
            connectionFactoryMock.Setup(x => x.Create(tcpClientMock.Object)).Returns(peerConnectionMock.Object).Verifiable();
            //newClientCallbackMock.Setup(x => x(peerConnectionMock.Object)).Callback(() => AcceptClientConnectionWasCalled.Set());
            tcpListenerMock.Setup(x => x.Stop()).Verifiable();
            agent.OnNewClientConnection += (object sender, NewClientConnectedEventArgs<byte[]> e) => AcceptClientConnectionWasCalled.Set();
            
            runner.Start();

            AcceptClientConnectionWasCalled.WaitOne(2000);

            runner.Stop();

            tcpListenerMock.Verify();
            //newClientCallbackMock.Verify();
        }
    }
}
