using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using AsyncSocks.Exceptions;
namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncMessagingClientTests
    {
        private Mock<IInboundMessageSpooler<byte[]>> inboundSpoolerMock;
        private Mock<IOutboundMessageSpooler<byte[]>> outboundSpoolerMock;
        private Mock<IMessagePoller<byte[]>> messagePollerMock;
        private Mock<ITcpClient> tcpClientMock;
        private Mock<ISocket> socketMock;
        private IAsyncClient<byte[]> connection;
        private Mock<IOutboundMessageFactory<byte[]>> messageFactoryMock;
        private ClientConfig clientConfig;

        [TestInitialize]
        public void BeforeEach()
        {
            inboundSpoolerMock = new Mock<IInboundMessageSpooler<byte[]>>();
            outboundSpoolerMock = new Mock<IOutboundMessageSpooler<byte[]>>();
            messagePollerMock = new Mock<IMessagePoller<byte[]>>();
            socketMock = new Mock<ISocket>();
            tcpClientMock = new Mock<ITcpClient>();
            messageFactoryMock = new Mock<IOutboundMessageFactory<byte[]>>();
            clientConfig = new ClientConfig(8 * 1024 * 1024);

            tcpClientMock.Setup(x => x.Socket).Returns(socketMock.Object);

            connection = new AsyncMessagingClient
            (
                inboundSpoolerMock.Object,
                outboundSpoolerMock.Object,
                messagePollerMock.Object,
                messageFactoryMock.Object,
                tcpClientMock.Object,
                clientConfig
            );
        }

        [TestMethod]
        [ExpectedException(typeof(MessageTooBigException))]
        public void SendMessageRejectsMessagesBiggerThanConfigMaxMessageSize()
        {
            byte[] messageBytes = new byte[15 * 1024 * 1024];
            var message = new OutboundMessage<byte[]>(messageBytes, null);

            messageFactoryMock.
                Setup(x => x.Create(messageBytes, null)).
                Returns(message).
                Verifiable();

            outboundSpoolerMock.Setup(x => x.Enqueue(message)).Verifiable();

            connection.SendMessage(messageBytes);

            messageFactoryMock.Verify();
            outboundSpoolerMock.Verify();
        }

    }
}
