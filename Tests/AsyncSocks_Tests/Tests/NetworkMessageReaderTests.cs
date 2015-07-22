using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Text;
using System.Net.Sockets;

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
            reader = new NetworkMessageReader(tcpClientMock.Object);
        }
        
        [TestMethod]
        public void ReadShouldReturnFullMessage()
        {
            string messageString = "This is a test message";

            Func<byte[], int, int, int> readImpl1 = (byte[] buffer, int offset, int lenght) => 
            { 
                byte[] size = BitConverter.GetBytes(messageString.Length);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] = size[i];
                }
                return size.Length; 
            };

            Func<byte[], int, int, int> readImpl2 = (byte[] buffer, int offset, int lenght) => 
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

            byte[] readMessage = reader.Read();

            Assert.AreEqual(messageString, Encoding.ASCII.GetString(readMessage));
            
        }

        [TestMethod]
        public void ZeroByteMessageFromTcpClientShouldReturnNullNotifyingDisconnection()
        {
            Func<byte[], int, int, int> readImpl1 = (byte[] buffer, int offset, int lenght) =>
            {
                return 0;
            };

            tcpClientMock.Setup(x => x.Read(It.IsAny<byte[]>(), 0, 4)).Returns(readImpl1).Verifiable();

            byte[] readMessage = reader.Read();

            Assert.IsNull(readMessage);

        }

        [TestMethod]
        public void ShouldImplementINetworkMessageReader()
        {
            Assert.IsTrue(reader is INetworkMessageReader);
        }
    }
}
