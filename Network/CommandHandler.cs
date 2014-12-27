using System;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Etc;
using HomeAutomation.Network;

namespace HomeAutomation
{
    public class CommandHandler : IDisposable
    {
        private readonly IDevice _device;
        private readonly TcpConnectionManager _connectionManager;

        public readonly List ServerCommands = new List(typeof(byte)) 
        {
            //Adding a command here means a) adding a corresponding private function and 
            //                            b) Subscribing the method to CmdReceived event in Init() method
            //                            c) unsubscribing to that event in Dispose() method

            (byte) 0x02, //Method SendTemp - Retreives CurrentTemp from IR Sensor
            (byte) 0x03, //Switch to Celcius readings
            
        };

        public void Init()
        {
            ((Device)_device).TempSensor.SensorUpdate += TempUpdated;
            ((Device)_device).MotionSensor.MotionTriggered += MotionDetected;


            _connectionManager.CommandReceived += SendTemp;
            _connectionManager.CommandReceived += SwitchToCelcius;


        }

        public CommandHandler(IConnectionManager connectionManager, IDevice device)
        {
            _device = ((Device) device);
            _connectionManager = ((TcpConnectionManager)connectionManager);
            
            
        }

        private void SwitchToCelcius(byte command)
        {
            if (command != 0x03) return;
            ((Device)_device).TempSensor.DefaultTemp = IrTempSensor.Temp.Celcius;
        }
        private void SendTemp(byte command)
        {
            if (command != 0x02) return;

            _connectionManager.SendUtf8(
                ((SingleDataConnection)_connectionManager.Connection).DataSocket,
                ((Device)_device).TempSensor.CurrentTemp);
        }

        private void TempUpdated(object sender, double value)
        {
            ((Device)_device).LcdDisplay.SetCursor(11, 0);
            ((Device)_device).LcdDisplay.WriteLine(value.ToString().Substring(0, 5));
        }

        private void MotionDetected()
        {
            ((Device)_device).LcdDisplay.SetCursor(0, 1);
            ((Device)_device).LcdDisplay.WriteLine("Motion Detected ", 0, true);

            Thread.Sleep(1000);

            ((Device)_device).LcdDisplay.SetCursor(0, 1);
            ((Device)_device).LcdDisplay.WriteLine("All Clear       ", 0, true);
        }

        public void Dispose()
        {
            _connectionManager.CommandReceived -= SendTemp;
        }
    }
}