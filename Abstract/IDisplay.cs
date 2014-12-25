namespace HomeAutomation.Abstract
{
    public interface IDisplay
    {
        int RowCount { get; }
        int ColumnCount { get; }
        int CurrentRow { get; set; }
        int CursorPosition { get; set; }

        bool IsActive { get; set; }

        
    }
}