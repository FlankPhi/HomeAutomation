using System;
using System.Collections;
using System.Net;
using System.Threading;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Controllers;
using HomeAutomation.Devices;
using HomeAutomation.Network;
using Json.NETMF;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware;


namespace HomeAutomation
{
    public static class Program
    {
        private static Device _device;
        public static void Main()
        {
            var serverConnection = new SingleDataConnection(IPAddress.Parse("192.168.168.101"),420);
            var connectionManager = new TcpConnectionManager(serverConnection) { KeepAliveActive = true, KeepAliveTime = 30000 };
            var netduino = new NetDuinoPlus2(connectionManager);
            _device = new Device(netduino);

            _device.TempSensor.Start();
            _device.TempSensor.SensorUpdate += TempUpdated;
            _device.MotionSensor.MotionTriggered += MotionDetected;
            
                      
            while (true)
            {               
                Debug.Print("Available RAM: " + Debug.GC(true));
                Thread.Sleep(30000);
            }          
        }

        private static void TempUpdated(object sender, double value)
        {
            _device.LcdDisplay.SetCursor(11, 0);
            _device.LcdDisplay.WriteLine(value.ToString().Substring(0,5));
        }

        private static void MotionDetected()
        {
            _device.LcdDisplay.SetCursor(0, 1);
            _device.LcdDisplay.WriteLine("Motion Detected ", 0, true);

            Thread.Sleep(1000);

            _device.LcdDisplay.SetCursor(0, 1);
            _device.LcdDisplay.WriteLine("All Clear       ", 0, true);
        }
    }
}

