using System;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD;
using HomeAutomation.Etc;
using HomeAutomation.Etc.Delegates;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation.Network
{
    public delegate void Command(byte value);
    public sealed class TcpConnectionManager : IConnectionManager
    {
        public event Command CommandReceived;

        private Thread _listenThread;
        public int KeepAliveTime { get; set; }
        private bool _keepAliveActive;
        public bool KeepAliveActive
        {
            get { return _keepAliveActive; }
            set { UpdateKeepAlive(value); }
        }

        private List _commandList;
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

        private readonly ITcpConnection _tcpConnection;
        public CommandHandler CommandHandler { get; private set; }

        public TcpConnectionManager(IDevice dev, IConnection connection)
        {
            Connection = connection;
            _tcpConnection = (ITcpConnection)Connection;
            var device = (Device) dev;
            
            CommandHandler = new CommandHandler(this,device);

            _listenThread = new Thread(ProcessCommands);
            _listenThread.Start();
            
            
        }

        private void ProcessCommands()      //Maybe refactor this someday to handle more than just a single byte, and to check whether more than just a single byte was sent...
        {                                   //
            while (true)                    //
            {
                Thread.Sleep(10);
                if (_tcpConnection.DataSocket == null) continue;

                var bytes = GetByteArray(_tcpConnection, _tcpConnection.DataSocket, 1, 0, 3000);
                if (bytes == null) continue;

                var command = CommandHandler.ServerCommands.FirstOrDefault(p => ((byte) p) == bytes[0]);
                if (command == null) continue;
                
                OnCommandReceived((byte)command);                
            }
        }

       

        private void KeepAlive() // bug - this is becoming more and more sloppy - a send and receive in the same method? talk about breaking srp... maybe plug it into server connection?
        {
            while (true)
            {
                Thread.Sleep(KeepAliveTime);

                if (!SendUtf8(_tcpConnection.DataSocket, "Hello?")) //changing temporarily - debugsocket to datasocket
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
        public bool SendUtf8(Socket socket, object data)
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

        private void OnCommandReceived(byte value)
        {
            if (CommandReceived != null) CommandReceived.Invoke(value);
        }
    }

    
}