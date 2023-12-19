using System;
using System.IO.Ports;

namespace nRadLite.DataComm.IR
{
    public interface IIRTransmitter
    {
        event Action<string> NewLogMessage;

        int AlarmCode { get; set; }
        int BaudRate { get; set; }
        bool COMFailure { get; set; }
        double CurrentLoadCurrent { get; set; }
        double CurrentLoadPower { get; set; }
        double CurrentLoadVoltage { get; set; }
        double CurrentPowerLevel { get; set; }
        int DataBits { get; set; }
        bool HasAlarm { get; }
        bool Idling { get; set; }
        bool IsAutoMode { get; set; }
        bool IsConnected { get; set; }
        bool IsOn { get; set; }
        short OnOffVal { get; set; }
        bool OverCurrent { get; set; }
        bool OverTemperature { get; set; }
        Parity Parity { get; set; }
        int SlaveAddress { get; set; }
        StopBits StopBits { get; set; }

        bool Connect();
        bool Disconnect();
        bool SetPowerLevel(double level);
        bool TurnOffIRTransmitter();
        bool TurnOnIRTransmitter();
        void UpdateStatus();
        bool WaitForPowerLevelChange(double pwrLvlRequested, int maxRetries, Func<bool> cancelRequested);
    }
}
