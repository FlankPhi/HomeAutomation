using System;
using System.Collections;
using System.Threading;
using HomeAutomation.Abstract;
using HomeAutomation.Etc.Delegates;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace HomeAutomation.Components.Sensors
{
    public sealed class MotionSensor : IComponent
    {

        public event Trigger MotionTriggered;
        private readonly InterruptPort _pir; //bug -DO NOT make this a local variable - for some reason it causes the netduino to not activate the interrupt port properly.
        public MotionSensor(Cpu.Pin inputPin)
        {
            _pir = new InterruptPort(inputPin, false, ResistorModes.PullDown, InterruptModes.InterruptEdgeHigh);
            Thread.Sleep(2500);
            _pir.OnInterrupt += OnMotionDetected;
        }

        private void OnMotionDetected(uint data1, uint data2, DateTime time)
        {
            Debug.Print("Motion sensor activated.");
            if (MotionTriggered != null) MotionTriggered.Invoke();
        }

        
    }
}