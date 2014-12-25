using System.Net;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD.Transfer_Protocols;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Etc.Generic;
using HomeAutomation.Network;

namespace HomeAutomation.Controllers
{
    public sealed class NetDuinoPlus2 : IController
    {
        public event SensorEvent RegisterEvent;
        
        public NetworkDaemon NetDaemon { get; private set; }
        public IPAddress ServerIp { get; private set; }
        public int ServerPort { get; private set; }
        public I2CBus TwiBus { get; private set; }
        public List Components { get; private set; }
      
        public NetDuinoPlus2(IPAddress serverAddress, int serverPort) 
        {           
            ServerIp = serverAddress;
            ServerPort = serverPort;
            TwiBus = I2CBus.GetInstance();
            NetDaemon = NetworkDaemon.GetInstance(this);
            Components = new List(typeof(IComponent));
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