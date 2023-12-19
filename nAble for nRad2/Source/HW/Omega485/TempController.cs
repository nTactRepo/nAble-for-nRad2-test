using System;

namespace nAble.DataComm
{
    public class TempController
    {
        #region Events

        /// <summary>The temp of this TC has changed.  TC ID, then current temperature</summary>
        public event Action<int, double> TempChanged;

        #endregion

        #region Properties

        public int ID { get; } = 0;
        public string Name { get; } = "";

        public double SetPoint { get; private set; } = -9999;
        public double NewSetPoint { get; private set; } = -9998;
        public bool Contacted { get; private set; } = false;
        public double Temperature { get; private set; } = 0.0;
        public bool RTDConnected { get; private set; } = false;

        #endregion

        #region Data Members

        private object _lock = new object();

        #endregion

        #region Functions

        public TempController(int id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            ID = id;
            Name = name;

            if (id <= 0 || id >= 8)
            {
                throw new ArgumentException($"ID:{id} is invalid for a Temp Controller", nameof(id));
            }

        }

        public void SetReadData(double currentTemp)
        {
            lock (_lock)
            {
                SetPoint = currentTemp;
                NewSetPoint = currentTemp;
                Contacted = true;
            }
        }

        public void ChangeSetPoint(double newValue)
        {
            lock (_lock)
            {
                NewSetPoint = newValue;
            }
        }

        public void ConfirmSetpoint()
        {
            lock (_lock)
            {
                SetPoint = NewSetPoint;
            }
        }

        public void UpdateRTDAndTemp(bool rtd, double temp)
        {
            lock (_lock)
            {
                RTDConnected = rtd;
                Temperature = temp;
                TempChanged?.Invoke(ID, Temperature);
            }
        }

        #endregion
    }
}
