using nAble.Data;
using nAble.DataComm.KeyenceLasers;
using nAble.Model;
using nTact.DataComm;

namespace nAble
{
    public partial class FormChuck : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private Color _colorControl = Color.FromName("Control");
		private bool _loading = false;
        private bool _applyingSetpoint = false;

        private GalilWrapper2 MC => _frmMain.MC;
		private MachineSettingsII MS => _frmMain.MS;
        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;

		public FormChuck(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ??  throw new ArgumentNullException(nameof(log));

            InitializeComponent();

            if (!MS.DualZoneLiftPinsEnabled)
            {
                tabControlLoadUnload.TabPages.Remove(tabPagePrimingChuck);
            }
        }

        private void FormChuck_Load(object sender, EventArgs e)
        {
			_loading = true;

            panelChuckTempController.Visible = MS.ChuckTempControlEnabled;
			buttonChuckTempSetPoint.Text = MS.ChuckTempControlSetPoint.ToString("0.0");
			groupBoxSelectiveZones.Visible = MS.UsesSelectiveAirVacZones;
			
            checkBoxSelectiveZone1.Checked = MS.SelectiveZone1Enabled;
			checkBoxSelectiveZone2.Checked = MS.NumChuckAirVacZones > 1 && MS.SelectiveZone2Enabled;
			checkBoxSelectiveZone3.Checked = MS.NumChuckAirVacZones > 2 && MS.SelectiveZone3Enabled;
            
            _log.log(LogType.TRACE, Category.ACTION, "User Has Entered FormChuck...", "Action");
            
            _loading = false;
        }

		public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            bool connected = (MC != null && MC.Connected && MC.IsHomed);
            bool moving = MC.Moving;

			// top row buttons...  Load/Unload, aligners, lift 'pins'
			buttonPrepareToLoad.Visible = MS.HasLoader || MS.AlignersEnabled;
            buttonPrepareToLoad.Enabled = connected && !moving;
			buttonLoad.Visible = MS.HasLoader || MS.AlignersEnabled;
            buttonLoad.Enabled = connected && !moving;
			buttonUnload.Visible = MS.HasLoader || MS.AlignersEnabled;
            buttonUnload.Enabled = connected && !moving;
			buttonAligners.Visible = MS.AlignersEnabled;
			buttonAligners.Text = MC.AlignersUpRequest ? "Lower Aligners" : "Raise Aligners";
			buttonAligners.Enabled = connected && !moving;
			buttonLiftPins.Visible = MS.LiftPinsEnabled;

            if (MC.ChuckLiftPinsStatus == LiftPinStatus.Raising)
            {
                buttonLiftPins.Text = "Raising Lift Pins";
            }
            else if (MC.ChuckLiftPinsStatus == LiftPinStatus.Lowering)
            {
                buttonLiftPins.Text = "Lowering Lift Pins";
            }
            else
            {
                buttonLiftPins.Text = MC.LiftPinsUpRequest ? "Lower Lift Pins" : "Raise Lift Pins";
            }

            buttonLiftPins.Enabled = connected && !moving;

            if (MS.DualZoneLiftPinsEnabled)
            {
                buttonPrepareToLoadPriming.Enabled = connected && !moving && MC.PrimingLiftPinsStatus==LiftPinStatus.Idle;
                buttonLoadPriming.Enabled = connected && !moving && MC.PrimingLiftPinsStatus == LiftPinStatus.Idle;
                buttonUnloadPriming.Enabled = connected && !moving && MC.PrimingLiftPinsStatus == LiftPinStatus.Idle;
                buttonLiftPinsPriming.Enabled = connected && !moving && MC.PrimingLiftPinsStatus == LiftPinStatus.Idle;

                if (MC.PrimingLiftPinsStatus == LiftPinStatus.Raising)
                {
                    buttonLiftPinsPriming.Text = "Raising Lift Pins";
                }
                else if (MC.PrimingLiftPinsStatus == LiftPinStatus.Lowering)
                {
                    buttonLiftPinsPriming.Text = "Lowering Lift Pins";
                }
                else
                {
                    buttonLiftPinsPriming.Text =  MC.PrimingLiftPinsUpRequest ? "Lower Lift Pins" : "Raise Lift Pins";
                }

                buttonLiftPinsPriming.Enabled = connected && !moving;
            }

