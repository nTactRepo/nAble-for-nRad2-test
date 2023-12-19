using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support2
{
    public enum LicenseFailCode
    {
        Valid = 0,
        UnknownError = 100,
        InvalidSerialNumber = 101,
        ValidationFailed = 102,
        DetectedClockRollback = 103,
        Expired = 104,
        InvalidGalilSerialNum = 105,
        ClockRollbackFileDeleted = 106,
        HWIDMismatch = 107,
        Uninitialized = 255
    }
}
