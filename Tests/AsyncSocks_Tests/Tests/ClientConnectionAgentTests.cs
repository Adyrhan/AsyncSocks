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
        private ClientConnectionAgent agent;
        private Mock<IClientConnectionAgentRunnable> runnableMock;

        [TestInitialize]
        public void BeforeEach()
        {
            runnableMock = new Mock<IClientConnectionAgentRunnable>();
            var runnable = runnableMock.Object;
            agent = new ClientConnectionAgent(runnable);
        }
        
        [TestMethod]
        public void ShouldFireOnNewClientConnectionEventWhenANewConnectionHappens()
        {
            AutoResetEvent newClientEventFired = new AutoResetEvent(false);
            NewClientConnectionDelegate newClientConnectionDelegate = delegate(ITcpClient client)
            {
                newClientEventFired.Set();
            };

            var tcpClient = new Mock<ITcpClient>().Object;

            agent.OnNewClientConnection += newClientConnectionDelegate;
            agent.Start();

            runnableMock.Raise(x => x.OnNewClientConnection += null, tcpClient);

            Assert.IsTrue(newClientEventFired.WaitOne(2000));

            agent.Stop();
        }
    }
}
