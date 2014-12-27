using System.Net;
using System.Net.Sockets;

namespace HomeAutomation.Abstract
{
    public interface ITcpConnection
    {
        //Socket DebugSocket { get; }
        Socket DataSocket { get; }
        IPAddress RemoteIpAddress { get; }
        int DataPort { get; }
        //int DebugPort { get; }

        void Connect();
        byte[] ReceiveData(Socket socket, int timeout, int size, int offset);
        //void SendData();

    }
}