using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Collections.Concurrent;
using System.Text;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class InboundMessageSpoolerTest
    {
        [TestMethod]
        public void SpoolShouldAddMessageInQueue()
        {
            Mock<INetworkMessageReader> readerMock = new Mock<INetworkMessageReader>();
            BlockingCollection<byte[]> queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            
            string messageString = "This is a test message";

            readerMock.Setup(x => x.Read()).Returns(Encoding.ASCII.GetBytes(messageString));

            InboundMessageSpooler spooler = new InboundMessageSpooler(readerMock.Object, queue);
            spooler.Spool();

            string storedMessage = Encoding.ASCII.GetString(queue.Take());

            Assert.AreEqual(messageString, storedMessage);
        }
    }
}
