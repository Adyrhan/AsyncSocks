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

        [TestInitialize]
        public void BeforeEach()
        {
            server = AsyncServer.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 40000));
            server.Start();
        }

        [TestCleanup]
        public void AfterEach()
        {
            server.Stop();
        }

        [TestMethod]
        public void AsyncServerIntegrationTest()
        {
            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 40000);
            TcpClient client = new TcpClient();

            string originalMessage = "This is a test";
            string incomingMessage = null;

            IPEndPoint incomingEndPoint = null;

            AutoResetEvent connectedEvent = new AutoResetEvent(false);
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            AutoResetEvent disconnectedEvent = new AutoResetEvent(false);

            server.OnNewClientConnected += delegate(IPeerConnection incomingClient)
            {
                incomingEndPoint = (IPEndPoint)incomingClient.RemoteEndPoint;
                connectedEvent.Set();
            };

            server.OnNewMessageReceived += delegate(IPeerConnection sender, byte[] message)
            {
                incomingMessage = Encoding.ASCII.GetString(message);
                incomingEndPoint = (IPEndPoint)sender.RemoteEndPoint;
                messageReceivedEvent.Set();
            };

            server.OnPeerDisconnected += delegate(IPeerConnection peer)
            {
                disconnectedEvent.Set();
            };

            client.Connect(clientEndPoint);
            bool didConnect = connectedEvent.WaitOne(2000);

            Assert.IsTrue(didConnect, "Server delegate for connection not called");
            Assert.AreEqual(client.Client.LocalEndPoint, incomingEndPoint, "End point for client doesn't match after connection");

            byte[] messageBytes = Encoding.ASCII.GetBytes(originalMessage);
            byte[] msgSizeBytes = BitConverter.GetBytes(messageBytes.Length);
            byte[] fullMsgBytes = new byte[messageBytes.Length + 4];

            for(int i = 0; i < messageBytes.Length + 4; i++)
            {
                if (i < 4)
                {
                    fullMsgBytes[i] = msgSizeBytes[i];
                }
                else
                {
                    fullMsgBytes[i] = messageBytes[i - 4];
                }
            }

            client.GetStream().Write(fullMsgBytes, 0, fullMsgBytes.Length);
            bool serverReceivedMessage = messageReceivedEvent.WaitOne(2000);

            Assert.IsTrue(serverReceivedMessage, "Server didn't received the message");
            Assert.AreEqual(originalMessage, incomingMessage, "Message content isn't the same as original");
            Assert.AreEqual(client.Client.LocalEndPoint, incomingEndPoint, "Endpoints doesn't match in OnNewMessageReceived delegate");

            client.Close();

            Assert.IsTrue(disconnectedEvent.WaitOne(2000));

        }

    }
}
