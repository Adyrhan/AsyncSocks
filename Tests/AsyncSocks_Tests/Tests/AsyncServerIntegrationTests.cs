using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncServerIntegrationTests
    {
        private AsyncServer server;
        private IPEndPoint serverEndPoint;

        [TestInitialize]
        public void BeforeEach()
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 40000);
            server = AsyncServer.Create(serverEndPoint);
            server.Start();
        }

        [TestCleanup]
        public void AfterEach()
        {
            server.Stop();
        }

        [TestMethod]
        public void AsyncServerEventsNominalCaseTest()
        {
            AsyncClient client = AsyncClient.Create();

            string originalMessage = "This is a test";
            string incomingMessage = null;

            IPEndPoint incomingEndPoint = null;

            AutoResetEvent connectedEvent = new AutoResetEvent(false);
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            AutoResetEvent disconnectedEvent = new AutoResetEvent(false);

            server.OnNewClientConnected += (object sender, NewClientConnectedEventArgs e) =>
            {
                incomingEndPoint = (IPEndPoint)e.Client.RemoteEndPoint;
                connectedEvent.Set();
            };

            server.OnNewMessageReceived += (object sender, NewMessageReceivedEventArgs e) =>
            {
                incomingMessage = Encoding.ASCII.GetString(e.Message);
                incomingEndPoint = (IPEndPoint)e.Sender.RemoteEndPoint;
                messageReceivedEvent.Set();
            };

            server.OnPeerDisconnected += (object sender, PeerDisconnectedEventArgs e) => disconnectedEvent.Set();

            client.Connect(serverEndPoint);
            bool didConnect = connectedEvent.WaitOne(2000);

            Assert.IsTrue(didConnect, "Server delegate for connection not called");
            Assert.AreEqual(client.LocalEndPoint, incomingEndPoint, "End point for client doesn't match after connection");

            byte[] messageBytes = Encoding.ASCII.GetBytes(originalMessage);

            bool clientReportedSuccess = false;
            SocketException sendMessageException = null;
            AutoResetEvent sendMessageCallbackEvent = new AutoResetEvent(false);

            client.SendMessage(messageBytes, (success, exception) =>
            {
                clientReportedSuccess = success;
                sendMessageException = exception;
                sendMessageCallbackEvent.Set();
            });

            bool serverReceivedMessage = messageReceivedEvent.WaitOne(2000);
            bool sendMessageCallbackCalled = sendMessageCallbackEvent.WaitOne(2000);

            Assert.IsTrue(serverReceivedMessage, "Server didn't received the message");
            Assert.AreEqual(originalMessage, incomingMessage, "Message content isn't the same as original");
            Assert.AreEqual(client.LocalEndPoint, incomingEndPoint, "Endpoints doesn't match in OnNewMessageReceived delegate");

            client.Close();

            Assert.IsTrue(disconnectedEvent.WaitOne(2000));

        }

        [TestMethod]
        [ExpectedException(typeof(SocketException))]
        public void ServerTryingToStartWithAlreadyUsedPort()
        {
            var serverUsingAlreadyUsedPort = AsyncServer.Create(serverEndPoint);
            serverUsingAlreadyUsedPort.Start();
        }

    }
}
