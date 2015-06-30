using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Net.Sockets;
using Moq;
using System.Threading;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class ClientConnectionAgentTest
    {
        [TestMethod]
        public void ShouldImplementIRunnable()
        {
            ClientConnectionAgent agent = new ClientConnectionAgent(new Mock<ITcpListener>().Object);
            Assert.IsTrue(agent is IRunnable);
        }
        
        [TestMethod]
        public void ShouldCallDelegateWhenNewClientConnects()
        {
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
            Mock<ITcpListener> tcpListenerMock = new Mock<ITcpListener>();
            Mock<NewClientConnectionDelegate> newClientCallbackMock = new Mock<NewClientConnectionDelegate>();

            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object).Verifiable();
            newClientCallbackMock.Setup(x => x(tcpClientMock.Object)).Verifiable();
            
            ClientConnectionAgent agent = new ClientConnectionAgent(tcpListenerMock.Object);
            agent.OnNewClientConnection += newClientCallbackMock.Object;
            agent.AcceptClientConnection();

            tcpListenerMock.Verify();
            newClientCallbackMock.Verify();
        }

        [TestMethod]
        public void RunShouldCallAcceptClientConnection()
        {
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
            Mock<ITcpListener> tcpListenerMock = new Mock<ITcpListener>();
            Mock<NewClientConnectionDelegate> newClientCallbackMock = new Mock<NewClientConnectionDelegate>();
            
            ClientConnectionAgent agent = new ClientConnectionAgent(tcpListenerMock.Object);
            ThreadRunner runner = new ThreadRunner(agent);
            AutoResetEvent AcceptClientConnectionWasCalled = new AutoResetEvent(false);

            tcpListenerMock.Setup(x => x.AcceptTcpClient()).Returns(tcpClientMock.Object);
            newClientCallbackMock.Setup(x => x(tcpClientMock.Object)).Callback(() => AcceptClientConnectionWasCalled.Set());      
            agent.OnNewClientConnection += newClientCallbackMock.Object;
            
            runner.Start();

            AcceptClientConnectionWasCalled.WaitOne(2000);

            runner.Stop();

            tcpListenerMock.Verify();
            newClientCallbackMock.Verify();
        }
    }
}
