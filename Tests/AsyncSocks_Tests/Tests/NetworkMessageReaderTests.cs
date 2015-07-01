﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AsyncSocks;
using System.Text;

namespace AsyncSocks_Tests
{
    [TestClass]
    public class NetworkMessageReaderTests
    {
        [TestMethod]
        public void ReadShouldReturnFullMessage()
        {
            Mock<ITcpClient> tcpClientMock = new Mock<ITcpClient>();
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

            NetworkMessageReader reader = new NetworkMessageReader(tcpClientMock.Object);
            byte[] readMessage = reader.Read();

            Assert.AreEqual(messageString, Encoding.ASCII.GetString(readMessage));
            
        }
    }
}
