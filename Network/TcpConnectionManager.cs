using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HomeAutomation.Abstract;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class TcpConnectionManager : IConnectionManager
    {
        public int KeepAliveTime { get; set; }
        private bool _keepAliveActive;
        public bool KeepAliveActive
        {
            get { return _keepAliveActive; }
            set { UpdateKeepAlive(value); }
        }

        private void UpdateKeepAlive(bool active)
        {
            var keepAliveThread = new Thread(KeepAlive);
            if (active)
            {
                if (_keepAliveActive) return;

                keepAliveThread.Start();
                _keepAliveActive = true;
            }
            else
            {
                if(_keepAliveActive) keepAliveThread.Abort();
                _keepAliveActive = false;
            }
        }

        public IConnection Connection { get; private set; }

        private ITcpConnection _tcpConnection;
        //private List _serverConnections;
        public TcpConnectionManager(IConnection connection)
        {
            //_serverConnections = new List(typeof(ServerConnection));
            //_serverConnections.Add(connection);
            Connection = connection;
            _tcpConnection = (ITcpConnection)connection;
        }
        
        private void KeepAlive() // bug - this is becoming more and more sloppy - a send and receive in the same method? talk about breaking srp... maybe plug it into server connection?
        {
            while (true)
            {
                Thread.Sleep(KeepAliveTime);

                if (!SendUtf8(_tcpConnection.DebugSocket, "Hello?"))
                {
                    _tcpConnection.Connect();
                    continue;
                }
                Debug.Print(GetString(_tcpConnection, _tcpConnection.DebugSocket, 6));                                  
            }          
        }

        public string GetString(ITcpConnection connection, Socket socket, int size = 4096, int offset = 0, int timeout = 10000)
        {
            var bytes = GetByteArray(connection, socket, size, offset, timeout);
            return bytes == null ? null : new String(Encoding.UTF8.GetChars(bytes));
        }

        public byte[] GetByteArray(ITcpConnection connection, Socket socket, int size = 4096, int offset = 0,
            int timeout = 10000)
        {
            return socket.Available == 0 ? null : connection.ReceiveData(socket, timeout, size, offset);
        }
        private static bool SendUtf8(Socket socket, object data)
        {
            if (socket == null) return false;
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(data.ToString()));
                return true;
            }
            catch (SocketException e)
            {
                Debug.Print("Could not send: error code " + e.ErrorCode);
                return false;
            }
            

        }
    }

    
}