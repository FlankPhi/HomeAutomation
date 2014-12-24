namespace HomeAutomation
{
    public interface ILcdTransferProtocol : ITransferProtocol
    {
        void SendLine(string text);

        void MoveCursor(byte column, byte row);

        void UpdateDisplayOptions();

        void Initialize(ILcd parent);
        void SendLine(string data, int delay, bool newLine);
        //void SendCommand(byte command);
    }
}