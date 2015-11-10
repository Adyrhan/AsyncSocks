using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using AsyncSocks.AsyncMessaging;
using System.Text;
using System.Net.Sockets;
using AsyncSocks.AsyncMessaging.Exceptions;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class NetworkMessageReaderTests
    {
        private NetworkMessageReader reader;
        private Mock<ITcpClient> tcpClientMock;

        [TestInitialize]
        public void BeforeEach()
        {
            tcpClientMock = new Mock<ITcpClient>();
            reader = new NetworkMessageReader(tcpClientMock.Object, 10 * 1024 * 1024);
        }
        
        [TestMethod]
        public void ReadShouldReturnFullMessage()
        {
            string messageString = "This is a test message";

            Func<byte[], int, int, int> readImpl1 = (byte[] buffer, int offset, int length) => 
            { 
                byte[] size = BitConverter.GetBytes(messageString.Length);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = size[i];
                }
                return size.Length; 
            };

            Func<byte[], int, int, int> readImpl2 = (byte[] buffer, int offset, int length) => 
            { 
                byte[] message = Encoding.ASCII.GetBytes(messageString);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = message[i];
                }
                return message.Length; 
            };

            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 4)).Returns(readImpl1).Verifiable();
            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, messageString.Length)).Returns(readImpl2).Verifiable();

            ReadResult<byte[]> readMessage = reader.Read();

            Assert.AreEqual(messageString, Encoding.ASCII.GetString(readMessage.Message));
            
        }

        [TestMethod]
        public void ZeroByteMessageFromTcpClientShouldReturnNullNotifyingDisconnection()
        {
            Func<byte[], int, int, int> readImpl1 = (byte[] buffer, int offset, int length) =>
            {
                return 0;
            };

            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 4)).Returns(readImpl1).Verifiable();

            ReadResult<byte[]> readMessage = reader.Read();

            Assert.IsNull(readMessage);

        }

        [TestMethod]
        public void ShouldReturnNullIfMessageSizeIsBiggerThanMaxMessageSizeField()
        {
            int size = 50 * 1024 * 1024;

            Func<byte[], int, int, int> readImpl = (byte[] buffer, int offset, int length) =>
            {
                byte[] sizeBytes = BitConverter.GetBytes(size);

                for (int i = 0; i < length; i++)
                {
                    buffer[i] = sizeBytes[i];
                }

                return length;
            };

            Func<byte[], int, int, int> readImpl2 = (byte[] buffer, int offset, int length) =>
            {
                return length;
            };

            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), 4)).
                Callback((byte[] buffer, int offset, int length) => readImpl(buffer, offset, length)).Returns(readImpl);
            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), size)).
                Callback((byte[] buffer, int offset, int length) => readImpl2(buffer, offset, length)).Returns(readImpl2);


            ReadResult<byte[]> result = reader.Read();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Error, typeof(MessageTooBigException));

            tcpClientMock.Verify(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [TestMethod]
        public void ShouldImplementINetworkMessageReader()
        {
            Assert.IsTrue(reader is INetworkMessageReader);
        }

        [TestMethod]
        public void ReturnsErrorIfMessageIsTooBig()
        {
            int messageSize = 50 * 1024 * 1024;
            Func<byte[], int, int, int> readImpl = (byte[] buffer, int offset, int length) =>
            {
                byte[] size = BitConverter.GetBytes(messageSize);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = size[i];
                }
                return size.Length;
            };

            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), 4)).
                Callback((byte[] buffer, int offset, int length) => readImpl(buffer, offset, length)).Returns(readImpl);

            ReadResult<byte[]> result = reader.Read();

            Assert.IsInstanceOfType(result.Error, typeof(MessageTooBigException));
        }
    }
}
