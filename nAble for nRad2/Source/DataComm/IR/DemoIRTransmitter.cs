using nAble.Data;
using System;
using System.IO.Ports;
using System.Threading;

namespace nRadLite.DataComm.IR
{
    public class DemoIRTransmitter : IIRTransmitter
    {
        #region Events

        public event Action<string> NewLogMessage;

        #endregion

        #region Parameters

        private MachineSettingsII MS { get; } = null;

        public int AlarmCode { get; set; } = -1;
        public int BaudRate { get; set; } = 19200;
        public bool COMFailure { get; set; } = false;
        public string COMPort { get; set; } = "COM4";
        public double CurrentLoadCurrent { get; set; } = 0.0;
        public double CurrentLoadPower { get; set; } = 0.0;
        public double CurrentLoadVoltage { get; set; } = 0.0;
        public double CurrentPowerLevel { get; set; } = 0.0;
        public int DataBits { get; set; } = 8;
        public bool HasAlarm { get; set; } = false;
        public bool Idling { get; set; } = false;
        public bool IsAutoMode { get; set; } = false;
        public bool IsConnected { get; set; } = false;
        public bool IsOn { get; set; } = false;
        public short OnOffVal { get; set; } = 0;
        public bool OverCurrent { get; set; } = false;
        public bool OverTemperature { get; set; } = false;
        public Parity Parity { get; set; } = Parity.None;
        public int SlaveAddress { get; set; } = 11;
        public StopBits StopBits { get; set; } = StopBits.One;

        #endregion

        #region Functions

        #region Constructors

        public DemoIRTransmitter(MachineSettingsII mach)
        {
            MS = mach ?? throw new ArgumentNullException(nameof(mach));
        }

        #endregion

        #region Public Functions

        public bool Connect()
        {
            IsConnected = true;
            return IsConnected;
        }

        public bool Disconnect()
        {
            IsConnected = false;
            return !IsConnected;
        }

        public bool SetPowerLevel(double level)
        {
            CurrentPowerLevel = level;
            return true;
        }

        public bool TurnOffIRTransmitter()
        {
            CurrentPowerLevel = 0;
            return true;
        }

        public bool TurnOnIRTransmitter()
        {
            CurrentPowerLevel = 0;
            return true;
        }

        public void UpdateStatus()
        {
            CheckAlarms();
        }

        public bool WaitForPowerLevelChange(double pwrLvlRequested, int maxRetries, Func<bool> cancelRequested)
        {
            Thread.Sleep(500);
            CurrentPowerLevel = pwrLvlRequested;
            return true;
        }

        #endregion

        #region Private Functions

        private void CheckAlarms()
        {
            if (IsConnected)
            {
                HasAlarm = false;
                AlarmCode = -1;
                OverCurrent = false;
                OverTemperature = false;
                COMFailure = false;
            }
        }

        private double GetCurrentLoadCurrent() => CurrentLoadCurrent;

        private bool HasCOMError() => false;

        private bool HasOverTempError() => OverTemperature;

        private void LoadCurrentInfo()
        {
        }

        #endregion

        #endregion
    }
}
