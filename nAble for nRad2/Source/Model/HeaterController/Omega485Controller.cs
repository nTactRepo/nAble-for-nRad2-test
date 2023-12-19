using nAble.Data;
using nTact.DataComm;
using System;
using System.Drawing;
using System.Timers;

namespace nAble.Model
{
    public class Omega485Controller
    {
        #region Properties

        private LogEntry Log { get; set; } = null;
        private MachineSettingsII MS { get; set; } = null;

        #endregion

        #region Data Members

        private Omega485Comm _omega485 = null;
        private System.Windows.Forms.Timer _timerReconnect = null;

        #endregion

        #region Functions

        #region Constructors

        public Omega485Controller(LogEntry log, MachineSettingsII ms)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            MS = ms ?? throw new ArgumentNullException(nameof(ms));

            _omega485 = new Omega485Comm(log);

            _timerReconnect = new System.Windows.Forms.Timer
            {
                Interval = 15000,
                Enabled = false
            };

            _timerReconnect.Tick += _timerReconnect_Tick;
        }

        #endregion

        #region Setup / Shutdown

        public void Setup()
        {
            Log.log(LogType.TRACE, Category.INFO, $"Attempting Connection to Temperature Controllers... Port:{MS.HeaterComPort}", "Info");

            if (MS.AnyHeaterEnabled)
            {
                _omega485 = new Omega485Comm(Log);
                _omega485.TempChanged += Omega485_TempChanged;

                try
                {
                    _omega485.Init(MS.HeaterComPort);
                }
                catch (Exception ex)
                {
                    string msg = $"Connection to Temperature Controllers Failed:  {ex.Message}";
                    Log.log(LogType.TRACE, Category.INFO, msg, "Warning");
                    _timerReconnect.Start();
                    return;
                }
            }

            Log.log(LogType.TRACE, Category.INFO, $"Connection to Temperature Controllers Complete. Port:{MS.HeaterComPort}", "Info");

            if (MS.ChuckTempControlEnabled && _omega485.IsOpen)
            {
                _omega485.AddController(MS.ChuckCOMID, "Chuck");
            }

            if (MS.DieTempControlEnabled && _omega485.IsOpen)
            {
                _omega485.AddController(MS.DieCOMID, "Die");
            }

            if (MS.ReservoirTempControlEnabled && _omega485.IsOpen)
            {
                _omega485.AddController(MS.ResvCOMID, "ReservoirA");

                if (MS.DualPumpInstalled)
                {
                    _omega485.AddController(MS.ResvBCOMID, "ReservoirB");
                }
            }

            if (MS.ReservoirLimitControlEnabled && _omega485.IsOpen)
            {
                _omega485.AddController(MS.ResvHeaterCOMID, "ResvHeaterA");

                if (MS.DualPumpInstalled && _omega485.IsOpen)
                {
                    _omega485.AddController(MS.ResvBHeaterCOMID, "ResvHeaterB");
                }
            }

            if (MS.AirKnifeHeaterInstalled && _omega485.IsOpen)
            {
                _omega485.AddController(MS.AirKnifeHeaterCOMID, "Air Knife");
                _omega485.AddController(MS.GasHeaterCOMID, "Gas Heater");
                _omega485.AddController(MS.GasHeaterReaderCOMID, "Gas Heater Reader");
            }
        }

        public void Shutdown()
        {
            _omega485.Shutdown();
        }

        #endregion

        #region Public Functions

        public double SetPoint(int id)
        {
            var tc = _omega485.FindControllerByID(id);
            return (tc is null || _omega485 is null) ? -999 : tc.SetPoint;
        }

        public string Temp(int id)
        {
            var tc = _omega485.FindControllerByID(id);

            if (tc is null || _omega485 is null)
            {
                return "COM N/C";
            }

            if (tc.RTDConnected)
            {
                return $"{tc.Temperature:0.0}";
            }
            else
            {
                return "RTD N/C";
            }
        }

        public Color TempColor(int id)
        {
            var tc = _omega485.FindControllerByID(id);

            if (tc is null || _omega485 is null || !tc.RTDConnected)
            {
                return Color.Red;
            }

            return Color.Lime;
        }

        internal bool ChangeTempSetPoint(int id, double newSetpoint)
        {
            var tc = _omega485.FindControllerByID(id);

            if (tc is null || _omega485 is null || !_omega485.IsOpen || !tc.Contacted)
            {
                return false;
            }

            tc.ChangeSetPoint(newSetpoint);
            MS.ChuckTempControlSetPoint = (float)newSetpoint;
            MS.Save();
            return true;
        }


        #endregion

        #region Event Handlers

        private void _timerReconnect_Tick(object sender, EventArgs e)
        {
            Setup();
        }

        private void Omega485_TempChanged(int ID, double temperature)
        {
            // Weirdly, nothing to do...  Hmmm...  Why did we need this?
        }

        #endregion

        #endregion
    }
}
