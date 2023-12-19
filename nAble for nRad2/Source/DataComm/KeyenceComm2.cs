using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using nAble;

namespace nTact.DataComm
{
    public enum KeyenceState {Unknown=-1, HIGH = 0, LOW = 1, GO = 2, HH=3, LL=4 } 
    public class KeyenceComm2
    {
        private Semaphore semy = new Semaphore(2, 5);

        #region Manager Variables
        //property variables
        private int _baudRate = 9600;
        private Parity _parity = Parity.None;
        private StopBits _stopBits = StopBits.None;
        private int _dataBits = 8;
        private string _portName = "COM1";
        // internal variables
        private ManualResetEvent _eventDataReady = new ManualResetEvent(false);
        private readonly LogEntry _log = null;
        private bool _bRequestedData = false;
        private string _sReadData = "";
        private SerialPort _comPort = new SerialPort();
        #endregion Manager Variables
        #region Manager Properties
        /// <summary>
        /// Property to get available port names, (to be used in GUI or validation)
        /// </summary>
        public string[] AvailPortNames
        {
            get { return SerialPort.GetPortNames(); }   
        }
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public int PortBaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }
        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public int PortParityAsInt
        {
            get { return (int)_parity; }
            set { _parity = (Parity)value; }
        }
        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public Parity PortParity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public int PortStopBitsAsInt
        {
            get { return (int)_stopBits; }
            set { _stopBits = (StopBits)value; }
        }
        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public StopBits PortStopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }
        /// <summary>
        /// Denotes open status of Serial port
        /// </summary>
        public Boolean IsOpen
        {
            get { return ((_comPort != null) && _comPort.IsOpen); }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// Throws 
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set 
            {
                string sTemp = value.Trim();
                bool bFound = false;
                foreach (String sPort in SerialPort.GetPortNames())
                {
                    if (sPort.ToUpper() == sTemp.ToUpper())
                    {
                        _portName = sTemp;
                        _log.log(LogType.TRACE, Category.INFO, String.Format("Setting Serial Port to '{0}'",_portName), "INFO");
                        bFound = true;
                        break;
                    }
                }

                if (!bFound)
                {
                    if (SerialPort.GetPortNames().Length > 0)
                    {
                        _portName = SerialPort.GetPortNames()[0];
                        _log.log(LogType.TRACE, Category.INFO, "Could not find specifed port. Setting to first available","INFO");
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Could not find any serial ports on this computer","ERROR");
                    }
                }
            }
        }
        #endregion Manager Properties

        #region Manager Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="par">Desired Parity (See System.IO.Ports.Parity)</param>
        /// <param name="sBits">Desired StopBits (See System.IO.Ports.StopBits)</param>
        /// <param name="name">Desired PortName</param>
        public KeyenceComm2(LogEntry log, int baud, int dBits, int par, int sBits, string portname)
        {
            _log = log;
            _baudRate = baud;
            _parity = (Parity)par;
            _stopBits = (StopBits)sBits;
            _dataBits = dBits;
            _portName = portname;
            //now add an event handler
            _comPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
        }
        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to 9600_8N1
        /// </summary>
        public KeyenceComm2(LogEntry log)
        {
            _log = log;
            _baudRate = 9600;
            _dataBits = 8;
            _parity = Parity.None;
            _stopBits = StopBits.One;
#if DEBUG
            _portName = "COM2";
#else
            _portName = "COM1";
#endif
            //add event handler
            _comPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
        }
        ~KeyenceComm2()
        {
            if (_comPort.IsOpen)
            {
                if (_comPort.BytesToWrite != 0)
                    _comPort.DiscardOutBuffer();
                if (_comPort.BytesToRead != 0)
                    _comPort.DiscardInBuffer();
                _comPort.Close();
                _comPort.Dispose();
                _comPort = null;
            }
        }
        #endregion

        #region OpenPort
        /// <summary>
        /// Opens the Serial Port specied by 'Portname'
        /// Sends a empty message to keyence to encourage channel sync
        /// </summary>
        /// <param name="bCloseExisting">If true, closes any open port and reopens with new params</param>
        /// <returns></returns>
        public bool OpenPort(bool bCloseExisting)
        {
            bool bRetVal = false;

            if (_comPort.IsOpen && !bCloseExisting)
                return true;

            try
            {
                //first check if the port is already open
                //if its open then close it
                if (_comPort.IsOpen)
                    _comPort.Close();

                //set the properties of our SerialPort Object
                _comPort.BaudRate = _baudRate;    //BaudRate
                _comPort.DataBits = _dataBits;    //DataBits
                _comPort.StopBits = _stopBits;    //StopBits
                _comPort.Parity = _parity;    //Parity
                _comPort.PortName = _portName;   //PortName
                _comPort.Handshake = Handshake.None;
                //now open the port
                _comPort.Open();
                Thread.Sleep(250);
                if (_comPort.IsOpen)
                {
                    if (_comPort.BytesToWrite != 0)
                        _comPort.DiscardOutBuffer();
                    if (_comPort.BytesToRead != 0)
                        _comPort.DiscardInBuffer();
                    _comPort.ReadExisting();
                }

                //string sTemp = "";
                //WriteData("\n");
                Thread.Sleep(100);
                bRetVal = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("Keyence: Exception while opening COM Port '{0}': {1}", _portName, ex.Message), "ERROR");
                bRetVal = false;
            }
            return bRetVal;
        }
        #endregion
        #region ClosePort
        public bool ClosePort()
        {
            bool bRetVal = false;
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (_comPort.IsOpen == true)
                {
                    if (_comPort.BytesToWrite != 0)
                        _comPort.DiscardOutBuffer();
                    if (_comPort.BytesToRead != 0)
                        _comPort.DiscardInBuffer();
                    _comPort.Close();
                }
                bRetVal = true;
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("Keyence: Exception while closing Keyence COM Port '{0}': {1}", _portName, ex.Message), "ERROR");
                bRetVal = false;
            }
            return bRetVal;
        }
        #endregion

        #region StringToBytes
        /// <summary>
        /// method to convert string to a array of bytes (non-unicode chars)
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] StringToBytes(string sMsg)
        {
            byte[] comBuffer = new byte[sMsg.Length];
            //loop through the length of the provided string
            //convert each set of characters to a single byte
            //and add to the array
            for (int i = 0; i < sMsg.Length; i++)
                comBuffer[i] = (byte)sMsg[i];
            //return the array
            return comBuffer;
        }
        #endregion CharToBytes
        #region BytesToString
        /// <summary>
        /// method to convert a byte array of non-unicode chars into a string 
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a string</returns>
        private string BytesToString(byte[] aByte)
        {
            string sRetVal = "";
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(aByte.Length * 3);
            //loop through each byte in the array
            //convert the byte to a char and add to the stringbuilder
            foreach (byte data in aByte)
                builder.Append(Convert.ToChar(data));
            //return the converted value
            sRetVal = builder.ToString().ToUpper();
            return sRetVal;
        }
        #endregion

        public bool WriteData(string sMsg)
        {
            bool bRetVal = false;
            //convert the message to byte array
            byte[] newMsg = StringToBytes(sMsg.ToUpper() + "\r\n");
            try
            {
                //send the message to the port
                _comPort.Write(newMsg, 0, newMsg.Length);
            }
            catch (Exception)
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("Keyence: Unable to write data to port."), "ERROR");
            }

            return bRetVal;
        }

        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //_log.log(LogType.TRACE, Category.INFO, "OnDataReceived()","DEBUG");

            
            //retrieve number of bytes in the buffer
            int nBytes = _comPort.BytesToRead;
            //create a byte array to hold the awaiting data
            byte[] bComBuffer = new byte[nBytes];
            //read the data and store it
            //_log.log(LogType.TRACE, Category.INFO, "Before _comPort.Read()", "DEBUG");
            int nRecv = _comPort.Read(bComBuffer, 0, nBytes);
            //_log.log(LogType.TRACE, Category.INFO, "After _comPort.Read() - nRecv: " + nRecv.ToString(), "DEBUG");
            if (nRecv > 0 && _bRequestedData)  // we got something and it was expected
            {
                _sReadData += BytesToString(bComBuffer);
                //_log.log(LogType.TRACE, Category.INFO, "Message from port: " + _sReadData,"DEBUG");
                if (_sReadData.Contains("\n") && nRecv > 2)
                {
                    //_log.log(LogType.TRACE, Category.INFO, "Signaling _eventDataReady", "DEBUG");
                    _eventDataReady.Set();
                }
                else
                {
                    //_log.log(LogType.TRACE, Category.INFO, "Received Message but length < 2: '" + _sReadData + "'", "DEBUG");
                }
            }
            else if (!_bRequestedData)  // prob a prior timeout.. so just eat the data
            {
                _log.log(LogType.TRACE, Category.INFO, "Message received from port but not requested: " + _sReadData, "INFO");
                _sReadData = "";
            }
        }

        public bool SetDataValue(int dataID, int val)
        {
            string sRC, sTemp = $"AW,{dataID},{val}";
            bool successful = false;

            _log.log(LogType.TRACE, Category.INFO, $"Keyence: SetDataValue({dataID}, {val}).", "INFO");
            sRC = SendMsg(sTemp, 5000);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: Setting GT2 Data Value Returned: " + sRC, "INFO");

            if (!sRC.Contains("ER"))
            {
                successful = true;
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Data Value Set Successfully!", "INFO");
            } else
            {
                _log.log(LogType.TRACE, Category.ERROR, "Keyence: Data Value Set Failed!", "INFO");
            }

            return successful;
        }

        public bool SetDataValue(int dataID, double val)
        {
            string sRC, sTemp = $"AW,{dataID},{val}";
            bool successful = false;

            _log.log(LogType.TRACE, Category.INFO, $"Keyence: SetDataValue({dataID}, {val}).", "INFO");
            sRC = SendMsg(sTemp, 5000);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: Setting GT2 Data Value Returned: " + sRC, "INFO");

            if (!sRC.Contains("ER"))
            {
                successful = true;
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Data Value Set Successfully!", "INFO");
            }
            else
            {
                _log.log(LogType.TRACE, Category.ERROR, "Keyence: Data Value Set Failed!", "INFO");
            }

            return successful;
        }

        public string ReadDataValue(int dataID)
        {
            string sRC, sTemp = $"SR,00,{dataID}";
            string sRetVal = "";

            _log.log(LogType.TRACE, Category.INFO, $"Keyence: ReadDataValue({dataID}).", "INFO");
            sRC = SendMsg(sTemp, 5000);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: Reading GT2 Data Value Returned: " + sRC, "INFO");

            if (!sRC.Contains("ER"))
            {
                string[] splitted = sRC.Split(',');
                sRetVal = splitted[3];
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Data Value Read Successfully!", "INFO");
            }
            else
            {
                _log.log(LogType.TRACE, Category.ERROR, "Keyence: Data Value Read Failed!", "INFO");
            }

            return sRetVal;
        }

        private string SendMsg(string sMsg, int nTimeout)
        {
            //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Keyence.SendMsg()","DEBUG");
            string sRetVal = "";
            if (!_comPort.IsOpen)
            {
                //_log.log(LogType.TRACE, Category.INFO, "Opening com port", "DEBUG");
                OpenPort(false);
            }
            if (_comPort.IsOpen && !_bRequestedData)
            {
                // clear out any existing buffer
                if (_comPort.BytesToRead != 0)
                    _comPort.DiscardInBuffer();
                _bRequestedData = true;
                _eventDataReady.Reset();
                _sReadData = "";
                //_log.log(LogType.TRACE, Category.INFO, "DEBUG: com port open.. sending message: " + sMsg, "DEBUG");
                WriteData(sMsg);
                //_log.log(LogType.TRACE, Category.INFO, "DEBUG: WriteData() returned, now we wait", "DEBUG");

                if (_eventDataReady.WaitOne(nTimeout, false))
                {
                    //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Waitone returned successfully - '" + _sReadData + "'", "DEBUG");
                    string sTemp = _sReadData;
                    _sReadData = "";
                    _bRequestedData = false; 
                    _eventDataReady.Reset();
                    sRetVal = sTemp.Trim();
                }
                else // we timed out on  FIRST try
                {
                    // see if we got ANYTHING previous, if so then assume the first part is a partial message
                    // then wait for teh rest ONE more time
                    _log.log(LogType.TRACE, Category.INFO, String.Format("ERROR: _eventDataReady.Wait timed out: {0} ms",nTimeout), "ERROR");
                    if (_sReadData.Length > 0) // SOMETHING was there but not CR
                    {
                        _log.log(LogType.TRACE, Category.INFO, String.Format("_sReadData wasnt empty: {0}; Waiting again...", _sReadData), "INFO");
                        if (_eventDataReady.WaitOne(nTimeout, false))
                        {
                            //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Waitone returned successfully - '" + _sReadData + "'", "DEBUG");
                            string sTemp = _sReadData;
                            _sReadData = "";
                            _bRequestedData = false;
                            _eventDataReady.Reset();
                            sRetVal = sTemp.Trim();
                        }
                        else
                        {
                            _log.log(LogType.TRACE, Category.INFO, String.Format("_sReadData was incomplete.", _sReadData), "ERROR");
                            _bRequestedData = false;
                            sRetVal = "";
                        }
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, String.Format("_sReadData was empty.", _sReadData), "ERROR");
                        _bRequestedData = false;
                        sRetVal = "";
                    }
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: com port NOT open or already sending message.. inside SendMsg()", "ERROR");
            }
            return sRetVal;
        }

        /// <summary>
        /// Communicates with keyence serial amp to 'zero-out' the keyence sensor, 
        /// and then apply the shim size value to the preset value for the GT2.
        /// </summary>
        /// <param name="shimSize">Size of Current Shim in MM.</param>
        /// <param name="bankNo">(Int) Selected GT2 Bank To Preset Values In.</param>
        /// <returns>bool: True if Preset was successful</returns>
        public bool PresetAll(double shimSize, int bankNo)
        {
            string sRC, sTemp = $"AW,072,{shimSize}";
            bool successful = false;
            bool BcONTINUE = ChangeBankNumber(bankNo);
            BcONTINUE &= ResetAll();
            if (BcONTINUE) 
            {
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Setting GT2 Preset Value for Bank: " + bankNo, "INFO");

                if (bankNo == 0)
                    sTemp = $"SW,00,064,+{shimSize:#000.0000}";
                else if (bankNo == 1)
                    sTemp = $"SW,00,069,+{shimSize:#000.0000}";
                else if (bankNo == 2)
                    sTemp = $"SW,00,074,+{shimSize:#000.0000}";
                else if (bankNo == 3)
                    sTemp = $"SW,00,079,+{shimSize:#000.0000}";
                else
                    sTemp = $"SW,00,064,+{shimSize:#000.0000}";

                _log.log(LogType.TRACE, Category.INFO, $"Keyence: Presetting Value To Shim Count:  { sTemp }", "INFO");
                sRC = SendMsg(sTemp, 5000);
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Setting GT2 Preset Value Returned: " + sRC, "INFO");

                if (!sRC.Contains("ER"))
                {
                    sTemp = sTemp.Replace(",00,", ",01,");
                    sRC = SendMsg(sTemp, 5000);
                    if (!sRC.Contains("ER"))
                    {
                        sTemp = $"AW,050,1";
                        sRC = SendMsg(sTemp, 5000);
                        if (sRC.Contains("AW,050"))
                        {
                            sTemp = $"AW,122,0";
                            sRC = SendMsg(sTemp, 5000);
                            if (sRC.Contains("AW,122"))
                            {
                                _log.log(LogType.TRACE, Category.INFO, "Keyence: Reset Request To Last Written Value...", "INFO");
                                successful = true;
                                _log.log(LogType.TRACE, Category.INFO, $"Keyence: GT2 Preset({shimSize},{bankNo}) Command response: " + sRC, "INFO");
                            }
                        }
                    }
                }
                else
                {
                    successful = false;
                    _log.log(LogType.TRACE, Category.INFO, "Keyence: GT2 Preset Values Failed To Complete! Preset Value For Bank " + bankNo + " Failed To Get Correct Response... " + sRC, "ERROR");
                }
            }

            return successful;
        }

        /// <summary>
        /// Communicates with keyence serial amp to 'zero-out' the keyence sensor
        /// </summary>
        /// <returns>bool: True if reset was successful</returns>
        public bool ResetAll()
        {
            bool bRetVal = false;
            //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Keyence.Reset()", "DEBUG");
            string sRC, sTemp = "AW,050,0";
            int nErrCnt=0;
            do
            {
                nErrCnt++;
                //_log.log(LogType.TRACE, Category.INFO, "DEBUG: Sending Message " + sTemp,"DEBUG");
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("SW") && nErrCnt < 20);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: Reset() Command1 response: " + sRC, "INFO");
            if (sRC.Contains("AW,050"))
            {
                Thread.Sleep(250);
                sTemp = "AW,053,1";
                sRC = SendMsg(sTemp, 5000);
                _log.log(LogType.TRACE, Category.INFO, "Keyence: Reset() Command2 response: " + sRC, "INFO");
                if (sRC.Contains("AW,053"))
                {
                    sTemp = "AW,110,1";
                    sRC = SendMsg(sTemp, 5000);
                    _log.log(LogType.TRACE, Category.INFO, "Keyence: Reverse Direction() Command response: " + sRC, "INFO");
                    bRetVal = true;
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Couldn't talk to keyence to reset " + sRC, "ERROR");
            }
            return bRetVal;
        }

        /// <summary>
        /// Communicates with keyence serial amps to Set Bank Number Used For All GT2s.
        /// </summary>
        /// <param name="bankNo">(Int) Bank Number Selected.</param>
        /// <returns>bool: True if Bank Change was successful</returns>
        public bool ChangeBankNumber(int bankNo)
        {
            bool bRetVal = false;
            string sRC, sTemp = $"AW,051,{bankNo}";
            int nErrCnt = 0;
            do
            {
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("AW") && nErrCnt < 5);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: ChangeBankNumber() Command response: " + sRC, "INFO");
            if (sRC.Contains("AW,051"))
            {
                _log.log(LogType.TRACE, Category.INFO, "Keyence: ChangeBankNumber() Command Complete!", "INFO");
                bRetVal = true;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Couldn't talk to keyence to reset " + sRC, "ERROR");
                bRetVal = false;
            }
            return bRetVal;
        }

        public bool SetResponseTime(int respTime)
        {
            bool bRetVal = false;
            if (respTime == 5) respTime = 1;
            else if (respTime == 10) respTime = 2;
            else if (respTime == 100) respTime = 3;
            else if (respTime == 500) respTime = 4;
            else if (respTime == 1000) respTime = 5;
            else if (respTime == 5000) respTime = 6;
            string sRC, sTemp = $"AW,103,{respTime}";
            int nErrCnt = 0;
            do
            {
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("AW") && nErrCnt < 5);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: SetResponseTime() Command response: " + sRC, "INFO");
            if (sRC.Contains("AW,103"))
            {
                _log.log(LogType.TRACE, Category.INFO, "Keyence: SetResponseTime() Command Complete!", "INFO");
                bRetVal = true;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Couldn't talk to keyence to reset " + sRC, "ERROR");
                bRetVal = false;
            }
            return bRetVal;
        }
        public double ReadPresetValue(int bankNo)
        {
            double presetVal = -1;
            _bStopRequested = false;
            String sRC;
            String sTemp = "SR,00,064";
            if (bankNo == 1)
                sTemp = "SR,00,069";
            else if (bankNo == 2)
                sTemp = "SR,00,074";
            else if (bankNo == 3)
                sTemp = "SR,00,079";

            int nErrCnt = 0;
            do
            {
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("SR") && nErrCnt < 5);
            _log.log(LogType.TRACE, Category.INFO, "Keyence: ReadPresetValue() Command response: " + sRC, "INFO");

            string[] splitted = sRC.Split(',');
            //if (splitted[3].Contains('+')) splitted[3].Remove('+');
            presetVal = double.Parse(splitted[3]);
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Current Preset Value: {presetVal}.", "INFO");
            
            return presetVal;
        }

        public enum GT2 { Left, Right }
        public int ReadCurrentBankNumber(GT2 gt2)
        {
            int bankNo = -1;
            _bStopRequested = false;
            String sRC;
            String sTemp = "SR,00,051";
            //String sTemp = gt2 == GT2.Right ? "SR,00,051" : "SR,01,051"; //returns data in format    SR,{SENSOR_ID[##]},051,{BANK_NO[#]}
                                                                           //command to read bank no.  SR,{SENSOR_ID[##]},051
            int nErrCnt = 0;
            do
            {
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("SR") && nErrCnt < 5);
            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Bank Set Complete.", "INFO");
            _log.log(LogType.TRACE, Category.INFO, "Keyence: ReadBankNumber() Command response: " + sRC, "INFO");
            sTemp.Remove(2);
            sTemp.Remove(2);
            sTemp.Remove(2);
            if (sRC.Contains(sTemp))
            {
                string[] splitted = sRC.Split(new char[] { ',', '\n', '\r' });
                bankNo = int.Parse(splitted[3]);
                _log.log(LogType.TRACE, Category.INFO, $"Keyence: [{gt2.ToString()}] Current Bank Number: {bankNo}.", "INFO");
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Couldn't talk to keyence to read bank number: " + sRC, "ERROR");
            }

            return bankNo;
        }
        public double ReadBankPresetValue(int bankNo)
        {
            double dPresetVal = 0;
            _bStopRequested = false;
            String sRC;
            String sTemp = "SR,00,064";
            int nErrCnt = 0;

            if (bankNo == 0) sTemp = "SR,00,064";
            else if (bankNo == 1) sTemp = "SR,00,069";
            else if (bankNo == 2) sTemp = "SR,00,074";
            else sTemp = "SR,00,079";

            do
            {
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
            } while (!sRC.Trim().StartsWith("SR") && nErrCnt < 5);

            _log.log(LogType.TRACE, Category.INFO, $"Keyence: Bank Check Complete.", "INFO");
            _log.log(LogType.TRACE, Category.INFO, "Keyence: ReadBankPresetValue() Command Response: " + sRC, "INFO");
            if (sRC.Contains(sTemp))
            {
                string[] splitted = sRC.Split(new char[] { ',', '\n', '\r' });
                dPresetVal = double.Parse(splitted[3]);
                _log.log(LogType.TRACE, Category.INFO, $"Keyence: [GT2 BANK {bankNo}] Current Preset Value: {dPresetVal}.", "INFO");
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Couldn't talk to keyence to read bank preset value number: " + sRC, "ERROR");
            }

            return dPresetVal;
        }

        private Boolean _bStopRequested = false;
        public void StopRead()
        {
            _bStopRequested = true;
            _eventDataReady.Set();
        }

        /// <summary>
        /// Reads the current measurement value from the Keyence Touch Sensor
        /// </summary>
        /// <param name="dValue">Value returned from the Keyence</param>
        /// <param name="nRetries">Number of .25 sec retries to look for valid responces</param>
        /// <returns>bool: True if read was successful</returns>
        public bool ReadAll(ref Double dValue0, ref KeyenceState eState0, ref Double dValue1, ref KeyenceState eState1, int nRetries)
        {
            bool bRetVal = false;
            _bStopRequested = false;
            nRetries++;
            String sRC;
            String sTemp = "MS"; //returns data in format    MS,XX,+000.0000,XX,+000.0000
                                 //where XX is bit number
                                 //XX    Binary 
                                 //00    00001 - HIGH Is on
                                 //01    00010 - LOW is on
                                 //02    00100 - GO is on
                                 //03    01000 - HH is on
                                 //04    10000 - LL is on
            int nErrCnt = 0, nTemp;
            do
            {
                if (nErrCnt != 0)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Retry keyence read #" + nErrCnt.ToString(), "Info");
                    Thread.Sleep(250);
                }
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
                _log.log(LogType.TRACE, Category.INFO, "Message returned Keyence: '" + sRC.ToString() + "'", "DEBUG");
            } while (!sRC.Trim().StartsWith("MS") && nErrCnt < nRetries && !_bStopRequested);

            if (nErrCnt <= nRetries && !_bStopRequested)
            {
                //_log.log(LogType.TRACE, Category.INFO, "Message From Keyence: " + sRC.ToString(), "DEBUG");
                String[] saParts;
                if (sRC.ToString().Trim() != "")
                {
                    saParts = sRC.ToString().Split(new Char[] { ',', '\n', '\r' });
                    try
                    {
                        //_log.log(LogType.TRACE, Category.INFO, "Parsing '" + saParts[3].Trim() + "'", "DEBUG");
                        nTemp = int.Parse(saParts[1]);
                        eState0 = (KeyenceState)nTemp;
                        dValue0 = Double.Parse(saParts[2]);
                        nTemp = int.Parse(saParts[3]);
                        eState1 = (KeyenceState)nTemp;
                        dValue1 = Double.Parse(saParts[4].Trim());
                        bRetVal = true;
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Exception looking for string parts - " + ex.ToString(), "ERROR");
                    }
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "Message From Keyence was empty string", "ERROR");
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Could not talk to keyence to read: " + sRC + " Retry count exceeded", "ERROR");
            }
            return bRetVal;
        }

        /// <summary>
        /// Reads the current measurement value from the Keyence Touch Sensor
        /// </summary>
        /// <param name="dValue">Value returned from the Keyence</param>
        /// <param name="nRetries">Number of .25 sec retries to look for valid responces</param>
        /// <returns>bool: True if read was successful</returns>
        public bool ReadValue(int nWhichOne, ref Double dValue, int nRetries)
        {
            bool bRetVal = false;
            _bStopRequested = false;

            String sRC;
            String sTemp = String.Format("SR,{0,2:D2},000",nWhichOne);
            int nErrCnt = 0;
            do
            {
                if (nErrCnt != 0)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Retry keyence read #"+nErrCnt.ToString(), "Info");
                    Thread.Sleep(250);
                }
                nErrCnt++;
                sRC = SendMsg(sTemp, 5000);
                _log.log(LogType.TRACE, Category.INFO, "Message returned Keyence: '" + sRC.ToString() + "'", "DEBUG");
            } while (!sRC.Trim().StartsWith("SR") && nErrCnt < nRetries && !_bStopRequested);

            if (nErrCnt < nRetries && !_bStopRequested)
            {
                //_log.log(LogType.TRACE, Category.INFO, "Message From Keyence: " + sRC.ToString(), "DEBUG");
                String[] saParts;
                if (sRC.ToString().Trim() != "")
                {
                    saParts = sRC.ToString().Split(new Char[] { ',', '\n', '\r' });
                    try
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Parsing '" + saParts[3].Trim() + "'", "DEBUG");
                        dValue = Double.Parse(saParts[3].Trim());
                        dValue = Math.Abs(dValue);
                        bRetVal = true;
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Exception looking for string parts - " + ex.ToString(), "ERROR");
                    }
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "Message From Keyence was empty string", "ERROR");
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Could not talk to keyence to read: " + sRC + " Retry count exceeded", "ERROR");
            }
            return bRetVal;
        }
    }
}
