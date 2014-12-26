using System;
using System.Net;
using System.Net.Sockets;
using HomeAutomation.Abstract;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class SingleDataConnection : IConnection, ITcpConnection
    {       
        public Socket DebugSocket { get; private set; }
        public Socket DataSocket { get; private set; }

        public IPAddress RemoteIpAddress { get; private set; }
        public int DataPort { get; private set; }
        public int DebugPort { get; private set; }

        private readonly EndPoint _dataEndPoint;
        private readonly EndPoint _debugEndPoint;


        public SingleDataConnection(IPAddress remoteAddress, int dataPort, int debugPort = 59400)
        {
            RemoteIpAddress = remoteAddress;
            DataPort = dataPort;
            DebugPort = debugPort;

            DebugSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            DataSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);

            _dataEndPoint = new IPEndPoint(RemoteIpAddress, DataPort);
            _debugEndPoint = new IPEndPoint(RemoteIpAddress, DebugPort);
            
            Connect();
        }

        public void Connect()
        {
            
            if (DebugSocket != null)
                if(DebugSocket.RemoteEndPoint.Equals(new IPEndPoint(RemoteIpAddress, DebugPort))) DebugSocket.Close();
            if(DataSocket != null)
                if (DataSocket.RemoteEndPoint.Equals(new IPEndPoint(RemoteIpAddress, DataPort))) DataSocket.Close();
            
            var tcpConnector = new Connector();
            DebugSocket = tcpConnector.ConnectTo(RemoteIpAddress, DebugPort);
            DataSocket = tcpConnector.ConnectTo(RemoteIpAddress, DataPort);
        }
        public byte[] ReceiveData(Socket socket, int timeout, int size, int offset)
        {
            var message = new byte[size];
            var received = 0;
            socket.ReceiveTimeout = timeout;
            do
            {
                try
                {
                    received += socket.Receive(message, offset + received, size - received, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    Debug.Print("Receive timeout reached. Error code: " + ex.Message);
                    break;
                }

            } while (received < size & socket.Poll(100, SelectMode.SelectRead));
            socket.ReceiveTimeout = 0;

            return message;
        }
        
    }
}