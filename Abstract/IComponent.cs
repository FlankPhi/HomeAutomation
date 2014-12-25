using System.Collections;

namespace HomeAutomation.Abstract
{
    public interface IComponent
    {
        IController Controller { get; }
        
    }
}