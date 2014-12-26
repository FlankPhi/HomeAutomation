using System.ComponentModel;
using System.Net;
using HomeAutomation.Components.Displays.LCD.Transfer_Protocols;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Network;

namespace HomeAutomation.Abstract
{
    public interface IController
    {
        TcpConnectionManager ConnectionManager { get; }
        
    }
}