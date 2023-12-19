using System;
using System.Drawing;
using System.Windows.Forms;
using nTact.DataComm;
using System.Diagnostics;
//using System.IO;
using nTact.PLC;
using nAble.Enums;

namespace nAble
{
    public partial class FormProcessStack : Form, IUpdateableForm
    {
        private FormMain _frmMain = null;
		private readonly LogEntry _log = null;

		private GalilWrapper2 MCPU { get { return _frmMain.MC; } }
		private PLCWrapper PLC { get { return _frmMain.PLC; } }
		private Color _controlColor = Color.FromName("Control");
		private bool bUpdateOneShot = false;
		private bool _bHasStack = false;
        private bool _bLDULDFailOneShot = false;

        public FormProcessStack(FormMain formMain, LogEntry log)
        {
            _frmMain = formMain;
			_log = log;

            InitializeComponent();

			_bHasStack = _frmMain.MS.HasStack;
			SetupTabPages();
		}

		private void FormProcessStack_Load(object sender, EventArgs e)
        {
		}

		private void SetupTabPages()
		{
			tabControlModules.Visible = _bHasStack;
			buttonGoToStackRecipes.Visible = _bHasStack;
			ConfigMod1Layout();
			ConfigMod2Layout();
			ConfigMod3Layout();
		}

		private bool _bMod1HasVac = false;
		private bool _bMod2HasVac = false;
		private bool _bMod3HasVac = false;

