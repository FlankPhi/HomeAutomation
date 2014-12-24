using System.Net;
using System.Net.Sockets;

namespace HomeAutomation
{
    public class ClientConnection
    {
        //private Guid _id;
        public Socket KeepAlive { get; private set; }
        public Socket CommSocket { get; private set; }
        public IPEndPoint Endpoint { get; set; }

        

        private int _remotePort;
        

        public IController Parent { get; private set; }


        public ClientConnection(IPAddress address, int port, IController parentComponent, int keepAlivePort = 59400)
        {
            _remotePort = port;
            
            Parent = parentComponent;
            Endpoint = new IPEndPoint(address, port);
            var keepAliveEndPoint = new IPEndPoint(address, keepAlivePort);
            CommSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            KeepAlive = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            KeepAlive.Connect(keepAliveEndPoint);
            CommSocket.Connect(Endpoint);
        }

        
    }
}