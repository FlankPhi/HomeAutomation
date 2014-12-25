using System.ComponentModel;
using System.Net;
using HomeAutomation.Components.Displays.LCD.Transfer_Protocols;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Network;

namespace HomeAutomation.Abstract
{
    public interface IController
    {
        
        //void AddComponent(IComponent component);
        I2CBus TwiBus { get; }
        NetworkDaemon NetDaemon { get; }
        IPAddress ServerIp { get; }
        int ServerPort { get; }
    }
}