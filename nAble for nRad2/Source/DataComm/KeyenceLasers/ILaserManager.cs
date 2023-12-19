using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm.KeyenceLasers
{
    public interface ILaserManager
    {
        bool Connected { get; }
        bool EnableAutoConnect { get; set; }
        bool Error { get; }
        int ErrorCode { get; }
        string IPAddress { get; }
        DateTime LastRead { get; }
        int NumLasers { get; }
        int ProgramNumber { get; }
        bool ReadingLasers { get; }
        bool WaitingForProgramNumber { get; }
        bool WritingProgramNumber { get; }

        bool Connect(string ipAddress);
        void Disconnect(bool closing = false);
        LaserData GetLaserData(int laserNum);
        bool GetLeftAndRightLaserData(out LaserData left, out LaserData right);
        bool GetLeftAndRightLaserData(out LaserData left, out LaserData right, DateTime timestamp);
        bool GetLeftAndRightLaserValues(out double left, out double right);
        bool GetLeftAndRightLaserValues(out double left, out double right, DateTime minTimestamp);
        bool ResetLasers();
        void SetIPAddress(string ipAddress);
        bool SetProgramNumber(int programNumber);
        void UpdateStatus();
    }
}
