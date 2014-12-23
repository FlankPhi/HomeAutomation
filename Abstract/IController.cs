namespace HomeAutomation
{
    public interface IController
    {
        I2CBus TwiBus { get; }
    }
}