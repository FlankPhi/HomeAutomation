using System.Net;
using System.Threading;
using HomeAutomation.Etc.Generic;
using Microsoft.SPOT.Hardware;

namespace HomeAutomation
{
    public sealed class NetDuinoPlus2 : IController
    {

        //private readonly I2CBus _twiBus;
        private readonly NetworkDaemon _netDaemon;
        
        private readonly ClientConnection _tcpConnection;

        public List Components { get; private set; }


        public event SensorEvent  RegisterEvent;

        public I2CBus TwiBus { get; private set; }

        private NetDuinoPlus2()
        {
            TwiBus = I2CBus.GetInstance();
            _netDaemon = NetworkDaemon.GetInstance();
            Components = new List(typeof(IComponent));                       
        }
        public NetDuinoPlus2(IPAddress serverAddress, int serverPort) : this()
        {           
            _tcpConnection = _netDaemon.CreateClientConnection(serverAddress, serverPort, this);            
        }

        public void AddComponent(IComponent component)
        {
            if(!Components.Contains(component)) Components.Add(component);
        }

        private void Execute(IComponent component)
        {
            if (RegisterEvent != null) RegisterEvent(component);
        }

        
    }
}