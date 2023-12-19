using nAble.Data;
using nAble.Model;
using nAble.Utils;
using nTact.DataComm;
using nTact.PLC;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormConfig : Form, IUpdateableForm
    {
        #region Constants

        private static readonly Color ColorControl = Color.FromName("Control");
        private const string PumpEOSMsg = "This Routine Will Determine The Maximum Stroke For the Attached {0} Pump";

        #endregion

        #region Properties

        private GalilWrapper2 MC => _frmMain.MC;
        private PLCWrapper PLC => _frmMain.PLC;
        private MachineSettingsII MS => _frmMain.MS;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;

        #endregion

        #region Fields

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private BackgroundWorker _bgwSetPwr = null;

        private TabPage _irTab = null;
        private TabPage _akTab = null;

        private bool _isLoading = false;
        private bool _findLimitsStarted = false;
        private bool _irCancelRequested = false;
        private bool _irSetPowerRunning = false;
        private bool _irSetPowerSucceeded = false;
        private bool _firstTime = true;

        #endregion

        #region Functions

        #region Constructors

        public FormConfig(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();

            _irTab = tabPageIR;
            _akTab = tabPageAirKnife;
#if !DESKTOP
            tabControlConfig.Controls.Remove(tabPageTesting);           
#endif
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible || _isLoading)
            {
                return;
            }

            if (_firstTime)
            {
                sbPumpSelection.Value = false;
                sbPumpSelection_ValueChanged(null, EventArgs.Empty);
                _firstTime = false;
            }

            ShowOrHideTabs();

            panelAutoLDULD.Visible = MS.AutoLDULD;
            if(MS.AutoLDULD)
            {
                pictureBoxRIO_DO0.BackColor = MC.ReadyToLoad ? Color.Lime : Color.Red;
                pictureBoxRIO_DO1.BackColor = MC.ReadyToUnload ? Color.Lime : Color.Red;
                pictureBoxRIO_DO2.BackColor = MC.OKToLDULD ? Color.Lime : Color.Red;

                pictureBoxRIO_DI0.BackColor = MC.UnloadRequest ? Color.Lime : Color.Red;
                pictureBoxRIO_DI1.BackColor = MC.UnloadComplete ? Color.Lime : Color.Red;
                pictureBoxRIO_DI2.BackColor = MC.LoadRequest ? Color.Lime : Color.Red;
                pictureBoxRIO_DI3.BackColor = MC.LoadComplete ? Color.Lime : Color.Red;
                pictureBoxRIO_DI4.BackColor = MC.AbortLDULDReq ? Color.Lime : Color.Red;
            }

            //Hide Pump Panels If Pump Not Installed
            labelConfigPumpVolTitle.Text = MC.SyringePumpDetected ? "Syringe Volume:" : "POH Volume";
            labelPumpHomingSpeedTitle.Text = MC.SyringePumpDetected ? "Syringe Homing Speed:" : "POH Homing Speed:";
            //Pump Selection Only Visible If Pump Tab Selected && Dual-Pump Installed
            sbPumpSelection.Visible = MS.DualPumpInstalled && (tabControlConfig.SelectedTab == tabPageConfigPumps);

            buttonScreenLock.BackColor = int.Parse(buttonScreenLock.Text) == MS.TimeOut ? ColorControl : Color.Yellow;
            buttonMinimumGap.BackColor = int.Parse(buttonMinimumGap.Text) == MS.MinimumGap ? ColorControl : Color.Yellow;
            labelPOHPrimingPreDelayUnits.Text = buttonTimeUnits.Text;
            labelPOHPrimingPostDelayUnits.Text = buttonTimeUnits.Text;

            buttonDoorInterlockControl.BackColor = PLC.CoaterDoorsBypassed ? Color.Red : SystemColors.Control;
            buttonDoorInterlockControl.Text = PLC.CoaterDoorsBypassed ? "Disabled" : "Enabled";
            buttonRobotDoorControl.BackColor = PLC.RobotDoorBypassed ? Color.Red : SystemColors.Control;
            buttonRobotDoorControl.Text = PLC.RobotDoorBypassed ? "Disabled" : "Enabled";

            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
            {
                if (MC.SyringePumpDetected)
                    buttonCfgSyringeVol.BackColor = double.Parse(buttonCfgSyringeVol.Text) == MS.SyringeVol ? ColorControl : Color.Yellow;
                else
                    buttonCfgSyringeVol.BackColor = double.Parse(buttonCfgSyringeVol.Text) == MS.POHVol ? ColorControl : Color.Yellow;

                if (MC.SyringePumpDetected)
                    buttonSyringeHomingSpeed.BackColor = (double.Parse(buttonSyringeHomingSpeed.Text) == _frmMain.Storage.SyringePrimingRate) ? ColorControl : Color.Yellow;
                else
                    buttonSyringeHomingSpeed.BackColor = (double.Parse(buttonSyringeHomingSpeed.Text) == MS.POHHomingRate) ? ColorControl : Color.Yellow;
            }
            else
            {
                if (MC.SyringePumpBDetected)
                    buttonCfgSyringeVol.BackColor = double.Parse(buttonCfgSyringeVol.Text) == MS.SyringeBVol ? ColorControl : Color.Yellow;
                else
                    buttonCfgSyringeVol.BackColor = double.Parse(buttonCfgSyringeVol.Text) == MS.POHBVol ? ColorControl : Color.Yellow;

                if (MC.SyringePumpBDetected)
                    buttonSyringeHomingSpeed.BackColor = (double.Parse(buttonSyringeHomingSpeed.Text) == _frmMain.Storage.SyringeBPrimingRate) ? ColorControl : Color.Yellow;
                else
                    buttonSyringeHomingSpeed.BackColor = (double.Parse(buttonSyringeHomingSpeed.Text) == MS.POHBHomingRate) ? ColorControl : Color.Yellow;
            }

            double dUsableVol;
            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
                dUsableVol = (MC.SyringePumpDetected ? _frmMain.Storage.MaxSyringePulses : _frmMain.Storage.MaxPOHPulses) / MC.uLConv / 1000;
            else
                dUsableVol = (MC.SyringePumpBDetected ? _frmMain.Storage.MaxSyringeBPulses : _frmMain.Storage.MaxPOHBPulses) / MC.uLConv / 1000;
            labelCurrentVolAvailable.Text = dUsableVol.ToString("0.000");

            labelSmoothingSamplesDetails.Text = $"The system is configured to sample values every {MS.DataUpdateRate} ms.";
            buttonDiePresSmoothingSamples.BackColor = int.Parse(buttonDiePresSmoothingSamples.Text) == _frmMain.Storage.PressureSmoothingSamples ? ColorControl : Color.Yellow;

            buttonCfgEditMeasureSpeed.BackColor = double.Parse(buttonCfgEditMeasureSpeed.Text) == MS.MeasureSpeed ? ColorControl : Color.Yellow;
            buttonCfgEditMeasureLoc.BackColor = double.Parse(buttonCfgEditMeasureLoc.Text) == MS.MeasureLoc ? ColorControl : Color.Yellow;
            buttonCfgEditMeasureHeight.BackColor = double.Parse(buttonCfgEditMeasureHeight.Text) == MS.MeasureHeight ? ColorControl : Color.Yellow;
            buttonPrimingMeasLoc.BackColor = double.Parse(buttonPrimingMeasLoc.Text) == MS.PrimingMeasureLoc ? ColorControl : Color.Yellow;
            labelPrimingLZPos.Text = MS.ZLMeasurePosForPriming.ToString("0.000");
            labelPrimingRZPos.Text = MS.ZRMeasurePosForPriming.ToString("0.000");
            var zLeft = MS.ZLZeroEncPosForChuck / MC.mmConv;
            var zRight = MS.ZRZeroEncPosForChuck / MC.mmConv;
            labelChuckLZPosCalib.Text = zLeft.ToString("0.000");
            labelChuckRZPosCalib.Text = zRight.ToString("0.000");

            buttonShimSize.BackColor = (int.Parse(buttonShimSize.Text) * .001) == MS.ShimSize ? ColorControl : Color.Yellow;
            buttonLevelingTolerance.BackColor = int.Parse(buttonLevelingTolerance.Text) == MS.LevelingTolerance ? ColorControl : Color.Yellow;
            buttonLevelingPercentMove.BackColor = int.Parse(buttonLevelingPercentMove.Text) == MS.LevelingMovePercentage ? ColorControl : Color.Yellow;
            buttonLevelingAttempts.BackColor = int.Parse(buttonLevelingAttempts.Text) == MS.MaxLevelingRetries ? ColorControl : Color.Yellow;

            labelFindPumpEndOfStroke.Text = string.Format(PumpEOSMsg, MC.SyringePumpDetected ? "Syringe" : "POH");
            buttonFindPumpEndOfStroke.Enabled = !MC.Moving && !_findLimitsStarted;
            buttonFindPumpEndOfStroke.BackColor = _findLimitsStarted ? Color.Yellow : ColorControl;

            if (MS.IRLampInstalled)
            {
                var IR = MC.IRTransmitter;

                bool enabled = IR != null;
                bool connected = enabled && IR.IsConnected;

                labelIRPowerLevel.Text = connected ? $"{IR.CurrentPowerLevel:##0.0} %" : "---";
                buttonDieIRDistance.BackColor = double.Parse(buttonDieIRDistance.Text) == MS.DieToIRPitch ? ColorControl : Color.Yellow;

                labelIRConnected.Text = connected ? "Connected" : "Not Connected";
                pictureBoxIRConnected.BackColor = connected ? Color.Lime : Color.Red;

                labelIRLampOn.Text = connected && IR.IsOn ? "Lamp Is On" : "Lamp Is Off";
                pbIRLampOn.BackColor = connected && IR.IsOn ? Color.Lime : Color.Red;

                bool comError = enabled && IR.COMFailure;
                labelIRComError.Text = comError ? "COM Error" : "No COM Error";
                pictureBoxIRComError.BackColor = comError ? Color.Lime : Color.Red;

                bool overTempError = enabled && IR.OverTemperature;
                labelIROverTempError.Text = overTempError ? "Over Temp Error" : "No Temp Error";
                pictureBoxIROverTempError.BackColor = overTempError ? Color.Lime : Color.Red;

                labelIRPowerLevel.Text = connected ? $"{IR.CurrentPowerLevel:#0.00} %" : "--NA--";
                labelIRLoadCurrent.Text = connected ? $"{IR.CurrentLoadCurrent:#0.00} Amps" : "--- Amps";
                labelIRVoltage.Text = connected ? $"{IR.CurrentLoadVoltage:#0.00} Volts" : "--- Volts";
                labelIRPower.Text = connected ? $"{IR.CurrentLoadPower:#0.00} kW" : "--- kW";

                bool irOn = IR?.IsOn ?? false;
            }

            groupBoxAirKnifeHeater.Visible = MS.AirKnifeHeaterInstalled;

            if (MS.AirKnifeInstalled && MS.AirKnifeHeaterInstalled)
            {
                buttonAirKnifeHeaterSetpoint.BackColor = double.Parse(buttonAirKnifeHeaterSetpoint.Text) == MS.AirKnifeHeaterSetPoint ?
                    ColorControl : Color.Yellow;
                buttonGasHeaterSetpoint.BackColor = double.Parse(buttonGasHeaterSetpoint.Text) == MS.GasHeaterSetPoint ?
                    ColorControl : Color.Yellow;
                buttonAirKnifeHeaterWarmupTime.BackColor = double.Parse(buttonAirKnifeHeaterWarmupTime.Text) == MS.AirKnifeHeaterWarmup ?
                    ColorControl : Color.Yellow;

                labelMaintGasHeaterTemp.Text = Heaters.Temp(MS.GasHeaterCOMID);
                labelMaintGasHeaterTemp.ForeColor = Heaters.TempColor(MS.GasHeaterCOMID);
                var setpoint = Heaters.SetPoint(MS.GasHeaterCOMID);
                labelMaintGasHeaterSP.Text = $"Setpoint:   {setpoint:0.0}".PadLeft(5);

                labelMaintKnifeHeaterTempOutput.Text = Heaters.Temp(MS.AirKnifeHeaterCOMID);
                labelMaintKnifeHeaterTempOutput.ForeColor = Heaters.TempColor(MS.AirKnifeHeaterCOMID);
                setpoint = Heaters.SetPoint(MS.AirKnifeHeaterCOMID);
                labelKnifeTempSP.Text = $"Setpoint:   {setpoint:0.0}".PadLeft(5);
            }

            // If a find limits just stopped, record the values and save them.
            if (!MC.IsFindingPumpLimits && _findLimitsStarted)
            {
                int pumpFullStroke = Math.Abs((int)MC.TheAnswer);
                _findLimitsStarted = false;

                labelPumpMaxPulses.Text = pumpFullStroke.ToString();

                if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
                {
                    if (MC.POHPumpDetected)
                    {
                        _frmMain.Storage.MaxPOHPulses = pumpFullStroke;
                    }
                    else if (MC.SyringePumpDetected)
                    {
                        _frmMain.Storage.MaxSyringePulses = pumpFullStroke;
                    }
                }
                else
                {
                    if (MC.POHPumpBDetected)
                    {
                        _frmMain.Storage.MaxPOHBPulses = pumpFullStroke;
                    }
                    else if (MC.SyringePumpBDetected)
                    {
                        _frmMain.Storage.MaxSyringeBPulses = pumpFullStroke;
                    }
                }

                _frmMain.Storage.Save();
            }

            if (_findLimitsStarted)
            {
                if (MC.IsFindingPumpLimits)
                {
                    labelPumpMaxPulses.Text = "Running";
                }
                else
                {
                    labelPumpMaxPulses.Text = "Starting";
                }
            }

            labelDiePressure.Text = $"{MC.DiePressurePSISmoothed.ToString("0.00")} PSI";
        }

        #endregion

        #region Control Event Handlers

        private void FormConfig_Load(object sender, EventArgs e)
        {
            _isLoading = true;
            InitTextBoxes();

            _bgwSetPwr = new BackgroundWorker();
            _bgwSetPwr.DoWork += _bgSetPwr_DoWork;
            _bgwSetPwr.RunWorkerCompleted += _bgSetPwr_RunWorkerCompleted;

            _isLoading = false;
        }

        private void buttonConfigSetup_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Config-Setup Button", "Action");
            _frmMain.LoadSubForm(_frmMain.frmSetup);
        }

        private void buttonConfigCancel_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to close?  Any unsaved changes will be discarded.", "Exit Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                InitTextBoxes();
                _frmMain.ShowPrevForm();
            }
        }

        private void buttonConfigSave_Click(object sender, EventArgs e)
        {
            double dTemp;
            int nTemp, nRecircCount = 0, nRecircTime = 0;

            int.TryParse(buttonRecircTime.Text, out nRecircTime);
            int.TryParse(buttonRecircCount.Text, out nRecircCount);

            if (nRecircCount > nRecircTime)
            {
                nRadMessageBox.Show(this, "Pump Recirculation is limited to once per minute. Please adjust the Time or Count.", "Pump Prime", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            _frmMain.Storage.RecirculationInterval = nRecircTime;
            _frmMain.Storage.RecirculationCount = nRecircCount;

            //Screen Lock
            MS.TimeOut = int.Parse(buttonScreenLock.Text);
            // Air Knife Post Coat Distance
            MS.DieToAirKnifePitch = double.Parse(buttonDieAirKnifeDistance.Text);

            MS.DieToIRPitch = double.Parse(buttonDieIRDistance.Text);

            MS.DataLogging = checkBoxDataLogging.Checked;

            if (int.TryParse(buttonMinimumGap.Text, out nTemp))
                MS.MinimumGap = nTemp;

            if (MS.AirKnifeInstalled && MS.AirKnifeHeaterInstalled)
            {
                MS.AirKnifeHeaterSetPoint = double.Parse(buttonAirKnifeHeaterSetpoint.Text);
                MS.GasHeaterSetPoint = double.Parse(buttonGasHeaterSetpoint.Text);
                MS.AirKnifeHeaterWarmup = int.Parse(buttonAirKnifeHeaterWarmupTime.Text);

                Heaters.ChangeTempSetPoint(MS.GasHeaterCOMID, MS.GasHeaterSetPoint);
                Heaters.ChangeTempSetPoint(MS.AirKnifeHeaterCOMID, MS.AirKnifeHeaterSetPoint);
            }

            _frmMain.Storage.PressureSmoothingSamples = int.Parse(buttonDiePresSmoothingSamples.Text);
            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
            {
                // Pump Params
                if (MC.SyringePumpDetected)
                {
                    if (double.TryParse(buttonCfgSyringeVol.Text, out dTemp))
                        MS.SyringeVol = dTemp;
                    if (double.TryParse(buttonSyringeHomingSpeed.Text, out dTemp))
                        _frmMain.Storage.SyringePrimingRate = dTemp;
                }
                else
                {
                    if (double.TryParse(buttonCfgSyringeVol.Text, out dTemp))
                        MS.POHVol = dTemp;
                    if (double.TryParse(buttonSyringeHomingSpeed.Text, out dTemp))
                        MS.POHHomingRate = dTemp;
                }
            }
            else
            {
                // Pump-B Params
                if (MC.SyringePumpBDetected)
                {
                    if (double.TryParse(buttonCfgSyringeVol.Text, out dTemp))
                        MS.SyringeBVol = dTemp;
                    if (double.TryParse(buttonSyringeHomingSpeed.Text, out dTemp))
                        _frmMain.Storage.SyringeBPrimingRate = dTemp;
                }
                else
                {
                    if (double.TryParse(buttonCfgSyringeVol.Text, out dTemp))
                        MS.POHBVol = dTemp;
                    if (double.TryParse(buttonSyringeHomingSpeed.Text, out dTemp))
                        MS.POHBHomingRate = dTemp;
                }
            }
            if (int.TryParse(buttonPOHPrimingPreDelay.Text, out nTemp))
                MS.PrimePreDelay = buttonTimeUnits.Text == "ms" ? nTemp : (nTemp * 1000);
            if (int.TryParse(buttonPOHPrimingPostDelay.Text, out nTemp))
                MS.PrimePostDelay = buttonTimeUnits.Text == "ms" ? nTemp : (nTemp * 1000);

            // Measure Params
            if (double.TryParse(buttonPrimingMeasLoc.Text, out dTemp))
                MS.PrimingMeasureLoc = dTemp;
            if (double.TryParse(buttonCfgEditMeasureLoc.Text, out dTemp))
                MS.MeasureLoc = dTemp;
            if (double.TryParse(buttonCfgEditMeasureSpeed.Text, out dTemp))
                MS.MeasureSpeed = dTemp;
            if (double.TryParse(buttonCfgEditMeasureHeight.Text, out dTemp))
                MS.MeasureHeight = dTemp;
            if (int.TryParse(buttonShimSize.Text, out nTemp))
                MS.ShimSize = nTemp * .001;

            //Time-Units Displayed
            MS.DisplayedTimeUnits = buttonTimeUnits.Text;
            buttonTimeUnits.BackColor = Color.FromKnownColor(KnownColor.White);

            //Safety Zone Params
            MS.Zone2Start = double.Parse(buttonSafetyZone2Start.Text);
            MS.Zone3Start = double.Parse(buttonSafetyZone3Start.Text);
            MS.Zone4Start = double.Parse(buttonSafetyZone4Start.Text);
            MS.Zone5Start = double.Parse(buttonSafetyZone5Start.Text);
            MS.Zone1MaxZ = double.Parse(buttonSafetyZone1MaxZ.Text);
            MS.Zone2MaxZ = double.Parse(buttonSafetyZone2MaxZ.Text);
            MS.Zone3MaxZ = double.Parse(buttonSafetyZone3MaxZ.Text);
            MS.Zone4MaxZ = double.Parse(buttonSafetyZone4MaxZ.Text);
            MS.Zone5MaxZ = double.Parse(buttonSafetyZone5MaxZ.Text);

            MS.LevelingTolerance = int.Parse(buttonLevelingTolerance.Text);
            MS.LevelingMovePercentage = int.Parse(buttonLevelingPercentMove.Text);
            MS.MaxLevelingRetries = int.Parse(buttonLevelingAttempts.Text);

            MS.Save();

            if (!_frmMain.Storage.Save() || !MS.Save())
            {
                nRadMessageBox.Show(this, "Settings could not be saved: \r\n" + _frmMain.Storage.LastError, "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MC.DownloadDelays();

                if (DialogResult.Yes == nRadMessageBox.Show(this, "Settings saved successfully\r\rDo you wish to exit Configuration?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    _frmMain.LoadSubForm(_frmMain.frmMaintenance);
                }
            }
        }

        private void buttonScreenLock_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Screen Lock Time Out\r\n'0' to disable", this, buttonScreenLock, "#", 0, 480, "", false);
            _log.log(LogType.TRACE, Category.INFO, "User Has Changed Screen Lockout Time To: " + buttonScreenLock.Text);
        }
        
        private void buttonCfgEditSyringeVol_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"{(MC.SyringePumpDetected ? "Syringe" : "POH")} Volume (ml)", this, buttonCfgSyringeVol, "0.0#", .1, 1000);
            _log.log(LogType.TRACE, Category.INFO, "User Has Changed Syringe Volume To: " + buttonCfgSyringeVol.Text + " (mL)");
        }
        
        private void buttonSyringHomingSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"{(MC.SyringePumpDetected ? "Syringe" : "POH")} Homing Speed (µl/s)\r\nRange: 1 - 500", this, buttonSyringeHomingSpeed, "0.0", 1, 100);
            _log.log(LogType.TRACE, Category.INFO, "User Has Changed Syringe Homing Speed To: " + buttonSyringeHomingSpeed.Text + "");
        }
        
        private void buttonPOHPrimingPreDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen($"POH Pre Pumping Valve Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec)\r\nRange: 0 - 60000" : "sec)\r\nRange: 0 - 60")}", this, buttonPOHPrimingPreDelay, "0", 0, MS.DisplayedTimeUnits == "sec" ? 60 : 60000, "", buttonTimeUnits.Text == "sec");
            _log.log(LogType.TRACE, Category.INFO, "User Has Changed POH PRiming Pre-Delay To: " + buttonPOHPrimingPreDelay.Text + "");

        }
        
        private void buttonPOHPrimingPostDelay_Click(object sender, EventArgs e)
        {
            //TODO Logging
            _frmMain.GotoNumScreen($"POH Post Pumping Valve Delay ({(MS.DisplayedTimeUnits == "ms" ? "msec)\r\nRange: 0 - 60000" : "sec)\r\nRange: 0 - 60")}", this, buttonPOHPrimingPostDelay, "0", 0, MS.DisplayedTimeUnits == "sec" ? 60 : 60000, "", buttonTimeUnits.Text == "sec");
        }
        
        private void buttonPrimingMeasLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Measurement X-Axis Location", this, buttonPrimingMeasLoc, "#.#", 1, MS.MaxXTravel);
        }
        
        private void buttonCfgEditMeasureLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Chuck Measurement X-Axis Location", this, buttonCfgEditMeasureLoc, "#.#", 1, MS.MaxXTravel);
        }
        
        private void buttonCfgEditMeasureSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measurement X-Axis Speed", this, buttonCfgEditMeasureSpeed, "#.#", 1, 100);
        }
        
        private void buttonCfgEditMeasureHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measurement Z-Axis Height (mm)", this, buttonCfgEditMeasureHeight, "#.###", 0.001, MS.MaxZTravel);
        }
        
        private void buttonCfgMeasLocTeach_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            buttonCfgEditMeasureLoc.Text = MC.XPos.ToString();
            buttonCfgEditMeasureHeight.Text = MC.ZRPos.ToString();
        }
        
        private void buttonPrimingMeasureZLoc_Click(object sender, EventArgs e)
        {
            MS.ZRMeasurePosForPriming = MC.ZRPos;
            MS.ZLMeasurePosForPriming = MC.ZLPos;
        }

        private void textBoxCfgAccessCode_TextChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void buttonRecircTime_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Recirculation Interval (mins)", this, buttonRecircTime, "#", 1, 3599, "", false);
        }

        private void buttonRecircCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Recirculation Stroke Count", this, buttonRecircCount, "#", 1, 25, "", false);
        }

        //private void buttonSafetyZone1End_Click(object sender, EventArgs e)
        //{
        //    _frmMain.GotoNumScreen("Safety Zone 1 End - Maintenance Area", this, buttonSafetyZone1End, "0.000", 0, MS.MaxXTravel);
        //}

        private void buttonSafetyZone1MaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 1 Maximum Z Distance", this, buttonSafetyZone1MaxZ, "0.000", 0, MS.MaxZTravel);
        }

        private void buttonSafetyZone2Start_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 2 Start - Priming Area", this, buttonSafetyZone2Start, "0.000", 0, MS.MaxXTravel);
        }

        //private void buttonSafetyZone2End_Click(object sender, EventArgs e)
        //{
        //    _frmMain.GotoNumScreen("Safety Zone 2 End - Priming Area", this, buttonSafetyZone2End, "0.000", 0, MS.MaxXTravel);
        //}

        private void buttonSafetyZone2MaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 2 Maximum Z Distance", this, buttonSafetyZone2MaxZ, "0.000", 0, MS.MaxZTravel);
        }

        private void buttonSafetyZone3Start_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 3 Start - Gap Area", this, buttonSafetyZone3Start, "0.000", 0, MS.MaxXTravel);
        }

        //private void buttonSafetyZone3End_Click(object sender, EventArgs e)
        //{
        //    _frmMain.GotoNumScreen("Safety Zone 3 End - Gap Area", this, buttonSafetyZone3End, "0.000", 0, MS.MaxXTravel);
        //}

        private void buttonSafetyZone3MaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 3 Maximum Z Distance", this, buttonSafetyZone3MaxZ, "0.000", 0, MS.MaxZTravel);
        }

        private void buttonSafetyZone4Start_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 4 Start - Chuck Area", this, buttonSafetyZone4Start, "0.000", 0, MS.MaxXTravel);
        }

        //private void buttonSafetyZone4End_Click(object sender, EventArgs e)
        //{
        //    _frmMain.GotoNumScreen("Safety Zone 4 End - Chuck Area", this, buttonSafetyZone4End, "0.000", 0, MS.MaxXTravel);
        //}

        private void buttonSafetyZone4MaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 4 Maximum Z Distance", this, buttonSafetyZone4MaxZ, "0.000", 0, MS.MaxZTravel);
        }

        private void buttonSafetyZone5Start_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 5 Start - Finish Area", this, buttonSafetyZone5Start, "0.000", 0, MS.MaxXTravel);
        }

        private void buttonSafetyZone5MaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Safety Zone 5 Maximum Z Distance", this, buttonSafetyZone5MaxZ, "0.000", 0, MS.MaxZTravel);
        }

        private void buttonMinimumGap_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Minimum Coating/Priming Gap", this, buttonMinimumGap, "#", 0, 5000, "", false);
        }

        private void buttonLevelingTolerance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Leveling 'in position' tolance (µm)", this, buttonLevelingTolerance, "#", 1, 20, "", false);
        }

        private void buttonLevelingAttempts_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Number of retries to reach leveling tolerance", this, buttonLevelingAttempts, "#", 1, 50, "", false);
        }

        private void buttonDieAirKnifeDistance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die to Air Knive Distance", this, buttonDieAirKnifeDistance, "#0.0", 0, 150);
        }

        private void FormConfig_Enter(object sender, EventArgs e)
        {
            labelDieAirKnifePitch.Visible = MS.AirKnifeInstalled;
            buttonDieAirKnifeDistance.Visible = MS.AirKnifeInstalled;
            labelDieAirKnifeDistanceUnits.Visible = MS.AirKnifeInstalled;
        }

        private void buttonFindPumpEndOfStroke_Click(object sender, EventArgs e)
        {
            string pumpName;
            int pumpNum = 0;

            if (!_findLimitsStarted)
            {
                if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
                {
                    pumpName = MC.SyringePumpDetected ? "Syringe-A" : "POH-A";
                    pumpNum = 0;
                }
                else
                {
                    pumpName = MC.SyringePumpBDetected ? "Syringe-B" : "POH-B";
                    pumpNum = 1;
                }

                if (DialogResult.Yes == nRadMessageBox.Show(this, $"OK to Start Find End of Stroke Test on {pumpName}?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, $"Find {pumpName} EndOfStroke requested FindingPumpLimits", "INFO");
                    _findLimitsStarted = true;
                    MC.StartFindPumpLimits(pumpNum);
                }
            }
        }

        private void buttonLevelingPercentMove_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Percent to move during Leveling (%)", this, buttonLevelingPercentMove, "#", 50, 100, "", false);
        }

        private void buttonDiePresSmoothingSamples_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Number of samples for Die Pressure Smooothing", this, buttonDiePresSmoothingSamples, "#", 1, 1000, "", false);
        }

        private void buttonCalibrateDiePresTransDisplay_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed Calibrate Die Pressure Display Button.  Specified Value: {buttonCurDiePresTransDisplay.Text} PSI", "Action");
            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Continue with a value of {buttonCurDiePresTransDisplay.Text} (PSI) for the Pressure Transducer's Display?", "Calibration", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, $"Previous Die Pressure Voltage Adjust Value: {MS.DiePressureInputVoltageAdjust.ToString("0.0000")}", "Info");
                double dCurVal = double.Parse(buttonCurDiePresTransDisplay.Text);
                MC.SetDiePressureVoltageAdjust(dCurVal, out double dTemp);
                _log.log(LogType.TRACE, Category.INFO, $"New  Die Pressure Voltage Adjust Value: {MS.DiePressureInputVoltageAdjust.ToString("0.0000")}", "Info");
                nRadMessageBox.Show(this, $"Calibration Complete.  Please wait {_frmMain.Storage.PressureSmoothingSamples} smoothing samples to verify results.", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User cancelled Die Pressure Display Calibration", "Action");
            }
        }

        private void labelPrimingMeasureZLoc_Click(object sender, EventArgs e)
        {

        }

        private void buttonDoorInterlockControl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (PLC.IsConnected)
            {
                if (PLC.CoaterDoorsBypassed)
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Enabled Coater Door Interlocks.", "Action");
                    PLC.CoaterDoorsBypassed = false;
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Disabled Coater Door Interlocks.", "Action");
                    PLC.CoaterDoorsBypassed = true;
                }
            }
        }

        private void buttonRobotDoorControl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (PLC.IsConnected)
            {
                if (PLC.RobotDoorBypassed)
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Enabled Robot Door Interlocks.", "Action");
                    PLC.RobotDoorBypassed = false;
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Disabled Robot Door Interlocks.", "Action");
                    PLC.RobotDoorBypassed = true;
                }
            }
        }

        private void buttonTeachMeasurePos_Click(object sender, EventArgs e)
        {
            MS.ZRMeasurePos = MC.ZRPos;
            MS.ZLMeasurePos = MC.ZLPos;
        }

        private void buttonShimSize_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Calibration Shim Size (µm):", this, buttonShimSize, "#0", 1, 999, "", false);
        }

        private void sbPumpSelection_ValueChanged(object sender, EventArgs e)
        {
            if (!sbPumpSelection.Value)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Setting Controller Pump Selected... Selected: Pump-A");
                MC.SetControllerPumpUsed(0);
                _frmMain.Storage.SelectedPump = 0;
                _frmMain.Storage.Save();
                _log.log(LogType.TRACE, Category.INFO, $"Machine Storage Saved!");
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, $"Setting Controller Pump Selected... Selected: Pump-B");
                MC.SetControllerPumpUsed(1);
                _frmMain.Storage.SelectedPump = 1;
                _frmMain.Storage.Save();
                _log.log(LogType.TRACE, Category.INFO, $"Machine Storage Saved!");
            }
            InitTextBoxes();
        }

        private void buttonTimeUnits_Click(object sender, EventArgs e)
        {
            if (buttonTimeUnits.Text == "sec")
            {
                buttonTimeUnits.Text = "ms";
                buttonPOHPrimingPostDelay.Text = (double.Parse(buttonPOHPrimingPostDelay.Text) * 1000).ToString("#0");
                buttonPOHPrimingPreDelay.Text = (double.Parse(buttonPOHPrimingPreDelay.Text) * 1000).ToString("#0");
                labelPOHPrimingPreDelayUnits.Text = "ms";
                labelPOHPrimingPostDelayUnits.Text = "ms";
            }
            else if (buttonTimeUnits.Text == "ms")
            {
                buttonTimeUnits.Text = "sec";
                double dTemp = double.Parse(buttonPOHPrimingPostDelay.Text);
                buttonPOHPrimingPostDelay.Text = (dTemp==0 ? 0 : (double.Parse(buttonPOHPrimingPostDelay.Text) / 1000)).ToString("#0.000");
                dTemp = double.Parse(buttonPOHPrimingPreDelay.Text);
                buttonPOHPrimingPreDelay.Text = (dTemp == 0 ? 0 : (double.Parse(buttonPOHPrimingPreDelay.Text) / 1000)).ToString("#0.000");
                labelPOHPrimingPreDelayUnits.Text = "sec";
                labelPOHPrimingPostDelayUnits.Text = "sec";
            }
            buttonTimeUnits.BackColor = Color.Yellow;
        }

        private void buttonTeachCalibPos_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.ACTION, $"User Pressed Teach Current Calibration Position:  ZL: {MC.ZLPos}  ZR: {MC.ZRPos}", "", SubCategory.NONE);
            MS.ZRZeroEncPosForChuck = MC.ZRPos;
            MS.ZLMeasurePosForPriming = MC.ZLPos;
        }

        private void buttonCfgEditMeasureHeight_Click_1(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measurement Z-Axis Height (mm)", this, buttonCfgEditMeasureHeight, "#.###", 0.001, 5);
        }

        private void buttonNewIRPower_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("New IR Power", this, buttonNewIRPower, "##0.0", 0, 100);
        }

        private void buttonSetNewIRPower_Click(object sender, EventArgs e)
        {
            if (buttonSetNewIRPower.Text == "Cancel")
            {
                _irCancelRequested = true;
            }
            else
            {
                if (_irSetPowerRunning)
                {
                    nRadMessageBox.Show(this, "Could not set new power level -- worker busy!", "Set Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (double.TryParse(buttonNewIRPower.Text, out var newPower))
                {
                    //MC.IRTransmitter?.SetPowerLevel(newPower);
                    _irCancelRequested = false;
                    _irSetPowerSucceeded = false;
                    buttonSetNewIRPower.Text = "Cancel";
                    _irSetPowerRunning = true;
                    _bgwSetPwr.RunWorkerAsync(newPower);
                }
                else
                {
                    nRadMessageBox.Show(this, "Could not parse new power level!", "Parse Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDieIRDistance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die to IR Distance", this, buttonDieIRDistance, "#0.0", 0, 250);
        }

        private void buttonCurDiePresTransDisplay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Current Pressure Transducer Display (PSI)", this, buttonCurDiePresTransDisplay, "0.00#", -1.8, 36.27, "", true);
        }

        private void buttonAirKnifeHeaterSetpoint_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Air Knife Heater Temperature (°C): ", this, buttonAirKnifeHeaterSetpoint, "##0.#", 0.0, MS.AirKnifeHeaterMaxTemperature, "", true);
            Trace.WriteLine($"User Changed Air Knife Temperature: {buttonAirKnifeHeaterSetpoint.Text}%", "INFO");
        }

        private void buttonAirKnifeHeaterWarmupTime_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Air Knife Heater Warmup Time", this, buttonAirKnifeHeaterWarmupTime, "##0", 0, 999, "", false);
        }

        private void buttonGasHeaterSetpoint_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Gas Heater Temperature (°C): ", this, buttonGasHeaterSetpoint, "##0.#", 0.0, MS.AirKnifeHeaterMaxTemperature, "", true);
            Trace.WriteLine($"User Changed Gas Heater Setpoint Temperature: {buttonGasHeaterSetpoint.Text}%", "INFO");
        }

        private void buttonIROnOff_Click(object sender, EventArgs e)
        {

        }

        private void buttonIRAutoManual_Click(object sender, EventArgs e)
        {

        }

        private void _bgSetPwr_DoWork(object sender, DoWorkEventArgs e)
        {
            double newPower = (double)e.Argument;
            _irSetPowerSucceeded = MC.IRTransmitter.WaitForPowerLevelChange(newPower, 10, () => _irCancelRequested);
        }

        private void _bgSetPwr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonSetNewIRPower.Text = "Set";

            if (!_irSetPowerSucceeded)
            {
                nRadMessageBox.Show(this, "IR Power Level could not be set.", "Set failed.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _irSetPowerRunning = false;
        }

        private void buttonIRLampOn_Click(object sender, EventArgs e)
        {
            var IR = MC.IRTransmitter;

            if (IR is null || !IR.IsConnected)
            {
                MessageBox.Show(this, $"IR not connected -- could not change the lamp on/off state.", "IR not Connected");
                return;
            }

            if (IR.IsOn)
            {
                _log.log(LogType.ACTIVITY, Category.INFO, "Turning IR Lamp Off");
                IR.TurnOffIRTransmitter();
            }
            else
            {
                _log.log(LogType.ACTIVITY, Category.INFO, "Turning IR Lamp On");
                IR.TurnOnIRTransmitter();
            }
        }

        private void buttonTurnLampOn_Click(object sender, EventArgs e)
        {
            var IR = MC.IRTransmitter;

            if (IR is null || !IR.IsConnected)
            {
                MessageBox.Show(this, $"IR not connected -- could not change the lamp on/off state.", "IR not Connected");
                return;
            }

            _log.log(LogType.ACTIVITY, Category.INFO, "Turning IR Lamp On");
            IR.TurnOnIRTransmitter();
        }

        private void buttonTurnLampOff_Click(object sender, EventArgs e)
        {
            var IR = MC.IRTransmitter;

            if (IR is null || !IR.IsConnected)
            {
                MessageBox.Show(this, $"IR not connected -- could not change the lamp on/off state.", "IR not Connected");
                return;
            }

            _log.log(LogType.ACTIVITY, Category.INFO, "Turning IR Lamp Off");
            IR.TurnOffIRTransmitter();
        }

        #endregion

        #region Private Functions

        private void ShowOrHideTabs()
        {
            tabControlConfig.ShowOrHideTab(_irTab, MS.IRLampInstalled);
            tabControlConfig.ShowOrHideTab(_akTab, MS.AirKnifeInstalled);
        }

        private void InitTextBoxes()
        {
            buttonScreenLock.Text = MS.TimeOut.ToString();
            buttonTimeUnits.Text = MS.DisplayedTimeUnits;

            buttonDoorInterlockControl.Visible = MS.HasStack;
            labelDoorInterlockControl.Visible = MS.HasStack;
            buttonRobotDoorControl.Visible = MS.HasStack;
            labelRobotDoorControl.Visible = MS.HasStack;

            buttonDieAirKnifeDistance.Text = MS.DieToAirKnifePitch.ToString("#0.0");
            labelDieAirKnifePitch.Visible = MS.AirKnifeInstalled;
            buttonDieAirKnifeDistance.Visible = MS.AirKnifeInstalled;
            labelDieAirKnifeDistanceUnits.Visible = MS.AirKnifeInstalled;
            checkBoxDataLogging.Checked = MS.DataLogging;
            checkBoxDataLogging.Visible = MS.DataLoggingEnabled;

            buttonDieIRDistance.Text = MS.DieToIRPitch.ToString("#0.0");
            double usableVol;
            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
                usableVol = MC.SyringePumpDetected ? MS.SyringeVol : MS.POHVol;
            else
                usableVol = MC.SyringePumpBDetected ? MS.SyringeBVol : MS.POHBVol;
            buttonCfgSyringeVol.Text = usableVol.ToString("#0.0");	// ml

            double homingRate;
            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
                homingRate = MC.SyringePumpDetected ? _frmMain.Storage.SyringePrimingRate : MS.POHHomingRate;
            else
                homingRate = MC.SyringePumpBDetected ? _frmMain.Storage.SyringeBPrimingRate : MS.POHBHomingRate;
            buttonSyringeHomingSpeed.Text = homingRate.ToString("#0.0");

            buttonPOHPrimingPreDelay.Text = MS.DisplayedTimeUnits == "ms" ? MS.PrimePostDelay.ToString("#0") : ((MS.PrimePostDelay / 1000).ToString("0"));
            buttonPOHPrimingPostDelay.Text = MS.DisplayedTimeUnits == "ms" ? MS.PrimePostDelay.ToString("#0") : ((MS.PrimePostDelay / 1000).ToString("0"));
            buttonMinimumGap.Text = MS.MinimumGap.ToString("#0");
            buttonPrimingMeasLoc.Text = MS.PrimingMeasureLoc.ToString();
            buttonCfgEditMeasureLoc.Text = MS.MeasureLoc.ToString();
            buttonCfgEditMeasureSpeed.Text = MS.MeasureSpeed.ToString();
            buttonCfgEditMeasureHeight.Text = MS.MeasureHeight.ToString();
            buttonShimSize.Text = (MS.ShimSize * 1000).ToString("#0");

            buttonDiePresSmoothingSamples.Text = _frmMain.Storage.PressureSmoothingSamples.ToString();

            buttonRecircTime.Text = _frmMain.Storage.RecirculationInterval.ToString();
            buttonRecircCount.Text = _frmMain.Storage.RecirculationCount.ToString();

            if (!MS.DualPumpInstalled || !sbPumpSelection.Value)
            {
                labelPumpMaxPulses.Text = (MC.SyringePumpDetected ? _frmMain.Storage.MaxSyringePulses : _frmMain.Storage.MaxPOHPulses).ToString();
            }
            else
            {
                labelPumpMaxPulses.Text = (MC.SyringePumpBDetected ? _frmMain.Storage.MaxSyringeBPulses : _frmMain.Storage.MaxPOHBPulses).ToString();
            }

            buttonSafetyZone2Start.Text = MS.Zone2Start.ToString("#0.000");
            buttonSafetyZone3Start.Text = MS.Zone3Start.ToString("#0.000");
            buttonSafetyZone4Start.Text = MS.Zone4Start.ToString("#0.000");
            buttonSafetyZone5Start.Text = MS.Zone5Start.ToString("#0.000");
            buttonSafetyZone1MaxZ.Text = MS.Zone1MaxZ.ToString("#0.000");
            buttonSafetyZone2MaxZ.Text = MS.Zone2MaxZ.ToString("#0.000");
            buttonSafetyZone3MaxZ.Text = MS.Zone3MaxZ.ToString("#0.000");
            buttonSafetyZone4MaxZ.Text = MS.Zone4MaxZ.ToString("#0.000");
            buttonSafetyZone5MaxZ.Text = MS.Zone5MaxZ.ToString("#0.000");

            buttonLevelingTolerance.Text = MS.LevelingTolerance.ToString("#0");
            buttonLevelingPercentMove.Text = MS.LevelingMovePercentage.ToString();
            buttonLevelingAttempts.Text = MS.MaxLevelingRetries.ToString();

            if (!MS.HasDiePressureTransducer)
            {
                tabControlConfig.TabPages.Remove(tabPageConfigDiePressure);
            }

            if (MS.AirKnifeInstalled && MS.AirKnifeHeaterInstalled)
            {
                buttonAirKnifeHeaterSetpoint.Text = MS.AirKnifeHeaterSetPoint.ToString("##0.#");
                buttonGasHeaterSetpoint.Text = MS.GasHeaterSetPoint.ToString("##0.#");
                buttonAirKnifeHeaterWarmupTime.Text = MS.AirKnifeHeaterWarmup.ToString();
            }
        }

        #endregion

#endregion

        private void labelInputsDI7_Click(object sender, EventArgs e)
        {

        }

        private void cbLiftUpState_CheckedChanged(object sender, EventArgs e)
        {
            MC.LiftUpState = cbLiftUpState.Checked;
        }

        private void cbSubstrateState_CheckedChanged(object sender, EventArgs e)
        {
            MC.SubstrateState = cbSubstrateState.Checked;
        }

        private void cbLiftDownState_CheckedChanged(object sender, EventArgs e)
        {
            MC.LiftDownState = cbLiftDownState.Checked;
        }

        private void cbHomeState_CheckedChanged(object sender, EventArgs e)
        {
            MC.HomedState = cbHomeState.Checked;
        }
    }
}
