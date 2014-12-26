namespace HomeAutomation.Abstract
{
    public interface IDevice
    {
        IController Controller { get; }
        IConnectionManager ConnectionManager { get; }
    }
}