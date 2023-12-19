using System;
using System.IO.Ports;
using System.Windows.Forms;
using static Cognex.InSight.Controls.Display.Internal.CvsDisplayableChart;

namespace nRadLite.DataComm.IR
{
    internal class IRModbus
    {
        #region Properties

        public bool IsConnected => _sp != null && _sp.IsOpen;

        public string ModbusStatus { get; private set; } = "";

        #endregion

        #region Data Members

        private SerialPort _sp = new SerialPort();

        #endregion

        #region Functions

        public IRModbus()
        {
        }

        ~IRModbus()
        {
        }

        public bool Open(string portName, int baudRate, int databits, Parity parity, StopBits stopBits)
        {
            _sp = new SerialPort();

            //Ensure port isn't already opened:
            if (!_sp.IsOpen)
            {
                //Assign desired settings to the serial port:
                _sp.PortName = portName;
                _sp.BaudRate = baudRate;
                _sp.DataBits = databits;
                _sp.Parity = parity;
                _sp.StopBits = stopBits;
                //These timeouts are default and cannot be editted through the class at this point:
                _sp.ReadTimeout = 1000;
                _sp.WriteTimeout = 1000;

                try
                {
                    _sp.Open();
                }
                catch (Exception err)
                {
                    ModbusStatus = $"Error opening {portName}: {err.Message}";
                    return false;
                }

                ModbusStatus = $"{portName} opened successfully";
                return true;
            }
            else
            {
                ModbusStatus = $"{portName} already opened";
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                _sp.Close();
                _sp = null;
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                return false;
            }
        }

        private void GetCRC(byte[] message, ref byte[] CRC)
        {
            //Function expects a modbus message of any length as well as a 2 byte CRC array in which to 
            //return the CRC values:

            ushort CRCFull = 0xFFFF;
            char CRCLSB;

            for (int i = 0; i < (message.Length) - 2; i++)
            {
                CRCFull = (ushort)(CRCFull ^ message[i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }

            CRC[1] = (byte)((CRCFull >> 8) & 0xFF);
            CRC[0] = (byte)(CRCFull & 0xFF);
        }

        private void BuildMessage(byte address, byte type, ushort start, ushort registers, ref byte[] message)
        {
            //Array to receive CRC bytes:
            byte[] CRC = new byte[2];

            message[0] = address;
            message[1] = type;
            message[2] = (byte)(start >> 8);
            message[3] = (byte)start;
            message[4] = (byte)(registers >> 8);
            message[5] = (byte)registers;

            GetCRC(message, ref CRC);

            message[message.Length - 2] = CRC[0];
            message[message.Length - 1] = CRC[1];
        }

        private bool CheckResponse(byte[] response)
        {
            byte[] CRC = new byte[2];

            GetCRC(response, ref CRC);

            return CRC[0] == response[response.Length - 2] && 
                   CRC[1] == response[response.Length - 1] ? 
                   true : false;
        }

        private void GetResponse(ref byte[] response)
        {
            //There is a bug in .Net 2.0 DataReceived Event that prevents people from using this
            //event as an interrupt to handle data (it doesn't fire all of the time).  Therefore
            //we have to use the ReadByte command for a fixed length as it's been shown to be reliable.
            for (int i = 0; i < response.Length; i++)
            {
                response[i] = (byte)_sp.ReadByte();
            }
        }

        public bool SendFc16(byte address, ushort start, ushort registers, short[] values)
        {
            //Ensure port is open:
            if (!_sp.IsOpen)
            {
                ModbusStatus = "Serial port not open";
                return false;
            }

            //Clear in/out buffers:
            _sp.DiscardOutBuffer();
            _sp.DiscardInBuffer();

            //Message is 1 addr + 1 fcn + 2 start + 2 reg + 1 count + 2 * reg vals + 2 CRC
            byte[] message = new byte[9 + 2 * registers];
            //Function 16 response is fixed at 8 bytes
            byte[] response = new byte[8];

            //Add bytecount to message:
            message[6] = (byte)(registers * 2);

            //Put write values into message prior to sending:
            for (int i = 0; i < registers; i++)
            {
                message[7 + 2 * i] = (byte)(values[i] >> 8);
                message[8 + 2 * i] = (byte)(values[i]);
            }

            //Build outgoing message:
            BuildMessage(address, (byte)16, start, registers, ref message);//Change To 6 If 16 Does Not Work

            //Send Modbus message to Serial Port:
            try
            {
                _sp.Write(message, 0, message.Length);
                GetResponse(ref response);
            }
            catch (Exception err)
            {
                ModbusStatus = "Error in write event: " + err.Message;
                return false;
            }

            //Evaluate message:
            if (CheckResponse(response))
            {
                ModbusStatus = "Write successful";
                return true;
            }
            else
            {
                ModbusStatus = "CRC error";
                return false;
            }
        }

        public bool SendFc4(byte address, ushort start, ushort registers, ref short[] values)
        {
            //Ensure port is open:
            if (_sp.IsOpen)
            {
                //Clear in/out buffers:
                _sp.DiscardOutBuffer();
                _sp.DiscardInBuffer();

                //Function 3 request is always 8 bytes:
                byte[] message = new byte[8];
                //Function 3 response buffer:
                byte[] response = new byte[5 + 2 * registers];

                BuildMessage(address, 4, start, registers, ref message);

                try
                {
                    _sp.Write(message, 0, message.Length);
                    GetResponse(ref response);
                }
                catch (Exception err)
                {
                    ModbusStatus = "Error in read event: " + err.Message;
                    return false;
                }

                //Evaluate message:
                if (CheckResponse(response))
                {
                    //Return requested register values:
                    for (int i = 0; i < (response.Length - 5) / 2; i++)
                    {
                        values[i] = response[2 * i + 3];
                        values[i] <<= 8;
                        values[i] += response[2 * i + 4];
                    }
                    ModbusStatus = "Read successful";
                    return true;
                }
                else
                {
                    ModbusStatus = "CRC error";
                    return false;
                }
            }
            else
            {
                ModbusStatus = "Serial port not open";
                return false;
            }
        }

        #endregion
    }
}