            panelKeyenceLasers.Visible = MS.UsingKeyenceLaser;

            if (MS.UsingKeyenceLaser && LaserMgr != null)
            {
                bool valid = LaserMgr.GetLeftAndRightLaserData(out var left, out var right);
                labelLeftLaser.Text = valid ? left.ValueAsString : "-FFFFFF";

                if (MS.NumberOfLasers > 1)
                {
                    labelRightLaser.Visible = true;
                    label4.Visible = true;

                    labelRightLaser.Text = valid ? right.ValueAsString : "-FFFFFF";
                }
                else
                {
                    label4.Visible = false;
                    labelRightLaser.Visible = false;
                }

                var progNo = LaserMgr.ProgramNumber;
                labelCurProgram.Text = LaserMgr.Connected ? $"Current Program: {progNo}" : "Current Program: -";
            }

            // middle row - Vac Zones
            groupBoxSelectiveZones.Visible = MS.UsesSelectiveAirVacZones;
			groupBoxSelectiveZones.Enabled = connected;
			groupBoxSelectiveZones.BackColor = MS.SelectiveZones != 0 ? Color.Transparent : Color.Red;

			checkBoxSelectiveZone1.BackColor = checkBoxSelectiveZone1.Checked ? Color.LightSeaGreen : Color.Gray;
			checkBoxSelectiveZone2.BackColor = MS.NumChuckAirVacZones < 2 ? Color.DarkGray : checkBoxSelectiveZone2.Checked ? Color.LightSeaGreen : Color.Gray;
            checkBoxSelectiveZone3.BackColor = MS.NumChuckAirVacZones < 3 ? Color.DarkGray : checkBoxSelectiveZone3.Checked ? Color.LightSeaGreen : Color.Gray;
            
            checkBoxSelectiveZone2.Enabled = MS.NumChuckAirVacZones > 1;
			checkBoxSelectiveZone3.Enabled = MS.NumChuckAirVacZones > 2;

            if (MS.HasAnalogInputs)
            {
                labelMainVacStatus.Text = $"Main\rVac.\r{MC.MainVac:0.0} {_frmMain.VacUnits}";
                labelMainVacStatus.BackColor = MC.MainVacOK ? Color.Lime : Color.Red;
                labelMainVacStatus.ForeColor = MC.MainVacOK ? Color.Black : Color.White;
            }
            else
            {
                labelMainVacStatus.Text = "Main\rVacuum";
                labelMainVacStatus.BackColor = MC.MainVacOK ? Color.Lime : Color.Red;
                labelMainVacStatus.ForeColor = MC.MainVacOK ? Color.Black : Color.White;
            }

            panelPrimingStatuspanelPrimingStatus.Enabled = MS.HasPrimingPlate;
			buttonPrimingVacuum.Enabled = connected && !moving;
			buttonPrimingVacuum.BackColor = !MS.HasPrimingPlate || !MC.PrimingVacEngaged ? _colorControl :  MC.PrimingVacOK ? Color.Lime : Color.Red;
			buttonPrimingVacuum.Text = !MS.HasPrimingPlate ? "Disabled" : MC.PrimingVacEngaged ? "Turn Vacuum Off" : "Turn Vacuum On";
			buttonPrimingVacuum.UseVisualStyleBackColor = buttonPrimingVacuum.BackColor == _colorControl;

