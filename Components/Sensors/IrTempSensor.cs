using System;
using System.Threading;
using HomeAutomation.Abstract;
using Microsoft.SPOT.Hardware;

namespace HomeAutomation.Components.Sensors
{
    
    public delegate void TemperatureUpdate(object sender, double temp);
    public class IrTempSensor : IComponent, IDisposable
    {

        public event TemperatureUpdate TempUpdate;
        public int UpdateInterval { get; set; }
        public RamRegisters DefaultRegister { get; set; }
        public Temp DefaultTemp { get; set; }
        public IController Controller { get; private set; }
        
        private readonly I2CDevice.Configuration _config;
        private readonly Thread _dataThread;
        private double _latestTemp;

        public IrTempSensor(byte i2CAddress, byte frequency, IController controller, 
            RamRegisters defaultRegister = RamRegisters.ObjectTempOne, Temp defaultTemp = Temp.Fahrenheit, 
            int updateInterval = 2500)
        {
            _config = new I2CDevice.Configuration(i2CAddress, frequency);
            Controller = controller;

            DefaultRegister = defaultRegister;
            DefaultTemp = defaultTemp;
            UpdateInterval = updateInterval;

            _dataThread = new Thread(UpdateTemps);
            _dataThread.Start();
            
        }
        private void UpdateTemps()
        {
            while (true)
            {
                Thread.Sleep(UpdateInterval);
                _latestTemp = CalculateTemp(Controller.TwiBus.ReadRegister(_config, (byte)DefaultRegister, 1000), DefaultTemp);
                OnUpdate();
            }
        }

        private static double CalculateTemp(byte[] registerValue, Temp units)
        {
            double temp = ((registerValue[1] & 0x007F) << 8) + registerValue[0];
            temp = (temp * .02) - 0.01; // 0.02 deg./LSB (MLX90614 resolution)
            var celcius = temp - 273.15;
            var fahrenheit = (celcius * 1.8) + 32;
            switch (units)
            {
                case Temp.Celcius:
                    return celcius;
                case Temp.Kelvin:
                    return temp;
                case Temp.Fahrenheit:
                    return fahrenheit;
                default:
                    return 0;
            }
        }
        public void Dispose()
        {
            _dataThread.Abort();
        }

        private void OnUpdate()
        {
            if (TempUpdate != null) TempUpdate(this, _latestTemp);
        }
        public enum RamRegisters : byte
        {
            AreaTemperature = 0x06,
            ObjectTempOne = 0x07,
            ObjectTempTwo = 0x08,
            RawIrChannelOne = 0x04,
            RawIrChannelTwo = 0x05
        }
        public enum Temp
        {
            Celcius,
            Kelvin,
            Fahrenheit
        }
        public enum PwmControl
        {
            PwmExtended = 0x00,
            PwmSingle = 0x01,
            PwmEnable = 0x02,
            PwmDisable = 0x00,
            SdaOpenDrain = 0x00,
            SdaPushPull = 0x04,
            ThermalRelaySelected = 0x08,
            PwmSelected = 0x00
        }                   
    }

    
}




//_obj1C = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempOne, 1000),Temp.Celcius);
//_obj1F = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempOne, 1000),Temp.Fahrenheit);
//_obj1K = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempOne, 1000),Temp.Kelvin);
//_obj2C = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempTwo, 1000),Temp.Celcius);
//_obj2F = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempTwo, 1000), Temp.Fahrenheit);
//_obj2K = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.ObjectTempTwo, 1000), Temp.Kelvin);
//_areaC = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.AreaTemperature, 1000),Temp.Celcius);
//_areaF = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.AreaTemperature, 1000), Temp.Fahrenheit);
//_areaK = CalculateTemp(Controller.TwiBus.ReadRegister(Config, (byte) RamRegisters.AreaTemperature, 1000), Temp.Kelvin);                                   