using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using AsyncSocks;
using Moq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AsyncSocks_Tests.Helpers;
using System.Net.Sockets;
using System.IO;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class OutboundMessageSpoolerRunnableTests
    {
        
        private OutboundMessageSpoolerRunnable<byte[]> spooler;
        private Mock<INetworkWriter<byte[]>> writerMock;
        private BlockingCollection<OutboundMessage<byte[]>> queue;

        [TestInitialize]
        public void BeforeEach()
        {
            queue = new BlockingCollection<OutboundMessage<byte[]>>(new ConcurrentQueue<OutboundMessage<byte[]>>());
            writerMock = new Mock<INetworkWriter<byte[]>>();
            spooler = new OutboundMessageSpoolerRunnable<byte[]>(writerMock.Object, queue);
        }
        
        [TestMethod]
        public void SpoolShouldCallNetworkWriterWriteMethod()
        {
            string messageString = "This is a test message";
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageString);

            byte[] size = BitConverter.GetBytes(messageBytes.Length);
            int totalLength = size.Length + messageBytes.Length;

            var message = new OutboundMessage<byte[]>(messageBytes, null);

            writerMock.Setup(x => x.Write(message.Message)).Verifiable();

            queue.Add(message);
            spooler.Spool();

            writerMock.Verify();
        }

        [TestMethod]
        public void ShouldImplementIRunnableAndIOuboundMessageSpoolerRunnable()
        {
            Assert.IsTrue(spooler is IRunnable && spooler is IOutboundMessageSpoolerRunnable);
        }

        [TestMethod]
        public void RunShouldCallSpool()
        {
            AutoResetEvent spoolerCalled = new AutoResetEvent(false);

            queue.Add(new OutboundMessage<byte[]>(new byte[5]{0,1,2,3,4}, null));
            writerMock.Setup(x => x.Write(It.IsAny<byte[]>())).Callback(() => spoolerCalled.Set()).Verifiable(); // If Write is called, it means Spool also has been called.

            ThreadRunner runner = new ThreadRunner(spooler);

            runner.Start();
            spoolerCalled.WaitOne(2000);
            runner.Stop();

            writerMock.Verify();

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

        [TestMethod]
        public void CallbackOnMessageObjectShouldBeCalledIfNotNull()
        {
            var callbackSuccess = false;
            SocketException callbackException = null;

            var messageBytes = Encoding.ASCII.GetBytes("Test");
            Action<bool, SocketException> callback = (success, exception) =>
            {
                callbackSuccess = success;
                callbackException = exception;
            };

            queue.Add(new OutboundMessage<byte[]>(messageBytes, callback));

            spooler.Spool();

            Assert.IsTrue(callbackSuccess);
            Assert.IsNull(callbackException);
        }

        [TestMethod]
        public void CallbackOnMessageObjectShouldBeCalledWithExceptionObjectOnSocketError()
        {
            var callbackSuccess = false;
            SocketException callbackException = null;

            var messageBytes = Encoding.ASCII.GetBytes("Test");

            Action<bool, SocketException> callback = (success, exception) =>
            {
                callbackSuccess = success;
                callbackException = exception;
            };

            queue.Add(new OutboundMessage<byte[]>(messageBytes, callback));

            writerMock.
                Setup(x => x.Write(It.IsAny<byte[]>())).
                Throws(new IOException("fake test exception", new SocketException()));

            spooler.Spool();

            Assert.IsFalse(callbackSuccess);
            Assert.IsNotNull(callbackException);
        }
    }
}
