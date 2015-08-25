using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncClientTest
    {
        private Mock<IInboundMessageSpooler> inboundSpoolerMock;
        private Mock<IOutboundMessageSpooler> outboundSpoolerMock;
        private Mock<IMessagePoller> messagePollerMock;
        private Mock<ITcpClient> tcpClientMock;
        private IAsyncClient connection;
        private Mock<IOutboundMessageFactory> messageFactoryMock;

        [TestInitialize]
        public void BeforeEach()
        {
            inboundSpoolerMock = new Mock<IInboundMessageSpooler>();
            outboundSpoolerMock = new Mock<IOutboundMessageSpooler>();
            messagePollerMock = new Mock<IMessagePoller>();
            tcpClientMock = new Mock<ITcpClient>();
            messageFactoryMock = new Mock<IOutboundMessageFactory>();

            connection = new AsyncClient
            (
                inboundSpoolerMock.Object,
                outboundSpoolerMock.Object,
                messagePollerMock.Object,
                messageFactoryMock.Object,
                tcpClientMock.Object
            );
        }

        [TestMethod]
        public void SendMessageShouldTellOutboundMessageSpoolerToEnqueueTheMessage()
        {
            var messageBytes = Encoding.ASCII.GetBytes("This is a test message");
            var message = new OutboundMessage(messageBytes, null);

            messageFactoryMock.
                Setup(x => x.Create(messageBytes, null)).
                Returns(message).
                Verifiable();

            outboundSpoolerMock.Setup(x => x.Enqueue(message)).Verifiable();
            connection.SendMessage(messageBytes);

            messageFactoryMock.Verify();
            outboundSpoolerMock.Verify();
        }

        [TestMethod]
        public void StartShouldStartInboundAndOutboundMessageSpoolingAndMessagePolling()
        {
            inboundSpoolerMock.Setup(x => x.Start()).Verifiable();
            outboundSpoolerMock.Setup(x => x.Start()).Verifiable();
            messagePollerMock.Setup(x => x.Start()).Verifiable();
            
            connection.Start();
            
            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
            messagePollerMock.Verify();
        }

        [TestMethod]
        public void CloseShouldStopSpoolersAndCloseConnectionWithPeer()
        {
            inboundSpoolerMock.Setup(x => x.Stop()).Verifiable();
            outboundSpoolerMock.Setup(x => x.Stop()).Verifiable();
            tcpClientMock.Setup(x => x.Close()).Verifiable();
            messagePollerMock.Setup(x => x.Stop()).Verifiable();

            connection.Close();

            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
            tcpClientMock.Verify();
            messagePollerMock.Verify();
        }

        [TestMethod]
        public void RemoteEndPointPropertyShouldReturnValuesAccordingToTcpClientObject()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80);
            var clientMock = new Mock<ISocket>();

            tcpClientMock.Setup(x => x.Client).Returns(clientMock.Object).Verifiable();
            clientMock.Setup(x => x.RemoteEndPoint).Returns(endPoint).Verifiable();

            Assert.AreEqual(endPoint, connection.RemoteEndPoint);
        }

        [TestMethod]
        public void LocalEndPointPropertyShouldReturnValuesAccordingToTcpClientObject()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.80"), 44526);
            var clientMock = new Mock<ISocket>();

            tcpClientMock.Setup(x => x.Client).Returns(clientMock.Object).Verifiable();
            clientMock.Setup(x => x.LocalEndPoint).Returns(endPoint).Verifiable();

            Assert.AreEqual(endPoint, connection.LocalEndPoint);
        }

        [TestMethod]
        public void IsActiveShouldReturnTrueIfBothSpoolersAndMessagePollerAreRunning()
        {
            bool inboundSpoolerStarted = false;
            bool outboundSpoolerStarted = false;
            bool messagePollerStarted = false;

            inboundSpoolerMock.Setup(x => x.Start()).Callback(() => inboundSpoolerStarted = true).Verifiable();
            outboundSpoolerMock.Setup(x => x.Start()).Callback(() => outboundSpoolerStarted = true).Verifiable();
            messagePollerMock.Setup(x => x.Start()).Callback(() => messagePollerStarted = true).Verifiable();

            inboundSpoolerMock.Setup(x => x.IsRunning()).Returns(() => { return inboundSpoolerStarted;}).Verifiable();
            outboundSpoolerMock.Setup(x => x.IsRunning()).Returns(() => { return outboundSpoolerStarted;}).Verifiable();
            messagePollerMock.Setup(x => x.IsRunning()).Returns(() => { return messagePollerStarted; }).Verifiable();

            connection.Start();

            bool active = connection.IsActive();
            
            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
            messagePollerMock.Verify();

            Assert.IsTrue(active);
            
        }

        [TestMethod]
        public void OnNewMessageReceivedIsFiredWhenMessagePollerEventDoes()
        {
            AutoResetEvent callbackCalledEvent = new AutoResetEvent(false);
            var messageBytes = Encoding.ASCII.GetBytes("This is a test");

            IAsyncClient senderArgument = null;
            byte[] messageArgument = null;

            var callback = new NewMessageReceived((object sender, NewMessageReceivedEventArgs e) =>
            {
                senderArgument = e.Sender;
                messageArgument = e.Message;
                callbackCalledEvent.Set();
            });

            connection.OnNewMessageReceived += callback;

            var ev = new NewMessageReceivedEventArgs(null, messageBytes);
            messagePollerMock.Raise(x => x.OnNewClientMessageReceived += null, ev);

            bool callbackCalled = callbackCalledEvent.WaitOne(2000);

            Assert.IsTrue(callbackCalled);
            Assert.AreEqual(connection, senderArgument);
            Assert.AreEqual(messageBytes, messageArgument);

        }

        [TestMethod]
        public void ShouldFireOnPeerDisconnectedEventWhenInboundMessageSpoolerDoesIt()
        {
            AutoResetEvent callbackCalledEvent = new AutoResetEvent(false);

            IAsyncClient peerArgument = null;
            var callback = new PeerDisconnected((object sender, PeerDisconnectedEventArgs e) =>
            {
                peerArgument = e.Peer;
                callbackCalledEvent.Set();
            });

            connection.OnPeerDisconnected += callback;

            inboundSpoolerMock.Raise(x => x.OnPeerDisconnected += null, new PeerDisconnectedEventArgs(null));

            Assert.IsTrue(callbackCalledEvent.WaitOne(2000), "Callback not called");
            Assert.AreEqual(connection, peerArgument);
        }

        [TestMethod]
        public void ConnectShouldCallConnectOnTcpClientObjectStartSpoolersAndPoller()
        {
            tcpClientMock.Setup(x => x.Connect()).Verifiable();
            inboundSpoolerMock.Setup(x => x.Start()).Verifiable();
            outboundSpoolerMock.Setup(x => x.Start()).Verifiable();
            messagePollerMock.Setup(x => x.Start()).Verifiable();

            connection.Connect();

            tcpClientMock.Verify();
            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
            messagePollerMock.Verify();
        }

        [TestMethod]
        public void SendMessageAcceptsCompletionDelegateAndTellsOutboundSpoolerToEnqueueThePair()
        {
            bool receivedResult = false;

            byte[] msgBytes = Encoding.ASCII.GetBytes("Some text");
            Action<bool, SocketException> callback = (success, error) => receivedResult = success;
            OutboundMessage outboundMessage = new OutboundMessage(msgBytes, callback);

            messageFactoryMock.
                Setup(x => x.Create(It.IsAny<byte[]>(), It.IsAny<Action<bool, SocketException>>())).
                Returns(outboundMessage)
                .Verifiable();

            outboundSpoolerMock.
                Setup(x => x.Enqueue(outboundMessage))
                .Verifiable();

            connection.SendMessage(msgBytes, callback);

            messageFactoryMock.Verify();
            outboundSpoolerMock.Verify();

        }
    }
}
