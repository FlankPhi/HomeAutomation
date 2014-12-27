using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD;
using HomeAutomation.Components.Sensors;
using HomeAutomation.Controllers;
using HomeAutomation.Network;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation
{
    public sealed class Device : IDevice
    {
        public IController Controller { get; private set; }
        public IConnectionManager ConnectionManager
        {
            get { return Controller.ConnectionManager; }
        }
        public IDisplay LcdDisplay { get; private set; }       
        public IrTempSensor TempSensor { get; private set; }
        public MotionSensor MotionSensor { get; private set; }

        public void Init(IController controller)
        {
            LcdDisplay = new Lmb162Abc(Pins.GPIO_PIN_D12, Pins.GPIO_PIN_D11,                               //rs and enable
                Pins.GPIO_PIN_D5, Pins.GPIO_PIN_D4, Pins.GPIO_PIN_D3, Pins.GPIO_PIN_D2,         //data pins, d4-d7
                16, 2, controller);
            TempSensor = new IrTempSensor(0x5a, 59, controller,updateInterval:1000);
            MotionSensor = new MotionSensor(Pins.GPIO_PIN_D8);
            Controller = controller;
            ((TcpConnectionManager)ConnectionManager).TcpCommandHandler.Init();
        } 
        
    }    
}