using nAble.Data;
using nAble.Model;
using nAble_for_nRad2.Properties;
using nTact.DataComm;
using System.Diagnostics;

namespace nAble
{
    public partial class FormMaintenance : Form, IUpdateableForm
    {
        #region Constants

        private static readonly Color ControlColor = Color.FromName("Control");

        #endregion

        #region Properties

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private bool _resetOneShot = false;

        #endregion

        #region Functions

        #region Constructors

        public FormMaintenance(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            bool connected = MC.Connected && MC.IsHomed;
            bool moving = MC.Moving;

            buttonBypassDoorInterlockControl.Text = MS.BypassSafetyGuards ? "Disable" : "Enable";
            buttonBypassDoorInterlockControl.BackColor = MS.BypassSafetyGuards ? Color.LimeGreen : Color.Gray;

            labelMaintPrimingVac.Enabled = MS.HasPrimingPlate;

            //With Analog Air/Vac Sensors
            if (MS.HasAnalogInputs)
            {
                labelMaintMainPressure.Text = $"Main Air\rPressure\r{MC.MainAirPressure:#0.0} PSI";
                labelMaintMainPressure.BackColor = (MC.MainAirValveOpen && MC.MainAirOK) ? Color.Lime : Color.Red;
                labelMaintMainPressure.ForeColor = (MC.MainAirValveOpen && MC.MainAirOK) ? Color.Black : Color.White;

                labelMaintMainVac.Text = $"Main\rVacuum\r{MC.MainVac:#0.0} {_frmMain.VacUnits}";
                labelMaintMainVac.BackColor = MC.MainVacOK ? Color.Lime : Color.Red;
                labelMaintMainVac.ForeColor = MC.MainVacOK ? Color.Black : Color.White;

                labelMaintPrimingVac.Text = string.Format("Priming\rVacuum") + (labelMaintPrimingVac.Enabled ? "" : "\r(Disabled)");
                labelMaintPrimingVac.BackColor = !MS.HasPrimingPlate ? Color.Gray : !MC.PrimingVacEngaged ? ControlColor : MC.PrimingVacOK ? Color.Lime : Color.Red;
                labelMaintPrimingVac.ForeColor = labelMaintPrimingVac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone1Vac.Enabled = !MS.UsesSelectiveAirVacZones || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone1Enabled);
                labelMaintZone1Vac.Text = string.Format("Zone1\rVacuum") + (labelMaintZone1Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone1Vac.BackColor = !labelMaintZone1Vac.Enabled ? Color.Gray : !MC.Zone1VacEngaged ? ControlColor : MC.Zone1VacOK ? Color.Lime : Color.Red;
                labelMaintZone1Vac.ForeColor = labelMaintZone1Vac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone2Vac.Enabled = (!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 1) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone2Enabled);
                labelMaintZone2Vac.Text = string.Format("Zone2\rVacuum") + (labelMaintZone2Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone2Vac.BackColor = !labelMaintZone2Vac.Enabled ? Color.Gray : !MC.Zone2VacEngaged ? ControlColor : MC.Zone2VacOK ? Color.Lime : Color.Red;
                labelMaintZone2Vac.ForeColor = labelMaintZone2Vac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone3Vac.Enabled = (!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 2) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone3Enabled);
                labelMaintZone3Vac.Text = string.Format("Zone3\rVacuum") + (labelMaintZone3Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone3Vac.BackColor = !labelMaintZone3Vac.Enabled ? Color.Gray : !MC.Zone3VacEngaged ? ControlColor : MC.Zone3VacOK ? Color.Lime : Color.Red;
                labelMaintZone3Vac.ForeColor = labelMaintZone3Vac.BackColor != Color.Red ? Color.Black : Color.White;
            }
            else
            {
                //With Digital Air/Vac Sensors
                labelMaintMainPressure.Text = string.Format("Main Air\rPressure");
                labelMaintMainPressure.BackColor = (MC.MainAirValveOpen && MC.MainAirOK) ? Color.Lime : Color.Red;
                labelMaintMainPressure.ForeColor = (MC.MainAirValveOpen && MC.MainAirOK) ? Color.Black : Color.White;

                labelMaintMainVac.Text = string.Format("Main\rVacuum");
                labelMaintMainVac.BackColor = MC.MainVacOK ? Color.Lime : Color.Red;
                labelMaintMainVac.ForeColor = MC.MainVacOK ? Color.Black : Color.White;

                labelMaintPrimingVac.Text = "Priming\rVacuum" + (labelMaintPrimingVac.Enabled ? "" : "\r(Disabled)");
                labelMaintPrimingVac.BackColor = !MS.HasPrimingPlate ? Color.Gray : !MC.PrimingVacEngaged ? ControlColor : MC.PrimingVacOK ? Color.Lime : Color.Red;
                labelMaintPrimingVac.ForeColor = labelMaintPrimingVac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone1Vac.Enabled = !MS.UsesSelectiveAirVacZones || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone1Enabled);
                labelMaintZone1Vac.Text = "Zone1\rVacuum" + (labelMaintZone1Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone1Vac.BackColor = !labelMaintZone1Vac.Enabled ? Color.Gray : !MC.Zone1VacEngaged ? ControlColor : MC.Zone1VacOK ? Color.Lime : Color.Red;
                labelMaintZone1Vac.ForeColor = labelMaintZone1Vac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone2Vac.Enabled = (!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 1) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone2Enabled);
                labelMaintZone2Vac.Text = "Zone2\rVacuum" + (labelMaintZone2Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone2Vac.BackColor = !labelMaintZone2Vac.Enabled ? Color.Gray : !MC.Zone2VacEngaged ? ControlColor : MC.Zone2VacOK ? Color.Lime : Color.Red;
                labelMaintZone2Vac.ForeColor = labelMaintZone2Vac.BackColor != Color.Red ? Color.Black : Color.White;

                labelMaintZone3Vac.Enabled = (!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 2) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone3Enabled);
                labelMaintZone3Vac.Text = "Zone3\rVacuum" + (labelMaintZone3Vac.Enabled ? "" : "\r(Disabled)");
                labelMaintZone3Vac.BackColor = !labelMaintZone3Vac.Enabled ? Color.Gray : !MC.Zone3VacEngaged ? ControlColor : MC.Zone3VacOK ? Color.Lime : Color.Red;
                labelMaintZone3Vac.ForeColor = labelMaintZone3Vac.BackColor != Color.Red ? Color.Black : Color.White;
            }

