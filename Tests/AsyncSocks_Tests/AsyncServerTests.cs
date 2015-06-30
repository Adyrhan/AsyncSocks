using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Sockets;
using AsyncSocks;
using System.Threading;
using System.Net;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class AsyncTcpServerTests
    {
        [TestMethod]
        public void CreateShouldCreateAnInstanceWithValidParameters()
        {
            MessageReceivedCallback callback = delegate(Message message, TcpClient sender) {};
            AsyncTcpServer server = AsyncTcpServer.Create(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 40400), callback);
            Assert.IsTrue(server != null && server is AsyncTcpServer);
        }

    }
}
