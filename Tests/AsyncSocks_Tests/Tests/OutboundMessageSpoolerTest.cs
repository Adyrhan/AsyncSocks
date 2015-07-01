using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using AsyncSocks;
using Moq;
using System.Text;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class OutboundMessageSpoolerTest
    {
        [TestMethod]
        public void SpoolShouldWriteMessageToTcpClient()
        {
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();

            string messageString = "This is a test message";
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);

            tcpClientMock.Setup(x => x.Write(messageBytes, 0, messageBytes.Length)).Returns(messageBytes.Length).Verifiable();
            OutboundMessageSpooler spooler = new OutboundMessageSpooler(tcpClientMock.Object, queue);

            queue.Add(messageBytes);

            spooler.Spool();

        }
    }
}
