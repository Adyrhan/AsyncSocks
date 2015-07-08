using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using AsyncSocks;
using Moq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncSocks_Tests.Helpers;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class OutboundMessageSpoolerTest
    {
        
        private OutboundMessageSpoolerRunnable spooler;
        private Mock<ITcpClient> tcpClientMock;
        private BlockingCollection<byte[]> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            queue = new BlockingCollection<byte[]>(new ConcurrentQueue<byte[]>());
            tcpClientMock = new Mock<ITcpClient>();
            spooler = new OutboundMessageSpoolerRunnable(tcpClientMock.Object, queue);
        }
        
        [TestMethod]
        public void SpoolShouldWriteMessageToTcpClient()
        {
            string messageString = "This is a test message";
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);

            tcpClientMock.Setup(x => x.Write(messageBytes, 0, messageBytes.Length)).Returns(messageBytes.Length).Verifiable();

            queue.Add(messageBytes);

            spooler.Spool();
        }

        [TestMethod]
        public void ShouldImplementIRunnable()
        {
            Assert.IsTrue(spooler is IRunnable);
        }

        [TestMethod]
        public void RunShouldCallSpool()
        {
            AutoResetEvent spoolerCalled = new AutoResetEvent(false);

            queue.Add(new byte[5]{0,1,2,3,4});
            tcpClientMock.Setup(x => x.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Callback(() => spoolerCalled.Set()).Returns(5).Verifiable(); // If Write is called, it means Spool also has been called.

            ThreadRunner runner = new ThreadRunner(spooler);

            runner.Start();
            spoolerCalled.WaitOne(2000);
            runner.Stop();

            tcpClientMock.Verify();

        }

        [TestMethod]
        public void StopShouldStopSpooler()
        {
            ThreadRunner runner = new ThreadRunner(spooler);
            AsyncStoppingThreadRunner asyncRunner = new AsyncStoppingThreadRunner(runner);
            
            runner.Start();
            asyncRunner.Stop();
            runner.Thread.Join(2000);
            
            Assert.IsFalse(runner.Thread.IsAlive);

        }
    }
}
