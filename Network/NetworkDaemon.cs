using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HomeAutomation.Abstract;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class NetworkDaemon
    {
        private static NetworkDaemon _daemon;
        public ServerConnection ServerConnection { get; private set; }
        
        private NetworkDaemon(IController parent)
        {
            ServerConnection = new ServerConnection(parent);
               
            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();
        }
        public static NetworkDaemon GetInstance(IController parent)
        {
            return _daemon ?? (_daemon = new NetworkDaemon(parent));
        }

        private void KeepAlive()
        {
            while (true)
            {
                Thread.Sleep(30000);
                
                if (!SendData(ServerConnection.KeepAlive, "Hello?"))
                {
                    Debug.Print("Error Sending Keepalive.");
                    ServerConnection.Connect();
                    continue;
                }
                Debug.Print("Sent Hello?...");
                var receivedData = ReceiveData(ServerConnection.KeepAlive, size: 6);
                var datastring = new String(Encoding.UTF8.GetChars(receivedData));
                Debug.Print(datastring);                                  
            }          
        }

        public static byte[] ReceiveData(Socket socket, int timeout = 10000, int size = 4096, int offset = 0)
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
                    Debug.Print("Receive timeout reached. Error code: "+ ex.Message);
                    break;
                }

            } while (received < size);
            socket.ReceiveTimeout = 0;
            
            return message;
        }

        
        public bool SendData(Socket socket, object data)
        {
            if (socket == null) return false;
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(data.ToString()));
            }
            catch (SocketException e)
            {
                Debug.Print("Disconnected: error code " + e.ErrorCode);
                return false;
            }
            
            return true;


        }
    }
}