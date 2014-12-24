using System;
using System.Collections;
using System.Text;
using System.Threading;
using Microsoft.SPOT.Hardware;

namespace HomeAutomation
{
    public class LcdGpioTransferProtocol : ILcdTransferProtocol
    {
        private ILcd _parent;

        private readonly OutputPort _rsPort;
        private readonly OutputPort _enablePort;
        private readonly OutputPort _d4;
        private readonly OutputPort _d5;
        private readonly OutputPort _d6;
        private readonly OutputPort _d7;
        private byte[] _rowAddress;
        private byte[] _secondHalfAddress;
        private byte[] _firstHalfAddress;


        public LcdGpioTransferProtocol(Cpu.Pin rsPin, Cpu.Pin enablePin,
            Cpu.Pin d4, Cpu.Pin d5, Cpu.Pin d6, Cpu.Pin d7)
        {
            
            _rsPort = new OutputPort(rsPin, false);
            _enablePort = new OutputPort(enablePin, false);
            _d4 = new OutputPort(d4, false);
            _d5 = new OutputPort(d5, false);
            _d6 = new OutputPort(d6, false);
            _d7 = new OutputPort(d7, false);            
        }

        public void UpdateDisplayOptions()
        {
            var command = (byte)Lmb162Abc.Command.DisplayMode;
            command |= _parent.IsActive ? (byte)Lmb162Abc.DisplayMode.DisplayOn : (byte)Lmb162Abc.DisplayMode.DisplayOff;
            command |= _parent.ShowCursor ? (byte)Lmb162Abc.DisplayMode.CursorOn : (byte)Lmb162Abc.DisplayMode.CursorOff;
            command |= _parent.BlinkCursor ? (byte)Lmb162Abc.DisplayMode.CursorBlinkOn : (byte)Lmb162Abc.DisplayMode.CursorBlinkOff;

            SendCommand(command);
        }

        public void Initialize(ILcd parentLcd)
        {
            _parent = parentLcd;

           

            _rowAddress = new byte[] { 0x00, 0x40, 0x14, 0x54 };
            _firstHalfAddress = new byte[] { 0x10, 0x20, 0x40, 0x80 };
            _secondHalfAddress = new byte[] { 0x01, 0x02, 0x04, 0x08 };

            _parent.IsActive = true;
            _parent.ShowCursor = false;
            _parent.BlinkCursor = false;

            Thread.Sleep(250);

            _rsPort.Write(false);
            _enablePort.Write(false); // Enable provides a clock function to syncrhonize data transfer

            Write(0x03, _secondHalfAddress);
            Thread.Sleep(4);
            Write(0x03, _secondHalfAddress);
            Thread.Sleep(4);
            Write(0x03, _secondHalfAddress);
            Thread.Sleep(150);
            Write(0x02, _secondHalfAddress);

            var rowValue = _parent.RowCount == 2 ? Lmb162Abc.LcdFunction.TwoLineDisplay : Lmb162Abc.LcdFunction.OneLineDisplay;
            SendCommand((byte)((byte)Lmb162Abc.Command.LcdFunction | ((byte)Lmb162Abc.LcdFunction.FourBitMode | (byte)rowValue | (byte)Lmb162Abc.LcdFunction.Font5X8)));

            UpdateDisplayOptions();

            SendCommand((byte)Lmb162Abc.Command.Clear);
            Thread.Sleep(2);

            SendCommand(((byte)Lmb162Abc.Command.EntryModeSet | (byte)Lmb162Abc.EntryMode.FromLeft |
                         (byte)Lmb162Abc.EntryMode.ShiftDecrement));

        }

        private void Write(byte[] data)
        {
            foreach (var value in data)
            {
                Write(value, _firstHalfAddress); // First half
                Write(value, _secondHalfAddress); // Second half
            }
        }

        private void Write(byte value, byte[] halfAddress)
        {
            _d4.Write((value & halfAddress[0]) > 0);
            _d5.Write((value & halfAddress[1]) > 0);
            _d6.Write((value & halfAddress[2]) > 0);
            _d7.Write((value & halfAddress[3]) > 0);

            _enablePort.Write(true);
            _enablePort.Write(false);

        }

        private void ResetLines()
        {
            if (_parent.CursorPosition == 0) return;
            if (_parent.CursorPosition % _parent.ColumnCount != 0) return;

            if (_parent.CurrentRow < 1) _parent.CurrentRow += 1;
            else
            {
                Thread.Sleep(500);
                SendCommand((byte)Lmb162Abc.Command.Clear);
                _parent.CurrentRow = 0;
            }
            MoveCursor(0, (byte)(_parent.CurrentRow));
        }

        public void SendCommand(byte command)
        {
            _rsPort.Write(false);
            Write(new[] { command });
            Thread.Sleep(5);
        }

        public void SendData(byte[] data)
        {
            _rsPort.Write(true);
            Write(data);
        }

        public byte[] ReceiveData()
        {
            throw new NotImplementedException();
        }

        public void MoveCursor(byte column, byte row)
        {
            row = (byte)(row % 2);
            SendCommand((byte)((byte)Lmb162Abc.Command.SetDdRam | (byte)(column + _rowAddress[row])));
        }

        public void SendLine(string text)
        {
            if (text.Length > _parent.ColumnCount)
            {
                SendLines(text);
                return;
            }
            SendData(Encoding.UTF8.GetBytes(text));

        }

        public void SendLine(string text, int delay, bool newLine)
        {
            if (newLine) _parent.CursorPosition = 0;
            foreach (var textChar in text.ToCharArray())
            {
                ResetLines();
                SendData(Encoding.UTF8.GetBytes(textChar.ToString()));
                _parent.CursorPosition += 1;
                Thread.Sleep(delay);
            }
        }

        private string[] SplitText(string str)
        {
            if (str.Length > _parent.ColumnCount * _parent.RowCount)
                str = str.Substring(0, _parent.ColumnCount * _parent.RowCount);

            var stringArrayCounter = 0;
            _parent.CursorPosition = 0;

            var charArray = str.ToCharArray();
            var arraySize = (int)Math.Ceiling((double)(str.Length + _parent.CursorPosition) / _parent.ColumnCount);
            var stringArray = new string[arraySize];

            for (var i = 0; i < charArray.Length; i++)
            {
                if (_parent.CursorPosition < _parent.ColumnCount)
                {
                    stringArray[stringArrayCounter] = stringArray[stringArrayCounter] + charArray[i];
                    _parent.CursorPosition += 1;
                }
                else
                {
                    _parent.CursorPosition = 1;
                    stringArrayCounter += 1;
                    stringArray[stringArrayCounter] = stringArray[stringArrayCounter] + charArray[i];
                }
            }
            return stringArray;
        }

        private void SendLines(string text)
        {
            var splitText = SplitText(text);

            foreach (var line in splitText)
            {
                MoveCursor(0, (byte)(_parent.CurrentRow));
                SendData(Encoding.UTF8.GetBytes(line));
                if (_parent.CurrentRow == _parent.RowCount - 1)
                {
                    Thread.Sleep(500);
                    _parent.CurrentRow = 0;
                }
                else _parent.CurrentRow += 1;
            }
        }
    }
}