using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Threading;
using Moq;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class ClientConnectionAgentTests
    {
        private ClientConnectionAgent<byte[]> agent;
        private Mock<IClientConnectionAgentRunnable<byte[]>> runnableMock;
        private Mock<ITcpListener> tcpListenerMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            runnableMock = new Mock<IClientConnectionAgentRunnable<byte[]>>();
            runnableMock.Setup(x => x.TcpListener).Returns(tcpListenerMock.Object);
            
            var runnable = runnableMock.Object;
            agent = new ClientConnectionAgent<byte[]>(runnable);
        }
        
        [TestMethod]
        public void ShouldFireOnNewClientConnectionEventWhenANewConnectionHappens()
        {
            AutoResetEvent newClientEventFired = new AutoResetEvent(false);
            NewClientConnected<byte[]>newClientConnectionDelegate = (object sender, NewClientConnectedEventArgs<byte[]> e) => newClientEventFired.Set();

            var peerConnectionMock = new Mock<IAsyncClient<byte[]>>();

            agent.OnNewClientConnection += newClientConnectionDelegate;
            agent.Start();

            var ev = new NewClientConnectedEventArgs<byte[]>(peerConnectionMock.Object);

            runnableMock.Raise(x => x.OnNewClientConnection += null, ev);

            Assert.IsTrue(newClientEventFired.WaitOne(2000));

            agent.Stop();
        }

        [TestMethod]
        public void StartMethodShouldStartTcpListener()
        {
            tcpListenerMock.Setup(x => x.Start()).Verifiable();
            agent.Start();
            tcpListenerMock.Verify();
        }
    }
}
