using System;
using System.Net;
using System.Net.Sockets;
using HomeAutomation.Abstract;
using HomeAutomation.Etc.Delegates;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class SingleDataConnection : IConnection, ITcpConnection
    {       
        //public Socket DebugSocket { get; private set; }
        public event Trigger Connected;
        public Socket DataSocket { get; private set; }

        public IPAddress RemoteIpAddress { get; private set; }
        public int DataPort { get; private set; }
        //public int DebugPort { get; private set; }

        public SingleDataConnection(IPAddress remoteAddress, int dataPort, bool connect = true, int debugPort = 59400)
        {
            RemoteIpAddress = remoteAddress;
            DataPort = dataPort;
            //DebugPort = debugPort;

            //DebugSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
            DataSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);    
            //if(connect) Connect();        
        }

        public void Connect()
        {
            
            //if (DebugSocket != null)
                //if(DebugSocket.RemoteEndPoint.Equals(new IPEndPoint(RemoteIpAddress, DebugPort))) DebugSocket.Close();
            if(DataSocket != null)
                if (DataSocket.RemoteEndPoint.Equals(new IPEndPoint(RemoteIpAddress, DataPort))) DataSocket.Close();
            
            var tcpConnector = new Connector();
            //DebugSocket = tcpConnector.ConnectTo(RemoteIpAddress, DebugPort);
            try
            {
                DataSocket = tcpConnector.ConnectTo(RemoteIpAddress, DataPort);
                OnConnected();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message + e.InnerException);
            }
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

        protected virtual void OnConnected()
        {
            if (Connected != null) Connected.Invoke();
        }
    }
}