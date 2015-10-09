using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using Moq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AsyncSocks.Exceptions;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncClientTest
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

            connection = new AsyncClient<byte[]>
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
        public void SendMessageShouldTellOutboundMessageSpoolerToEnqueueTheMessage()
        {
            var messageBytes = Encoding.ASCII.GetBytes("This is a test message");
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

        [TestMethod]
        public void SendMessageAcceptsCompletionDelegateAndTellsOutboundSpoolerToEnqueueThePair()
        {
            bool receivedResult = false;

            byte[] msgBytes = Encoding.ASCII.GetBytes("Some text");
            Action<bool, SocketException> callback = (success, error) => receivedResult = success;
            OutboundMessage<byte[]> outboundMessage = new OutboundMessage<byte[]>(msgBytes, callback);

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
           
            Assert.IsNull(connection.RemoteEndPoint); // Socket is not connected, so properties are null

            socketMock.Setup(x => x.RemoteEndPoint).Returns(endPoint);
            connection.Connect(); // This saves end points from Socket object in AsyncClient object, so they aren't lost on disconnection.

            Assert.AreEqual(endPoint, connection.RemoteEndPoint);
        }

        [TestMethod]
        public void LocalEndPointPropertyShouldReturnValuesAccordingToTcpClientObject()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse("192.168.1.80"), 44526);

            Assert.IsNull(connection.LocalEndPoint); // Socket is not connected, so properties are null

            socketMock.Setup(x => x.LocalEndPoint).Returns(endPoint);
            connection.Connect(); // This saves end points from Socket object in AsyncClient object, so they aren't lost on disconnection.

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

            IAsyncClient<byte[]> senderArgument = null;
            byte[] messageArgument = null;

            var callback = new NewMessageReceived<byte[]>((object sender, NewMessageReceivedEventArgs<byte[]> e) =>
            {
                senderArgument = e.Sender;
                messageArgument = e.Message;
                callbackCalledEvent.Set();
            });

            connection.OnNewMessageReceived += callback;

            var ev = new NewMessageReceivedEventArgs<byte[]>(null, messageBytes);
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

            IAsyncClient<byte[]> peerArgument = null;
            var callback = new PeerDisconnected<byte[]>((object sender, PeerDisconnectedEventArgs<byte[]> e) =>
            {
                peerArgument = e.Peer;
                callbackCalledEvent.Set();
            });

            connection.OnPeerDisconnected += callback;

            inboundSpoolerMock.Raise(x => x.OnPeerDisconnected += null, new PeerDisconnectedEventArgs<byte[]>(null));

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
            socketMock.Setup(x => x.RemoteEndPoint).Returns(new IPEndPoint(IPAddress.Parse("80.80.80.80"), 80)).Verifiable();
            socketMock.Setup(x => x.LocalEndPoint).Returns(new IPEndPoint(IPAddress.Parse("192.168.80.80"), 45465)).Verifiable();
            connection.Connect();

            tcpClientMock.Verify();
            inboundSpoolerMock.Verify();
            outboundSpoolerMock.Verify();
            messagePollerMock.Verify();
            socketMock.Verify();
        }

        [TestMethod]
        public void ClientConfigPropertyReturnsClientConfigObject()
        {
            Assert.AreEqual(clientConfig, connection.ClientConfig);
        }

    }
}
