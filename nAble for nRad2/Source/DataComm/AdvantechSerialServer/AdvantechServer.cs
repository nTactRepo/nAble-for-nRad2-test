using nAble.DataComm.AdvantechSerialServer.AdvantechComms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace nAble.DataComm.AdvantechSerialServer
{
    public class AdvantechServer : IDisposable
    {
        #region Properties

        public int MinAINumber { get; private set; } = 0;

        public int MaxAINumber { get; private set; } = 0;

        public int NumAnalogInputs => _inputs.Count;

        public string IP { get; set; } = "";
        public int Port { get; set; } = 502;

        public bool Disposed { get; private set; } = false;

        #endregion

        #region Member Data

        private LogEntry _log = null;

        private Thread _thread = null;
        private object _lock = new object();
        private bool _threadRunning = false;
        private bool _requestThreadCancel = false;

        private Dictionary<int, AnalogInput> _inputs = new Dictionary<int, AnalogInput>();

        private ICommsBase _comms = null;

        #endregion

        #region Functions

        #region Constructors

        public AdvantechServer(ICommsBase comms, LogEntry logger)
        {
            _comms = comms ?? throw new ArgumentNullException(nameof(comms));
            _log = logger ?? throw new ArgumentNullException(nameof(logger));

            MinAINumber = _comms.MinAINumber;
            MaxAINumber = _comms.MaxAINumber;
        }

        public AdvantechServer(ICommsBase commsBase, LogEntry logger, IEnumerable<AnalogInput> inputs) : 
            this(commsBase, logger)
        {
            AddAnalogInputs(inputs);
        }

        ~AdvantechServer() => Dispose(false);

        #endregion

        #region Public Functions

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            if (_threadRunning)
            {
                return;
            }

            _thread = new Thread(new ThreadStart(PollServerData))
            {
                Name =  "SerialServerPollingThread",
                IsBackground = true,
            };

            _requestThreadCancel = false;
            _thread.Start();
            _threadRunning = true;
        }

        public void Stop()
        {
            _requestThreadCancel = true;
        }

        public void AddAnalogInputs(IEnumerable<AnalogInput> inputs)
        {
            foreach (var input in inputs)
            {
                AddAnalogInput(input);
            }
        }

        public void AddAnalogInput(AnalogInput input)
        {
            if (input.AIONumber < MinAINumber || input.AIONumber > MaxAINumber)
            {
                throw new ArgumentOutOfRangeException($"The AIONumber needs to be between {MinAINumber}-{MaxAINumber}!");
            }

            if (_inputs.ContainsKey(input.AIONumber))
            {
                throw new Exception($"Duplicate AIONumber: {input.AIONumber}.  More than one input has this number.");
            }

            lock (_lock)
            {
                _inputs.Add(input.AIONumber, input);
            }
        }

        public AnalogInput GetAnalogInput(int index)
        {
            if (_inputs.ContainsKey(index))
            {
                return _inputs[index];
            }

            return null;
        }

        public AnalogInput GetAnalogInputByName(string name)
        {
            foreach(var input in _inputs.Values)
            {
                if (input.Name == name)
                {
                    return input;
                }
            }

            return null;
        }

        public void ClearAnalogInputs()
        {
            lock (_lock)
            {
                _inputs.Clear();
            }
        }

        #endregion

        #region Private Functions

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose of managed objects here
                Stop();
            }

            // Dispose of unmanaged objects here

            Disposed = true;
        }

        #endregion

        #region Thread Functions

        private void PollServerData()
        {
            _log.log(LogType.TRACE, Category.INFO, $"AdvantechServer started polling for analog input data.");

            while (!_requestThreadCancel)
            {
                try
                {
                    if (!_comms.Connected)
                    {
                        ConnectComms();
                    }

                    if (_comms.Connected)
                    {
                        lock (_lock)
                        {
                            _comms.GetValues(out var values);
                            _comms.GetStatus(out var status);
                            _comms.GetEnabled(out var enabled);

                            for (int i = 0; i <= _comms.Total; i++)
                            {
                                var input = GetAnalogInput(i + _comms.MinAINumber);

                                if (input != null)
                                {
                                    input.SetRawInput(values[i], status[i], enabled[i]);
                                }
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.ERROR, $"Exception caught in AdvantechServer.PollServerData():  {ex}");
                }
            }

            _log.log(LogType.TRACE, Category.INFO, $"AdvantechServer is shutting down the polling cycle.");
            _threadRunning = false;
            _requestThreadCancel = false;
        }

        private void ConnectComms()
        {
            _comms.Connect(IP, Port);
        }

        #endregion

        #endregion
    }
}
