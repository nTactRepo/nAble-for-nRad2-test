using nAble.Data;
using nAble.DataComm.KeyenceLasers;
using nAble.Model.Enums;
using nTact.DataComm;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormJogZ : Form, IUpdateableForm
    {
        #region Properties

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;
        private bool CalRunning => _bgCalibrate.IsBusy;

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private Color _controlColor = Color.FromName("Control");
        private bool _calibrating = false;
        private bool _measuring = false;
        private bool _updating = false;

        private BackgroundWorker _bgCalibrate = null;
        private ConcurrentQueue<string> _msgs = new ConcurrentQueue<string>();

        #endregion

        #region Functions

        #region Constructors

        public FormJogZ(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            InitializeComponent();
            InitializeBackgroundWorker();

            MC.CalibrationMessage += msg => _msgs.Enqueue(msg);
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            if (_msgs.TryDequeue(out string msg))
            {
                nRadMessageBox.Show(this, msg, "Die Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information, MS.UsingKeyenceLaser);
                MC.ContinueCalibration();
            }

            bool connected = (MC != null && MC.Connected);
            bool notMoving = !MC.Moving || (MC.Moving && MC.PumpPrimeRunning);
            bool axisMoving = MC.XMoving || MC.ZMoving;
            bool safetyOK = ((MS.BypassSafetyGuards || !MC.SafetyGuardsActive) || !MC.SafetyGuardsActive) || (MC.SafetyGuardsActive && checkBoxJogSetupMode.Checked);

            buttonAbortJog.Visible = connected && MC.JogZActive;

            checkBoxJogSetupMode.Visible = MC.SafetyGuardsActive;

            labelRightInMotion.Visible = MC.ZRightMoving;
            labelLeftInMotion.Visible = MC.ZLeftMoving;

            if (MC.ZLeftMoving)
            {
                labelLZPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrLZ / MC.mmConv):#0.000}";
            }
            else
            {
                labelLZPosError.Text = $"Max Err: {Math.Abs(MC.LZMaxPosErr):#0.000}";
            }

            if (MC.ZRightMoving)
            {
                labelRZPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrRZ / MC.mmConv):#0.000}";
            }
            else
            {
                labelRZPosError.Text = $"Max Err: {Math.Abs(MC.RZMaxPosErr):#0.000}";
            }

            if (CalRunning)
            {
                buttonCalibrate.Enabled = true;
                buttonCalibrate.BackColor = Color.Yellow;
                buttonCalibrate.Text = "Cancel Calib.";
            }
            else
            {
                buttonCalibrate.Enabled = connected && notMoving && (MS.BypassSafetyGuards || !MC.SafetyGuardsActive);
                buttonCalibrate.BackColor = _controlColor;
                buttonCalibrate.Text = "Calibrate";
            }

            buttonJogZDownBoth.Enabled = connected && notMoving && !axisMoving && safetyOK;
            buttonJogZDownLeft.Enabled = connected && notMoving && !axisMoving && safetyOK;
            buttonJogZDownRight.Enabled = connected && notMoving && !axisMoving && safetyOK;
            buttonJogZUpBoth.Enabled = connected && notMoving && !axisMoving && safetyOK;
            buttonJogZUpLeft.Enabled = connected && notMoving && !axisMoving && safetyOK;
            buttonJogZUpRight.Enabled = connected && notMoving && !axisMoving && safetyOK;

            labelZPosRight.Text = $"{MC.ZRPos:00.000} mm";
            labelZPosLeft.Text = $"{MC.ZLPos:00.000} mm";

            buttonJogZSetMaxZ.BackColor = MC.IsPrimingMaxZSet ? _controlColor : Color.Yellow;

            labelJogLZTorqueVal.Text = $"{MC.TorqueLZ:#0.00}";
            labelJogRZTorqueVal.Text = $"{MC.TorqueRZ:#0.00}";

            //Keyence Laser Stuff
            buttonMeasure.Enabled = connected && !_calibrating && (notMoving || _measuring);
            buttonMeasure.BackColor = MC.GT2sExtended ? Color.Yellow : _controlColor;
            buttonMeasure.Text = _measuring ? "Retract" : "Measure Height";
            buttonMeasure.Visible = !MS.UsingKeyenceLaser;

            bool lasers = MS.UsingKeyenceLaser && LaserMgr != null;
            labelKeyenceProgramNum.Visible = lasers;

            if (lasers)
            {
                comboBoxKeyenceProgramNumber.Visible = MS.UsingKeyenceLaser;
                comboBoxKeyenceProgramNumber.Enabled = !LaserMgr.WaitingForProgramNumber;

                if (!comboBoxKeyenceProgramNumber.DroppedDown)
                {
                    comboBoxKeyenceProgramNumber.SelectedIndex = LaserMgr.ProgramNumber;
                }
            }
            else
            {
                comboBoxKeyenceProgramNumber.Visible = false;
                comboBoxKeyenceProgramNumber.Enabled = false;
            }

            labelLeftGT2Val.Visible = MS.UsingKeyenceLaser && MC.CalibratingDie;
            labelRightGT2Val.Visible = MS.UsingKeyenceLaser && MC.CalibratingDie;

            if (lasers)
            {
                bool valid = LaserMgr.GetLeftAndRightLaserData(out var left, out var right);
                labelLeftMeasureVal.Text = valid ? left.ValueAsString : "-FFFFFF";
                labelRightMeasureVal.Text = valid ? right.ValueAsString : "-FFFFFF";

                labelLeftGT2Val.Text = MC.LeftGT2Pos.ToString("#0.0000");
                labelRightGT2Val.Text = MC.RightGT2Pos.ToString("#0.0000");
            }
            else
            {
                labelLeftMeasureVal.Text = $"{_frmMain.KeyenceLeftVal:0.000}";
                labelRightMeasureVal.Text =$"{_frmMain.KeyenceRightVal:0.000}";
            }

            if (MC.CalibratingDie)
            {
                labelLevelAttempt.Visible = true;
                labelLevelAttempt.Text = $"Level Attempt:{MC.LevelAttempt}";
            }
            else
                labelLevelAttempt.Visible=false;
        }

        #endregion

        #region Calibrate Thread

        private void InitializeBackgroundWorker()
        {
            _bgCalibrate = new BackgroundWorker() { WorkerSupportsCancellation = true };
            _bgCalibrate.DoWork += new DoWorkEventHandler(DoCalibrate);
            _bgCalibrate.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);
        }

        private void StartCalibrate()
        {
            if (!_calibrating && !CalRunning)
            {
                _calibrating = true;
                _bgCalibrate.RunWorkerAsync();
            }
            else
            {
                nRadMessageBox.Show(this, $"", $"", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DoCalibrate(object sender, DoWorkEventArgs e)
        {
            MC.SimpleDieCalibration(() => _bgCalibrate.CancellationPending);

            while (MC.CalibrationPaused) { Thread.Sleep(200); }
        }

        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _calibrating = false;
            _log.log(LogType.TRACE, Category.INFO, $"Calibration background worker has completed.");
        }

        #endregion

        #region Private Functions

        private bool LimitsOK(JogDirection dir, string Axis)  //TODO  This should really be part of the wrapper
        {
            bool retVal = true;
            switch (dir)
            {
                case JogDirection.Forward:
                    switch (Axis)
                    {
                        case "Z":
                            if (MC.ZLFLS == 0 || MC.ZRFLS == 0)
                                retVal = false;
                            break;
                        case "RZ":
                            if (MC.ZRFLS == 0)
                                retVal = false;
                            break;
                        case "LZ":
                            if (MC.ZLFLS == 0)
                                retVal = false;
                            break;
                    }
                    break;
                case JogDirection.Reverse:
                    switch (Axis)
                    {
                        case "Z":
                            if (MC.ZLRLS == 0 || MC.ZRRLS == 0)
                                retVal = false;
                            break;
                        case "RZ":
                            if (MC.ZRRLS == 0)
                                retVal = false;
                            break;
                        case "LZ":
                            if (MC.ZLRLS == 0)
                                retVal = false;
                            break;
                    }
                    break;
            }

            if (!retVal)
                nRadMessageBox.Show(this, "Jog Not Possible! Limit Switch Reached!", "Jog Z", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return retVal;
        }

        private void StartMeasuring()
        {
            MC.ExtendGT2s();
            _frmMain.StartGetHeight();
            _measuring = true;
        }

        private void StopMeasuring()
        {
            MC.RetractGT2s();
            _frmMain.StopGetHeight();
            _measuring = false;
        }

        #endregion

        #region Event Handlers

        private void buttonJogZUpLeft_Click(object sender, EventArgs e)
        {
            if (!LimitsOK(JogDirection.Reverse, "LZ"))
            {
                return;
            }

            _frmMain.LastClick = DateTime.Now;
            double dSpeed, dDistance, dTemp, dLocation;
            dSpeed = Convert.ToDouble(buttonZJogSpeedLeft.Text);
            dDistance = Convert.ToDouble(buttonZJogDistanceLeft.Text);
            dLocation = Convert.ToDouble(labelZPosLeft.Text.Substring(0, 6));
            if (!MC.MainAirOK)
            {
                nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    MC.LowerLiftPins();
                    MC.LowerAligners();
                }
            }
            else if (dDistance <= 0)
            {
                nRadMessageBox.Show(this, "Please specify a distance to move", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (dDistance <= dLocation)
            {
                if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                {
                    nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, string.Format("Left Z Jog Up: {0} mm from {1}", dDistance, dLocation));
                    try
                    {
                        //_galilWrapper.StopMotionOverride = checkBoxJogSetupMode.Checked;
                        try { MC.JogZLeft(dSpeed, dLocation - dDistance, false); }
                        catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                    }
                    catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                }
            }
            else
            {
                dTemp = dLocation;
                string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel.", dTemp.ToString("#.###"));
                nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonJogZDownLeft_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LimitsOK(JogDirection.Forward, "LZ"))
                {
                    return;
                }
                _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed Jog FWD button (Speed: {0} : Distance {1})", buttonZJogSpeedLeft.Text, buttonZJogDistanceLeft.Text));
                _frmMain.LastClick = DateTime.Now;
                double dSpeed, dDistance, dTemp, dLocation;
                dSpeed = Convert.ToDouble(buttonZJogSpeedLeft.Text);
                dDistance = Convert.ToDouble(buttonZJogDistanceLeft.Text);
                dLocation = Convert.ToDouble(labelZPosLeft.Text.Substring(0, 6));
                double dMaxZ = MC.MaxZAtCurrentLoc(MC.XPos);

                if (!MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
                        MC.LowerLiftPins();
                        MC.LowerAligners();
                    }
                }
                else if (dDistance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move!", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (dDistance + dLocation <= dMaxZ)
                {
                    if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                    {
                        nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        if (MC.MainAirOK)
                        {
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Left Z Jog Down: {0} mm from {1}", dDistance, dLocation));
                            try { MC.JogZLeft(dSpeed, dDistance + dLocation, false); }
                            catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                        }
                        else
                        {
                            nRadMessageBox.Show(this, "Main Air has failed. Jog Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else
                {
                    dTemp = dMaxZ - dLocation;
                    string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel at this location.", dTemp.ToString("#.###"));
                    nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception during Jog Button: " + exJog.ToString());
            }
        }

        private void buttonZJogSpeedLeft_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z-Axis Jog Speed", this, buttonZJogSpeedLeft, "##", 1, 30);
        }

        private void buttonZJogDistanceLeft_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z-Axis Jog Distance", this, buttonZJogDistanceLeft, "0.000", 0.001, MS.MaxZTravel);
        }

        private void buttonJogZUpBoth_Click(object sender, EventArgs e)
        {
            if (!LimitsOK(JogDirection.Reverse, "Z"))
            {
                return;
            }
            _frmMain.LastClick = DateTime.Now;
            double dSpeed, dDistance, dTemp, dLocationLeft, dLocationRight;
            dSpeed = Convert.ToDouble(buttonZJogSpeedBoth.Text);
            dDistance = Convert.ToDouble(buttonZJogDistanceBoth.Text);
            dLocationLeft = Convert.ToDouble(labelZPosLeft.Text.Substring(0, 6));
            dLocationRight = Convert.ToDouble(labelZPosRight.Text.Substring(0, 6));
            if (!MC.MainAirOK)
            {
                nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    MC.LowerLiftPins();
                    MC.LowerAligners();
                }
            }
            else if (dDistance <= 0)
            {
                nRadMessageBox.Show(this, "Please specify a distance to move", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (dDistance <= dLocationLeft && dDistance <= dLocationRight)
            {
                if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                {
                    nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, string.Format("Both Z Jog Up: {0} mm from {1},{2}", dDistance, dLocationRight, dLocationLeft));
                    try
                    {
                        //_galilWrapper.StopMotionOverride = checkBoxJogSetupMode.Checked;
                        try { MC.JogZBoth(dSpeed, dLocationRight - dDistance, dLocationLeft - dDistance, false); }
                        catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                    }
                    catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                }
            }
            else
            {
                if (dLocationLeft > dLocationRight)
                    dTemp = dLocationLeft;
                else
                    dTemp = dLocationRight;
                string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel.", dTemp.ToString("#.###"));
                nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonJogZDownBoth_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LimitsOK(JogDirection.Forward, "Z"))
                {
                    return;
                }
                _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed Jog Z Both Down button (Speed: {0} : Distance {1})", buttonZJogSpeedBoth.Text, buttonZJogDistanceBoth.Text));
                _frmMain.LastClick = DateTime.Now;
                double dSpeed, dDistance, dTemp, dLocationLeft, dLocationRight;
                dSpeed = Convert.ToDouble(buttonZJogSpeedBoth.Text);
                dDistance = Convert.ToDouble(buttonZJogDistanceBoth.Text);
                dLocationLeft = Convert.ToDouble(labelZPosLeft.Text.Substring(0, 6));
                dLocationRight = Convert.ToDouble(labelZPosRight.Text.Substring(0, 6));

                double dMaxZ = MC.MaxZAtCurrentLoc(MC.XPos);

                if (!MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
                        MC.LowerLiftPins();
                        MC.LowerAligners();
                    }
                }
                else if (dDistance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (dDistance + dLocationLeft <= dMaxZ && dDistance + dLocationRight <= dMaxZ)
                {
                    if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                    {
                        nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        _log.log(LogType.TRACE, Category.INFO, string.Format("Both Z Jog Down: {0} mm from {1},{2}", dDistance, dLocationRight, dLocationLeft));
                        try
                        {
                            //_galilWrapper.StopMotionOverride = checkBoxJogSetupMode.Checked;
                            try { MC.JogZBoth(dSpeed, dLocationRight + dDistance, dLocationLeft + dDistance, false); }
                            catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                        }
                        catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                    }
                }
                else
                {
                    if (dDistance + dLocationLeft > dMaxZ)
                        dTemp = dMaxZ - dLocationLeft;
                    else
                        dTemp = dMaxZ - dLocationRight;
                    string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel at this location.", dTemp.ToString("#.###"));
                    nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception during Jog Button: " + exJog.ToString());
            }

        }

        private void buttonZJogSpeedBoth_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z-Axis Jog Speed", this, buttonZJogSpeedBoth, "##", 1, 30);
        }

        private void buttonZJogDistanceBoth_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Z-Axis Jog Distance", this, buttonZJogDistanceBoth, "0.000", 0.001, MS.MaxZTravel);
        }

        private void buttonJogZUpRight_Click(object sender, EventArgs e)
        {
            if (!LimitsOK(JogDirection.Reverse, "RZ"))
            {
                return;
            }
            _frmMain.LastClick = DateTime.Now;
            double dSpeed, dDistance, dTemp, dLocation;
            dSpeed = Convert.ToDouble(buttonZJogSpeedRight.Text);
            dDistance = Convert.ToDouble(buttonZJogDistanceRight.Text);
            dLocation = Convert.ToDouble(labelZPosRight.Text.Substring(0, 6));
            if (!MC.MainAirOK)
            {
                nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    MC.LowerLiftPins();
                    MC.LowerAligners();
                }
            }
            else if (dDistance <= 0)
            {
                nRadMessageBox.Show(this, "Please specify a distance to move", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (dDistance <= dLocation)
            {
                if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                {
                    nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, string.Format("Right Z Jog Up: {0} mm from {1}", dDistance, dLocation));
                    try
                    {
                        //_galilWrapper.StopMotionOverride = checkBoxJogSetupMode.Checked;
                        try { MC.JogZRight(dSpeed, dLocation - dDistance, false); }
                        catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                    }
                    catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                }
            }
            else
            {
                dTemp = dLocation;
                string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel.", dTemp.ToString("#.###"));
                nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonJogZDownRight_Click(object sender, EventArgs e)
        {
            try
            {
                if (!LimitsOK(JogDirection.Forward, "RZ"))
                {
                    return;
                }
                _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed Jog Z Right Down button (Speed: {0} : Distance {1})", buttonZJogSpeedRight.Text, buttonZJogDistanceRight.Text));
                _frmMain.LastClick = DateTime.Now;
                double dSpeed, dDistance, dTemp, dLocation;
                dSpeed = Convert.ToDouble(buttonZJogSpeedRight.Text);
                dDistance = Convert.ToDouble(buttonZJogDistanceRight.Text);
                dLocation = Convert.ToDouble(labelZPosRight.Text.Substring(0, 6));
                double dMaxZ = MC.MaxZAtCurrentLoc(MC.XPos);

                if (!MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "Z Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Z Axis Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
                        MC.LowerLiftPins();
                        MC.LowerAligners();
                    }
                }
                else if (dDistance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move!", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (dDistance + dLocation <= dMaxZ)
                {
                    if (checkBoxJogSetupMode.Checked && (dDistance > 1 || dSpeed > 10))
                    {
                        nRadMessageBox.Show(this, "Safety Guards currently limit speed and distance to 1mm @ 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        if (MC.MainAirOK)
                        {
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Right Z Down Right: {0} mm from {1}", dDistance, dLocation));
                            try { MC.JogZRight(dSpeed, dDistance + dLocation, false); }
                            catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                        }
                        else
                        {
                            nRadMessageBox.Show(this, "Main Air has failed. Jog Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else
                {
                    dTemp = dMaxZ - dLocation;
                    string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel at this location.", dTemp.ToString("#.###"));
                    nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception during Jog Button: " + exJog.ToString());
            }
        }

        private void buttonZJogSpeedRight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z-Axis Jog Speed", this, buttonZJogSpeedRight, "#", 1, 30);
        }

        private void buttonZJogDistanceRight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z-Axis Jog Distance", this, buttonZJogDistanceRight, "0.000", 0.001, MS.MaxZTravel);
        }

        private void buttonCalibrate_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (CalRunning)
            {
                _bgCalibrate.CancelAsync();
                return;
            }

            string msg = "Are You Ready to Calibrate? Please Make Sure That You Have Already Levelled The Die With " +
                "Shims Before Calibrating.  If You Have Not, Please Press No.";

            if (DialogResult.Yes == nRadMessageBox.Show(this, msg, "Calibration", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (_measuring)
                {
                    StopMeasuring();
                    Thread.Sleep(500);
                }

                StartCalibrate();
            }
        }

        private void buttonAbortJog_Click(object sender, EventArgs e)
        {
            MC.AbortJogZ();
            nRadMessageBox.Show(this, "Manual Z Jog Aborted at user request", "Motion Abort", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonJogZSetMaxZ_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (MS.MeasureZonePrimingStart > MC.XPos || MC.XPos > MS.MeasureZonePrimingEnd)
            {
                nRadMessageBox.Show(this, "The current X Position is not within the Priming Plate Measure Zone. Set Max Z Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Set Max Z Height for Prming Plate?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (MC.MainAirOK)
                {
                    MC.SetMaxZ(true, false);
                }
                else
                {
                    nRadMessageBox.Show(this, "Main Air has failed. Calibrate Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }

        private void FormJogZ_Load(object sender, EventArgs e)
        {
            _updating = true;

            comboBoxKeyenceProgramNumber.SelectedIndex = LaserMgr?.ProgramNumber ?? -1;

            _updating = false;
        }

        private void buttonLocations_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmSetup);
            _frmMain.frmSetup.LoadVisionTeachingPage(this);
        }

        private void buttonExtendGT2s_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (_measuring)
            {
                StopMeasuring();
            }
            else
            {
                StartMeasuring();
            }
        }

        private void buttonZeroGT2s_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (DialogResult.Yes == nRadMessageBox.Show(_frmMain, "Are you sure you want to Zero the GT2 Presets?", "Zero GT2s Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                MC.OpenKeyence();
                MC.PresetAll(0, 0);
                _log.log(LogType.TRACE, Category.INFO, "X Jog: User Pressed Zero GT2s button.", "Action");
            }
        }

        private void comboBoxKeyenceProgramNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxKeyenceProgramNumber.SelectedIndex < 0 || _updating || LaserMgr.WaitingForProgramNumber)
            {
                return;
            }

            if (!int.TryParse(comboBoxKeyenceProgramNumber.SelectedItem.ToString(), out var programNumber))
            {
                _log.log(LogType.TRACE, Category.DEBUG, $"Program Number change failed: {comboBoxKeyenceProgramNumber.SelectedItem}");
                return;
            }

            if (programNumber != LaserMgr.ProgramNumber)
            {
                bool numberSet = LaserMgr.SetProgramNumber(programNumber);
                comboBoxKeyenceProgramNumber.Enabled = !numberSet;
            }
        }

        private void labelRightGT2Val_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
