using nAble.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm.KeyenceLasers
{
    public class DemoLaserManager : ILaserManager
    {
        #region Properties

        public bool Connected { get; set; } = false;

        public bool EnableAutoConnect { get; set; } = true;

        public bool Error { get; set; } = false;

        public int ErrorCode { get; set; } = 0;

        public string IPAddress => _ipAddress;

        public DateTime LastRead { get; set; } = DateTime.MinValue;

        public int NumLasers { get; }

        public int ProgramNumber { get; set; } = 0;

        public bool ReadingLasers { get; set; } = false;

        public bool WaitingForProgramNumber { get; set; } = false;

        public bool WritingProgramNumber { get; set; } = false;

        private LogEntry Log { get; } = null;

        private MachineSettingsII MS { get; } = null;

        #endregion

        #region Data Members


        private DateTime _completeProgramNumberWrite = DateTime.MinValue;
        private int _futureProgramNumber = -1;

        private string _ipAddress = "";

        private LaserData _left = new LaserData()
        {
            Alarm = false,
            Go = true,
            Lo = false,
            Hi = false,
            HeadNumber = 0,
            Invalid = false,
            ReadTimestamp = DateTime.Now,
            Waiting = false,
            Value = 1.23
        };
        private LaserData _right = new LaserData()
        {
            Alarm = false,
            Go = true,
            Lo = false,
            Hi = false,
            HeadNumber = 1,
            Invalid = false,
            ReadTimestamp = DateTime.Now,
            Waiting = false,
            Value = 1.23
        };

        #endregion

        #region Functions

        public DemoLaserManager(LogEntry logEntry, MachineSettingsII ms)
        {
            MS = ms ?? throw new ArgumentNullException(nameof(ms));
            Log = logEntry;
            SetIPAddress(MS.KeyenceLaserIP);
            NumLasers = MS.NumberOfLasers;
        }

        public bool Connect(string ipAddress)
        {
            Connected = true;
            SetIPAddress(ipAddress);
            return true;
        }

        public void Disconnect(bool closing = false)
        {
            Connected = false;
        }

        public LaserData GetLaserData(int laserNum)
        {
            switch (laserNum)
            {
                case 0: return _left;
                case 1: return _right;
                default: return null;
            }
        }

        public bool GetLeftAndRightLaserData(out LaserData left, out LaserData right)
        {
            left = _left;
            left.ReadTimestamp = DateTime.Now;
            right = _right;
            right.ReadTimestamp = DateTime.Now;
            return true;
        }

        public bool GetLeftAndRightLaserData(out LaserData left, out LaserData right, DateTime timestamp)
        {
            return GetLeftAndRightLaserData(out left, out right);
        }

        public bool GetLeftAndRightLaserValues(out double left, out double right)
        {
            left = -2.34;
            right = -2.34;
            return true;
        }

        public bool GetLeftAndRightLaserValues(out double left, out double right, DateTime minTimestamp)
        {
            return GetLeftAndRightLaserValues(out left, out right);
        }

        public bool ResetLasers()
        {
            return true;
        }

        public void SetIPAddress(string ipAddress)
        {
            _ipAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
        }

        public bool SetProgramNumber(int programNumber)
        {
            if (programNumber < 0 || programNumber > 7)
            {
                return false;
            }

            WaitingForProgramNumber = true;
            _futureProgramNumber = programNumber;
            _completeProgramNumberWrite = DateTime.Now.AddMilliseconds(1500);

            return true;
        }

        public void UpdateStatus()
        {
            if (_futureProgramNumber != -1)
            {
                if (DateTime.Now > _completeProgramNumberWrite)
                {
                    WaitingForProgramNumber = false;
                    _completeProgramNumberWrite = DateTime.MinValue;
                    ProgramNumber = _futureProgramNumber;
                    _futureProgramNumber = -1;
                }
            }
        }

        #endregion
    }
}
