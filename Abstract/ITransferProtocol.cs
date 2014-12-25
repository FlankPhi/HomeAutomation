namespace HomeAutomation.Abstract
{
    public interface ITransferProtocol
    {
        void SendCommand(byte command);
        void SendData(byte[] data);
        byte[] ReceiveData();

    }
}