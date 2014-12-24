using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using HomeAutomation.Etc.Generic;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HomeAutomation
{
    class NetworkDaemon
    {
        private static NetworkDaemon _daemon;
        public List Connections { get; private set; }

        private NetworkDaemon()
        {
            Connections = new List(typeof (ClientConnection));
            var keepAliveThread = new Thread(KeepAlive);
            keepAliveThread.Start();
        }
        public static NetworkDaemon GetInstance()
        {
            return _daemon ?? (_daemon = new NetworkDaemon());
        }


        private void KeepAlive()
        {
            var threadcounter = 0;
            while (true)
            {
                Thread.Sleep(30000);
                if (Connections.Count() <= 0) continue;
                foreach (var connection in Connections)
                {
                    if (!SendData(((ClientConnection)connection).KeepAlive, "Hello?"))
                    {
                        Debug.Print("Error Sending Keepalive.");
                        continue;
                    }
                    Debug.Print("Sent Hello?...");
                    var receivedData = ReceiveData(((ClientConnection) connection).KeepAlive, size: 6);
                    var datastring = new String(Encoding.UTF8.GetChars(receivedData));
                    Debug.Print(datastring);
                    

                }
                threadcounter++;
                Debug.Print(threadcounter.ToString());
            }
            


            //Connections.Remove(connection);
            //Connections.Remove(_dataConnection);
            //bug - ADD FUNCTIONALITY TO ACCEPT RESPONSE JUST IN CASE - THEN YOU CAN ADD IN TIMESTAMPS FOR SECURITY
        }
        public ClientConnection CreateClientConnection(IPAddress address, int port, IController parent)
        {          
            var newConnection = new ClientConnection(address, port, parent);
            Connections.Add(newConnection);
            return newConnection;
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
           
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(data.ToString()));
            }
            catch (SocketException e)
            {
                if (e.ErrorCode.Equals(10035))
                    Debug.Print("Still connected, but the send would block.");
                else
                {
                    Debug.Print("Disconnected: error code " + e.ErrorCode);
                }
                return false;
            }
            //Debug.Print(Connected(socket, SelectMode.SelectWrite).ToString());
            return true;


        }
    }
}