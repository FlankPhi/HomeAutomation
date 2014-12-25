using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HomeAutomation.Network
{
    public class Connector
    {
        private IPEndPoint _remoteEndPoint;
        private AutoResetEvent _waitForConnection;
        Socket _socket;

        public Socket ConnectTo(IPAddress address, int port)
        {
            _remoteEndPoint = new IPEndPoint(address, port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (true)
            {
                _waitForConnection = new AutoResetEvent(false);
                var connectorThread = new Thread(WaitForConnection);
                connectorThread.Start();
                if (!_waitForConnection.WaitOne(4000, false))
                {
                    connectorThread.Abort();
                    _socket.Close();
                    _socket = null;
                }
                return _socket;
            }
        }

        private void WaitForConnection()
        {
            _socket.Connect(_remoteEndPoint);
            _waitForConnection.Set();
        }
    }
}