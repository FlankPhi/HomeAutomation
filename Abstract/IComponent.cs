namespace HomeAutomation
{
    public interface IComponent
    {
        IController Parent { get; set; }
    }
}