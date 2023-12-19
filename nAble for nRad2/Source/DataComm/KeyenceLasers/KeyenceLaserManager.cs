using nAble.Data;
using Sres.Net.EEIP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace nAble.DataComm.KeyenceLasers
{
    public class KeyenceLaserManager : ILaserManager
    {
        #region Constants

        private const int MaxNumMSToWaitForTimestamp = 500;
        private const int ProgramNumberWriteTimeoutInSeconds = 5;

        private const int MaxNumReadFails = 5;
        private const int RetryTimeInMS = 5000;

        #endregion

        #region Properties

        public string IPAddress { get; private set; }

        public bool Connected { get; private set; } = false;

        public bool EnableAutoConnect { get; set; } = false;

        public int NumLasers { get; private set; } = 0;

        public bool ReadingLasers { get; private set; } = false;

        public bool WritingProgramNumber { get; private set; } = false;

        public bool WaitingForProgramNumber { get; private set; } = false;

        public bool Error { get; private set; } = false;

        public int ErrorCode { get; private set; } = 0;

        public int ProgramNumber => _programNumber;

        public DateTime LastRead { get; private set; } = DateTime.MinValue;


        private bool ThreadRunning => _thread?.IsAlive ?? false;

        private LogEntry Log { get; } = null;
        
        private MachineSettingsII MS { get; } = null;

        #endregion

        #region Data Members

        private readonly List<LaserData> _laserData = new List<LaserData>();

        private EEIPClient _eeipClient = new EEIPClient();

        private int _programNumber = 0;
        private DateTime _programNumberTimeout = DateTime.MinValue;
        private DateTime _nextConnectAttemptTime = DateTime.MinValue;
        private int _numConsecutiveReadFails = 0;

        // Thread Vars
        private Thread _thread = null;
        private object _lock = new object();
        private bool _cancelRequested = false;

        #endregion

        #region Functions

        #region Constructors

        public KeyenceLaserManager(LogEntry log, MachineSettingsII ms)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            MS = ms ?? throw new ArgumentNullException(nameof(ms));

            SetIPAddress(MS.KeyenceLaserIP);

            var numLasers = MS.NumberOfLasers;
            NumLasers = (numLasers >= 1 || numLasers <= 4) ? numLasers : throw new ArgumentException("NumLasers out of range!", nameof(numLasers));

            for (int i = 0; i < numLasers; i++)
            {
                _laserData.Add(new LaserData { HeadNumber = i });
            }

            StartThread();
        }

        ~KeyenceLaserManager()
        {
            StopThread();
        }

        #endregion

        #region Public Functions

        public void SetIPAddress(string ipAddress)
        {
            IPAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        }

        public bool Connect(string ipAddress)
        {
            Connected = false;
            _nextConnectAttemptTime = DateTime.Now.AddMilliseconds(RetryTimeInMS);

            try
            {
                SetIPAddress(ipAddress);
                _eeipClient = new EEIPClient() { IPAddress = IPAddress };

                if (_eeipClient.RegisterSession() != 0)
                {
                    StartThread();
                    Connected = true;
                    _numConsecutiveReadFails = 0;
                    Log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Manager correctly connected to the Keyence Laser Controller.");
                }
            }
            catch (Exception ex)
            {
                Log.log(LogType.TRACE, Category.ERROR, $"Caught exception logging into the Keyence Lasers: {ex.Message}");
            }

            return Connected;
        }

        public void Disconnect(bool closing = false)
        {
            bool wasConnected = Connected;
            Connected = false;

            if (closing)
            {
                StopThread();
            }

            if (!wasConnected)
            {
                return;
            }

            Connected = false;
            _eeipClient?.UnRegisterSession();
            Log.log(LogType.TRACE, Category.INFO, $"Keyence Laser Manager correctly disconnected.");
            _numConsecutiveReadFails = 0;
        }

        public void UpdateStatus()
        {
        }

        public LaserData GetLaserData(int laserNum)
        {
            if (laserNum < 0 || laserNum >= NumLasers)
            {
                throw new ArgumentException($"Get Laser failed because laserNum was out of range!", nameof(laserNum));
            }

            LaserData data;

            lock (_lock)
            {
                data = Connected ? (LaserData)_laserData[laserNum].Clone() : null;
            }

            return data;
        }

        public bool GetLeftAndRightLaserData(out LaserData left, out LaserData right)
        {
            left = GetLaserData(0);
            right = GetLaserData(1);

            return Connected;
        }

        public bool GetLeftAndRightLaserData(out LaserData left, out LaserData right, DateTime timestamp)
        {
            if (WaitForCorrectTimestamp(timestamp))
            {
                return GetLeftAndRightLaserData(out left, out right);
            }
            else
            {
                left = right = null;
                return false;
            }
        }

        public bool GetLeftAndRightLaserValues(out double left, out double right)
        {
            lock (_lock)
            {
                left = _laserData[0].Value;
                right = _laserData[1].Value;
            }

            return Connected;
        }

        public bool GetLeftAndRightLaserValues(out double left, out double right, DateTime minTimestamp)
        {
            if (WaitForCorrectTimestamp(minTimestamp))
            {
                return GetLeftAndRightLaserValues(out left, out right);
            }
            else
            {
                left = right = 0;
                return false;
            }
        }

        public bool ResetLasers()
        {

            return false;
        }

        public bool SetProgramNumber(int programNumber)
        {
            bool newNumberSet = false;

            try
            {
                lock (_lock)
                {
                    WritingProgramNumber = true;
                    var data = _eeipClient.AssemblyObject.getInstance(0x65);

                    if (data.Length != 80)
                    {
                        Log.log(LogType.TRACE, Category.WARN, $"Laser Collection Thread has received an unexpected number of bytes in the set data: {data.Length}.  Discarding.");
                        return false;
                    }

                    Log.log(LogType.TRACE, Category.INFO, $"Old command bytes: {ByteArrayToString(data)}");
                    var array = BitConverter.GetBytes(programNumber);
                    Log.log(LogType.TRACE, Category.INFO, $"Program number bytes: {ByteArrayToString(array)}");
                    array.CopyTo(data, 24);
                    //data[0] = (byte)(data[0] | 0x04);
                    Log.log(LogType.TRACE, Category.INFO, $"Midway command bytes: {ByteArrayToString(data)}");
                    var ba = GetBoolsFromData(data, 0, 2);
                    ba[2] = true;
                    ba.CopyTo(data, 0);
                    Log.log(LogType.TRACE, Category.INFO, $"New command bytes: {ByteArrayToString(data)}");

                    _eeipClient.AssemblyObject.setInstance(0x65, data);
                    WaitingForProgramNumber = true;
                    _programNumberTimeout = DateTime.Now.AddSeconds(ProgramNumberWriteTimeoutInSeconds);
                    Thread.Sleep(500);
                    newNumberSet = true;
                }
            }
            catch (Exception Ex)
            {
                Log.log(LogType.TRACE, Category.INFO, $"Caught Exception trying to set program number for lasers: {Ex.Message}");
            }
            finally
            {
                WritingProgramNumber = false;
            }

            return newNumberSet;
        }

        #endregion

        #region Thread Functions

        private void StartThread()
        {
            if (ThreadRunning)
            {
                return;
            }

            _thread = new Thread(CollectionThread) { Name = "nTact_LaserCollectionThread" };

            _thread.Start();
            Log.log(LogType.TRACE, Category.INFO, $"Laser Collection Thread started.");
        }

        private void StopThread()
        {
            if (!ThreadRunning)
            {
                return;
            }

            int numTries = 5;
            _cancelRequested = true;

            while (ThreadRunning && numTries-- > 0)
            {
                Thread.Sleep(100);
            }

            if (numTries == 0)
            {
                Log.log(LogType.TRACE, Category.ERROR, $"Laser Collection Thread did not correctly shut down.  Forcing it closed");
                _thread.Abort();
            }
            else
            {
                Log.log(LogType.TRACE, Category.INFO, $"Laser Collection Thread has closed correctly");
            }
        }

        private void CollectionThread()
        {
            while (!_cancelRequested)
            {
                try
                {
                    Thread.Sleep(100);

                    lock (_lock)
                    {
                        if (Connected)
                        {
                            if (MS.UsingKeyenceLaser)
                            {
                                ReadingLasers = true;
                                ReadLaserData();
                                _numConsecutiveReadFails = 0;
                                ReadingLasers = false;
                            }
                            else
                            {
                                Disconnect();
                            }
                        }
                        else if (EnableAutoConnect && MS.UsingKeyenceLaser && !Connected && DateTime.Now > _nextConnectAttemptTime)
                        {
                            Connect(IPAddress);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    Log.log(LogType.TRACE, Category.ERROR, $"Laser Collection Thread has been forced closed.");
                    return;
                }
                catch (Exception ex)
                {
                    Log.log(LogType.TRACE, Category.ERROR, $"Laser Collection Thread has caught an exception while running:  {ex.Message}");

                    if (++_numConsecutiveReadFails > MaxNumReadFails)
                    {
                        Disconnect();
                    }
                }
            }
        }

        private void ReadLaserData()
        {
            byte[] data = _eeipClient.AssemblyObject.getInstance(0x64);
            LastRead = DateTime.Now;

            if (data.Length != 248)
            {
                Log.log(LogType.TRACE, Category.WARN, $"Laser Collection Thread has read an unexpected number of bytes: {data.Length}.  Discarding.");
                return;
            }

            var gos = GetBoolsFromData(data, 24, 2);
            var los = GetBoolsFromData(data, 22, 2);
            var his = GetBoolsFromData(data, 20, 2);
            var valids = GetBoolsFromData(data, 4, 2);
            var alarms = GetBoolsFromData(data, 10, 2);
            var systemStatus = GetBoolsFromData(data, 0, 2);
            var statusBits = GetBoolsFromData(data, 28, 2);

            Error = systemStatus[0];
            ErrorCode = BitConverter.ToInt16(data, 52);

            if (WaitingForProgramNumber)
            {
                bool stopWaiting = false;

                if (statusBits[2])
                {
                    Log.log(LogType.TRACE, Category.INFO, $"Got verification of program set.  Clearing command info");
                    stopWaiting = true;
                }
                else if (DateTime.Now > _programNumberTimeout)
                {
                    Log.log(LogType.TRACE, Category.ERROR, $"Timeout waiting for Program Number Write.  Reverting back to old value.");
                    stopWaiting = true;
                }

                if (stopWaiting)
                {
                    WaitingForProgramNumber = false;
                    ClearCommandInfo();
                }
            }

            _programNumber = GetProgramNumber(data);

            foreach (LaserData ld in _laserData)
            {
                int index = ld.HeadNumber;
                ld.Go = gos[index];
                ld.Lo = los[index];
                ld.Hi = his[index];
                ld.Invalid = !valids[index];
                ld.Alarm = alarms[index];
                ld.Waiting = false;
                ld.ReadTimestamp = LastRead;

                ld.Value = GetLaserValue(data, index);
            }
        }

        #endregion

        #region Private Functions

        private bool WaitForCorrectTimestamp(DateTime timestamp)
        {
            if (!Connected)
            {
                Log.log(LogType.TRACE, Category.INFO, $"The lasers are not connected, so the wait for a timestamp is cancelled.");
                return false;
            }

            if (timestamp > LastRead && timestamp.Subtract(LastRead).TotalMilliseconds > MaxNumMSToWaitForTimestamp)
            {
                Log.log(LogType.TRACE, Category.INFO, $"The desired timestamp for reading lasers was too far away.");
                return false;
            }

            while (LastRead < timestamp)
            {
                Thread.Sleep(50);
            }

            return true;
        }

        private double GetLaserValue(byte[] data, int index)
        {
            var newIndex = 68 + 4 * index;
            var rawValue = BitConverter.ToInt32(data, newIndex);
            return rawValue / 1000.0;
        }

        private int GetProgramNumber(byte[] data)
        {
            return BitConverter.ToInt32(data, 60);
        }

        private BitArray GetBoolsFromData(byte[] data, int offset, int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            Array.Copy(data, offset, bytes, 0, byteCount);
            BitArray ba = new BitArray(bytes);
            return ba;
        }

        private string ByteArrayToString(byte[] array)
        {
            StringBuilder sb = new StringBuilder($"Array -- Bytes:{array.Length}, [");

            foreach (byte n in array)
            {
                sb.Append($" {n:X}");
            }

            sb.Append(" ]");
            return sb.ToString();
        }

        private void ClearCommandInfo()
        {
            try
            {
                lock (_lock)
                {
                    byte[] data = new byte[80];

                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = 0;
                    }

                    _eeipClient.AssemblyObject.setInstance(0x65, data);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception Ex)
            {
                Log.log(LogType.TRACE, Category.INFO, $"Caught Exception trying to clear command data for lasers: {Ex.Message}");
            }
        }

        #endregion

        #endregion
    }
}
