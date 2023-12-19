using System;
using System.Windows.Forms;
using System.Diagnostics;
using nAble.Data;
using System.Drawing;
using nTact.DataComm;
using nAble.DataComm.KeyenceLasers;
using nAble.Model.Enums;

namespace nAble
{
    public partial class FormJogX : Form, IUpdateableForm
    {
        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private bool _measuring = false;
        private Color _controlColor = Color.FromName("Control");
        private bool _leaveForNum = false;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;

        #endregion

        #region Functions

        #region Constructors

        public FormJogX(FormMain formMain, LogEntry log)
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

            bool connected = MC?.Connected ?? false;
            bool notMoving = !MC.Moving || (MC.Moving && MC.PumpPrimeRunning);
            bool axisMoving = MC.XMoving || MC.ZMoving;

            buttonMeasure.Visible = !MS.UsingKeyenceLaser;
            buttonMeasure.Enabled = connected && (notMoving || _measuring);
            buttonMeasure.BackColor = MC.GT2sExtended ? Color.Yellow : _controlColor;
            buttonMeasure.Text = _measuring ? "Retract" : "Measure Height";

            buttonZeroGT2s.Visible = !MS.UsingKeyenceLaser;

            labelXPos.Text = $"{MC.XPos:000.000} mm";

            buttonJogXFwd.Enabled = !_measuring && connected && notMoving && !axisMoving && (MS.BypassSafetyGuards || !MC.SafetyGuardsActive);
            buttonJogXRev.Enabled = !_measuring && connected && notMoving && !axisMoving && (MS.BypassSafetyGuards || !MC.SafetyGuardsActive);

            labelKeyenceLeft.Text = MS.UsingKeyenceLaser ? "Left Laser" : "Left GT2";
            labelKeyenceRight.Text = MS.UsingKeyenceLaser ? "Right Laser" : "Right GT2";

            if (MS.UsingKeyenceLaser && LaserMgr != null)
            {
                bool valid = LaserMgr.GetLeftAndRightLaserData(out var left, out var right);
                labelKeyenceLeftVal.Text = valid ? left.ValueAsString : "-FFFFFF";
                labelKeyenceRightVal.Text = valid ? right.ValueAsString : "-FFFFFF";
            }
            else
            {
                labelKeyenceLeftVal.Text = _measuring ? _frmMain.KeyenceLeftVal.ToString("0.000") : "------";
                labelKeyenceRightVal.Text = _measuring ? _frmMain.KeyenceRightVal.ToString("0.000") : "------";
            }

            //panelAirKnife.Visible = MS.AirKnifeInstalled;
            //buttonAirKnife.Enabled = bConnected && !MC.RunningRecipe;
            //buttonAirKnife.Text = MC.AirKnifeOn ? "Disable Air Knife" : "Enable Air Knife";
            //buttonAirKnife.BackColor = MC.AirKnifeOn ? Color.Yellow : _controlColor;
            buttonAbortJog.Visible = connected && MC.JogXActive;
            labelInMotion.Visible = axisMoving;
            labelJogXTorqueVal.Text = MC.TorqueX.ToString("#0.00");

