namespace HomeAutomation
{
    public interface ILcd : IDisplay, IComponent
    {
        void WriteLine(string text);
        void WriteLine(string text, int delay, bool newLine);
        void Write(string text);
        void Clear();
        void SetCursor(byte column, byte row);
        //bool IsVisible { get; set; }
        bool ShowCursor { get; set; }
        bool BlinkCursor { get; set; }
    }
}