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
