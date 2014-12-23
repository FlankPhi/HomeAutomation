using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation
{
    public class MotionSensor : IComponent
    {


        private readonly InterruptPort _pir;
        //private readonly Lcd _display;

        public MotionSensor(Cpu.Pin inputPin, IController parent)
        {
            _pir = new InterruptPort(inputPin, false, ResistorModes.PullDown, InterruptModes.InterruptEdgeHigh);
            Parent = parent;
        }

        public IController Parent { get; set; }

        public void Activate()
        {
            _pir.OnInterrupt += OnTrip;
        }

        public void Deactivate()
        {
            _pir.OnInterrupt -= OnTrip;
        }

        private void OnTrip(uint data1, uint data2, DateTime time)
        {
            Debug.Print("Alarm tripped.");
            Thread.Sleep(50);
            _pir.ClearInterrupt();
        }


    }
}