            if (MC.XMoving)
            {
                labelXPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrX / MC.mmConv):#0.000}";
            }
            else
            {
                labelXPosError.Text = $"Max Err: {Math.Abs(MC.XMaxPosErr):#0.000}";
            }
        }

        #endregion

        #region Control Event Handlers

        private void FormJogX_Load(object sender, EventArgs e)
        {

        }

        private void buttonJogXFwd_Click(object sender, EventArgs e)
        {
            PerformJog(JogDirection.Forward);
        }

        private void buttonJogXRev_Click(object sender, EventArgs e)
        {
            PerformJog(JogDirection.Reverse);
        }

        private void buttonXJogSpeed_Click(object sender, EventArgs e)
        {
            _leaveForNum = true;
            _frmMain.GotoNumScreen("X-Axis Jog Speed", this, buttonXJogSpeed, "#.#", 1, 100);
        }

        private void buttonXJogDistance_Click(object sender, EventArgs e)
        {
            _leaveForNum = true;
            _frmMain.GotoNumScreen("X-Axis Jog Distance", this, buttonXJogDistance, "0.000", 0.001, MS.MaxXTravel);
        }

        private void buttonMeasure_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, $"X Jog: User requested Measure {(_measuring ? "Stop" : "Start")} .  Loc: {MC.XPos:0.000}", "Action");
            ToggleGetHeight();
        }

        private void FormJogX_Leave(object sender, EventArgs e)
        {
            if (!_leaveForNum)
            {
                if (_measuring)
                {
                    ToggleGetHeight();
                }

                //if (!MC.RunningRecipe && MC.AirKnifeOn)
                //  MC.ToggleAirKnife();
            }

            _leaveForNum = false;
        }

        private void FormJogX_Activated(object sender, EventArgs e)
        {
            _leaveForNum = false;
        }

        private void FormJogX_Enter(object sender, EventArgs e)
        {
            _leaveForNum = false;
        }

        private void FormJogX_Deactivate(object sender, EventArgs e)
        {
            _leaveForNum = false;
        }

        private void buttonAbortJog_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"X Jog: User Pressed Abort Jog Button.  Loc: {MC.XPos.ToString("0.000")}", "Action");
            MC.AbortJogX();
            nRadMessageBox.Show(this, "Manual X Jog Aborted at user request", "Motion Abort", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonAirKnife_Click(object sender, EventArgs e)
        {
            //_log.log(LogType.TRACE, Category.INFO, "X Jog: User Pressed Toggle Air Knife button.", "Action");
            //MC.ToggleAirKnife();
        }

        private void buttonLocations_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmSetup);
            _frmMain.frmSetup.LoadVisionTeachingPage(this);
        }

        private void buttonZeroGT2s_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (DialogResult.Yes == nRadMessageBox.Show(_frmMain, "Are you sure you want to Zero the GT2 Presets?", 
                "Zero GT2s Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                MC.OpenKeyence();
                MC.PresetAll(0, 0);
                _log.log(LogType.TRACE, Category.INFO, "X Jog: User Pressed Zero GT2s button.", "Action");
            }
        }

        #endregion

        #region Private Functions

        private void PerformJog(JogDirection direction)
        {
            try
            {
                bool isFwd = direction == JogDirection.Forward;
                string dirString = isFwd ? "Fwd" : "Rev";

                _log.log(LogType.TRACE, Category.INFO, $"User Pressed Jog {dirString} button (Speed: {buttonXJogSpeed.Text} : Distance {buttonXJogDistance.Text})");
                _frmMain.LastClick = DateTime.Now;

                double allowed = 0;
                var speed = Convert.ToDouble(buttonXJogSpeed.Text);
                var distance = Convert.ToDouble(buttonXJogDistance.Text);
                var location = Convert.ToDouble(labelXPos.Text.Substring(0, 7));
                var target = isFwd ? location + distance : location - distance;

                if (!MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "X Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", 
                        "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (MC.LiftPinsUpRequest || MC.AlignersUpRequest)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, 
                        "X Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", 
                        "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
                        MC.LowerLiftPins();
                        MC.LowerAligners();
                    }
                }
                else if (MS.HasLoader && !MC.LoaderIsClear)
                {
                    nRadMessageBox.Show(this, "X Jog not possible! Loader is not in a safe position.", 
                        "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (distance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move!", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (!MC.CanJogTo(target))
                {
                    nRadMessageBox.Show(this, "Current Die height prevents jogging that distance.  Raise the die to allow the operation. ", 
                        "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (DistanceOK(direction, distance, ref allowed))
                {
                    if (_measuring && speed > 10)
                    {
                        nRadMessageBox.Show(this, "'Measure Height' limits the speed to 10 mm/sec.\r\nPlease validate the requested move values and retry the operation", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        if (MC.MainAirOK)
                        {
                            _log.log(LogType.TRACE, Category.INFO, $"X Jog {dirString}: {distance} mm from {MC.XPos}");
                            
                            try
                            {
                                MC.JogX(speed, target, false);
                            }
                            catch (Exception ex)
                            {
                                _log.log(LogType.TRACE, Category.INFO, $"Exception during Jog: {ex}");
                            }
                        }
                        else
                        {
                            nRadMessageBox.Show(this, "Main Air has failed. Jog Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
                else
                {
                    string msg = $"Error: Machine specs. only allow\r\n{allowed:#.###} mm of additional travel.";
                    nRadMessageBox.Show(this, msg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Exception during Jog Button: {exJog}");
            }
        }

        private void ToggleGetHeight()
        {
            if (_measuring)
            {
                _measuring = false;
                _frmMain.StopGetHeight();

                // turn it off...
                if (MC.GT2sExtended)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Turning Off Extended GT2s", "DEBUG");
                    MC.RetractGT2s();
                }
            }
            else
            {
                // turn it on...
                if (!MC.GT2sExtended)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Turning on Extended GT2s", "DEBUG");
                    MC.ExtendGT2s();
                }

                _frmMain.StartGetHeight();
                _measuring = true;
            }
        }

        private bool DistanceOK(JogDirection direction, double distance, ref double allowed)
        {
            bool distanceIsOK = false;
            double dLoc = MC.XPos;
            MachineSettingsII ms = MS;

            if (direction == JogDirection.Forward)
            {
                if (_measuring)
                {
                    if (ms.MeasureZonePrimingStart <= dLoc && dLoc <= ms.MeasureZonePrimingEnd)
                    {
                        distanceIsOK = dLoc + distance <= MS.MeasureZonePrimingEnd;
                        allowed = MS.MeasureZonePrimingEnd - dLoc;
                    }
                    else if (ms.MeasureZoneChuckStart <= dLoc && dLoc <= ms.MeasureZoneChuckEnd)
                    {
                        distanceIsOK = dLoc + distance <= MS.MeasureZoneChuckEnd;
                        allowed = MS.MeasureZoneChuckEnd - dLoc;
                    }
                    else
                    {
                        allowed = 0;
                    }
                }
                else
                {
                    distanceIsOK = distance + dLoc <= MS.MaxXTravel;
                    allowed = MS.MaxXTravel - dLoc;
                }
            }
            else
            {
                if (_measuring)
                {
                    if (ms.MeasureZonePrimingStart <= dLoc && dLoc <= ms.MeasureZonePrimingEnd)
                    {
                        distanceIsOK = dLoc - distance >= MS.MeasureZonePrimingStart;
                        allowed = dLoc - MS.MeasureZonePrimingStart;
                    }
                    else if (ms.MeasureZoneChuckStart <= dLoc && dLoc <= ms.MeasureZoneChuckEnd)
                    {
                        distanceIsOK = dLoc - distance >= MS.MeasureZoneChuckStart;
                        allowed = dLoc - MS.MeasureZoneChuckStart;
                    }
                    else
                    {
                        allowed = 0;
                    }
                }
                else
                {
                    distanceIsOK = distance <= dLoc + 1;
                    allowed = dLoc + 1;
                }
            }

            return distanceIsOK;
        }

        #endregion

        #endregion
    }
}