            buttonLogout.Visible = true;
            buttonMaintReset.Enabled = MC != null && MC.Connected;
            
            buttonLdUldDie.Enabled = connected && !moving;
            buttonLdUldDie.Visible = MS.AlignersEnabled;
            
            buttonDisableMainAir.Enabled = !moving;
            buttonDisableMainAir.Text = MC.MainAirValveOpen ? "Disable\r\nMain Air" : "Enable\r\nMain Air";
            
            buttonInitMotors.Enabled = !MC.IsHomed;
            buttonDisableMainAir.BackColor = MC.MainAirValveOpen ? ControlColor : Color.Red;

            if (buttonDisableMainAir.BackColor != Color.Red)
            {
                buttonDisableMainAir.UseVisualStyleBackColor = true;
            }

            buttonShutdown.Enabled = !moving;

            pictureBoxServoXAxis.BackColor = MC.X_MotorOff ? Color.Red : Color.Lime;
            pictureBoxServoXAxisFLS.BackColor = (MC.XFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxServoXAxisRLS.BackColor = (MC.XRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxServoRightZ.BackColor = MC.RZ_MotorOff ? Color.Red : Color.Lime;
            pictureBoxServoRightZFLS.BackColor = (MC.ZRFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxServoRightZRLS.BackColor = (MC.ZRRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxServoLeftZ.BackColor = MC.LZ_MotorOff ? Color.Red : Color.Lime;
            pictureBoxServoLeftZFLS.BackColor = (MC.ZLFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxServoLeftZRLS.BackColor = (MC.ZLRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxServoPump.BackColor = MC.PumpA_MotorOff ? Color.Red : Color.Lime;
            pictureBoxServoPumpFLS.BackColor = (MC.PumpFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxServoPumpRLS.BackColor = (MC.PumpRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxServoPumpB.Visible = MS.DualPumpInstalled;
            pictureBoxServoPumpBFLS.Visible = MS.DualPumpInstalled;
            pictureBoxServoPumpBRLS.Visible = MS.DualPumpInstalled;
            labelPumpBLabel.Visible = MS.DualPumpInstalled;

            pictureBoxServoPumpB.BackColor = MC.PumpB_MotorOff ? Color.Red : Color.Lime;
            pictureBoxServoPumpBFLS.BackColor = (MC.PumpBFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxServoPumpBRLS.BackColor = (MC.PumpBRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxServoLoader.Visible = MS.HasLoader;
            pictureBoxServoLoaderFLS.Visible = MS.HasLoader;
            pictureBoxServoLoaderRLS.Visible = MS.HasLoader;
            labelServoLoader.Visible = MS.HasLoader;

            if (MS.HasLoader)
            {
                pictureBoxServoLoader.BackColor = MC.Loader_MotorOff ? Color.Red : Color.Lime;
                //TODO
                //pictureBoxServoLoaderFLS.BackColor = (MC.FFLS == 0.0) ? Color.Lime : Color.Red;
                //pictureBoxServoLoaderRLS.BackColor = (MC.FRLS == 0.0) ? Color.Lime : Color.Red;
            }

            labelDoorInterlockControl.Visible = MS.HasDoorInterlocks;
            buttonBypassDoorInterlockControl.Visible = MS.HasDoorInterlocks;

            panelLiftPins.Visible = MS.LiftPinsEnabled;
            pictureBoxInputsDI9.BackColor = MC.LiftPin1IsDown ? Color.Lime : Color.Red;
            pictureBoxInputsDI10.BackColor = MC.LiftPin1IsUp ? Color.Lime : Color.Red;
            pictureBoxInputsDI11.BackColor = MC.LiftPin2IsDown ? Color.Lime : Color.Red;
            pictureBoxInputsDI12.BackColor = MC.LiftPin2IsUp ? Color.Lime : Color.Red;
            panelAligners.Visible = false;
            pictureBoxInputsDI13.BackColor = MC.Aligner1IsDown ? Color.Lime : Color.Red;
            pictureBoxInputsDI14.BackColor = MC.Aligner2IsDown ? Color.Lime : Color.Red;
            pictureBoxInputsDI15.BackColor = MC.Aligner3IsDown ? Color.Lime : Color.Red;

            if (MS.ChuckTempControlEnabled)
            {
                labelMaintChuckTempOutput.Text = Heaters.Temp(MS.ChuckCOMID);
                labelMaintChuckTempOutput.ForeColor = Heaters.TempColor(MS.ChuckCOMID);
                labelMaintChuckSP.Text = $"Setpoint:   {Heaters.SetPoint(MS.ChuckCOMID):0.0}".PadLeft(5);
            }

            if (MS.DieTempControlEnabled)
            {
                labelMaintDieTempOutput.Text = Heaters.Temp(MS.DieCOMID);
                labelMaintDieTempOutput.ForeColor = Heaters.TempColor(MS.DieCOMID);
                labelMaintDieSP.Text = $"Setpoint:   {Heaters.SetPoint(MS.DieCOMID):0.0}".PadLeft(5);
            }

            if (MS.ReservoirTempControlEnabled)
            {
                labelMaintResvTempOutput.Text = Heaters.Temp(MS.ResvCOMID);
                labelMaintResvTempOutput.ForeColor = Heaters.TempColor(MS.ResvCOMID);
                labelMaintResvSP.Text = $"Setpoint:   {Heaters.SetPoint(MS.ResvCOMID):0.0}".PadLeft(5);

                if (MS.DualPumpInstalled)
                {
                    labelMaintResvBTempOutput.Text = Heaters.Temp(MS.ResvBCOMID);
                    labelMaintResvBTempOutput.ForeColor = Heaters.TempColor(MS.ResvBCOMID);
                    labelMaintResvBSP.Text = $"Setpoint:   {Heaters.SetPoint(MS.ResvBCOMID):0.0}".PadLeft(5);
                }
            }

            if (MS.ReservoirLimitControlEnabled)
            {
                labelMaintResvHeaterTempOutput.Text = Heaters.Temp(MS.ResvHeaterCOMID);
                labelMaintResvHeaterTempOutput.ForeColor = Heaters.TempColor(MS.ResvHeaterCOMID);
                labelMaintResvHeaterSP.Text = $"Setpoint:   {Heaters.SetPoint(MS.ResvHeaterCOMID):0.0}".PadLeft(5);
            }

            if (_resetOneShot && _frmMain.StatusMessage == "Initialization Required.")
            {
                _resetOneShot = false;
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Initialization Required!\r\nDo you wish to home the nRad at this time?", "Connection Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _frmMain.CallInitAll();
                }
            }

            buttonResetEMO.Visible = MS.HasStack;
        }

        #endregion

        #region Control Event Handlers

        private void FormMaintenance_Load(object sender, EventArgs e)
        {
            labelMaintChuckTemp.Visible = MS.ChuckTempControlEnabled;
            labelMaintChuckTempOutput.Visible = MS.ChuckTempControlEnabled;
            labelMaintChuckSP.Visible = MS.ChuckTempControlEnabled;

            labelMaintDieTemp.Visible = MS.DieTempControlEnabled;
            labelMaintDieTempOutput.Visible = MS.DieTempControlEnabled;
            labelMaintDieSP.Visible = MS.DieTempControlEnabled;

            panelResvATemp.Visible = MS.ReservoirTempControlEnabled;
            panelResvBTemp.Visible = MS.ReservoirTempControlEnabled && MS.DualPumpInstalled;

            labelMaintResvHeaterTemp.Visible = MS.ReservoirLimitControlEnabled;
            labelMaintResvHeaterTempOutput.Visible = MS.ReservoirLimitControlEnabled;
            labelMaintResvHeaterSP.Visible = MS.ReservoirLimitControlEnabled;
        }

        private void buttonMaintInitialize_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Initialize Button", "Action");

            if (MC.ELOPressed)
            {
                nRadMessageBox.Show(this, "Emergency Stop button must be released before reset!", "Reset Request", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Re-initializing the system will cause temporary loss of motor control.\r\nPlease confirm the die is at a safe location and that you would like to continue?", "Reset Request", MessageBoxButtons.YesNo, MessageBoxIcon.Hand))
            {
                //_scheduleTimer.Enabled = false;
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Initialize Reqest", "Action");
                _frmMain.ResetMC();
                _resetOneShot = true;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Initialize Reqest", "Action");
            }
        }

        private void buttonMaintLogs_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Logs Button", "Action");

            if (Settings.Default.UseLogDB)
            {
                _frmMain.LoadSubForm(_frmMain.frmLogViewer);
            }
            else
            {
                _frmMain.LoadSubForm(_frmMain.frmLogListing);
            }
        }

        private void buttonMaintConfig_Click(object sender, EventArgs e)
        {

            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Config Button", "Action");

            if (_frmMain.UserCanEditRecipe)
            {
                _frmMain.LoadSubForm(_frmMain.frmConfig);
            }
            else
            {
                nRadMessageBox.Show(this, "Current User does not have Edit Configuration Permissions", "Authentication", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Logout Button", "Action");
            _frmMain.ShowLoginForm();
        }

        private void buttonMaintReset_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Reset Button", "Action");

            if (MC.ELOPressed)
            {
                nRadMessageBox.Show(this, "Emergency Stop button must be released before reset!", "Reset Request", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Re-initializing the system will cause temporary loss of motor control.\r\nPlease confirm the die is at a safe location and that you would like to continue?", "Reset Request", MessageBoxButtons.YesNo, MessageBoxIcon.Hand))
            {
                //_scheduleTimer.Enabled = false;
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Reset Request", "Action");
                _frmMain.ResetMC(false); // do a RS instead of #reset
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Reset Request", "Action");
            }
        }

        private void buttonLdUldDie_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Die Load/Unload Button", "Action");

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Move Shuttle to Load/Unload Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Is the Lip Guard Attached?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Confirmed Lip Guards are in place and Moveto Request", "Action");
                    _frmMain.LastClick = DateTime.Now;
                    MC.RunGotoDieLoad();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined Lip Guards in place Request", "Action");
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Moveto Die Load/Unload Request", "Action");
            }
        }

        private void buttonShutdown_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Shutdown Button", "Action");

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Shutdown the nRad2?\rThis will secure the machine and power off the HMI.", "Shutdown Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Accepted Shutdown Request", "Action");
                MC.SetAirState(false);
                _frmMain.Shutdown();
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Shutdown Request", "Action");
            }
        }

        private void buttonDisableMainAir_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Maintenance-Toggle Main Air Button", "Action");

            if (MC.MainAirValveOpen)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you sure you wish to disable main air?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Accepted Disable Main Air Request", "Action");
                    MC.SetAirState(false);
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined Disable Main Air Request", "Action");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to enable main air?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Accepted Enable Main Air Request", "Action");
                    MC.SetAirState(true);
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined Enable Main Air Request", "Action");
                }
            }
        }

        private void buttonResetEMO_Click(object sender, EventArgs e)
        {
            _frmMain.PLC.ResetStackError();
            _frmMain.PLC.ResetEMO();
        }

        private void buttonInitMotors_Click(object sender, EventArgs e)
        {
            if (!MC.AirPressureFaultDuringInit)
            {
                InitializeMotors();
            }
            else
            {
                nRadMessageBox.Show(_frmMain, "Cannot Initialize Motors While Main Air Pressure Is LOW!\nPlease Check Main Air Pressure.", "Main Air Too Low!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonBypassDoorInterlockControl_Click(object sender, EventArgs e)
        {
            MS.BypassSafetyGuards = !MS.BypassSafetyGuards;
            Trace.WriteLine($"Bypass Safety Guards = {MS.BypassSafetyGuards}", "INFO");
            MS.Save();
        }

        #endregion

        #region Private Functions

        private bool InitializeMotors()
        {
            bool initialized = false;

            if (MC.ELOWasTripped)
            {
                initialized = false;
                _log.log(LogType.TRACE, Category.INFO, string.Format("Galil connection to: {0}.  System Reports Emergency Stop Activated.", MC.Address), "ERROR");
                nRadMessageBox.Show(this, "Emergency Stop was activated.  Please dis-engage and reset the PLC", "Connection Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (!MC.MainAirOK && MC.MainAirValveOpen)
            {
                initialized = false;
                _log.log(LogType.TRACE, Category.INFO, string.Format("Air Pressure On Connect: {0}", MC.MainAirPressure.ToString("0.0")), "INFO");
                _log.log(LogType.TRACE, Category.INFO, string.Format("Galil connection to: {0}.  System Reports Low Air Pressure: {1} PSI", MC.Address, MC.MainAirPressure.ToString("0.0")), "ERROR");
                nRadMessageBox.Show(this, "Main Air pressure is too low to allow operation.\r\nRestore air supply before continuing", "Connection Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (!MC.IsHomed && MC.MainAirOK)
            {
                if (MC.SafetyGuardsActive && !MS.BypassSafetyGuards)
                {
                    initialized = false;
                    nRadMessageBox.Show(this, "Initialization Required!\r\nSafety guards prevent the system from being initialized at this time.  Use Maintenance screen to initialize system.", "Connection Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (DialogResult.Yes == nRadMessageBox.Show(this, "Initialization Required!\r\nDo you wish to set up the nRad at this time?", "Connection Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    initialized = MC.CallInitAll();
                }
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, String.Format("Galil connection to: {0}.  System Reports OK and Homed.", MC.Address), "INFO");
                initialized = true;
            }

            return initialized;
        }

        #endregion

        #endregion
    }
}
