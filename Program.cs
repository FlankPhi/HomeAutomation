using System.Collections;
using System.Net;
using System.Threading;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Controllers;
//using HomeAutomation.Devices;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Network;
using Json.NETMF;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware;


namespace HomeAutomation
{
    public static class Program
    {
 
        public static void Main()
        {
            // Usage - first, instantiate an IDevice.
            // Then, instantiate an IConnection, and pass both the device and connection into
            // an instantiated IConnectionManager.  Then pass the IConnectionManager to an instantiated
            // IController. Finally, initialize the device, passing in your controller.
            // Any components should be added in your particular Device class
            // Any commands added should be placed in the CommandHandler class - instructions included.

            var device = new Device(); 
            var serverConnection = new SingleDataConnection(IPAddress.Parse("192.168.168.101"),420);
            var connectionManager = new TcpConnectionManager(device, serverConnection) { KeepAliveActive = true, KeepAliveTime = 30000 };
            var netduino = new NetDuinoPlus2(connectionManager);

            device.Init(netduino);
            
            while (true)
            {               
                Debug.Print("Available RAM: " + Debug.GC(true));
                Thread.Sleep(30000);
            }          
        }
    }
}

