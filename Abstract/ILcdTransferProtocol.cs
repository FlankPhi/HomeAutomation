namespace HomeAutomation.Abstract
{
    public interface ILcdTransferProtocol : ITransferProtocol
    {
        void SendLine(string text);
        void MoveCursor(byte column, byte row);
        void UpdateDisplayOptions();
        void Initialize(IDisplay parent);
        void SendLine(string data, int delay, bool newLine);
        
    }
}