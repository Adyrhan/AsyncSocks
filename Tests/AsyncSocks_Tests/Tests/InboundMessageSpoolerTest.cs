using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace AsyncSocks_Tests.Tests
{
    [TestClass]
    public class InboundMessageSpoolerTest
    {

        class AsyncThreadRunner
        {
            private ThreadRunner runner;

            public AsyncThreadRunner(ThreadRunner runner)
            {
                this.runner = runner;
            }

            public async void Stop()
            {
                await Task.Run(() => runner.Stop());
            }
        }

        private InboundMessageSpooler spooler;
        private BlockingCollection<byte[]> queue;
        private Mock<INetworkMessageReader> readerMock;

        [TestInitialize]
        public void BeforeEach()
        {
            readerMock = new Mock<INetworkMessageReader>();
            queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            spooler = new InboundMessageSpooler(readerMock.Object, queue);
        }

        [TestMethod]
        public void SpoolShouldAddMessageInQueue()
        {
            string messageString = "This is a test message";

            readerMock.Setup(x => x.Read()).Returns(Encoding.ASCII.GetBytes(messageString));

            spooler.Spool();

            string storedMessage = Encoding.ASCII.GetString(queue.Take());

            Assert.AreEqual(messageString, storedMessage);
        }

        
    }
}
