using System.Net;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD.Transfer_Protocols;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Network;

namespace HomeAutomation.Controllers
{
    public sealed class NetDuinoPlus2 : IController
    {

        public TcpConnectionManager ConnectionManager { get; private set; }
        public I2CBus TwiBus { get; private set; }

        public NetDuinoPlus2(TcpConnectionManager manager)
        {
            
            TwiBus = I2CBus.GetInstance();
            ConnectionManager = manager;
        }
    }
}