		private void ConfigMod1Layout()
		{
			if (_frmMain.MS.StackMod1Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod1Type;
				groupBoxMod1.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod1Name}";
				tabPageModule1.Text = $"Mod 1 {NAbleEnums.ModuleTypeName(modType, true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						labelMod1StepTimeTitle.Visible = true;
						labelMod1StepTimeRemaining.Visible = true;
						labelMod1ChuckTemp.Visible = true;
						labelMod1ChuckTempSP.Visible = true;

						buttonMod1Load.Text = "Load\rHotplate";
						buttonMod1Unload.Text = "Unload\rHotplate";

						labelMod1VacState.Visible = false;
						labelMod1DoorOpenDisabled.Visible = false;
						panelMod1OpenEnable.Visible = false;
						buttonMod1ViewTrending.Visible = false;
						_bMod1HasVac = false;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
						labelMod1StepTimeTitle.Visible = true;
						labelMod1StepTimeRemaining.Visible = true;
						labelMod1ChuckTemp.Visible = true;
						labelMod1ChuckTempSP.Visible = true;

						buttonMod1Load.Text = "Load\rVacuum\rBake";
						buttonMod1Unload.Text = "Unload\rVacuum\rBake";

						labelMod1VacState.Visible = true;
						labelMod1DoorOpenDisabled.Visible = true;
						panelMod1OpenEnable.Visible = true;
						buttonMod1ViewTrending.Visible = false;
						_bMod1HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						labelMod1StepTimeTitle.Visible = true;
						labelMod1StepTimeRemaining.Visible = true;
						labelMod1ChuckTemp.Visible = true;
						labelMod1ChuckTempSP.Visible = true;

						buttonMod1Load.Text = "Load\rVacuum\rBake";
						buttonMod1Unload.Text = "Unload\rVacuum\rBake";

						labelMod1VacState.Visible = true;
						labelMod1DoorOpenDisabled.Visible = true;
						panelMod1OpenEnable.Visible = true;
						buttonMod1ViewTrending.Visible = true;
						_bMod1HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
						labelMod1StepTimeTitle.Visible = false;
						labelMod1StepTimeRemaining.Visible = false;
						labelMod1ChuckTemp.Visible = false;
						labelMod1ChuckTempSP.Visible = false;

						buttonMod1Load.Text = "Load\rVacuum\rDry";
						buttonMod1Unload.Text = "Unload\rVacuum\rDry";

						labelMod1VacState.Visible = true;
						labelMod1DoorOpenDisabled.Visible = true;
						panelMod1OpenEnable.Visible = false;
						buttonMod1ViewTrending.Visible = false;
						_bMod1HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						labelMod1StepTimeTitle.Visible = false;
						labelMod1StepTimeRemaining.Visible = false;
						labelMod1ChuckTemp.Visible = false;
						labelMod1ChuckTempSP.Visible = false;

						buttonMod1Load.Text = "Load\rVacuum\rDry";
						buttonMod1Unload.Text = "Unload\rVacuum\rDry";

						labelMod1VacState.Visible = true;
						labelMod1DoorOpenDisabled.Visible = true;
						panelMod1OpenEnable.Visible = true;
						buttonMod1ViewTrending.Visible = true;
						_bMod1HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
						labelMod1ChuckTemp.Visible = false;
						labelMod1ChuckTempSP.Visible = false;
						labelMod1VacState.Visible = false;
						labelMod1DoorOpenDisabled.Visible = false;
						panelMod1OpenEnable.Visible = false;
						buttonMod1ViewTrending.Visible = false;
						_bMod1HasVac = false;
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule1);
			}

		}

		private void ConfigMod2Layout()
		{
			if (_frmMain.MS.StackMod2Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod2Type;
				groupBoxMod2.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod2Name}";
				tabPageModule2.Text = $"Mod 2 {NAbleEnums.ModuleTypeName(modType, true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						labelMod2StepTimeTitle.Visible = true;
						labelMod2StepTimeRemaining.Visible = true;
						labelMod2ChuckTemp.Visible = true;
						labelMod2ChuckTempSP.Visible = true;

						buttonMod2Load.Text = "Load\rHotplate";
						buttonMod2Unload.Text = "Unload\rHotplate";

						labelMod2VacState.Visible = false;
						labelMod2DoorOpenDisabled.Visible = false;
						panelMod2OpenEnable.Visible = false;
						buttonMod2ViewTrending.Visible = false;
						_bMod2HasVac = false;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
						labelMod2StepTimeTitle.Visible = true;
						labelMod2StepTimeRemaining.Visible = true;
						labelMod2ChuckTemp.Visible = true;
						labelMod2ChuckTempSP.Visible = true;

						buttonMod2Load.Text = "Load\rVacuum\rBake";
						buttonMod2Unload.Text = "Unload\rVacuum\rBake";

						labelMod2VacState.Visible = true;
						labelMod2DoorOpenDisabled.Visible = true;
						panelMod2OpenEnable.Visible = true;
                        buttonMod2ViewTrending.Visible = false;
						_bMod2HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						labelMod2StepTimeTitle.Visible = true;
						labelMod2StepTimeRemaining.Visible = true;
						labelMod2ChuckTemp.Visible = true;
						labelMod2ChuckTempSP.Visible = true;

						buttonMod2Load.Text = "Load\rVacuum\rBake";
						buttonMod2Unload.Text = "Unload\rVacuum\rBake";

						labelMod2VacState.Visible = true;
						labelMod2DoorOpenDisabled.Visible = true;
						panelMod2OpenEnable.Visible = true;
                            //TODO Uncomment when advanced vacuum update is complete
                            //buttonMod2ViewTrending.Visible = true;
						_bMod2HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
						labelMod2StepTimeTitle.Visible = false;
						labelMod2StepTimeRemaining.Visible = false;
						labelMod2ChuckTemp.Visible = false;
						labelMod2ChuckTempSP.Visible = false;

						buttonMod2Load.Text = "Load\rVacuum\rDry";
						buttonMod2Unload.Text = "Unload\rVacuum\rDry";

						labelMod2VacState.Visible = true;
						labelMod2DoorOpenDisabled.Visible = true;
						panelMod2OpenEnable.Visible = true;
						buttonMod2ViewTrending.Visible = false;
						_bMod2HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						labelMod2StepTimeTitle.Visible = false;
						labelMod2StepTimeRemaining.Visible = false;
						labelMod2ChuckTemp.Visible = false;
						labelMod2ChuckTempSP.Visible = false;

						buttonMod2Load.Text = "Load\rVacuum\rDry";
						buttonMod2Unload.Text = "Unload\rVacuum\rDry";

						labelMod2VacState.Visible = true;
						labelMod2DoorOpenDisabled.Visible = true;
						panelMod2OpenEnable.Visible = true;
						buttonMod2ViewTrending.Visible = true;
						_bMod1HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
						_bMod2HasVac = false;
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule2);
			}
		}

		private void ConfigMod3Layout()
		{
			if (_frmMain.MS.StackMod3Installed)
			{
				NAbleEnums.StackModuleType modType = (NAbleEnums.StackModuleType)_frmMain.MS.StackMod3Type;
				groupBoxMod3.Text = $"{NAbleEnums.ModuleTypeName(modType)} - {_frmMain.MS.StackMod3Name}";
				tabPageModule3.Text = $"Mod 3 {NAbleEnums.ModuleTypeName(modType, true)}";
				switch (modType)
				{
					case NAbleEnums.StackModuleType.Hotplate:
					{
						labelMod3StepTimeTitle.Visible = true;
						labelMod3StepTimeRemaining.Visible = true;
						labelMod3ChuckTemp.Visible = true;
						labelMod3ChuckTempSP.Visible = true;

						buttonMod3Load.Text = "Load\rHotplate";
						buttonMod3Unload.Text = "Unload\rHotplate";

						labelMod3VacState.Visible = false;
						labelMod3DoorOpenDisabled.Visible = false;
						panelMod3OpenEnable.Visible = false;
						buttonMod3ViewTrending.Visible = false;
						_bMod3HasVac = false;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBake:
					{
						labelMod3StepTimeTitle.Visible = true;
						labelMod3StepTimeRemaining.Visible = true;
						labelMod3ChuckTemp.Visible = true;
						labelMod3ChuckTempSP.Visible = true;

						buttonMod3Load.Text = "Load\rVacuum\rBake";
						buttonMod3Unload.Text = "Unload\rVacuum\rBake";

						labelMod3VacState.Visible = true;
						labelMod3DoorOpenDisabled.Visible = true;
						panelMod3OpenEnable.Visible = true;
						buttonMod3ViewTrending.Visible = false;
						_bMod3HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuBakePlus:
					{
						labelMod3StepTimeTitle.Visible = true;
						labelMod3StepTimeRemaining.Visible = true;
						labelMod3ChuckTemp.Visible = true;
						labelMod3ChuckTempSP.Visible = true;

						buttonMod3Load.Text = "Load\rVacuum\rBake";
						buttonMod3Unload.Text = "Unload\rVacuum\rBake";

						labelMod3VacState.Visible = true;
						labelMod3DoorOpenDisabled.Visible = true;
						panelMod3OpenEnable.Visible = true;
						buttonMod3ViewTrending.Visible = true;
						_bMod3HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDry:
					{
						labelMod3StepTimeTitle.Visible = false;
						labelMod3StepTimeRemaining.Visible = false;
						labelMod3ChuckTemp.Visible = false;
						labelMod3ChuckTempSP.Visible = false;

						buttonMod3Load.Text = "Load\rVacuum\rDry";
						buttonMod3Unload.Text = "Unload\rVacuum\rDry";

						labelMod3VacState.Visible = true;
						labelMod3DoorOpenDisabled.Visible = true;
						panelMod3OpenEnable.Visible = true;
						buttonMod3ViewTrending.Visible = false;
						_bMod3HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.VacuDryPlus:
					{
						labelMod3StepTimeTitle.Visible = false;
						labelMod3StepTimeRemaining.Visible = false;
						labelMod3ChuckTemp.Visible = false;
						labelMod3ChuckTempSP.Visible = false;

						buttonMod3Load.Text = "Load\rVacuum\rDry";
						buttonMod3Unload.Text = "Unload\rVacuum\rDry";

						labelMod3VacState.Visible = true;
						labelMod3DoorOpenDisabled.Visible = true;
						panelMod3OpenEnable.Visible = true;
						buttonMod3ViewTrending.Visible = true;
						_bMod3HasVac = true;
					}
					break;
					case NAbleEnums.StackModuleType.ChillPlate:
					{
						_bMod3HasVac = false;
					}
					break;
				}
			}
			else
			{
				tabControlModules.TabPages.Remove(tabPageModule3);
			}
		}

		public void UpdateStatus()
        {
            if (_frmMain == null || !Visible)
            {
                bUpdateOneShot = true;
                return;
            }
			checkBoxConvAtStack.Visible = _frmMain.IsDemoMode;
			checkBoxConvHasSub.Visible = _frmMain.IsDemoMode;
			if (bUpdateOneShot)
            {
                bUpdateOneShot = false;
            }
            bool bConnected = (MCPU != null && MCPU.Connected && MCPU.IsHomed);
            bool bMoving = MCPU.Moving;
            bool bTransferMoving = MCPU.Loader_Moving;
			bool bLoaderMoving = PLC.LoaderMoving;
            bool bLoaded = !MCPU.AlignersUpRequest && MCPU.LiftPinsDownRequest && MCPU.Zone1VacOK && MCPU.Zone2VacOK && MCPU.Zone3VacOK &&
                            MCPU.Zone1VacEngaged && MCPU.Zone2VacEngaged && MCPU.Zone3VacEngaged;

            bool bUnloaded = MCPU.LiftPinsUpRequest;
            if (PLC.LDULDFailure && !_bLDULDFailOneShot)
            {
                _bLDULDFailOneShot = true;
                panelLDULDFailure.Visible = true;
            }
            else if (!PLC.LDULDFailure && _bLDULDFailOneShot)
                _bLDULDFailOneShot = false;

			//Update Mod 1 Status
			if (_bMod1HasVac)
				SetVacModState(PLC.Mod1State, labelMod1Status);
			else
				SetRegModState(PLC.Mod1State, labelMod1Status);
			SetRegModState(PLC.Mod1State, labelMod1Status);
            SetSubstrateState(PLC.Mod1SubstratePresent, labelMod1Substrate);
            labelMod1ChuckTemp.Text = PLC.Mod1ChuckTemp.ToString("#0.0") + "°C";
            labelMod1ChuckTempSP.Text = PLC.Mod1ChuckSetPoint.ToString("#0") + "°C";
			labelMod1StepTimeRemaining.Text = PLC.Mod1ProcessTimeRemaining;
			labelMod1TotalTimeRemaining.Text = PLC.Mod1ProcessTotalTimeRemaining;
			buttonMod1AbortProcess.Enabled = PLC.Mod1ProcessActive;
            buttonMod1Load.Enabled = PLC.Mod1LoadEnabled;
			buttonMod1Unload.Enabled = PLC.Mod1UnloadEnabled;

			string sMod1ModType = NAbleEnums.ModuleTypeName(_frmMain.MS.StackMod1Type);
            buttonMod1AbortProcess.BackColor = PLC.Mod1Aborting ? Color.Orange : SystemColors.Control;
            buttonMod1AbortProcess.Text = PLC.Mod1Aborting ? "Aborting Process" : "Abort Process";
            buttonMod1Load.BackColor = PLC.Mod1Loading ? Color.Yellow : SystemColors.Control;
            buttonMod1Load.Text = PLC.Mod1Loading ? $"Loading\r{sMod1ModType}" : $"Load\r{sMod1ModType}";

			buttonMod1Unload.BackColor = PLC.Mod1Unloading ? Color.Yellow : SystemColors.Control;
            buttonMod1Unload.Text = PLC.Mod1Unloading ? $"Unloading\r{sMod1ModType}" : $"Unload\r{sMod1ModType}";

			//Update Mod2 Status
			if (_bMod2HasVac)
				SetVacModState(PLC.Mod2State, labelMod2Status);
			else
				SetRegModState(PLC.Mod2State, labelMod2Status);
			SetSubstrateState(PLC.Mod2SubstratePresent, labelMod2Substrate);
			labelMod2ChuckTemp.Text = PLC.Mod2ChuckTemp.ToString("#0.0") + "°C";
			labelMod2ChuckTempSP.Text = PLC.Mod2ChuckSetPoint.ToString("#0") + "°C";
			labelMod2StepTimeRemaining.Text = PLC.Mod2ProcessTimeRemaining;
			labelMod2TotalTimeRemaining.Text = PLC.Mod2ProcessTotalTimeRemaining;
			buttonMod2AbortProcess.Enabled = PLC.Mod2ProcessActive;
			buttonMod2Load.Enabled = PLC.Mod2LoadEnabled;
			buttonMod2Unload.Enabled = PLC.Mod2UnloadEnabled;
            labelMod2VacState.Visible= PLC.Mod2VacSensor;
            labelMod2DoorOpenDisabled.Visible = PLC.Mod2VacEnableLockDoorOpen;
            panelMod2OpenEnable.Visible = PLC.Mod2VacDoorLockoutCntdwn;
            //labelMod2EnabledTimeRemaining.Text = PLC.Mod2EnableDoorTimeRemaining.ToString("#0.0");

            string sMod2ModType = NAbleEnums.ModuleTypeName(_frmMain.MS.StackMod2Type);
			buttonMod2AbortProcess.BackColor = PLC.Mod2Aborting ? Color.Orange : SystemColors.Control;
			buttonMod2AbortProcess.Text = PLC.Mod2Aborting ? "Aborting Process" : "Abort Process";
			buttonMod2Load.BackColor = PLC.Mod2Loading ? Color.Yellow : SystemColors.Control;
			buttonMod2Load.Text = PLC.Mod2Loading ? $"Loading\r{sMod2ModType}" : $"Load\r{sMod2ModType}";

			buttonMod2Unload.BackColor = PLC.Mod2Unloading ? Color.Yellow : SystemColors.Control;
			buttonMod2Unload.Text = PLC.Mod2Unloading ? $"Unloading\r{sMod2ModType}" : $"Unload\r{sMod2ModType}";

			//Update Mod3 Status
			if (_bMod3HasVac)
				SetVacModState(PLC.Mod3State, labelMod3Status);
			else
				SetRegModState(PLC.Mod3State, labelMod3Status);
			SetSubstrateState(PLC.Mod3SubstratePresent, labelMod3Substrate);
			labelMod3ChuckTemp.Text = PLC.Mod3ChuckTemp.ToString("#0.0") + "°C";
			labelMod3ChuckTempSP.Text = PLC.Mod3ChuckSetPoint.ToString("#0") + "°C";
			labelMod3StepTimeRemaining.Text = PLC.Mod3ProcessTimeRemaining;
			labelMod3TotalTimeRemaining.Text = PLC.Mod3ProcessTotalTimeRemaining;
			buttonMod3AbortProcess.Enabled = PLC.Mod3ProcessActive;
			buttonMod3Load.Enabled = PLC.Mod3LoadEnabled;
			buttonMod3Unload.Enabled = PLC.Mod3UnloadEnabled;

			string sMod3ModType = NAbleEnums.ModuleTypeName(_frmMain.MS.StackMod3Type);
			buttonMod3AbortProcess.BackColor = PLC.Mod3Aborting ? Color.Orange : SystemColors.Control;
			buttonMod3AbortProcess.Text = PLC.Mod3Aborting ? "Aborting Process" : "Abort Process";
			buttonMod3Load.BackColor = PLC.Mod3Loading ? Color.Yellow : SystemColors.Control;
			buttonMod3Load.Text = PLC.Mod3Loading ? $"Loading\r{sMod3ModType}" : $"Load\r{sMod3ModType}";

			buttonMod3Unload.BackColor = PLC.Mod3Unloading ? Color.Yellow : SystemColors.Control;
			buttonMod3Unload.Text = PLC.Mod3Unloading ? $"Unloading\r{sMod3ModType}" : $"Unload\r{sMod3ModType}";


			//Update Others
			buttonAbortLDULD.Enabled = _frmMain.PLCIsConnected && _frmMain.PLC.LoaderHomed && !_frmMain.PLC.StackInMaintMode && PLC.EnableLDULDAbort;
            buttonReturnToUnload.Enabled = _frmMain.PLCIsConnected && _frmMain.PLC.LoaderHomed && !_frmMain.PLC.StackInMaintMode && !bLoaderMoving;
            buttonAbortLDULD.BackColor = PLC.AbortLDULDRequest ? Color.Yellow : SystemColors.Control;
            buttonAbortLDULD.Text = PLC.AbortLDULDRequest ? "Aborting LD/ULD" : "ABORT LD/ULD";
			buttonReturnToUnload.BackColor = PLC.MoveToLoadPosReq ? Color.Yellow : SystemColors.Control;
            buttonReturnToUnload.Text = PLC.MoveToLoadPosReq ? "Moving To LD/ULD" : "Move To LD/ULD";
			buttonParkEndEffector.Enabled = _frmMain.PLCIsConnected && _frmMain.PLC.LoaderHomed && !_frmMain.PLC.StackInMaintMode && PLC.EndEffectorPresent & PLC.LockQuickChange & !bLoaderMoving;
			buttonAttachEndEffector.Enabled = _frmMain.PLCIsConnected && _frmMain.PLC.LoaderHomed && !_frmMain.PLC.StackInMaintMode && !PLC.LockQuickChange & !bLoaderMoving;
            buttonTransferToChuck.Enabled = bConnected && !bTransferMoving;
            buttonTransferToLDULD.Enabled = bConnected && !bTransferMoving;


        }

        private bool SetVacLevelState(bool bState, Label refLabel)
        {
            string modState = "";
            Color bgColor;
            Color fgColor;

            bool retVal = true;
            switch (bState)
            {
                case false:
                    modState = "Not @ Vac SetPnt";
                    bgColor = Color.Red;
                    fgColor = Color.White;
                    break;
                case true:
                    modState = "@ Vac SetPnt";
                    bgColor = Color.LimeGreen;
                    fgColor = Color.Black;
                    break;
                default:
                    modState = "NONE";
                    bgColor = Color.Gray;
                    fgColor = Color.White;
                    retVal = false;
                    break;
            }
            refLabel.Text = modState;
            refLabel.BackColor = bgColor;
            refLabel.ForeColor = fgColor;
            return retVal;
        }
        private bool SetSubstrateState(bool bState, Label refLabel)
        {
            string modState = "";
            Color bgColor;
            Color fgColor;

            bool retVal = true;
            switch (bState)
            {
                case false:
                    modState = "NO SUBSTR";
                    bgColor = Color.Black;
                    fgColor = Color.White;
                    break;
                case true:
                    modState = "SUBSTRATE";
                    bgColor = Color.LimeGreen;
                    fgColor = Color.Black;
                    break;
                default:
                    modState = "NONE";
                    bgColor = Color.Black;
                    fgColor = Color.White;
                    retVal = false;
                    break;
            }
            refLabel.Text = modState;
            refLabel.BackColor = bgColor;
            refLabel.ForeColor = fgColor;
            return retVal;
        }
        private bool SetRegModState(int nState, Label refLabel)
        {
            string modState = "";
            Color bgColor;
            Color fgColor;

            bool retVal = true;
            switch (nState)
            {
                case 0:
                    modState = "NOT ACTIVE";
                    bgColor = Color.Gray;
                    fgColor = Color.White;
                    break;
                case 1:
                    modState = "PROX BK";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 2:
                    modState = "CONT BK";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 3:
                    modState = "COOLING";
                    bgColor = Color.Cyan;
                    fgColor = Color.Black;
                    break;
                case 4:
                    modState = "COMPLETE";
                    bgColor = Color.LimeGreen;
                    fgColor = Color.Black;
                    break;
                case 5:
                    modState = "MOVING";
                    bgColor = Color.Orange;
                    fgColor = Color.Black;
                    break;
                default:
                    modState = "NONE";
                    bgColor = Color.Gray;
                    fgColor = Color.White;
                    retVal = false;
                    break;
            }
            refLabel.Text = modState;
            refLabel.BackColor = bgColor;
            refLabel.ForeColor = fgColor;
            return retVal;
        }
        private bool SetVacModState(int nState, Label refLabel)
        {
            string modState = "";
            Color bgColor;
            Color fgColor;

            bool retVal = true;
            switch (nState)
            {
                case 0:
                    modState = "NOT ACTIVE";
                    bgColor = Color.Gray;
                    fgColor = Color.White;
                    break;
                case 1:
                    modState = "PROX BK";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 2:
                    modState = "CONT BK";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 3:
                    modState = "COOLING";
                    bgColor = Color.Cyan;
                    fgColor = Color.Black;
                    break;
                case 4:
                    modState = "COMPLETE";
                    bgColor = Color.LimeGreen;
                    fgColor = Color.Black;
                    break;
                case 5:
                    modState = "MOVING";
                    bgColor = Color.Orange;
                    fgColor = Color.Black;
                    break;
                case 7:
                    modState = "EVACUATING";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 8:
                    modState = "EVAC HOLD";
                    bgColor = Color.Yellow;
                    fgColor = Color.Black;
                    break;
                case 9:
                    modState = "VAC RLS";
                    bgColor = Color.Cyan;
                    fgColor = Color.Black;
                    break;
                default:
                    modState = "NONE";
                    bgColor = Color.Gray;
                    fgColor = Color.White;
                    retVal = false;
                    break;
            }
            refLabel.Text = modState;
            refLabel.BackColor = bgColor;
            refLabel.ForeColor = fgColor;
            return retVal;
        }

        string GetUpDown(bool bState)
        {
            if (bState)
            {
                return "DWN";
            }
            else
            {
                return "UP";
            }
        }

        private void buttonGoToStackMaint_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmProcessStackMaint);
        }

        private void buttonAbortLDULD_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Abort Load/Unload Operation ", "Action");
			PLC.MoveZToStackMod2Low();
        }

        private void buttonReturnToUnload_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested End Effector to return to Load/Unload Position ", "Action");
			if (DialogResult.Yes == nRadMessageBox.Show(this, "Move To Load/Unload Position?", "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
				PLC.MoveEndEffector2LDULDP();
            }
        }

		private void buttonMod1ViewTrending_Click(object sender, EventArgs e)
		{

		}

		private void buttonMod1AbortProcess_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested abort process for Module 1 - {_frmMain.MS.StackMod1Name} ", "Action");
			PLC.AbortModuleProcess(1);
        }

        private void buttonMod2AbortProcess_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested abort process for Module 2 - {_frmMain.MS.StackMod2Name} ", "Action");
			PLC.AbortModuleProcess(2);
        }

		private void buttonMod3AbortProcess_Click(object sender, EventArgs e)
		{
			_log.log(LogType.TRACE, Category.INFO, $"User Requested abort process for Module 3 - {_frmMain.MS.StackMod3Name} ", "Action");
			PLC.AbortModuleProcess(3);
		}

		private void buttonLoadMod1_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Load for Module 1 - {_frmMain.MS.StackMod1Name} ", "Action");
			if (!PLC.Mod1DoorClosed)
            {
                nRadMessageBox.Show(this, "Module 1 '{_frmMain.MS.StackMod1Name}' Door Is Not Closed!", "Load Hotplate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
			PLC.LoadModule(1);
        }

        private void buttonUnloadMod1_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested UnLoad for Module 1 - {_frmMain.MS.StackMod1Name} ", "Action");
			PLC.UnLoadModule(1);
        }

        private void buttonLoadMod2_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Load for Module 2 - {_frmMain.MS.StackMod2Name} ", "Action");
			if (!PLC.Mod2DoorClosed)
            {
                nRadMessageBox.Show(this, $"Module 2 '{_frmMain.MS.StackMod2Name}' Door Is Not Closed!", "Load Vacuum Dry", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
			PLC.LoadModule(2);
        }

        private void buttonUnloadMod2_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested UnLoad for Module 2 - {_frmMain.MS.StackMod2Name} ", "Action");
			PLC.UnLoadModule(2);
        }

        private void buttonGotoPrevScreen_Click(object sender, EventArgs e)
        {
            if (_frmMain._prevForm != null)
                _frmMain.LoadSubForm(_frmMain._prevForm);
            else
                _frmMain.LoadSubForm(_frmMain.frmMainMenu);
        }

        private void buttonParkEndEffector_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, $"User Requested Park End Effector", "Action");
			if (MCPU.IsClose(_frmMain.MC.LoaderPos, _frmMain.MS.ConvUnloadPosition, 2))
            {
                if(_frmMain.MC.TransferSubstratePresent)
                {
                    if (!PLC.ZAtDockingPos && !PLC.ZAtLDULDPos)
                    {
                        nRadMessageBox.Show(this, "Substrate On Transfer Conveyor! Remove Substrate First!", "Park End Effector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
            }
            else
            {
                nRadMessageBox.Show(this, "Transfer Conveyor Not At Load/Unload or Chuck Position. Please Move Transfer To LD/ULD or Chuck Then Retry.", "Park End Effector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (PLC.EndEffectorSubstratePresent)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Substrate On End Effector! Park End Effector?", "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
					PLC.ParkEndEffector();
                }
                else
                {
					_log.log(LogType.TRACE, Category.INFO, $"User Declined Park End Effector with substrate detected", "Action");
					return;
                }
            }
            else if (DialogResult.Yes == nRadMessageBox.Show(this, "Park End Effector?", "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
				PLC.ParkEndEffector();
			}
			else
			{
				_log.log(LogType.TRACE, Category.INFO, $"User Declined Park End Effector", "Action");
				return;
			}
		}

        private void buttonAttachEndEffector_Click(object sender, EventArgs e)
        {

            if (!MCPU.IsClose(_frmMain.MC.LoaderPos,_frmMain.MS.ConvUnloadPosition,2))
            {
                if (!MCPU.IsClose(_frmMain.MC.LoaderPos, _frmMain.MS.ConvCoaterPosition,2))
                {
                    nRadMessageBox.Show(this, "Transfer Conveyor Not At Chuck or LD/ULD Position! Move Conveyor To Chuck or LD/ULD Position First! ", "Get End Effector", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Get End Effector?", "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
				PLC.PickUpEndEffector();
            }
        }

        private void buttonTransferToLDULD_Click(object sender, EventArgs e)
        {
			_log.log(LogType.TRACE, Category.INFO, "User Pressed Stack-'Goto Chuck' button.", "Action");
			if (_bHasStack && !PLC.ZAtClearPos)
            {
                nRadMessageBox.Show(this, "Loader Not At Clear Height! Move To Clear Height First!", "Move Transfer", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
			else
			{
				if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Move Loader to Load/Unload Pos?", "Confirm Request", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					_frmMain.MC.MoveConeyorToStack();
				}
				else
				{
				}
			}

        }

        private void buttonTransferToChuck_Click(object sender, EventArgs e)
        {
            if(PLC.EndEffectorSubstratePresent)
            {
                if(_frmMain.MC.LiftPinsUpRequest)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Glass On Transfer and Lift Pins Up! Lower Lift Pins?", "Move Transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        _frmMain.MC.LowerLiftPins();
                    }
                    else
                    {
                        return;
                    }
                }

            }
            else if(!_frmMain.MC.LiftPinsUpRequest)
            {
                if(DialogResult.Yes== nRadMessageBox.Show(this, "Lift Pins Not Up! Raise Lift Pins?", "Move Transfer", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _frmMain.MC.UnLoadSubstrate();
                }
                else
                {
                    return;
                }

            }
            int nTimeOut = 0;
            while(!_frmMain.MC.LiftPinsUpRequest && nTimeOut<200)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(50);
                nTimeOut++;
            }
            if(nTimeOut<200 && _frmMain.MC.LiftPinsUpRequest)
                _frmMain.MC.MoveConveyorToChuck();
        }

        private void buttonGoToStackRecipes_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmStackRecipes);
        }

        private void buttonGoToLoaderOperation_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmLoaderOperation);
        }

		private void buttonMod2ViewTrending_Click(object sender, EventArgs e)
		{
			_frmMain.LoadSubForm(_frmMain.frmVacTrend);
		}

        private void buttonLDULDFailContinue_Click(object sender, EventArgs e)
        {
            panelLDULDFailure.Visible = false;
        }

        private void buttonLDULDFailContinue_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed Continue After Load/Unload Failure ", "Action");
            PLC.LDULDFailContinue = true;
        }

        private void buttonLDULDFailContinue_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"User Released Continue After Load/Unload Failure ", "Action");
            PLC.LDULDFailContinue = false;
        }

        private void buttonLDULDFailAbort_MouseDown(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed Abort After Load/Unload Failure ", "Action");
            PLC.LDULDFailAbort = true;
        }

        private void buttonLDULDFailAbort_MouseUp(object sender, MouseEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, $"User Released Abort After Load/Unload Failure ", "Action");
            PLC.LDULDFailAbort = false;
        }

        private void buttonLDULDFailAbort_Click(object sender, EventArgs e)
        {
            panelLDULDFailure.Visible = false;
        }
    }
}
