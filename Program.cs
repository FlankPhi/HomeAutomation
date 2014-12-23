using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;


namespace HomeAutomation
{
    public static class Program
    {
        public static void Main()
        {
            // write your code here
            var netduino = new NetDuinoPlus2();

            var gpioLcd = new Lmb162Abc(Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D11,                               //rs and enable
                            Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D2,         //data pins, d4-d7
                            16,2);                                                                          //rows and columns    

            var tempSensor = new IrTempSensor(0x5a, 59);

            var motionSensor = new MotionSensor(Pins.GPIO_PIN_D7, netduino);
            motionSensor.Activate();

            while (true)
            {
                var registerValue = netduino.TwiBus.ReadRegister(tempSensor.Config, (byte)IrTempSensor.RamRegisters.ObjectTempOne, 1000);
                gpioLcd.SetCursor(11,0);
                gpioLcd.WriteLine(tempSensor.CalculateTemp(registerValue, IrTempSensor.Temp.Fahrenheit).ToString().Substring(0,5));
                

            }
            
            netduino.AddComponent(gpioLcd);

        }

        

    }
}

