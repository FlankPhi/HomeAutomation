namespace HomeAutomation
{
    public interface ITransferProtocol
    {
        void SendCommand(byte command);
        void SendData(byte[] data);
        byte[] ReceiveData();

    }
}