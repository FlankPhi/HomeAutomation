using System;
using HomeAutomation.Abstract;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation.Components.Sensors
{
    public class MotionSensor : IComponent
    {


        private readonly InterruptPort _pir;

        public IController Parent { get; private set; }

        //private readonly Lcd _display;

        public MotionSensor(Cpu.Pin inputPin, IController parent, bool onByDefault)
        {
            _pir = new InterruptPort(inputPin, false, ResistorModes.PullDown, InterruptModes.InterruptEdgeHigh);
            Parent = parent;
            Parent.AddComponent(this);
            if(onByDefault) Activate();
        }
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
            //Debug.Print("Alarm tripped.");
            //Thread.Sleep(50);
            //_pir.ClearInterrupt();
        }


    }
}