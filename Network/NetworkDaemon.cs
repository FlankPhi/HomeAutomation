using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Etc.Generic;
using Microsoft.SPOT;

namespace HomeAutomation.Network
{
    public class ConnectionManager
    {
        private static ConnectionManager _daemon;
        
        private ServerConnection _serverConnection;
        private IController _parent;
        private List _serverConnections;
        public ConnectionManager(ServerConnection connection)
        {
            _serverConnections = new List(typeof(ServerConnection));
            _serverConnections.Add(connection);
            _serverConnection = connection;            
            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();
        }
        
        private void KeepAlive() // bug - this is becoming more and more sloppy - a send and receive in the same method? talk about breaking srp... maybe plug it into server connection?
        {
            while (true)
            {
                Thread.Sleep(30000);

                if (!SendUtf8(_serverConnection.DebugSocket, "Hello?"))
                {
                    Debug.Print("Error Sending Keep-alive. Attempting reconnection.");
                    _serverConnection.Connect();
                    continue;
                }
                
                Debug.Print(GetString(_serverConnection,_serverConnection.DebugSocket));                                  
            }          
        }

        public string GetString(ServerConnection connection, Socket socket, int size = 4096, int offset = 0, int timeout = 10000)
        {
            if (socket.Available == 0) return null;

            var receivedData = connection.ReceiveData(socket,timeout,size,offset);
            return new String(Encoding.UTF8.GetChars(receivedData));
        }

        public byte[] GetByteArray(ServerConnection connection, Socket socket, int size = 4096, int offset = 0,
            int timeout = 10000)
        {
            return socket.Available == 0 ? null : connection.ReceiveData(socket, timeout, size, offset);
        }

        //public static byte[] ReceiveData(Socket socket, int timeout = 10000, int size = 4096, int offset = 0)
        //{
        //    var message = new byte[size];
        //    var received = 0;
        //    socket.ReceiveTimeout = timeout;
        //    do
        //    {
        //        try
        //        {
        //            received += socket.Receive(message, offset + received, size - received, SocketFlags.None);
        //        }
        //        catch (SocketException ex)
        //        {
        //            Debug.Print("Receive timeout reached. Error code: "+ ex.Message);
        //            break;
        //        }

        //    } while (received < size & socket.Poll(100,SelectMode.SelectRead) );
        //    socket.ReceiveTimeout = 0;
            
        //    return message;
        //}
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