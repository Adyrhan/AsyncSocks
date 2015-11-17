using Microsoft.VisualStudio.TestTools.UnitTesting;
using AsyncSocks;
using AsyncSocks.AsyncMessaging;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using AsyncSocks.AsyncMessaging.Exceptions;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class AsyncMessagingServerIntegrationTests
    {
        private AsyncMessagingServer server;
        private IPEndPoint serverEndPoint;

        [TestInitialize]
        public void BeforeEach()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("MaxMessageSize", (10 * 1024 * 1024).ToString());

            serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 40000);
            server = AsyncMessagingServer.Create(serverEndPoint, new ClientConfig(dict));
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
            var client = (AsyncMessagingClient) new AsyncMessagingClientFactory().Create();

            string originalMessage = "This is a test";
            string incomingMessage = null;

            IPEndPoint incomingEndPoint = null;

            AutoResetEvent connectedEvent = new AutoResetEvent(false);
            AutoResetEvent messageReceivedEvent = new AutoResetEvent(false);
            AutoResetEvent disconnectedEvent = new AutoResetEvent(false);

            server.OnNewClientConnected += (object sender, NewClientConnectedEventArgs<byte[]> e) =>
            {
                incomingEndPoint = (IPEndPoint)e.Client.RemoteEndPoint;
                connectedEvent.Set();
            };

            server.OnNewMessageReceived += (object sender, NewMessageReceivedEventArgs<byte[]> e) =>
            {
                incomingMessage = Encoding.ASCII.GetString(e.Message);
                incomingEndPoint = (IPEndPoint)e.Sender.RemoteEndPoint;
                messageReceivedEvent.Set();
            };

            server.OnPeerDisconnected += (object sender, PeerDisconnectedEventArgs<byte[]> e) => disconnectedEvent.Set();

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
            var serverUsingAlreadyUsedPort = AsyncMessagingServer.Create(serverEndPoint);
            serverUsingAlreadyUsedPort.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(MessageTooBigException))]
        public void SendingBigMessage()
        {
            var kvp = new Dictionary<string, string>();
            kvp.Add("MaxMessageSize", (8 * 1024 * 1024).ToString());
            var client = (AsyncMessagingClient)new AsyncMessagingClientFactory(new AsyncMessagingClientConfig(kvp)).Create(serverEndPoint);

            int messageLength = 50 * 1024 * 1024;
            byte[] messageBytes = new byte[messageLength];

            for (int i = 0; i < messageLength; i++)
            {
                messageBytes[i] = (byte)((i+255) % 255);
            }

            client.Start();

            try
            {
                client.SendMessage(messageBytes);
            }
            catch (MessageTooBigException e)
            {
                throw e;
            }
            finally
            {
                client.Close();
            }
        }

        [TestMethod]
        public void MultipleConnectionsAndDisconnections()
        {
            AsyncClient<byte[]>[] clients = new AsyncClient<byte[]>[200];
            AutoResetEvent allConnectedEvent = new AutoResetEvent(false);
            int connectedClients = 0;

            server.OnNewClientConnected += (sender, e) =>
            {
                connectedClients++;
                if (connectedClients == clients.Length)
                {
                    allConnectedEvent.Set();
                }
            };

            
            for(int i = 0; i < clients.Length; i++)
            {
                clients[i] = (AsyncMessagingClient) new AsyncMessagingClientFactory().Create(serverEndPoint);
                clients[i].Start();
            }

            Assert.IsTrue(true);

            Assert.IsTrue(allConnectedEvent.WaitOne(6000), "Not all clients connected to the server");

            AutoResetEvent allDisconnectedEvent = new AutoResetEvent(false);
            server.OnPeerDisconnected += (sender, e) =>
            {
                connectedClients--;
                if (connectedClients == 0)
                {
                    allDisconnectedEvent.Set();
                }
            };

            server.ConnectionManager.CloseAllConnections();

            Assert.IsTrue(allDisconnectedEvent.WaitOne(6000), "Not all clients were disconnected from the server");


        }

        [TestMethod]
        public void MultipleConnectionsAndDisconnectionsFromPeers()
        {
            AsyncClient<byte[]>[] clients = new AsyncClient<byte[]>[200];
            AutoResetEvent allConnectedEvent = new AutoResetEvent(false);
            int connectedClients = 0;

            server.OnNewClientConnected += (sender, e) =>
            {
                connectedClients++;
                if (connectedClients == clients.Length)
                {
                    allConnectedEvent.Set();
                }
            };


            for (int i = 0; i < clients.Length; i++)
            {
                clients[i] = (AsyncMessagingClient)new AsyncMessagingClientFactory().Create(serverEndPoint);
                clients[i].Start();
            }

            Assert.IsTrue(true);

            Assert.IsTrue(allConnectedEvent.WaitOne(6000), "Not all clients connected to the server");

            AutoResetEvent allDisconnectedEvent = new AutoResetEvent(false);
            server.OnPeerDisconnected += (sender, e) =>
            {
                connectedClients--;
                if (connectedClients == 0)
                {
                    allDisconnectedEvent.Set();
                }
            };

            foreach(AsyncClient<byte[]> client in clients)
            {
                client.Close();
            }

            Assert.IsTrue(allDisconnectedEvent.WaitOne(6000), "Not all clients were disconnected from the server");
        }

        [TestMethod]
        public void ReceivingBigMessage()
        {

            AsyncMessagingClient clientServerReference = null;
            AutoResetEvent clientConnected = new AutoResetEvent(false);
            server.OnNewClientConnected += (object o, NewClientConnectedEventArgs<byte[]> e) =>
            {
                clientServerReference = (AsyncMessagingClient)e.Client;
                clientConnected.Set();
            };

            var kvp = new Dictionary<string, string>();
            kvp.Add("MaxMessageSize", (60 * 1024 * 1024).ToString());
            var client = (AsyncMessagingClient)new AsyncMessagingClientFactory(new AsyncMessagingClientConfig(kvp)).Create(serverEndPoint); // It's not gonna reject the message on the client

            int messageLength = 12 * 1024 * 1024;
            byte[] messageBytes = new byte[messageLength];

            for (int i = 0; i < messageLength; i++)
            {
                messageBytes[i] = (byte)((i + 255) % 255);
            }

            client.Start();

            AutoResetEvent disconnectedEvent = new AutoResetEvent(false);
            AutoResetEvent errorEvent = new AutoResetEvent(false);
            clientServerReference.OnReadError += (object o, ReadErrorEventArgs e) =>
            {
                errorEvent.Set();
            };
            client.OnPeerDisconnected += (object sender, PeerDisconnectedEventArgs<byte[]> e) =>
            {
                disconnectedEvent.Set();
            };

            client.SendMessage(messageBytes);
            Assert.IsTrue(errorEvent.WaitOne(2000), "Server client reference didn't trigger error event");
            Assert.IsTrue(disconnectedEvent.WaitOne(2000), "Server didn't disconnect client when it sent the message");
        }
    }
}
