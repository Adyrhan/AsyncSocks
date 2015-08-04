﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Threading;
using Moq;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class ClientConnectionAgentTests
    {
        private ClientConnectionAgent agent;
        private Mock<IClientConnectionAgentRunnable> runnableMock;
        private Mock<ITcpListener> tcpListenerMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpListenerMock = new Mock<ITcpListener>();
            runnableMock = new Mock<IClientConnectionAgentRunnable>();
            runnableMock.Setup(x => x.TcpListener).Returns(tcpListenerMock.Object);
            
            var runnable = runnableMock.Object;
            agent = new ClientConnectionAgent(runnable);
        }
        
        [TestMethod]
        public void ShouldFireOnNewClientConnectionEventWhenANewConnectionHappens()
        {
            AutoResetEvent newClientEventFired = new AutoResetEvent(false);
            NewPeerConnectionDelegate newClientConnectionDelegate = delegate(IAsyncClient client)
            {
                newClientEventFired.Set();
            };

            var peerConnectionMock = new Mock<IAsyncClient>().Object;

            agent.OnNewClientConnection += newClientConnectionDelegate;
            agent.Start();

            runnableMock.Raise(x => x.OnNewClientConnection += null, peerConnectionMock);

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
