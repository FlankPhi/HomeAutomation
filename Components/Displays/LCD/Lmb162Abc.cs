using System.Collections;
using HomeAutomation.Abstract;
using HomeAutomation.Components.Displays.LCD.Transfer_Protocols;
using Microsoft.SPOT.Hardware;

namespace HomeAutomation.Components.Displays.LCD
{
    public class Lmb162Abc : IComponent, IDisplay
    {
        public enum Command : byte
        {
            Clear = 0x01,
            Home = 0x02,
            EntryModeSet = 0x04,
            DisplayMode = 0x08,
            CursorShift = 0x10,
            LcdFunction = 0x20,
            SetCgRam = 0x40,
            SetDdRam = 0x80
        }

        public enum DisplayMode : byte
        {
            DisplayOn = 0x04,
            DisplayOff = 0x00,
            CursorOn = 0x02,
            CursorOff = 0x00,
            CursorBlinkOn = 0x01,
            CursorBlinkOff = 0x00
        }

        public enum EntryMode : byte
        {
            FromRight = 0x00,
            FromLeft = 0x02,
            ShiftIncrement = 0x01,
            ShiftDecrement = 0x00
        }

        public enum LcdFunction : byte
        {
            FourBitMode = 0x00,
            TwoLineDisplay = 0x08,
            OneLineDisplay = 0x00,
            Font5X11 = 0x04,
            Font5X8 = 0x00
        }
        public IController Controller { get; private set; }
        private bool _isVisible;
        private bool _showCursor;
        private bool _isBlinking;
        
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public int CurrentRow { get; set; }
        public int CursorPosition { get; set; }

        public bool IsActive
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                _transferProtocol.UpdateDisplayOptions();
            }
        }
        public bool ShowCursor
        {
            get { return _showCursor; }
            set
            {
                _showCursor = value;
                _transferProtocol.UpdateDisplayOptions();
            }
        }

        public bool BlinkCursor
        {
            get { return _isBlinking; }
            set
            {
                _isBlinking = value;
                _transferProtocol.UpdateDisplayOptions();
            }
        }
        
        private readonly ILcdTransferProtocol _transferProtocol;

        public Lmb162Abc(Cpu.Pin rsPin, Cpu.Pin enablePin,
            Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7, int columnCount, int rowCount, IController controller)
        {
            Controller = controller;
            _transferProtocol = new LcdGpioTransferProtocol(rsPin, enablePin, d4, d5, d6, d7);
            ColumnCount = columnCount;
            RowCount = rowCount;
            _transferProtocol.Initialize(this);                      
            //Controller.AddComponent(this);
        }

        public void WriteLine(string text)
        {
            _transferProtocol.SendLine(text,0,true);
        }

        public void WriteLine(string text, int delay, bool newLine)
        {
            _transferProtocol.SendLine(text, delay, newLine);
        }

        public void Write(string text)
        {
            _transferProtocol.SendLine(text,0,false);
        }

        public void Clear()
        {
            _transferProtocol.SendCommand((byte)Command.Clear);
        }

        public void SetCursor(byte column, byte row)
        {
            _transferProtocol.MoveCursor(column, row);
        }
    }
}