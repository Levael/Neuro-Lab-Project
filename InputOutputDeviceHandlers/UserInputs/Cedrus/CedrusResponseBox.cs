using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

namespace InputOutputDeviceHandlers.UserInputs.Cedrus
{
    public class CedrusResponseBox : IUserInputController
    {
        private SerialPort _serialPort;
        
        /*private const int BTN_START  = 255;
        private const int BTN_LEFT   = 254;
        private const int BTN_RIGHT  = 253;
        private const int BTN_DOWN   = 252;
        private const int BTN_UP     = 251;*/

        private const int StartPress = 255;
        private const int LeftPress = 254;
        private const int RightPress = 253;
        private const int DownPresss = 252;
        private const int UpPress = 251;

        public CedrusResponseBox()
        {
            _serialPort = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();
        }

        public void FlushBuffer()
        {
            _serialPort.BaseStream.Flush();
        }

        public bool IsStartButtonPressed()
        {
            byte[] value = new byte[1];
            if (_serialPort.BytesToRead > 0)
            {
                if (_serialPort.Read(value, 0, 1) == 1 && value[0] == StartPress)
                {
                    return true;
                }
            }

            return false;
        }

        public bool LoadButtonsMapping()
        {
            throw new NotImplementedException();
        }

        public PressType SubjectChoice()
        {
            byte[] value = new byte[1];
            if (_serialPort.BytesToRead > 0)
            {
                if (_serialPort.Read(value, 0, 1) == 1)
                {
                    switch (value[0])
                    {
                        case RightPress:
                            return PressType.Right;
                        case LeftPress:
                            return PressType.Left;
                        case UpPress:
                            return PressType.Up;
                        case DownPresss:
                            return PressType.Down;
                        default:
                            break;
                    }
                }
            }

            return PressType.None;
        }
    }
}
