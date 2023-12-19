using System;
using System.Drawing;
using System.Windows.Forms;
using nTact.DataComm;
using nTact.PLC;
using nAble.Enums;
using System.Diagnostics;

namespace nAble
{
    public partial class FormProcessStackMaint : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private readonly LogEntry _log = null;

		private GalilWrapper2 MCPU { get { return _frmMain.MC; } }
		private PLCWrapper PLC { get { return _frmMain.PLC; } }
		private Color _controlColor = Color.FromName("Control");
		private bool bMaintEnabled = false;
        private bool bFormLoading = true;
        private bool bUpdateOneShot = false;

        public FormProcessStackMaint(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
			_log = log;

            InitializeComponent();
			SetupTabPages();
		}

		private void FormProcessStack_Load(object sender, EventArgs e)
        {
            bFormLoading = false;
		}

		private void SetupTabPages()
		{
			ConfigMod1Layout();
			ConfigMod2Layout();
			ConfigMod3Layout();
		}

		private void ConfigMod1Layout()
		{
			if (_frmMain.MS.StackMod1Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod1Type;
				labelMod1Name.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod1Name}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						buttonMod1AntiVacOn.Text = "Float";

					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageMod1);
			}

		}

		private void ConfigMod2Layout()
		{
			if (_frmMain.MS.StackMod2Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod2Type;
				labelMod2Name.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod2Name}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						buttonMod2AntiVacOn.Text = "Float";
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageMod2);
			}
		}

		private void ConfigMod3Layout()
		{
			if (_frmMain.MS.StackMod3Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod3Type;
				labelMod3Name.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod3Name}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageMod3);
			}
		}

		public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
            {
                bUpdateOneShot = true;
                return;
            }
            if(bUpdateOneShot)
            {
                labelMod1ChuckTempSP.Text = PLC.Mod1ChuckSetPoint.ToString("#0") + "°C";
                labelMod2ChuckTempSP.Text = PLC.Mod2ChuckSetPoint.ToString("#0") + "°C";
                bUpdateOneShot = false;
            }
            bool bConnected = (_frmMain.MC != null && _frmMain.MC.Connected && _frmMain.MC.IsHomed);
            bool bMoving = _frmMain.MC.Moving;
			bMaintEnabled = PLC.StackInMaintMode; 
            buttonEnableStackMaint.BackColor = bMaintEnabled ? Color.Orange : SystemColors.Control;

			//Hotplate Status Update
			bool bMod1MotorErrorResetting = PLC.Mod1ResetMotorError; 
            if(bMod1MotorErrorResetting)
            {
				if (PLC.Mod1MotorErrorResetting) 
				{
					PLC.Mod1ResetMotorError = false; 
				}
            }
			bool bMod2MotorErrorResetting = PLC.Mod2ResetMotorError;
			if (bMod2MotorErrorResetting)
			{
				if (PLC.Mod2MotorErrorResetting)
				{
					PLC.Mod2ResetMotorError = false;
				}
			}
			bool bMod3MotorErrorResetting = PLC.Mod3ResetMotorError;
			if (bMod3MotorErrorResetting)
			{
				if (PLC.Mod3MotorErrorResetting)
				{
					PLC.Mod3ResetMotorError = false;
				}
			}

			// Module 1 Status Update
			if (labelMod1Name.Visible)
			{
				buttonMod1ResetMotorError.Visible = PLC.Mod1MotorError || bMod1MotorErrorResetting;
				buttonMod1ResetMotorError.Text = bMod1MotorErrorResetting ? "Resetting Motor Error" : "Reset Motor Error";
				buttonMod1ResetMotorError.BackColor = bMod1MotorErrorResetting ? Color.Yellow : SystemColors.Control;
				buttonMod1ResetMotorError.Enabled = bMaintEnabled;
				buttonMod1OpenDoor.Enabled = bMaintEnabled;
				buttonMod1CloseDoor.Enabled = bMaintEnabled;
				buttonMod1VacOn.Enabled = bMaintEnabled;
				buttonMod1AntiVacOn.Enabled = bMaintEnabled;

				buttonMod1TempContactor.Enabled = bMaintEnabled;
				buttonMod1Home.Enabled = bMaintEnabled;
				buttonMod1ContBake.Enabled = bMaintEnabled;
				buttonMod1ProxBake.Enabled = bMaintEnabled;
				buttonMod1LdUld.Enabled = bMaintEnabled;
				buttonMod1ResetModule.Enabled = bMaintEnabled;

				labelMod1DoorIsOpen.Visible = PLC.Mod1DoorIsOpen;
				labelMod1DoorIsClosed.Visible = PLC.Mod1DoorClosed;
				labelMod1VacOK.Visible = PLC.Mod1VacSensor;
				labelMod1AntiVacON.Visible = PLC.Mod1AntiVacValve;
				buttonMod1OpenDoor.BackColor = PLC.Mod1DoorOpenValve ? Color.Orange : SystemColors.Control;
				buttonMod1CloseDoor.BackColor = PLC.Mod1DoorCloseValve ? Color.Orange : SystemColors.Control;
				buttonMod1VacOn.BackColor = PLC.Mod1VacValve ? Color.Orange : SystemColors.Control;
				buttonMod1AntiVacOn.BackColor = PLC.Mod1AntiVacValve ? Color.Orange : SystemColors.Control;
				buttonMod1TempContactor.BackColor = PLC.Mod1TempContactor ? Color.Orange : SystemColors.Control;

				buttonMod1ResetModule.BackColor = PLC.Mod1ResetModule ? Color.Orange : SystemColors.Control;
				buttonMod1ContBake.BackColor = PLC.Mod1MoveToContBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod1ProxBake.BackColor = PLC.Mod1MoveToProxBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod1LdUld.BackColor = PLC.Mod1MoveToLDULDReqActv ? Color.Orange : SystemColors.Control;
				buttonMod1Home.BackColor = PLC.Mod1MoveToHomeReqActv ? Color.Orange : SystemColors.Control;
				labelMod1ChuckTemp.Text = PLC.Mod1ChuckTemp.ToString("#0.0") + "°C";
				//labelMod1ChuckTemp.Enabled = bMaintEnabled;
				//labelMod1ChuckTempSP.Enabled = bMaintEnabled;

				SetLiftPinState(PLC.Mod1LiftPinPostion, labelMod1LiftPinState);
				SetSubstrateState(labelMod1SubstrateSensor, PLC.Mod1SubstrateSensor);
				labelMod1DoorOpenDisabled.Visible = PLC.Mod1VacEnableLockDoorOpen;
				panelMod1OpenEnable.Visible = PLC.Mod1VacDoorLockoutCntdwn;
				labelMod1EnabledTimeRemaining.Text = PLC.Mod1EnableDoorTimeRemaining.ToString("#0.0");
			}
			//Module 2 Status Update
			if (labelMod2Name.Visible)
			{
				buttonMod2ResetMotorError.Visible = PLC.Mod2MotorError || bMod2MotorErrorResetting;
				buttonMod2ResetMotorError.Text = bMod2MotorErrorResetting ? "Resetting Motor Error" : "Reset Motor Error";
				buttonMod2ResetMotorError.BackColor = bMod2MotorErrorResetting ? Color.Yellow : SystemColors.Control;
				buttonMod2ResetMotorError.Enabled = bMaintEnabled;
				buttonMod2OpenDoor.Enabled = bMaintEnabled;
				buttonMod2CloseDoor.Enabled = bMaintEnabled;
				buttonMod2VacOn.Enabled = bMaintEnabled;
				buttonMod2AntiVacOn.Enabled = bMaintEnabled;
                buttonMod2VacSlowRelease.Enabled = bMaintEnabled;

                buttonMod2TempContactor.Enabled = bMaintEnabled;
				buttonMod2Home.Enabled = bMaintEnabled;
				buttonMod2ContBake.Enabled = bMaintEnabled;
				buttonMod2ProxBake.Enabled = bMaintEnabled;
				buttonMod2LdUld.Enabled = bMaintEnabled;
				buttonMod2ResetModule.Enabled = bMaintEnabled;

				labelMod2DoorIsOpen.Visible = PLC.Mod2DoorIsOpen;
				labelMod2DoorIsClosed.Visible = PLC.Mod2DoorClosed;
				labelMod2VacOK.Visible = PLC.Mod2VacSensor;
				buttonMod2OpenDoor.BackColor = PLC.Mod2DoorOpenValve ? Color.Orange : SystemColors.Control;
				buttonMod2CloseDoor.BackColor = PLC.Mod2DoorCloseValve ? Color.Orange : SystemColors.Control;
				buttonMod2VacOn.BackColor = PLC.Mod2VacValve ? Color.Orange : SystemColors.Control;
				buttonMod2AntiVacOn.BackColor = PLC.Mod2FastRelease ? Color.Orange : SystemColors.Control;
                buttonMod2VacSlowRelease.BackColor = PLC.Mod2SlowRelease ? Color.Orange : SystemColors.Control;
                buttonMod2TempContactor.BackColor = PLC.Mod2TempContactor ? Color.Orange : SystemColors.Control;

				buttonMod2ResetModule.BackColor = PLC.Mod2ResetModule ? Color.Orange : SystemColors.Control;
				buttonMod2ContBake.BackColor = PLC.Mod2MoveToContBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod2ProxBake.BackColor = PLC.Mod2MoveToProxBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod2LdUld.BackColor = PLC.Mod2MoveToLDULDReqActv ? Color.Orange : SystemColors.Control;
				buttonMod2Home.BackColor = PLC.Mod2MoveToHomeReqActv ? Color.Orange : SystemColors.Control;
				labelMod2ChuckTemp.Text = PLC.Mod2ChuckTemp.ToString("#0.0") + "°C";
				labelMod2ChuckTemp.Enabled = bMaintEnabled;
				labelMod2ChuckTempSP.Enabled = bMaintEnabled;

				SetLiftPinState(PLC.Mod2LiftPinPostion, labelMod2LiftPinState);
				SetSubstrateState(labelMod2SubstrateSensor, PLC.Mod2SubstrateSensor);
				labelMod2DoorOpenDisabled.Visible = PLC.Mod2VacEnableLockDoorOpen;
				panelMod2OpenEnable.Visible = PLC.Mod2VacDoorLockoutCntdwn;
				labelMod2EnabledTimeRemaining.Text = PLC.Mod2EnableDoorTimeRemaining.ToString("#0.0");
			}
			//Module 3 Status Update
			if (labelMod3Name.Visible)
			{
				buttonMod3ResetMotorError.Visible = PLC.Mod3MotorError || bMod3MotorErrorResetting;
				buttonMod3ResetMotorError.Text = bMod3MotorErrorResetting ? "Resetting Motor Error" : "Reset Motor Error";
				buttonMod3ResetMotorError.BackColor = bMod3MotorErrorResetting ? Color.Yellow : SystemColors.Control;
				buttonMod3ResetMotorError.Enabled = bMaintEnabled;
				buttonMod3OpenDoor.Enabled = bMaintEnabled;
				buttonMod3CloseDoor.Enabled = bMaintEnabled;
				buttonMod3VacOn.Enabled = bMaintEnabled;
				buttonMod3AntiVacOn.Enabled = bMaintEnabled;

				buttonMod3TempContactor.Enabled = bMaintEnabled;
				buttonMod3Home.Enabled = bMaintEnabled;
				buttonMod3ContBake.Enabled = bMaintEnabled;
				buttonMod3ProxBake.Enabled = bMaintEnabled;
				buttonMod3LdUld.Enabled = bMaintEnabled;
				buttonMod3ResetModule.Enabled = bMaintEnabled;

				labelMod3DoorIsOpen.Visible = PLC.Mod3DoorIsOpen;
				labelMod3DoorIsClosed.Visible = PLC.Mod3DoorClosed;
				labelMod3VacOK.Visible = PLC.Mod3VacSensor;
				buttonMod3OpenDoor.BackColor = PLC.Mod3DoorOpenValve ? Color.Orange : SystemColors.Control;
				buttonMod3CloseDoor.BackColor = PLC.Mod3DoorCloseValve ? Color.Orange : SystemColors.Control;
				buttonMod3VacOn.BackColor = PLC.Mod3VacValve ? Color.Orange : SystemColors.Control;
				buttonMod3AntiVacOn.BackColor = PLC.Mod3AntiVacValve ? Color.Orange : SystemColors.Control;
				buttonMod3TempContactor.BackColor = PLC.Mod3TempContactor ? Color.Orange : SystemColors.Control;

				buttonMod3ResetModule.BackColor = PLC.Mod3ResetModule ? Color.Orange : SystemColors.Control;
				buttonMod3ContBake.BackColor = PLC.Mod3MoveToContBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod3ProxBake.BackColor = PLC.Mod3MoveToProxBakeReqActv ? Color.Orange : SystemColors.Control;
				buttonMod3LdUld.BackColor = PLC.Mod3MoveToLDULDReqActv ? Color.Orange : SystemColors.Control;
				buttonMod3Home.BackColor = PLC.Mod3MoveToHomeReqActv ? Color.Orange : SystemColors.Control;
				labelMod3ChuckTemp.Text = PLC.Mod3ChuckTemp.ToString("#0.0") + "°C";
				labelMod3ChuckTemp.Enabled = bMaintEnabled;
				labelMod3ChuckTempSP.Enabled = bMaintEnabled;

				SetLiftPinState(PLC.Mod3LiftPinPostion, labelMod3LiftPinState);
				SetSubstrateState(labelMod3SubstrateSensor, PLC.Mod3SubstrateSensor);
				labelMod3DoorOpenDisabled.Visible = PLC.Mod3VacEnableLockDoorOpen;
				panelMod3OpenEnable.Visible = PLC.Mod3VacDoorLockoutCntdwn;
				labelMod3EnabledTimeRemaining.Text = PLC.Mod3EnableDoorTimeRemaining.ToString("#0.0");
			}
        }

        private void SetSubstrateState(Label lblTemp, bool bState)
        {
            lblTemp.BackColor = bState ? Color.LimeGreen : Color.Gray;
            lblTemp.Text = bState ? "Substrate Present" : "No Substrate";
            lblTemp.BackColor = bState ? Color.LimeGreen : Color.DarkGray;
        }
        private void SetLiftPinState(int nState, Label refLabel)
        {
            string modState = "";
            Color bgColor;
            Color fgColor;

            if (nState > 9)
            {
                modState = "Motor Error:" + nState.ToString();
                bgColor = Color.Yellow;
                fgColor = Color.Black;
            }
            else
            {
                switch (nState)
                {
                    case 0:
                        modState = "Unknown or Moving";
                        bgColor = Color.Orange;
                        fgColor = Color.White;
                        break;
                    case 1:
                        modState = "Contact Bake";
                        bgColor = Color.White;
                        fgColor = Color.Black;
                        break;
                    case 2:
                        modState = "Proximity Bake";
                        bgColor = Color.White;
                        fgColor = Color.Black;
                        break;
                    case 3:
                        modState = "Load/Unload";
                        bgColor = Color.White;
                        fgColor = Color.Black;
                        break;
                    case 4:
                        modState = "Home";
                        bgColor = Color.White;
                        fgColor = Color.Black;
                        break;
                    case 5:
                        modState = "Error Reset";
                        bgColor = Color.Orange;
                        fgColor = Color.White;
                        break;
                    case 6:
                        modState = "RLS Occured";
                        bgColor = Color.Yellow;
                        fgColor = Color.Black;
                        break;
                    case 7:
                        modState = "FLS Occured";
                        bgColor = Color.Yellow;
                        fgColor = Color.Black;
                        break;
                    case 8:
                        modState = "RLS/FLS Occured";
                        bgColor = Color.Yellow;
                        fgColor = Color.Black;
                        break;
                    default:
                        modState = "NONE";
                        bgColor = Color.Gray;
                        fgColor = Color.White;
                        break;
                }
            }
            refLabel.Text = modState;
            refLabel.BackColor = bgColor;
            refLabel.ForeColor = fgColor;
        }

		private void buttonGoToStack_Click(object sender, EventArgs e)
        {
            if (_frmMain._prevForm != null)
                _frmMain.LoadSubForm(_frmMain._prevForm);
            else
                _frmMain.LoadSubForm(_frmMain.frmProcessStack);
        }

		private void buttonGoToStackSettings_Click(object sender, EventArgs e)
		{
			_frmMain.LoadSubForm(_frmMain.frmProcessStackSettings);
		}

		private void buttonGoToLoaderOperation_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmLoaderOperation);
        }

        private void buttonEnableStackMaint_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format("Stack Maint: User Toggled Stack Maint Mode to '{0}'", PLC.StackInMaintMode ? "Off" : "On"), "Action");
			PLC.EnableStackMaintMode(-1);
        }

        private void labelMod1ChuckTempSP_Click(object sender, EventArgs e)
        {
            if(bMaintEnabled)
                _frmMain.GotoNumScreen($"{_frmMain.MS.StackMod1Name} Setpoint (1-190°C)", this, labelMod1ChuckTempSP, "#0", 1, 190, "", false, false);
        }

        private void labelMod1ChuckTempSP_TextChanged(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: {_frmMain.MS.StackMod1Name} Chuck Setpoint being set to {labelMod1ChuckTempSP.Text}", "Action");
            if (!bFormLoading && bMaintEnabled)
            {
                string strTemp = labelMod1ChuckTempSP.Text.Replace("°C", "");
				PLC.Mod1ChuckSetPoint = Convert.ToInt32(strTemp); //PLC.SetDevice("W200", Convert.ToInt32(strTemp));
            }
        }

        private void buttonMod1ResetModule_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: Resetting Module 1", "Action");
			PLC.Mod1ResetModule = true; //PLC.SetDevice("B0C7", 1);
        }

		private void buttonMod1ResetMotorError_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: Resetting Module 1 Motor Error", "Action");
			PLC.Mod1ResetMotorError = true; //.SetDevice("B01D", 1);
		}
		private void buttonMod1TempContactor_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Contactor to '{0}'", PLC.Mod1TempContactor ? "Off" : "On"), "Action");
			PLC.Mod1TempContactor = !PLC.Mod1TempContactor;
        }

        private void buttonMod1ChuckVacuum_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Chuck Vac to '{0}'", PLC.Mod1VacValve ? "Off" : "On"), "Action");
			PLC.Mod1ToggleVac();
		}

		private void buttonMod1Float_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Chuck Float to '{0}'", PLC.Mod1AntiVacValve ? "Off" : "On"), "Action");
			PLC.Mod1ToggleAntiVac();
        }

        private void buttonMod1OpenDoor_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Door Open Valve to '{0}'", PLC.Mod1DoorOpenValve ? "Off" : "On"), "Action");
			PLC.Mod1ToggleOpenDoorValve();
        }

        private void buttonMod1CloseDoor_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Door Close Valve to '{0}'", PLC.Mod1DoorCloseValve ? "Off" : "On"), "Action");
			PLC.Mod1ToggleCloseDoorValve();
        }

        private void buttonMod1Home_MouseDown(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Home button", "Action");
			PLC.Mod1HomeBit = true; //.SetDevice("B05F", 1);
        }

        private void buttonMod1Home_MouseUp(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Home button", "Action");
			PLC.Mod1HomeBit = false; //.SetDevice("B05F", 0);
        }

        private void buttonMod1ContBake_MouseDown(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Contact Bake button", "Action");
			PLC.Mod1ContactBakeBit = true; // PLC.SetDevice("B05C", 1);
        }

        private void buttonMod1ContBake_MouseUp(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Contact Bake button", "Action");
			PLC.Mod1ContactBakeBit = false; // PLC.SetDevice("B05C", 0);
        }

        private void buttonMod1ProxBake_MouseDown(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Proximity Bake button", "Action");
			PLC.Mod1ProximityBakeBit = true; //.SetDevice("B05D", 1);
        }

        private void buttonMod1ProxBake_MouseUp(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Proximity Bake button", "Action");
			PLC.Mod1ProximityBakeBit = false; //.SetDevice("B05D", 0);
        }

        private void buttonMod1LdUld_MouseDown(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Load/Unload Height button", "Action");
			PLC.Mod1LoadUnloadHeightBit = true; //.SetDevice("B05E", 1);
        }

        private void buttonMod1LdUld_MouseUp(object sender, MouseEventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Load/Unload Height button", "Action");
			PLC.Mod1LoadUnloadHeightBit = false; //.SetDevice("B05E", 0);
        }

        private void buttonMod2OpenDoor_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Door Open Valve to '{0}'", PLC.Mod2DoorOpenValve ? "Off" : "On"), "Action");
			PLC.Mod2ToggleOpenDoorValve();
        }

        private void buttonMod2CloseDoor_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Door Close Valve to '{0}'", PLC.Mod2DoorCloseValve ? "Off" : "On"), "Action");
			PLC.Mod2ToggleCloseDoorValve();
        }

        private void buttonMod2VacOn_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Chuck Vac to '{0}'", PLC.Mod2VacValve ? "Off" : "On"), "Action");
			PLC.Mod2ToggleVac();
        }

        private void buttonMod2VacRelief_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Fast Vac Relief to '{0}'", PLC.Mod2FastRelease ? "Off" : "On"), "Action");
            if (PLC.Mod2FastRelease)
                PLC.Mod2ResetVacRelease();
            else
                PLC.Mod2SetFastRelease();
        }

        private void buttonMod2ResetModule_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: Resetting Module 2", "Action");
			PLC.Mod2ResetModule = true; //PLC.SetDevice("B0CA", 1);
        }

		private void buttonMod2ResetMotorError_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: Resetting Module 2 Motor Error", "Action");
			PLC.Mod2ResetMotorError = true; //.SetDevice("B022", 1);
		}

		private void buttonMod3VacOn_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 1 Vac to '{0}'", PLC.Mod3VacValve ? "Off" : "On"), "Action");
			PLC.Mod3ToggleVac();
		}

		private void labelMod2ChuckTempSP_Click(object sender, EventArgs e)
		{
			if (bMaintEnabled)
				_frmMain.GotoNumScreen($"{_frmMain.MS.StackMod2Name} Setpoint (1-190°C)", this, labelMod2ChuckTempSP, "#0", 1, 190, "", false, false);
		}

		private void labelMod2ChuckTempSP_TextChanged(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"Stack Maint: {_frmMain.MS.StackMod2Name} Chuck Setpoint being set to {labelMod2ChuckTempSP.Text}", "Action");
			if (!bFormLoading && bMaintEnabled)
			{
				string strTemp = labelMod2ChuckTempSP.Text.Replace("°C", "");
				PLC.Mod2ChuckSetPoint = Convert.ToInt32(strTemp); //PLC.SetDevice("W200", Convert.ToInt32(strTemp));
			}
		}

        private void buttonMod2TempContactor_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Contactor to '{0}'", PLC.Mod2TempContactor ? "Off" : "On"), "Action");
            PLC.Mod2TempContactor = !PLC.Mod2TempContactor;
        }

        private void buttonMod2Home_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Mod 2 Home button", "Action");
            PLC.Mod2HomeBit = true; //.SetDevice("B068", 1);
        }

        private void buttonMod2Home_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Mod 2 Home button", "Action");
            PLC.Mod2HomeBit = false; //.SetDevice("B068", 1);
        }

        private void buttonMod2ProxBake_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Mod 2 Proximity Bake button", "Action");
            PLC.Mod2ProximityBakeBit = true; //.SetDevice("B066", 1);
        }

        private void buttonMod2ProxBake_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Mod 2 Proximity Bake button", "Action");
            PLC.Mod2ProximityBakeBit = false; //.SetDevice("B066", 1);
        }

        private void buttonMod2ContBake_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Mod 2 Contact Bake button", "Action");
            PLC.Mod2ContactBakeBit = true; // PLC.SetDevice("B065", 1);
        }

        private void buttonMod2ContBake_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Mod 2 Contact Bake button", "Action");
            PLC.Mod2ContactBakeBit = false; // PLC.SetDevice("B065", 1);
        }

        private void buttonMod2LdUld_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Pushed Mod 2 LD/ULD Bake button", "Action");
            PLC.Mod2LoadUnloadHeightBit = true; // PLC.SetDevice("B067", 1);
        }

        private void buttonMod2LdUld_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"Stack Maint: User Released Mod 2 LD/ULD Bake button", "Action");
            PLC.Mod2LoadUnloadHeightBit = false; // PLC.SetDevice("B067", 1);
        }

        private void buttonMod2VacSlowRelease_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, string.Format($"Stack Maint: User Toggled Mod 2 Slow Vac Relief to '{0}'", PLC.Mod2SlowRelease ? "Off" : "On"), "Action");
            if (PLC.Mod2SlowRelease)
                PLC.Mod2ResetVacRelease();
            else
                PLC.Mod2SetSlowRelease();
        }
    }
}
