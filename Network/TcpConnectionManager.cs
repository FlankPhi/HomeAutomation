using System;
using System.Collections;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD;
using HomeAutomation.Etc;
using HomeAutomation.Etc.Delegates;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using Json.NETMF;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation.Network
{
    
    public sealed class TcpConnectionManager : IConnectionManager
    {
        public event Command CommandReceived;

        public IConnection Connection { get; private set; }
        public TcpCommandHandler TcpCommandHandler { get; private set; }
        public int KeepAliveTime { get; set; }
        public bool KeepAliveActive
        {
            get { return _keepAliveActive; }
            set { UpdateKeepAlive(value); }
        }

        private Thread _listenThread;        
        private bool _keepAliveActive;    
       
        public TcpConnectionManager(IDevice dev, IConnection connection)
        {
            Connection = connection;
            //_tcpConnection = (ITcpConnection)Connection;
            var device = (Device) dev;
            
            TcpCommandHandler = new TcpCommandHandler(this,device);

            ((ITcpConnection)Connection).Connect();
            
            
            _listenThread = new Thread(ProcessCommands);
            _listenThread.Start();                       
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
        

        private void OnCommandReceived(byte value)
        {
            if (CommandReceived != null) CommandReceived.Invoke(value);
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
                if (_keepAliveActive) keepAliveThread.Abort();
                _keepAliveActive = false;
            }
        }
        private static string GetMacAddress()
        {
            var netInterface = NetworkInterface.GetAllNetworkInterfaces();
            var macAddress = "";

            const string hexChars = "0123456789ABCDEF";

            for (int b = 0; b < 6; b++)
            {
                // Grab the top 4 bits and append the hex equivalent to the return string.
                macAddress += hexChars[netInterface[0].PhysicalAddress[b] >> 4];

                // Mask off the upper 4 bits to get the rest of it.
                macAddress += hexChars[netInterface[0].PhysicalAddress[b] & 0x0F];

                // Add the dash only if the MAC address is not finished.
                if (b < 5) macAddress += "-";
            }

            return macAddress;
        }
        private void ProcessCommands()      //Maybe refactor this someday to handle more than just a single byte, and to check whether more than just a single byte was sent...
        {                                   //
            while (true)                    //
            {
                Thread.Sleep(10);
                if (((ITcpConnection)Connection).DataSocket == null) continue;

                var bytes = GetByteArray(((ITcpConnection)Connection), ((ITcpConnection)Connection).DataSocket, 1, 0, 3000);
                if (bytes == null) continue;

                //var command = TcpCommandHandler.ServerCommands.FirstOrDefault(p => ((byte)p) == bytes[0]);
                var command = TcpCommandHandler.ServerCommandTable.Values.FirstOrDefault(p => ((byte)p) == bytes[0]);

                if (command == null) continue;

                OnCommandReceived((byte)command);
            }
        }
        private void KeepAlive() // bug - this is becoming more and more sloppy - a send and receive in the same method? talk about breaking srp... maybe plug it into server connection?
        {
            while (true)
            {
                Thread.Sleep(KeepAliveTime);

                if (!SendUtf8(((ITcpConnection)Connection).DataSocket, " ")) 
                {
                    ((ITcpConnection)Connection).Connect();
                    continue;
                }
                Debug.Print(GetString(((ITcpConnection)Connection), ((ITcpConnection)Connection).DataSocket));
            }
        }

        public bool SendJson(Socket dataSocket, Hashtable hashtable)
        {
            if (dataSocket == null) return false;
            try
            {
                var jsonString = JsonSerializer.SerializeObject(hashtable);
                SendUtf8(dataSocket, jsonString);
                return true;
            }
            catch (SocketException e)
            {
                Debug.Print("Could not send: error code " + e.ErrorCode);
                return false;
            }
        }
        public bool SendUtf8(Socket socket, object data)
        {
            if (socket == null) return false;
            try
            {
                //socket.Send(Encoding.UTF8.GetBytes(data.ToString()));
                var datatoSend = Encoding.UTF8.GetBytes(data.ToString());
                
                socket.Send(datatoSend, datatoSend.Length, SocketFlags.None);
                Debug.Print("Sent something.");
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