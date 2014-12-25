using System;
using System.Collections;
using System.Net;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Controllers;
using Json.NETMF;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace HomeAutomation
{
    public static class Program
    {
        public static void Main()
        {
            var netduino = new NetDuinoPlus2(IPAddress.Parse("192.168.168.101"),420);
            var device = new Device(netduino);

            device.ActivateMotionSense(true);
            device.ActivateTempReadings(true);
            
            while (true)
            {               
                Debug.Print("Available RAM: " + Debug.GC(true));
                Thread.Sleep(30000);
            }          
        }
      
    }

    public sealed class Device
    {
        public IController Controller { get; private set; }     
        public IDisplay LcdDisplay { get; private set; }
        public IrTempSensor TempSensor { get; private set; }
        public MotionSensor MotionSensor { get; private set; }

        public Device(IController controller)
        {
            Controller = controller;
            CreateDevices(Controller);
        }

        private void CreateDevices(IController controller)
        {
            LcdDisplay = new Lmb162Abc(Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D11,                               //rs and enable
                            Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D2,         //data pins, d4-d7
                            16, 2, controller);
            TempSensor = new IrTempSensor(0x5a, 59, controller);
            MotionSensor = new MotionSensor(Pins.GPIO_PIN_D7, controller);
            
        }

        public void ActivateMotionSense(bool activate) //active by default
        {
            if (activate)
                MotionSensor.MotionTriggered += AlarmTripped;
            else
            {
                if (MotionSensor != null) MotionSensor.MotionTriggered -= AlarmTripped;
            }
        }

        public void ActivateTempReadings(bool activate)
        {
            if (activate)
                TempSensor.TempUpdate += TempUpdated;
            else
            {
                if (MotionSensor != null) TempSensor.TempUpdate -= TempUpdated;
            }
        }

        private void TempUpdated(object sender, double temp)
        {

            LcdDisplay.SetCursor(11,0);
            LcdDisplay.WriteLine(temp.ToString().Substring(0,5), 0, true);
        }

        private void AlarmTripped()
        {
            LcdDisplay.SetCursor(0,1);
            LcdDisplay.WriteLine("Motion Detected",0,true);
            Thread.Sleep(1000);
            LcdDisplay.WriteLine("All Clear",0,true);
        }
    }
}

