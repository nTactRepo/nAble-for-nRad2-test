using nAble.Data;
using System.IO.Ports;

namespace nRadLite.DataComm.IR
{
    public class IRTransmitter : IIRTransmitter
    {
        #region Events

        public event Action<string> NewLogMessage;

        #endregion

        #region Properties

        private readonly MachineSettingsII MS = null;
        private IRModbus Master { get; set; } = new IRModbus();

        public bool IsAutoMode { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public bool IsOn { get; set; } = false;
        public short OnOffVal { get; set; } = 0;
        public bool HasAlarm => COMFailure || OverTemperature || OverCurrent;
        public bool COMFailure { get; set; } = false;
        public bool OverTemperature { get; set; } = false;
        public bool OverCurrent { get; set; } = false;
        public bool Idling { get; set; } = false;
        public double CurrentPowerLevel { get; set; } = 0.0;
        public double CurrentLoadCurrent { get; set; } = 0.0;
        public double CurrentLoadVoltage { get; set; } = 0.0;
        public double CurrentLoadPower { get; set; } = 0.0;
        public short FiringModeConfiguration { get; set; } = 2;
        public int SlaveAddress { get; set; } = 11;
        public int BaudRate { get; set; } = 19200;
        public Parity Parity { get; set; } = Parity.None;
        public StopBits StopBits { get; set; } = StopBits.One;
        public int DataBits { get; set; } = 8;
        public int AlarmCode { get; set; } = -1;
        public enum Alarms { OverCurrent = 0, OverTemperature = 1, OfflineCOM = 2 }

        #endregion

        #region Data Members

        private object _lock = new object();

        #endregion

        #region Constructor

        public IRTransmitter(MachineSettingsII mach)
        {
            MS = mach ?? throw new ArgumentNullException(nameof(mach));
        }

        ~IRTransmitter()
        {
            SetPowerLevel(0.0);
        }

        #endregion

        #region Public Functions

        public bool Connect()
        {
            bool status = Master.Open(MS.IRCOMPort, BaudRate, DataBits, Parity, StopBits);
            status &= TurnOffIRTransmitter();
            status &= SetPowerLevel(0.0);

            LogMessage(IsConnected ? $"Connected to IR Controller at Port {MS.IRCOMPort}." : 
                $"Failed to connect to the IR controller.  Was trying Port {MS.IRCOMPort}");

            IsConnected = status;
            return IsConnected;
        }

        public bool Disconnect()
        {
            if (IsConnected)
            {
                LogMessage("IR Controller has been disconnected");
            }

            Master.Close();
            IsConnected = Master.IsConnected;

            return !IsConnected;
        }

        public bool SetPowerLevel(double level)
        {
            bool setPower = false;
            byte address = Convert.ToByte(MS.IRSlaveAddress);
            ushort start = Convert.ToUInt16(56);
            short[] value = new short[1];
            value[0] = Convert.ToInt16(level / .1);

            try
            {
                lock (_lock)
                {
                    setPower = Master.SendFc16(address, start, 1, value);
                    LogMessage($"IR Controller set the power level to: {level}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Caught Exception trying to set power level for the IR controller:  {ex.Message}");
            }

            return setPower;
        }

        public bool TurnOnIRTransmitter()
        {
            return SetOnOffConfig(on: true, auto: false);
        }

        public bool TurnOffIRTransmitter()
        {
            return SetOnOffConfig(on: false, auto: false);
        }

        public bool WaitForPowerLevelChange(double pwrLvlRequested, int maxRetries, Func<bool> cancelRequested)
        {
            double oldVoltage = CurrentLoadVoltage;
            int counter = 0;

            if (CurrentPowerLevel == pwrLvlRequested)
            {
                return true;
            }

            while (!cancelRequested() && oldVoltage == CurrentLoadVoltage && CurrentPowerLevel != pwrLvlRequested && counter <= maxRetries)
            {
                SetPowerLevel(pwrLvlRequested);
                counter++;
                Thread.Sleep(200);
            }

            return !cancelRequested() && counter <= maxRetries;
        }

        public void UpdateStatus()
        {
            if (!IsConnected)
            {
                return;
            }

            lock (_lock)
            {
                GetLoadCurrentInfo();
                GetPowerStatus();
                CheckAlarms();
            }
        }

        #endregion

        #region Private Functions

        private bool GetRegisters(short registerStart, ushort numRegs, out short[] regValue)
        {
            bool gotRegisters = false;
            int timeout = 0;
            int maxTimeOut = 3;
            regValue = null;

            if (!IsConnected)
            {
                return false;
            }

            byte address = Convert.ToByte(MS.IRSlaveAddress);
            ushort start = Convert.ToUInt16(registerStart);
            short[] values = new short[numRegs];

            try
            {
                while (!Master.SendFc4(address, start, numRegs, ref values) && timeout < maxTimeOut)
                {
                    timeout++;
                    Thread.Sleep(10);
                }

                if (timeout == maxTimeOut)
                {
                    Disconnect();
                }
                else
                {
                    gotRegisters = true;
                    regValue = values;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Caught Exception with IR controller trying to read register {registerStart} :  {ex.Message}");
            }

            return gotRegisters;
        }

        private bool GetRegister(short registerStart, out short regValue)
        {
            regValue = 0;
            bool gotRegs = GetRegisters(registerStart, 1, out short[] regValues);

            if (gotRegs)
            {
                regValue = regValues[0];
                return true;
            }

            return false;
        }

        private bool SetOnOffConfig(bool on, bool auto)
        {
            bool setPower = false;
            byte address = Convert.ToByte(MS.IRSlaveAddress);
            ushort start = Convert.ToUInt16(55);
            short[] value = new short[1];

            int config = (on ? 0 : 8) + (auto ? 0 : 16);
            value[0] = (short)config;

            try
            {
                lock (_lock)
                {
                    setPower = Master.SendFc16(address, start, 1, value);
                    LogMessage($"IR Controller set the on off config to - on:{on}, auto:{auto}");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Caught Exception trying to set power level for the IR controller:  {ex.Message}");
            }

            return setPower;
        }

        private bool GetPowerStatus() 
        {
            // 55 = OnOffStatus
            // 56 = Power Level

            bool gotRegisters = GetRegisters(55, 2, out var regs);

            OnOffVal = gotRegisters ? regs[0] : (short)0;
            IsOn = (OnOffVal & 8) == 0;
            IsAutoMode = (OnOffVal & 16) == 0;

            CurrentPowerLevel = gotRegisters ? Convert.ToDouble(regs[1]) * .10 : 0;

            return gotRegisters;
        }

        private bool HasCOMError()
        {
            //TODO: Fix this
            COMFailure = false;
            return true;
        }

        private bool HasOverTempError()
        {
            //TODO: Fix this
            OverTemperature = false;
            return true;
        }

        private bool CheckAlarms() => HasOverTempError() || HasCOMError();

        private bool GetLoadCurrentInfo()
        {
            // 104 = CurrentLoadCurrent
            // 105 = CurrentLoadVoltage
            // 106 = CurrentLoadPower

            bool gotRegisters = GetRegisters(104, 3, out var regs);

            CurrentLoadCurrent = gotRegisters ? Convert.ToDouble(regs[0]) * 0.10 : 0;
            CurrentLoadVoltage = gotRegisters ? Convert.ToDouble(regs[1]) * 0.10 : 0;
            CurrentLoadPower = gotRegisters ? Convert.ToDouble(regs[2]) * 0.010 : 0;

            return gotRegisters;
        }

        private void LogMessage(string msg)
        {
            NewLogMessage?.Invoke(msg);
        }

        #endregion
    }
}
