using System.Net;
using HomeAutomation.Etc.Delegates;

namespace HomeAutomation.Abstract
{
    public interface IController
    {
        event SensorEvent RegisterEvent;
        void AddComponent(IComponent component);
        //I2CBus TwiBus { get; }
        //NetworkDaemon NetDaemon { get; }
        IPAddress ServerIp { get; }
        int ServerPort { get; }
    }
}