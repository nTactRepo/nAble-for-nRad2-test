using System;
using System.Windows.Forms;
using System.Diagnostics;
using nAble.Data;
using nTact.DataComm;
using System.Drawing;

namespace nAble
{
    public partial class FormJogTransfer : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private bool _bOneShot = false;
        public FormJogTransfer(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
            _log = log;

            InitializeComponent();
        }

        public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
            {
                _bOneShot = true;
                return;
            }
            if (_bOneShot)
            {
                labelChuckPos.Text = _frmMain.MS.ConvCoaterPosition.ToString("#0.000") + " mm";
                labelStackPos.Text = _frmMain.MS.ConvUnloadPosition.ToString("#0.000") + " mm";
                _bOneShot = false;
            }
            bool bConnected = (_frmMain.MC != null && _frmMain.MC.Connected);
            bool bAxisMoving = _frmMain.MC.Loader_Moving;
            buttonJogFwd.Enabled = bConnected && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonJogRev.Enabled = bConnected && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGotoChuckPos.Enabled = bConnected && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonGotoLoadPos.Enabled = bConnected && !bAxisMoving && (_frmMain.MS.BypassSafetyGuards || !_frmMain.MC.SafetyGuardsActive);
            buttonLiftPins.Text = !_frmMain.MC.LiftPinsUpRequest ? "Raise Pins" : "Lower Pins";
            buttonAligners.Text = !_frmMain.MC.AlignersUpRequest ? "Raise Aligners" : "Lower Aligners";
            labelPos.Text = _frmMain.MC.LoaderPos.ToString("#0.000") + " mm";

			labelSubstrate.Visible = _frmMain.MC.TransferSubstratePresent;
			labelInMotion.Visible = bAxisMoving;

