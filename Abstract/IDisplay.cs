namespace HomeAutomation.Abstract
{
    public interface IDisplay : IComponent
    {
        int RowCount { get; }
        int ColumnCount { get; }
        int CurrentRow { get; set; }
        int CursorPosition { get; set; }
        bool IsActive { get; set; }
        bool ShowCursor { get; set; }
        bool BlinkCursor { get; set; }
        void WriteLine(string text);
        void WriteLine(string text, int delay, bool newLine);
        void Write(string text);
        void Clear();
        void SetCursor(byte column, byte row);


    }
}