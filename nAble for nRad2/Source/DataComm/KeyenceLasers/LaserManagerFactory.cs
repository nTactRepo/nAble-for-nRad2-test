using nAble.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm.KeyenceLasers
{
    public static class LaserManagerFactory
    {
        public static ILaserManager CreateLaserManager(LogEntry log, MachineSettingsII ms, bool isDemoMode)
        {
            if (isDemoMode)
            {
                return new DemoLaserManager(log, ms);
            }
            else
            {
                return new KeyenceLaserManager(log, ms);
            }
        }
    }
}