			pictureBoxPinsUp.BackColor = _frmMain.MC.LiftPinsUpRequest ? Color.Lime : Color.Red;
			pictureBoxPinsDown.BackColor = _frmMain.MC.LiftPinsDownRequest || _frmMain.MC.LiftPinsAreDown ? Color.Lime : Color.Red;
			pictureBoxAlignersUp.BackColor = _frmMain.MC.AlignersUpRequest ? Color.Lime : Color.Red;
			pictureBoxAlignersDown.BackColor = !_frmMain.MC.AlignersUpRequest ? Color.Lime : Color.Red;
		}

		private void buttonJogFwd_Click(object sender, EventArgs e)
        {                                                          
            try
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed JogTransfer-Jog FWD button (Speed: {0} : Distance {1})", buttonJogSpeed.Text, buttonJogDistance.Text));
                _frmMain.LastClick = DateTime.Now;
                double dSpeed, dDistance, dLocation, dAllowed = 0;
                dSpeed = Convert.ToDouble(buttonJogSpeed.Text);
                dDistance = Convert.ToDouble(buttonJogDistance.Text);
                string strTemp = labelPos.Text.Replace(" mm", "");
                dLocation = double.Parse(strTemp);
				dLocation = Convert.ToDouble(labelPos.Text.Replace("mm", "").Trim());
				if (!_frmMain.MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "Transfer Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (_frmMain.MC.LiftPinsUpRequest && _frmMain.MC.TransferSubstratePresent)
				{
					if (DialogResult.Yes == nRadMessageBox.Show(this, "Transfer Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
					{
						_log.log(LogType.TRACE, Category.INFO, "User Confirmed lower lift pins operation", "Action");
						_frmMain.MC.LowerLiftPins();
						_frmMain.MC.LowerAligners();
					}
				}
				else if (_frmMain.MC.Zone1VacOK)
				{
					nRadMessageBox.Show(this, "Chuck Shows as loaded.  Please unload glass from chuck before continuing.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (dDistance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move!", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
				else if (!_frmMain.MC.CarriageIsClear)
				{
					nRadMessageBox.Show(this, "Loader Jog not possible! Carriage is not in a safe position.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (DistanceOK(true, dDistance, ref dAllowed)) //dDistance + dLocation <= _frmMain.MS.MaxXTravel)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
						_log.log(LogType.TRACE, Category.INFO, "User Confirmed conveyor move forward operation", "Action");
						if (_frmMain.MC.MainAirOK)
                        {
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Transfer Jog Fwd: {0} mm from {1}", dDistance, _frmMain.MC.LoaderPos));
                            try { _frmMain.MC.JogTransfer(dSpeed, dDistance + dLocation, false); }
                            catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                        }
                        else
                        {
                            nRadMessageBox.Show(this, "Main Air has failed. Jog Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
					else
					{
						_log.log(LogType.TRACE, Category.INFO, "User declined conveyor jog forward operation", "Action");
					}
				}
                else
                {
                    string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel.", dAllowed.ToString("#.###"));
                    nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception during Jog Button: " + exJog.ToString());
            }

        }

        private void buttonJogRev_Click(object sender, EventArgs e)
        {
            try
            {
                _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed JogTransfer-Jog REV button (Speed: {0} : Distance {1})", buttonJogSpeed.Text, buttonJogDistance.Text));
                _frmMain.LastClick = DateTime.Now;
                double dSpeed, dDistance, dLocation, dAllowed = 0;
                dSpeed = Convert.ToDouble(buttonJogSpeed.Text);
                dDistance = Convert.ToDouble(buttonJogDistance.Text);
                dLocation = Convert.ToDouble(labelPos.Text.Replace("mm", "").Trim());
                if (!_frmMain.MC.MainAirOK)
                {
                    nRadMessageBox.Show(this, "Transfer Jog not possible! Main Air not available.\r\n\r\nRestore Main air to enable Move.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (_frmMain.MC.LiftPinsUpRequest && _frmMain.MC.TransferSubstratePresent)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Transfer Jog not possible!\r\n\r\nLift Pins or Aligners show in UP position.\r\nLower the obstruction to enable move?", "Move Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                    {
						_log.log(LogType.TRACE, Category.INFO, "User Confirmed lower lift pins operation", "Action");
						_frmMain.MC.LowerLiftPins();
                        _frmMain.MC.LowerAligners();
                    }
                }
                else if (dDistance <= 0)
                {
                    nRadMessageBox.Show(this, "Please specify a distance to move!", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (DistanceOK(false, dDistance, ref dAllowed)) //(dDistance <= dLocation + 1)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
						_log.log(LogType.TRACE, Category.INFO, "User Confirmed conveyor jog reverse operation", "Action");
						if (_frmMain.MC.MainAirOK)
                        {
                            _log.log(LogType.TRACE, Category.INFO, string.Format("Transfer Jog Fwd: {0} mm from {1}", dDistance, _frmMain.MC.LoaderPos));
                            try { _frmMain.MC.JogTransfer(dSpeed, dLocation - dDistance, false); }
                            catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                        }
                        else
                        {
                            nRadMessageBox.Show(this, "Main Air has failed. Jog Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
					else
					{
						_log.log(LogType.TRACE, Category.INFO, "User declined conveyor jog reverse operation", "Action");
					}
				}
                else
                {
                    string sMsg = string.Format("Error: Machine specs. only allow\r\n{0} mm of additional travel.",dAllowed.ToString("#.###"));
                    nRadMessageBox.Show(this, sMsg, "Move Request Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception exJog)
            {
                _log.log(LogType.TRACE, Category.INFO, "Exception during Jog Button: " + exJog.ToString());
            }
        }

        private void buttonJogSpeed_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Transfer Jog Speed", this, buttonJogSpeed, "#0", 1, 100,"",false,false);
        }

        private void buttonJogDistance_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Transfer Jog Distance", this, buttonJogDistance, "#0.000", 0.001, _frmMain.MS.MaxLoaderTravel);
        }

        private bool DistanceOK(bool bIsFwd, double dDistance, ref double dAllowed)
        {
            bool bRetVal = false;
            double dLoc = _frmMain.MC.LoaderPos;
            MachineSettingsII ms = _frmMain.MS;
            if (bIsFwd)
            {
                bRetVal = (dDistance + dLoc <= _frmMain.MS.MaxLoaderTravel);
                dAllowed = _frmMain.MS.MaxLoaderTravel - dLoc;
            }
            else
            {
                bRetVal = (dDistance <= dLoc + 1);
                dAllowed = dLoc + 1;
            }
            return bRetVal;
        }

        private void buttonTeachChuckPos_Click(object sender, EventArgs e)
        {
            double dPos = _frmMain.MC.LoaderPos;
			string prevVal = labelChuckPos.Text;
			labelChuckPos.Text = dPos.ToString("#0.000") + " mm";
            _frmMain.MS.ConvCoaterPosition= _frmMain.MC.LoaderPos;
            _frmMain.MS.Save();
            _frmMain.MC.DownloadTransferSettings();
			_log.log(LogType.TRACE, Category.INFO, $"User Taught new Chuck Position @ {labelChuckPos.Text}.  Previous Val: {prevVal}", "Action");

		}

        private void buttonTeachLoadPos_Click(object sender, EventArgs e)
        {
            double dPos = _frmMain.MC.LoaderPos;
			string prevVal = labelStackPos.Text;
			labelStackPos.Text = dPos.ToString("#0.000") + " mm";
            _frmMain.MS.ConvUnloadPosition = _frmMain.MC.LoaderPos;
            _frmMain.MS.Save();
            _frmMain.MC.DownloadTransferSettings();
			_log.log(LogType.TRACE, Category.INFO, $"User Taught new (un)Load Position @ {labelStackPos.Text}.  Previous Val: {prevVal}", "Action");
		}

		private void buttonGotoLoadPos_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, "User Pressed JogTransfer-'Goto Load Pos' button.", "Action");
			if (_frmMain.MC.LoaderPos.ToString("#0.000") == _frmMain.MS.ConvUnloadPosition.ToString("#0.000"))
                return;
            if (!_frmMain.MC.LiftPinsAreDown)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Lift Pins Not Down! Lower Lift Pins?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    if (_frmMain.MC.MainAirOK)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "Request Lower Lift Pins", "INFO");
                        try
                        {
                            _frmMain.MC.LowerLiftPins();
                        }
                        catch (Exception ex)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Exception during Lower Lift Pins: " + ex.ToString());
                            return;
                        }
                    }
                    else
                    {
                        nRadMessageBox.Show(this, "Main Air has failed. Lower Lift Pins Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                else
                {
                    if (DialogResult.No == nRadMessageBox.Show(this, "Continue With Lift Pins Up?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        return;
                    }
                }
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move loader to (Un)Load Position?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (_frmMain.MC.MainAirOK)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Request Transfer To Stack", "INFO");
                    try { _frmMain.MC.MoveConeyorToStack(); }
                    catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Transfer To Stack: " + ex.ToString()); }
                }
                else
                {
                    nRadMessageBox.Show(this, "Main Air has failed. Transfer To Stack Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void buttonGotoChuckPos_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, "User Pressed JogTransfer-'Goto Chuck' button.", "Action");
			if (!_frmMain.MC.LiftPinsUpRequest && !_frmMain.MC.TransferSubstratePresent)
			{
				if (DialogResult.Yes == nRadMessageBox.Show(this, "Lift Pins Not Up! Raise Lift Pins?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					_log.log(LogType.TRACE, Category.INFO, "User Confirmed Request Raise Lift Pins", "Action");
					if (_frmMain.MC.MainAirOK)
					{
						try
						{
							_frmMain.MC.RaiseLiftPins();
						}
						catch (Exception ex)
						{
							_log.log(LogType.TRACE, Category.INFO, "Exception during Raise Lift Pins: " + ex.ToString());
							return;
						}
					}
					else
					{
						nRadMessageBox.Show(this, "Main Air has failed. Raise Lift Pins Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}
				else
				{
					_log.log(LogType.TRACE, Category.INFO, "User Declined Request Raise Lift Pins", "Action");
					if (DialogResult.No == nRadMessageBox.Show(this, "Continue With Lift Pins Down?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						_log.log(LogType.TRACE, Category.INFO, "User Declined Continue With Lift Pins Down  - Goto Chuck Action cancelled.", "Action");
						return;
					}
				}
			}
			else if (_frmMain.MC.LiftPinsUpRequest && _frmMain.MC.TransferSubstratePresent)
			{
				if (DialogResult.Yes == nRadMessageBox.Show(this, "Lift Pins Are Up and Glass is on Loader\nLower Lift Pins?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					if (_frmMain.MC.MainAirOK)
					{
						_log.log(LogType.TRACE, Category.INFO, "User Confirmed Request Lower Lift Pins", "Action");
						try
						{
							_frmMain.MC.LowerLiftPins();
						}
						catch (Exception ex)
						{
							_log.log(LogType.TRACE, Category.INFO, "Exception during Lower Lift Pins: " + ex.ToString());
							return;
						}
					}
					else
					{
						nRadMessageBox.Show(this, "Main Air has failed. Lower Lift Pins Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						return;
					}
				}
				else
				{
					_log.log(LogType.TRACE, Category.INFO, "User Declined Continue lower Lift Pins - Goto Chuck Action cancelled.", "Action");
					return;
				}
			}

			if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to move loader to Chuck Position?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
				_log.log(LogType.TRACE, Category.INFO, "User Confirmed Ready to move Loader to chuck position", "Action");
				if (_frmMain.MC.MainAirOK)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Request Transfer To Chuck", "INFO");
                    try { _frmMain.MC.MoveConveyorToChuck(); }
                    catch (Exception ex) { _log.log(LogType.TRACE, Category.INFO, "Exception during Jog: " + ex.ToString()); }
                }
                else
                {
                    nRadMessageBox.Show(this, "Main Air has failed. Transfer To Chuck Request Cancelled.", "Move Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
			else
			{
				_log.log(LogType.TRACE, Category.INFO, "User Declined Ready to move Loader", "Action");
			}
		}

        private void buttonLiftPins_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Pressed JogTransfer-'{buttonLiftPins.Text}' button.", "Action");
			if (!_frmMain.MC.LiftPinsUpRequest)
            {
                if (_frmMain.MC.Zone1VacEngaged || _frmMain.MC.Zone2VacEngaged || _frmMain.MC.Zone3VacEngaged)
                {
					if (DialogResult.Yes == nRadMessageBox.Show(this, "Chuck Vacuum Is On! Turn Off?", "Motion Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						_log.log(LogType.TRACE, Category.INFO, $"User Confirmed disable chuck vac.", "Action");
						_frmMain.MC.ReleaseVacuum(Zones.AllChuck);
					}
					else
					{
						_log.log(LogType.TRACE, Category.INFO, $"User Declined disable chuck vac.", "Action");
						return;
					}
                }
                _frmMain.MC.RaiseLiftPins();
            }
            else
                _frmMain.MC.LowerLiftPins();
        }

        private void buttonAligners_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Pressed JogTransfer-'{buttonAligners.Text}' button.", "Action");
			if (!_frmMain.MC.AlignersUpRequest)
                _frmMain.MC.RaiseAligners();
            else
                _frmMain.MC.LowerAligners();
        }
    }
}
