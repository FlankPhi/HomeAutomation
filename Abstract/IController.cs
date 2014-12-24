namespace HomeAutomation
{
    public interface IController
    {
        event SensorEvent RegisterEvent;
        void AddComponent(IComponent component);
        I2CBus TwiBus { get; }
    }
}