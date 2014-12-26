using System;
using System.Net;
using System.Net.Sockets;
using HomeAutomation.Abstract;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class ServerConnection
    {       
        public Socket DebugSocket { get; private set; }
        public Socket DataSocket { get; private set; }

        public IPAddress ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        
        private readonly int _keepAlivePort;

        public ServerConnection(IPAddress serverAddress, int serverPort, int keepAlivePort = 59400)
        {
            ServerIp = serverAddress;
            ServerPort = serverPort;
            _keepAlivePort = keepAlivePort;
            
            Connect();
        }

        public void Connect()
        {
            if (DebugSocket != null) DebugSocket.Close();
            if (DataSocket != null) DataSocket.Close();

            var tcpConnector = new Connector();
            DebugSocket = tcpConnector.ConnectTo(ServerIp, _keepAlivePort);
            DataSocket = tcpConnector.ConnectTo(ServerIp, ServerPort);
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