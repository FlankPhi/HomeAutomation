using System;
using System.Collections;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Etc;
using HomeAutomation.Etc.Delegates;
using HomeAutomation.Network;

namespace HomeAutomation
{
    public class TcpCommandHandler : ICommandHandler, IDisposable
    {
        private readonly IDevice _device;
        private readonly TcpConnectionManager _connectionManager;

       
        //Adding a command here means a) adding a corresponding private function and 
        //                            b) Subscribing the method to CmdReceived event in Init() method
        //                            c) unsubscribing to that event in Dispose() method
        //                            d) adding corresponding command in HomeAutomationServer code  
        public readonly Hashtable ServerCommandTable = new Hashtable()
        {
            {"Get Command List", (byte) 0x00},
            {"Get Current Temp", (byte) 0x02},
            {"Temp - Celcius", (byte) 0x03},
            {"Temp - Fahrenheit", (byte) 0x04},
            {"Temp - Kelvin",(byte) 0x05},
            {"Temp - Set to Object2",(byte) 0x06},
            {"Temp - Set to Object1",(byte) 0x07},
            {"Temp - Set to Area1",(byte) 0x08},
            {"Start Motion Sensor Notifications",(byte) 0x09},
            {"Stop Motion Sensor Notifications",(byte) 0x0A},            
        };
        

        private void SendCommandTable(byte command)
        {
            if (command != 0x00) return;

            _connectionManager.SendJson(
                ((SingleDataConnection) _connectionManager.Connection).DataSocket, ServerCommandTable);            
        }

        public void Init()
        {         
            ((Device)_device).TempSensor.SensorUpdate += TempUpdated;
            ((Device)_device).MotionSensor.MotionTriggered += MotionDetected;

            _connectionManager.CommandReceived += SendTemp;
            _connectionManager.CommandReceived += SwitchToCelcius;
            _connectionManager.CommandReceived += SwitchToFahrenheit;
            _connectionManager.CommandReceived += StartMotionNotify;
            _connectionManager.CommandReceived += StopMotionNotify;
            _connectionManager.CommandReceived += SendCommandTable;
            _connectionManager.CommandReceived += SwitchToKelvin;
            _connectionManager.CommandReceived += SwitchToObject1;
            _connectionManager.CommandReceived += SwitchToObject2;
            _connectionManager.CommandReceived += SwitchToArea1;
        }
        public void Dispose()
        {
            _connectionManager.CommandReceived -= SendTemp;
            _connectionManager.CommandReceived -= SwitchToCelcius;
            _connectionManager.CommandReceived -= SwitchToFahrenheit;
            _connectionManager.CommandReceived -= StartMotionNotify;
            _connectionManager.CommandReceived -= StopMotionNotify;
            _connectionManager.CommandReceived -= SendCommandTable;
            _connectionManager.CommandReceived -= SwitchToKelvin;
            _connectionManager.CommandReceived -= SwitchToObject1;
            _connectionManager.CommandReceived -= SwitchToObject2;
            _connectionManager.CommandReceived -= SwitchToArea1;

        }

        private void SwitchToArea1(byte command)
        {
            if (command != 0x08) return;
            ((Device) _device).TempSensor.DefaultRegister = IrTempSensor.RamRegisters.AreaTemperature;
        }

        private void SwitchToObject2(byte command)
        {
            if (command != 0x06) return;
            ((Device)_device).TempSensor.DefaultRegister = IrTempSensor.RamRegisters.ObjectTempTwo;

        }

        private void SwitchToObject1(byte command)
        {
            if (command != 0x07) return;
            ((Device)_device).TempSensor.DefaultRegister = IrTempSensor.RamRegisters.ObjectTempOne;

        }

        private void StartMotionNotify(byte command)
        {
            if (command != 0x09) return;
            ((Device)_device).MotionSensor.MotionTriggered += MotionNotify;
        }

        private void StopMotionNotify(byte command)
        {
            if (command != 0x0A) return;
            ((Device)_device).MotionSensor.MotionTriggered -= MotionNotify;
        }

        private void MotionNotify()
        {
            _connectionManager.SendUtf8(
                ((SingleDataConnection)_connectionManager.Connection).DataSocket,
                "Motion detected!");
        }
        private void SendTemp(byte command)
        {
            if (command != 0x02) return;

            _connectionManager.SendUtf8(
                ((SingleDataConnection)_connectionManager.Connection).DataSocket,
                ((Device)_device).TempSensor.CurrentTemp);
        }
        private void SwitchToCelcius(byte command)
        {
            if (command != 0x03) return;
            ((Device)_device).TempSensor.DefaultTemp = IrTempSensor.Temp.Celcius;
        }
        private void SwitchToFahrenheit(byte command)
        {
            if (command != 0x04) return;
            ((Device)_device).TempSensor.DefaultTemp = IrTempSensor.Temp.Fahrenheit;
        }

        private void SwitchToKelvin(byte command)
        {
            if (command != 0x05) return;
            ((Device)_device).TempSensor.DefaultTemp = IrTempSensor.Temp.Kelvin;
        }


        private void TempUpdated(object sender, object value)
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

        
        #region Constructor
        public TcpCommandHandler(TcpConnectionManager connectionManager, IDevice device)
        {
            _device = ((Device)device);
            _connectionManager = connectionManager;

        }
        #endregion
    }

    public interface ICommandHandler
    {
    }
}