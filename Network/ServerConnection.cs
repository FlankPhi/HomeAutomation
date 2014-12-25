using System;
using System.Net.Sockets;
using HomeAutomation.Abstract;

namespace HomeAutomation.Network
{
    public class ServerConnection
    {
        
        public Socket KeepAlive { get; private set; }
        private Socket CommSocket { get; set; }
        //public IPEndPoint DataEndPoint { get; private set; }
        //public IPEndPoint KeepAliveEndPoint { get; private set; }
        private readonly int _keepAlivePort;

        private IController Parent { get; set; }


        public ServerConnection(IController parent, int keepAlivePort = 59400)
        {
            Parent = parent;
            _keepAlivePort = keepAlivePort;
                      
            Connect();
        }

        public void Connect()
        {
            if(Parent == null) throw new Exception("What? You tried to connect with a null Parent...");

            var tcpConnector = new Connector();
            KeepAlive = tcpConnector.ConnectTo(Parent.ServerIp, _keepAlivePort);
            CommSocket = tcpConnector.ConnectTo(Parent.ServerIp, Parent.ServerPort);
        }
    }
}