			panelZone1Status.Enabled = !MS.UsesSelectiveAirVacZones || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone1Enabled);
			buttonZone1Vacuum.Enabled = connected && !moving;
			buttonZone1Vacuum.BackColor = (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone1Enabled) || !MC.Zone1VacEngaged ? _colorControl : MC.Zone1VacOK ? Color.Lime : Color.Red;
			buttonZone1Vacuum.Text = (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone1Enabled) ? "Disabled" : MC.Zone1VacEngaged ? "Turn Vacuum Off" : "Turn Vacuum On";
			buttonZone1Vacuum.UseVisualStyleBackColor = buttonZone1Vacuum.BackColor == _colorControl;

			panelZone2Status.Enabled = checkBoxSelectiveZone2.Enabled && ((!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 1) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone2Enabled));
			buttonZone2Vacuum.Enabled = connected && !moving;
			buttonZone2Vacuum.BackColor = (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone2Enabled) || !MC.Zone2VacEngaged ? _colorControl : panelZone2Status.Enabled && MC.Zone2VacOK && MC.Zone2VacEngaged ? Color.Lime : Color.Red;
			buttonZone2Vacuum.Text = ((!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones < 2) || (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone2Enabled)) ? "Disabled" : MC.Zone2VacEngaged ? "Turn Vacuum Off" : "Turn Vacuum On";
			buttonZone2Vacuum.UseVisualStyleBackColor = buttonZone2Vacuum.BackColor == _colorControl;

			panelZone3Status.Enabled = checkBoxSelectiveZone3.Enabled && ((!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones > 2) || (MS.UsesSelectiveAirVacZones && MS.SelectiveZone3Enabled));
			buttonZone3Vacuum.Enabled = connected && !moving;
			buttonZone3Vacuum.BackColor = (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone3Enabled) || !MC.Zone3VacEngaged ? _colorControl : panelZone3Status.Enabled && MC.Zone3VacOK && MC.Zone3VacEngaged ? Color.Lime : Color.Red;
			buttonZone3Vacuum.Text = ((!MS.UsesSelectiveAirVacZones && MS.NumChuckAirVacZones < 3) || (MS.UsesSelectiveAirVacZones && !MS.SelectiveZone3Enabled)) ? "Disabled" : MC.Zone3VacEngaged ? "Turn Vacuum Off" : "Turn Vacuum On";
            buttonZone3Vacuum.UseVisualStyleBackColor = buttonZone3Vacuum.BackColor == _colorControl;

			// bottom 'row' - heater stuffs
            if (MS.ChuckTempControlEnabled)
            {
                labelChuckTempOutput.Text = Heaters.Temp(MS.ChuckCOMID);
                labelChuckTempOutput.ForeColor = Heaters.TempColor(MS.ChuckCOMID);

                if (buttonChuckTempSetPoint.Text != $"{Heaters.SetPoint(MS.ChuckCOMID):0.0}")
                {
                    if (_applyingSetpoint)
                    {
                        buttonChuckSetPointApply.Enabled = false;
                        labelChuckSetPointChanges.Text = "Changes are being applied.";
                        labelChuckSetPointChanges.Visible = true;
                    }
                    else
                    {
                        buttonChuckSetPointApply.Enabled = true;
                        labelChuckSetPointChanges.Text = "Changes have not been applied.";
                        labelChuckSetPointChanges.Visible = true;
                    }
                }
                else
                {
                    _applyingSetpoint = false;
                    buttonChuckSetPointApply.Enabled = false;
                    labelChuckSetPointChanges.Visible = false;
                }
            }
        }

        private void buttonPrepareToLoad_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
			_log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Prepare To Load Button", "Action");

			if ((MC.Zone1VacEngaged && MC.Zone1VacOK) || (MC.Zone2VacEngaged && MC.Zone2VacOK) || (MC.Zone3VacEngaged && MC.Zone3VacOK))
			{
				nRadMessageBox.Show(this, "One or more zones show vacuum.  Please correct before Preparing to Load!", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}

			if (!MC.LiftPinsAreDown && !MS.LiftPinsDuringPrep2Load)
			{
				if (DialogResult.Yes == nRadMessageBox.Show(this, "Lift Pins are Up. Lower before Prepare to Load?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					_log.log(LogType.TRACE, Category.ACTION, "User Confirmed Lower Lift Pins", "Action");
					MC.LowerLiftPins();
				}
				else
				{
					_log.log(LogType.TRACE, Category.ACTION, "User Declined Lower Lift Pins", "Action");
				}
			}

			if (DialogResult.Yes == nRadMessageBox.Show(this, "Prepare to Load a Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				_log.log(LogType.TRACE, Category.ACTION, "User Confirmed Prepare To Load", "Action");
				MC.PrepareToLoadSusbtrate();
			}
			else
			{
				_log.log(LogType.TRACE, Category.ACTION, "User Declined Prepare To Load", "Action");
			}
		}

		private void buttonLoad_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
			_log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Load Button", "Action");

			if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Chuck Load operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Load operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!MC.MainVacOK)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Chuck Load operation cancelled due to no main vacuum.", "ERROR");
                nRadMessageBox.Show(this, "The Load operation failed.\r\nPlease check main vacuum status before continuing.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Load Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Load Request");
                MC.LoadSubstrate();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Load Request");
            }
        }

        private void buttonUnload_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
			_log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Unload Button", "Action");

			if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Chuck UnLoad operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Unload operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Unload Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Unload Request");
                MC.UnLoadSubstrate();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Unload Request");
            }
		}

		private void buttonLiftPins_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
			_log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Lift Pins Button", "Action");

			if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Chuck Lift Pins operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Lift Pins operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!MC.LiftPinsUpRequest)
            {
                if (MC.Zone1VacEngaged || MC.Zone2VacEngaged || MC.Zone3VacEngaged)
                {
                    nRadMessageBox.Show(this, "Lift pins can not be raised at this time.  One or more zones have vacuum acutated.\r\nRelease vacuum from all zones to continue.", "Invalid Opertion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Raise Lift Pins?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Raise Lift Pins");
                    MC.RaiseLiftPins();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Raise Lift Pins");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Lower Lift Pins?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Lower Lift Pins");
                    MC.LowerLiftPins();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Lower Lift Pins");
                }
            }
        }

        private void buttonAligners_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
			_log.log(LogType.TRACE, Category.INFO, "User Pressed Chuck-Aligners Button", "Action");

			if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.ACTION, "ERROR: Chuck Aligners operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Aligners operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!MC.AlignersUpRequest)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Raise Aligners?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Raise Aligners");
                    MC.RaiseAligners();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Raise Aligners");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Lower Aligners?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Lower Aligners");
                    MC.LowerAligners();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Lower Aligners");
                }
            }
        }

		private void buttonPrimingVacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Priming Vacuum Button", "Action");

            if (MC.PrimingVacEngaged)
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Release Vacuum", "Action");
                MC.ReleaseVacuum(Zones.Priming);
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Engage Vacuum", "Action");
                MC.EngageVacuum(Zones.Priming);
            }
        }

        private void buttonZone1Vacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Zone1 Vacuum Button", "Action");

            if (MC.Zone1VacEngaged)
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Release Vacuum", "Action");
                MC.ReleaseVacuum(Zones.Zone1);
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Engage Vacuum", "Action");
                MC.EngageVacuum(Zones.Zone1);
            }
        }

        private void buttonZone2Vacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Zone2 Vacuum Button", "Action");

            if (MC.Zone2VacEngaged)
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Release Vacuum", "Action");
                MC.ReleaseVacuum(Zones.Zone2);
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Engage Vacuum", "Action");
                MC.EngageVacuum(Zones.Zone2);
            }
        }

        private void buttonZone3Vacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Chuck-Zone3 Vacuum Button", "Action");

            if (MC.Zone3VacEngaged)
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Release Vacuum", "Action");
                MC.ReleaseVacuum(Zones.Zone3);
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "Action is Engage Vacuum", "Action");
                MC.EngageVacuum(Zones.Zone3);
            }
        }

        private void buttonChuckTempSetPoint_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Chuck Temperature Controller Setpoint", this, buttonChuckTempSetPoint, "0.0", 15, MS.ChuckMaxTemp);
            _log.log(LogType.TRACE, Category.ACTION, $"User Changed Chuck Temp Set Point: {buttonChuckTempSetPoint.Text}", "Action");
        }

        private void buttonChuckSetPointApply_Click(object sender, EventArgs e)
        {
            double newSetPoint = double.Parse(buttonChuckTempSetPoint.Text);
            _log.log(LogType.TRACE, Category.ACTION, $"User Pressed Chuck-Apply New Setpoint Button:  {newSetPoint}", "Action");
			_frmMain.LastClick = DateTime.Now;
            _applyingSetpoint = true;
            Heaters.ChangeTempSetPoint(MS.ChuckCOMID, newSetPoint);
        }

		private void checkBoxSelectiveZone_CheckedChanged(object sender, EventArgs e)
		{
            if (_loading)
            {
                return;
            }

            string zones = (checkBoxSelectiveZone1.Checked ? "Zone 1 / " : " / ") + (checkBoxSelectiveZone1.Checked ? "Zone 1 / " : " / ") + (checkBoxSelectiveZone1.Checked ? "Zone 3" : "");
            MC.SetSelectiveZones(checkBoxSelectiveZone1.Checked, checkBoxSelectiveZone2.Checked, checkBoxSelectiveZone3.Checked);
            _log.log(LogType.TRACE, Category.ACTION, $"Selective Vacuum Zones Set: {zones}", "Action");

			if (!checkBoxSelectiveZone1.Checked && MC.Zone1VacEngaged)
            {
                MC.ReleaseVacuum(Zones.Zone1);
                _log.log(LogType.TRACE, Category.ACTION, "User Deselected Vacuum Zone 1 From Selected Zones", "Action");
            }
			if (!checkBoxSelectiveZone2.Checked && MC.Zone2VacEngaged)
            {
                MC.ReleaseVacuum(Zones.Zone2);
                _log.log(LogType.TRACE, Category.ACTION, "User Deselected Vacuum Zone 2 From Selected Zones", "Action");
            }
			if (!checkBoxSelectiveZone3.Checked && MC.Zone3VacEngaged)
            {
                MC.ReleaseVacuum(Zones.Zone3);
                _log.log(LogType.TRACE, Category.ACTION, "User Deselected Vacuum Zone 3 From Selected Zones", "Action");
            }
		}

        private void buttonPrepareToLoadPriming_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Priming-Prepare To Load Button", "Action");

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Prepare to Load a Priming Chuck?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Priming-Prepare To Load", "Action");
                MC.PrepareToLoadPriming();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Declined Priming-Prepare To Load", "Action");
            }
        }

        private void buttonLoadPriming_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Priming-Load Button", "Action");

            if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Priming Load operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Priming Load operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!MC.MainVacOK)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Priming Load operation cancelled due to no main vacuum.", "ERROR");
                nRadMessageBox.Show(this, "The Priming Load operation failed.\r\nPlease check main vacuum status before continuing.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Load Priming Chuck?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Priming Load Request");
                MC.LoadPriming();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Priming Load Request");
            }
        }

        private void buttonUnloadPriming_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Priming-Unload Button", "Action");

            if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR: Priming UnLoad operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Priming Unload operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Unload Priming Chuck?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Priming Unload Request");
                MC.UnLoadPriming();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Priming Unload Request");
            }
        }

        private void buttonLiftPinsPriming_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.ACTION, "User Pressed Priming-Lift Pins Button", "Action");

            if (!MC.MainAirOK || !MC.MainAirValveOpen)
            {
                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Priming Lift Pins operation cancelled due to low air pressure.", "ERROR");
                nRadMessageBox.Show(this, "The Priming Lift Pins operation failed.\r\nPlease enable Air before retry.", "Operation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!MC.PrimingLiftPinsUpRequest)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Raise Priming Lift Pins?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Raise Priming Lift Pins");
                    MC.RaisePrimingLiftPins();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Raise Priming Lift Pins");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Lower Priming Lift Pins?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Confirmed Lower Priming Lift Pins");
                    MC.LowerPrimingLiftPins();
                }
                else
                {
                    _log.log(LogType.TRACE, Category.ACTION, "User Cancelled Lower Priming Lift Pins");
                }
            }
        }
    }
}
