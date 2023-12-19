using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nAble.DataComm.AdvantechSerialServer
{
    public enum ErrorStates
    {
        NoFaultDetected = 0,
        UartTimeout = 1,
        OverRange = 2,
        UnderRange = 4,
        OpenCircuitBurnout = 8,
        CalibrationError = 0x200
    }

    public class ErrorState
    {
        #region Properties

        public int RawState { get; set; } = (int)ErrorStates.NoFaultDetected;

        public bool NoError => RawState == 0;

        public bool UartTimeout => (RawState & (int)ErrorStates.UartTimeout) > 0;

        public bool OverRange => (RawState & (int)ErrorStates.OverRange) > 0;

        public bool UnderRange => (RawState & (int)ErrorStates.UnderRange) > 0;

        public bool OpenCircuitBurnout => (RawState & (int)ErrorStates.OpenCircuitBurnout) > 0;

        public bool CalibrationError => (RawState & (int)ErrorStates.CalibrationError) > 0;

        #endregion
    }
}
