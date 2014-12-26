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
  
        public ConnectionManager NetDaemon { get; private set; }
        public I2CBus TwiBus { get; private set; }
        
        public NetDuinoPlus2(ServerConnection connection)
        {
            
            TwiBus = I2CBus.GetInstance();
            NetDaemon = new ConnectionManager(connection);
        }
    }
}