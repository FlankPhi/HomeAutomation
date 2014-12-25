namespace HomeAutomation.Abstract
{
    public interface IComponent
    {
        IController Parent { get; }
    }
}