using System;
using System.Net;
using System.Threading;
using HomeAutomation.Components.Displays.LCD;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Controllers;

using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace HomeAutomation
{
    public static class Program
    {
        
        public static void Main()
        {
            
            //Should this be here??
            var netduino = new NetDuinoPlus2(IPAddress.Parse("192.168.168.101"),420);

            var gpioLcd = new Lmb162Abc(Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D11,                               //rs and enable
                            Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D2,         //data pins, d4-d7
                            16,2, netduino);                                                                          //rows and columns    
            var tempSensor = new IrTempSensor(0x5a, 59, netduino);
            var motionSensor = new MotionSensor(Pins.GPIO_PIN_D7, netduino, true);

            while (true)
            {
                var registerValue = netduino.TwiBus.ReadRegister(tempSensor.Config, (byte)IrTempSensor.RamRegisters.ObjectTempOne, 1000);
                var temp = tempSensor.CalculateTemp(registerValue, IrTempSensor.Temp.Fahrenheit);

                gpioLcd.SetCursor(11,0);
                gpioLcd.WriteLine(temp.ToString().Substring(0, 5));

                Thread.Sleep(1000);
            }          
        }
    }
}

