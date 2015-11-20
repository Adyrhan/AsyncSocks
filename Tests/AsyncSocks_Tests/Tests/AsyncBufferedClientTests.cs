using AsyncSocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncBufferedClientTests
    {
        private AsyncBufferedClient client;
        private AsyncBufferedClientConfig clientConfig;
        private Mock<IInboundMessageSpooler<byte[]>> inboundSpoolerMock;
        private Mock<IOutboundMessageFactory<byte[]>> messageFactoryMock;
        private Mock<IMessagePoller<byte[]>> messagePollerMock;
        private Mock<IOutboundMessageSpooler<byte[]>> outboundSpoolerMock;
        private Mock<ITcpClient> tcpClientMock;

        [TestInitialize]
        public void BeforeEach()
        {
            inboundSpoolerMock = new Mock<IInboundMessageSpooler<byte[]>>();
            outboundSpoolerMock = new Mock<IOutboundMessageSpooler<byte[]>>();
            messagePollerMock = new Mock<IMessagePoller<byte[]>>();
            messageFactoryMock = new Mock<IOutboundMessageFactory<byte[]>>();
            tcpClientMock = new Mock<ITcpClient>();

            var dict = new Dictionary<string, string>();
            dict.Add("BufferSize", (1024 * 12).ToString());

            clientConfig = new AsyncBufferedClientConfig(dict);
            client = new AsyncBufferedClient(inboundSpoolerMock.Object, outboundSpoolerMock.Object, messagePollerMock.Object, messageFactoryMock.Object, tcpClientMock.Object, clientConfig);
        }

        [TestMethod]
        public void ShouldSubclassAsyncClient()
        {
            Assert.IsInstanceOfType(client, typeof(AsyncClient<byte[]>));
        }

        [TestMethod]
        public void ShouldDisconnectWhenError()
        {
            tcpClientMock.Setup(x => x.Close()).Verifiable();
            messagePollerMock.Setup(x => x.Stop()).Verifiable();
            inboundSpoolerMock.Setup(x => x.Stop()).Verifiable();
            outboundSpoolerMock.Setup(x => x.Stop()).Verifiable();

            messagePollerMock.Raise(x => x.OnReadError += null, messagePollerMock.Object, new ReadErrorEventArgs(new System.Exception("Fake test exception")));

            tcpClientMock.Verify();
            messagePollerMock.Verify();
            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
        }

    }
}
