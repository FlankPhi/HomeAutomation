using System;
using System.Collections;
using HomeAutomation.Abstract;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation.Components.Sensors
{
    public delegate void Trigger();
    public sealed class MotionSensor : IComponent
    {

        public event Trigger MotionTriggered;

        private readonly InterruptPort _pir;

        public MotionSensor(Cpu.Pin inputPin, IController controller)
        {
            _pir = new InterruptPort(inputPin, false, ResistorModes.PullDown, InterruptModes.InterruptEdgeHigh);
            Controller = controller;
            //Controller.AddComponent(this);
            _pir.OnInterrupt += OnMotionDetected;
        }
        private void OnMotionDetected(uint data1, uint data2, DateTime time)
        {
            if (MotionTriggered != null) MotionTriggered.Invoke();
        }

        public IController Controller { get; private set; }
    }
}