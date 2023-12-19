using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using nAble;
using nAble.DataComm;

namespace nTact.DataComm
{
    public class Omega485Comm : IDisposable
    {
        #region Events

        public event Action<int, double> TempChanged;

        #endregion

        #region Properties

        public bool IsConnected { get; private set; } = false;
        public string CommPort { get; private set; } = "COM3";
        public bool Disposed { get; private set; } = false;
        public bool Running { get; private set; } = false;

        public bool IsOpen => Running && !(_comPort is null) && _comPort.IsOpen;

        #endregion

        #region Member Data

        private Thread _workerThread = null;
        private SerialPort _comPort = new SerialPort();
        private readonly LogEntry _log = null;

        private List<TempController> _tempControllers = new List<TempController>(8);

        private bool _cancelThread = false;

        #endregion

        #region Functions

        #region Constructors

        public Omega485Comm(LogEntry log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _comPort = new SerialPort
            {
                BaudRate = 9600,    //BaudRate
                DataBits = 7,    //DataBits
                StopBits = StopBits.One,    //StopBits
                Parity = Parity.Odd,    //Parity
                PortName = CommPort,   //PortName
                Handshake = Handshake.None,
                ReadTimeout = 2000,
                NewLine = "\r"
            };
        }

        ~Omega485Comm() => Dispose(false);

        #endregion

        #region Public Functions

        public void Init(string commPort)
        {
            if (string.IsNullOrEmpty(commPort))
            {
                throw new ArgumentNullException(nameof(commPort), $"Omega485: Init() failed: COM port not specified");
            }

            CommPort = commPort;

            //now open the port
            try
            {
                _comPort = new SerialPort
                {
                    PortName = commPort,
                    BaudRate = 9600,
                    DataBits = 7,
                    Parity = Parity.Odd,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 2000,
                    NewLine = "\r"
                };
               

                _comPort.Open();
                Thread.Sleep(250);
            }
            catch (Exception ex)
            {
                throw new Exception($"Omega485: Could not open com port: {ex.Message}");
            }

            if (!_comPort.IsOpen)
            {
                throw new Exception($"Omega485: COM port to temp controller could not be opened. - port {CommPort}");
            }

            IsConnected = true;

            if (_comPort.BytesToWrite != 0)
            {
                _comPort.DiscardOutBuffer();
            }

            if (_comPort.BytesToRead != 0)
            {
                _comPort.DiscardInBuffer();
            }

            _comPort.ReadExisting();
            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Opened COM port to Omega controllers on port {CommPort}", "Info");

            _workerThread = new Thread(new ThreadStart(this.ThreadProc));
            _workerThread.Start();

            Running = true;
        }

        public bool Shutdown()
        {
            bool wasShutdown = false;
            Running = false;
            _cancelThread = true;

            if (_workerThread != null && _workerThread.IsAlive)
            {
                wasShutdown = _workerThread.Join(1000);
            }

            if (_comPort != null && _comPort.IsOpen)
            {
                _comPort.Close();
            }

            var tag = wasShutdown ? "" : "not";
            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Was {tag} shutdown correctly.");

            return wasShutdown;
        }

        public TempController FindControllerByName(string name)
        {
            foreach (var tc in _tempControllers)
            {
                if (tc.Name == name) return tc;
            }

            return null;
        }

        public TempController FindControllerByID(int id)
        {
            foreach (var tc in _tempControllers)
            {
                if (tc.ID == id) return tc;
            }

            return null;
        }

        public bool AddController(int id, string name)
        {
            TempController controller = null;

            if (_tempControllers.Any(tc => tc.ID == id))
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Omega485: The ID {id} already exists, and cannot be used a second time.");
                return false;
            }

            if (_tempControllers.Count >= 7)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Omega485: The Omega has assigned all IDs already -- cannot add another!");
                return false;
            }

            try
            {
                controller = new TempController(id, name);
                controller.TempChanged += (i, t) => TempChanged?.Invoke(i, t);
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Omega485: Failed constructing a new TempController: {ex.Message}");
                return false;
            }

