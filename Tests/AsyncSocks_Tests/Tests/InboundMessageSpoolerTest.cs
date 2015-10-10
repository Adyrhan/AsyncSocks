using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Collections.Concurrent;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class InboundMessageSpoolerTest
    {
        [TestMethod]
        public void CreateShouldCreateInstance()
        {
            var tcpClientMock = new Mock<ITcpClient>();
            var readerMock = new Mock<INetworkReader<byte[]>>();

            InboundMessageSpooler<byte[]> spooler = InboundMessageSpooler<byte[]>.Create(readerMock.Object);

            Assert.IsTrue(spooler != null);
        }

        [TestMethod]
        public void ShouldBehaveLikeAThreadRunner()
        {
            var runnableMock = new Mock<IInboundMessageSpoolerRunnable<byte[]>>();
            var runnable = runnableMock.Object;
            var spooler = new InboundMessageSpooler<byte[]>(runnable);

            Assert.IsTrue(spooler is ThreadRunner);
        }

        [TestMethod]
        public void ConstructorShouldTakeRunnable()
        {
            var runnableMock = new Mock<IInboundMessageSpoolerRunnable<byte[]>>();
            var runnable = runnableMock.Object;
            InboundMessageSpooler<byte[]> spooler = new InboundMessageSpooler<byte[]>(runnable);
        }

        [TestMethod]
        public void PeerDisconnectedFiresUpWhenRunnableEventDoes()
        {
            var runnableMock = new Mock<IInboundMessageSpoolerRunnable<byte[]>>();
            var spooler = new InboundMessageSpooler<byte[]>(runnableMock.Object);

            AutoResetEvent callbackCalledEvent = new AutoResetEvent(false);
            spooler.OnPeerDisconnected += (object sender, PeerDisconnectedEventArgs<byte[]> e) => callbackCalledEvent.Set();

            runnableMock.Raise(x => x.OnPeerDisconnected += null, new PeerDisconnectedEventArgs<byte[]>(null));

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000));
        }

    }
}
