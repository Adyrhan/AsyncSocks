using System.Net;

namespace AsyncSocks
{
    public interface ITcpClient
    {
        bool Connected { get; }
        ISocket Socket { get; }
        int Read(byte[] buffer, int offset, int length);
        void Write(byte[] buffer, int offset, int length);
        void Close();
        void Connect();
        void Connect(IPEndPoint remoteEndPoint);
    }
}