            _tempControllers.Add(controller);

            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Added heater {name} (ID {id})");
            return true;
        }

        public bool RemoveController(TempController tc) => _tempControllers.Remove(tc);

        #endregion

        #region Thread Functions

        private void ThreadProc()
        {
            int loop = 0;

            _log.log(LogType.TRACE, Category.INFO, "Omega485: ThreadProc Started", "Info");

            // do until shutdown
            while (!_cancelThread)
            {
                if (!_comPort.IsOpen)
                {
                    Thread.Sleep(500);
                    continue;
                }

                // Read setpoints for controllers that have not been contacted
                foreach (var tc in _tempControllers.Where(t => !t.Contacted))
                {
                    if (_cancelThread)
                    {
                        break;
                    }

                    try
                    {
                        _comPort.Write($"*{tc.ID:00}R01\r");
                        string response = _comPort.ReadLine();  // make sure newline = \r no response will be timeout

                        if (response.StartsWith("?"))
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error during Setpoint Read - '{response}'", "ERROR");
                        }
                        else if (response.Contains("R01") && response.Length >= 10)
                        {
                            // Get Setpoint from Server
                            var tempStr = response.Substring(7);
                            int rawTemp = int.Parse(tempStr, System.Globalization.NumberStyles.HexNumber, null);
                            double currentTemp = (double)rawTemp / 10;

                            // Set setpoint(s) into the TempController
                            tc.SetReadData(currentTemp);
                        }
                    }
                    catch (TimeoutException)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Omega485: Timeout Reading Setpoint from ID : '{tc.ID}'", "ERROR");
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error Reading Setpoint from ID:{tc.ID} -- {ex.Message}", "ERROR");
                    }
                }

                if (!_comPort.IsOpen)
                {
                    Thread.Sleep(500);
                    continue;
                }

                // Do any NewSetPoints as required
                foreach (var tc in _tempControllers)
                {
                    if (_cancelThread)
                    {
                        break;
                    }

                    // skip the ones that are at set point already
                    if (tc.NewSetPoint == tc.SetPoint)
                    {
                        continue;
                    }

                    try
                    {
                        int setPoint = (int)(tc.NewSetPoint * 10);
                        var idStr = $"{tc.ID:00}";

                        _comPort.Write($"*{idStr}W01200{setPoint:X3}\r");
                        string response = _comPort.ReadLine();  // make sure newline = \r no response will be timeout

                        if (response.StartsWith("?"))
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error during Temp Read - '{response}'", "ERROR");
                        }
                        else if (response.Contains("W01"))
                        {
                            tc.ConfirmSetpoint();
                        }
                    }
                    catch (TimeoutException)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Omega485: Timeout Writing new Setpoint for ID: {tc.ID}", "ERROR");
                    }
                    catch (Exception ex)
                    {
                        _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error Writing Setpoint for ID: {tc.ID}", "ERROR");
                        _log.log(LogType.TRACE, Category.INFO, $"Omega485: {ex.Message}", "ERROR");
                    }
                }

                if (!_comPort.IsOpen)
                {
                    Thread.Sleep(500);
                    continue;
                }

                // Get temperature readings every other loop
                if (loop++ % 2 == 0)
                {
                    foreach (var tc in _tempControllers)
                    {
                        if (_cancelThread)
                        {
                            break;
                        }

                        try
                        {
                            var idStr = $"{tc.ID:00}";
                            _comPort.Write($"*{idStr}X01\r");
                            string response = _comPort.ReadLine();  // make sure newline = \r no response will be timeout

                            if (response.StartsWith("?"))
                            {
                                _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error during Temp Read ID: {tc.ID} - '{response}'", "ERROR");
                            }
                            else if (response.Contains("X01"))
                            {
                                if (response.Contains("?+9999"))
                                {
                                    tc.UpdateRTDAndTemp(false, 9999);
                                }
                                else if (response.Length >= 10)
                                {
                                    int start = response.IndexOf("X01");
                                    string tempStr = response.Substring(start + 3);
                                    var currentTemp = double.Parse(tempStr);
                                    tc.UpdateRTDAndTemp(true, currentTemp);
                                }
                            }
                            else
                            {
                                _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error during Temp Read - '{response}'", "ERROR");
                            }
                        }
                        catch (TimeoutException)
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Timeout Reading Temp from ID: '{tc.ID}'", "ERROR");
                        }
                        catch (Exception ex)
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"Omega485: Error Reading Temp from ID : '{tc.ID}'", "ERROR");
                            _log.log(LogType.TRACE, Category.INFO, $"Omega485: {ex.Message}", "ERROR");
                        }
                    }
                }

                if (!_comPort.IsOpen)
                {
                    IsConnected = false;
                }

                Thread.Sleep(50);
            }

            _log.log(LogType.TRACE, Category.INFO, "Omega485: ThreadProc Shutdown", "Info");
        }

        #endregion

        #region IDisposable Functions

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose of managed objects here
                Shutdown();
            }

            // Dispose of unmanaged objects here

            Disposed = true;
        }

        #endregion

        #endregion
    }
}
