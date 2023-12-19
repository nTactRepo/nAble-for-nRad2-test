using nAble.Data;
using nAble.DataComm.KeyenceLasers;
using nAble.Enums;
using nAble.Model;
using nAble.Utils;
using nAble_for_nRad2.Properties;
using nTact.DataComm;
using System.Diagnostics;
using System.IO.Ports;
using System.Net;

namespace nAble
{
    public partial class FormSetup : Form, IUpdateableForm
    {
        #region Constants

        private static readonly Color ColorDarkControl = Color.FromName("ControlDark");
        private static readonly Color ColorControl = Color.FromName("Control");

        #endregion

        #region Properties

        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;
        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private Omega485Controller Heaters => _frmMain.Omega485Controller;

        public bool ShowLockout { get; set; } = true;

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;

        private Dictionary<int, string> _tcCodes;

        private TabPage _irTab = null;
        private TabPage _akTab = null;
        private TabPage _loaderTab = null;
        private TabPage _stackTab = null;

        private int _pressDisplayType = 0;
        private bool _loadingMachineSettings = false;
        private bool _updating = false;
        private double _angle = 0;
        private string _tempStr;
        private bool _oneShot = false;
        private bool _bVisibleOneShot = false;

        #endregion

        #region Functions

        public FormSetup(FormMain formMain, LogEntry log)
        {
            _updating = true;

            InitializeComponent();

            _frmMain = formMain ?? throw new ArgumentNullException(nameof(formMain));
            _log = log ?? throw new ArgumentNullException(nameof(log));

            _irTab = tabPageIRLampFS;
            _akTab = tabPageAirKnifeFS;
            _loaderTab = tabPageLoader;
            _stackTab = tabPageStack;

            _updating = false;
        }

        private void FormSetup_Load(object sender, EventArgs e)
        {
            _updating = true;

            panelSetupHide.Visible = ShowLockout;
            checkBoxUsingLaser.Checked = MS.KeyenceLaserInstalled && MS.UsingKeyenceLaser;
            checkBoxUsingLaser.Visible = MS.KeyenceLaserInstalled;
            ipAddressInputLasers.Value = MS.KeyenceLaserIP;
            groupBoxAnalogs.Visible = Settings.Default.HasAnalogInputs;

            panelSetupHide.Dock = ShowLockout ? DockStyle.Fill : DockStyle.None;

            if (ShowLockout)
                panelSetupHide.BringToFront();

            groupBoxSetupLoaderServo.Enabled = MS.HasLoader;

            foreach (string port in SerialPort.GetPortNames())
            {
                if (port.StartsWith("COM"))
                {
                    comboBoxKeyenceComPort.Items.Add(port);
                    comboBoxChuckComPort.Items.Add(port);
                    comboBoxDieComPort.Items.Add(port);
                    comboBoxResvComPort.Items.Add(port);
                    //comboBoxResvHeaterComPort.Items.Add(port);
                    comboBoxIRCOM.Items.Add(port);
                }
            }

            comboBoxModule1Type.Items.Clear();
            comboBoxModule2Type.Items.Clear();
            comboBoxModule3Type.Items.Clear();

            var values = Enum.GetValues(typeof(NAbleEnums.StackModuleType));

            //int nCounter = 0;
            foreach (var type in values)
            {
                Debug.WriteLine($"StackModuleType - {type}");
                comboBoxModule1Type.Items.Insert((int)type, type);
                comboBoxModule2Type.Items.Insert((int)type, type);
                comboBoxModule3Type.Items.Insert((int)type, type);
            }

            PopuplateControlsFromMachineSettings();

            if (ShowLockout)
            {
                panelSetupHide.BringToFront();
                textBoxAccessCode.Focus();
            }
            else
            {
                buttonSetupSave.BringToFront();
                buttonLock.BringToFront();
                buttonClose.BringToFront();
            }

            comboBoxPrgmNum.SelectedIndex = LaserMgr?.ProgramNumber ?? -1;

            LoadTCCodes();

            _updating = false;
        }

        private void FormSetup_Shown(object sender, EventArgs e)
        {
            textBoxAccessCode.Focus();
        }


        private void PopuplateControlsFromMachineSettings()
        {
            _loadingMachineSettings = true;

            if (!MS.HasLoader)
            {
                //tabPageLoader.Hide();
                tabControlSetup.TabPages.Remove(tabPageLoader);
            }

            if (!MS.HasStack)
            {
                //tabPageStack.Hide();
                tabControlSetup.TabPages.Remove(tabPageStack);
            }

            if (MC.SyringePumpDetected || MC.SyringePumpBDetected)
            {
                //tabPageSetupPOH.Hide();
                tabControlSetup.TabPages.Remove(tabPageSetupPOH);
            }

            //MessageBox.Show("Raspberry Pi: " + MS.HasRaspberryPiRotary.ToString());
            labelInputsDI6.Text = MS.AutoLDULD ? "DI6 - Lift Pins/Aligner Down" : MS.DualZoneLiftPinsEnabled ? "DI6 - Coating Lift Pins Down" : "DI6 - Reserved";
            labelInputsDI7.Text = MS.AutoLDULD ? "DI7 - Lift Pins/Aligner Up" : "DI7 - Reserved";
            labelInputsDI1.Text = !MS.HasRaspberryPiRotary ? "DI1 - Rotary Valve-A Home" : MS.AutoLDULD ? "DI1 - Substrate Sensor" : "DI1 - Reserved";
            labelInputsDI9.Text = !MS.HasRaspberryPiRotary ? "DI09 - Rotary Valve-B Home" : "DI09 - Reserved";

            checkBoxEnableLooper.Checked = MS.EnableLooperMode;
            checkBoxDemoMode.Checked = MC.IsDemo;
            checkBoxHasFlowMEter.Checked = MS.HasPumpFlowMeters;
            labelSetupIP.Text = MS.IPAddress;
            comboBoxKeyenceComPort.Text = MS.KeyenceCOMPort;
            comboBoxChuckComPort.Text = MS.ChuckCOMPort;
            buttonDieComID.Text = MS.DieCOMID.ToString();
            comboBoxDieComPort.Text = MS.DieCOMPort;
            buttonChuckComID.Text = MS.ChuckCOMID.ToString();
            checkBoxSetupAligners.Checked = MS.AlignersEnabled;
            checkBoxSetupLiftPins.Checked = MS.LiftPinsEnabled;
            checkBoxHasAutoLDULD.Checked = MS.AutoLDULD;
            checkBoxHasPrimingArea.Checked = MS.HasPrimingArea;
            checkBoxSetupDualZoneLiftPins.Checked = MS.DualZoneLiftPinsEnabled;
            checkBoxLiftPinsPrep2Load.Checked = MS.LiftPinsDuringPrep2Load;
            checkBoxHasLoader.Checked = MS.HasLoader;
            buttonCarriageSafeLoc.Text = $"{MS.SafeCarriageLoc:0.000}";
            buttonLoaderSafeLoc.Text = $"{MS.SafeLoaderLoc:0.000}";
            comboBoxNumOfLasers.Text = $"{MS.NumberOfLasers}";

            checkBoxSetupHasPrimingPlate.Checked = MS.HasPrimingPlate;
            buttonChuckNumZones.Text = MS.NumChuckAirVacZones.ToString();
            checkBoxDiePressureTransducer.Checked = MS.HasDiePressureTransducer;
            buttonDiePressVAdjustment.Text = MS.DiePressureInputVoltageAdjust.ToString("0.0##");
            checkBoxSelectiveZones.Checked = MS.UsesSelectiveAirVacZones;

            textBoxCfgOperatorAccessCode.Text = MS.OperatorPW;
            textBoxCfgEditorAccessCode.Text = MS.EditorPW;
            textBoxCfgAdminAccessCode.Text = MS.AdminPW;

            //Locations
            buttonXVisionPos.Text = MS.VisionXPos.ToString("#0.000");
            buttonZVisionPos.Text = MS.VisionZPos.ToString("#0.000");

            comboBoxResvComPort.Text = MS.ResvCOMPort;
            buttonResvFluidComID.Text = MS.ResvCOMID.ToString();
            buttonResvHeaterComID.Text = MS.ResvHeaterCOMID.ToString();
            buttonResvBFluidComID.Text = MS.ResvBCOMID.ToString();
            buttonResvBHeaterComID.Text = MS.ResvBHeaterCOMID.ToString();

            groupBoxSetupOmegaChuck.Enabled = MS.ChuckTempControlEnabled;
            checkBoxChuckHeater.Checked = MS.ChuckTempControlEnabled;

            groupBoxSetupOmegaDie.Enabled = MS.DieTempControlEnabled;
            checkBoxDieHeater.Checked = MS.DieTempControlEnabled;

            groupBoxSetupOmegaResv.Enabled = MS.ReservoirTempControlEnabled;
            checkBoxResvHeater.Checked = MS.ReservoirTempControlEnabled;
            checkBoxResvLimitCtl.Checked = MS.ReservoirLimitControlEnabled;

            buttonSetupXKP.Text = MS.X_KP.ToString();
            buttonSetupXKI.Text = MS.X_KI.ToString();
            buttonSetupXKD.Text = MS.X_KD.ToString();
            buttonSetupXFV.Text = MS.X_FV.ToString();
            buttonSetupXFA.Text = MS.X_FA.ToString();
            buttonSetupXPL.Text = MS.X_PL.ToString();

            buttonSetupZRKP.Text = MS.ZR_KP.ToString();
            buttonSetupZRKI.Text = MS.ZR_KI.ToString();
            buttonSetupZRKD.Text = MS.ZR_KD.ToString();
            buttonSetupZRFV.Text = MS.ZR_FV.ToString();
            buttonSetupZRFA.Text = MS.ZR_FA.ToString();
            buttonSetupZRPL.Text = MS.ZR_PL.ToString();

            buttonSetupZLKP.Text = MS.ZL_KP.ToString();
            buttonSetupZLKI.Text = MS.ZL_KI.ToString();
            buttonSetupZLKD.Text = MS.ZL_KD.ToString();
            buttonSetupZLFV.Text = MS.ZL_FV.ToString();
            buttonSetupZLFA.Text = MS.ZL_FA.ToString();
            buttonSetupZLPL.Text = MS.ZL_PL.ToString();

            buttonSetupSyringePumpKP.Text = MS.SyringeA_KP.ToString();
            buttonSetupSyringePumpKI.Text = MS.SyringeA_KI.ToString();
            buttonSetupSyringePumpKD.Text = MS.SyringeA_KD.ToString();
            buttonSetupSyringePumpFV.Text = MS.SyringeA_FV.ToString();
            buttonSetupSyringePumpFA.Text = MS.SyringeA_FA.ToString();
            buttonSetupSyringePumpPL.Text = MS.SyringeA_PL.ToString();

            buttonSetupPOHPumpKP.Text = MS.POHA_KP.ToString();
            buttonSetupPOHPumpKI.Text = MS.POHA_KI.ToString();
            buttonSetupPOHPumpKD.Text = MS.POHA_KD.ToString();
            buttonSetupPOHPumpFV.Text = MS.POHA_FV.ToString();
            buttonSetupPOHPumpFA.Text = MS.POHA_FA.ToString();
            buttonSetupPOHPumpPL.Text = MS.POHA_PL.ToString();

            buttonSetupSyringeBKP.Text = MS.SyringeB_KP.ToString();
            buttonSetupSyringeBKI.Text = MS.SyringeB_KI.ToString();
            buttonSetupSyringeBKD.Text = MS.SyringeB_KD.ToString();
            buttonSetupSyringeBFV.Text = MS.SyringeB_FV.ToString();
            buttonSetupSyringeBFA.Text = MS.SyringeB_FA.ToString();
            buttonSetupSyringeBPL.Text = MS.SyringeB_PL.ToString();

            buttonSetupPOHBKP.Text = MS.POHB_KP.ToString();
            buttonSetupPOHBKI.Text = MS.POHB_KI.ToString();
            buttonSetupPOHBKD.Text = MS.POHB_KD.ToString();
            buttonSetupPOHBFV.Text = MS.POHB_FV.ToString();
            buttonSetupPOHBFA.Text = MS.POHB_FA.ToString();
            buttonSetupPOHBPL.Text = MS.POHB_PL.ToString();

            buttonSetupFKP.Text = MS.Loader_KP.ToString();
            buttonSetupFKI.Text = MS.Loader_KI.ToString();
            buttonSetupFKD.Text = MS.Loader_KD.ToString();
            buttonSetupFFV.Text = MS.Loader_FV.ToString();
            buttonSetupFFA.Text = MS.Loader_FA.ToString();
            buttonSetupFPL.Text = MS.Loader_PL.ToString();

            buttonSetupMaxXLoc.Text = MS.MaxXTravel.ToString("0.000");
            buttonSetupMaxZLoc.Text = MS.MaxZTravel.ToString("0.000");
            buttonSetupMaxLoaderPos.Text = MS.MaxLoaderTravel.ToString("0.000");
            buttonSetupDieLoadXPos.Text = MS.XDieLoadLoc.ToString("0.000");

            buttonSetupMaintXLoc.Text = MS.XMaintLoc.ToString("0.000");
            buttonSetupMaintZLoc.Text = MS.ZMaintLoc.ToString("0.000");

            buttonCrossbarWidth.Text = MS.CrossBarWidth.ToString();

            buttonMinAirPressure.Text = MS.MinAirPressure.ToString();
            buttonMinVac.Text = MS.MinVacuum.ToString();

            switch (MS.VacuumUnits)
            {
                case ("inHg"):
                    comboBoxVacUnits.SelectedIndex = 0;
                    break;
                case ("mBar"):
                    comboBoxVacUnits.SelectedIndex = 1;
                    break;
                case ("kPa"):
                    comboBoxVacUnits.SelectedIndex = 2;
                    break;
                case ("Torr"):
                    comboBoxVacUnits.SelectedIndex = 3;
                    break;
            }

            buttonPOHPistonSize.Text = MS.POHDiam.ToString("0.000");
            buttonPOHScrewPitch.Text = MS.POHScrewPitch.ToString("0.000");

            buttonPOHMotorGearCount.Text = MS.POHMotorGearTeeth.ToString("#");
            buttonPOHPistonGearCount.Text = MS.POHPistonGearTeeth.ToString("#");

            buttonRotaryValveBOffset.Text = MS.ValveBOffset.ToString();

            buttonZROffset.Text = MS.ZROffset.ToString("0.000");
            buttonZLOffset.Text = MS.ZLOffset.ToString("0.000");

            checkBoxAirKnife.Checked = MS.AirKnifeInstalled;
            checkBoxAirKnifeHeaterInstalled.Checked = MS.AirKnifeHeaterInstalled;
            buttonAirKnifeHeaterMaxTemperature.Text = _frmMain.MS.AirKnifeHeaterMaxTemperature.ToString("##0.#");
            buttonAirKnifeHeaterComID.Text = _frmMain.MS.AirKnifeHeaterCOMID.ToString();
            buttonGasHeaterComID.Text = _frmMain.MS.GasHeaterCOMID.ToString();
            buttonGasHeaterReaderComID.Text = _frmMain.MS.GasHeaterReaderCOMID.ToString();

            checkBoxIRInstalled.Checked = MS.IRLampInstalled;
            checkBoxIRInstalled.Checked = _frmMain.MS.IRLampInstalled;
            comboBoxIRCOM.Text = _frmMain.MS.IRCOMPort;
            buttonIRSlaveAddr.Text = _frmMain.MS.IRSlaveAddress.ToString("#0");

            buttonGT2SettleDelay.Text = MS.GT2SettleDelay.ToString();
            buttonAirPuffDelay.Text = MS.AirPuffDelay.ToString();
            buttonRotaryValveMoveDelay.Text = MS.RotaryValveMoveDelay.ToString();
            buttonAlignersDownDelay.Text = MS.AlignersDownDelay.ToString();
            buttonAlignersUpDelay.Text = MS.AlignersUpDelay.ToString();
            buttonLiftPinsDownDelay.Text = MS.LiftPinsDownDelay.ToString();
            buttonLiftPinsUpDelay.Text = MS.LiftPinsUpDelay.ToString();

            buttonPrimingPlateStart.Text = MS.PrimingPlateStart.ToString("0.0");
            buttonPrimingPlateEnd.Text = MS.PrimingPlateEnd.ToString("0.0");
            buttonChuckStart.Text = MS.ChuckStart.ToString("0.0");
            buttonChuckEnd.Text = MS.ChuckEnd.ToString("0.0");

            buttonMeasureStartPriming.Text = MS.MeasureZonePrimingStart.ToString("0.0");
            buttonMeasureEndPriming.Text = MS.MeasureZonePrimingEnd.ToString("0.0");
            buttonMeasureStartChuck.Text = MS.MeasureZoneChuckStart.ToString("0.0");
            buttonMeasureEndChuck.Text = MS.MeasureZoneChuckEnd.ToString("0.0");

            groupBoxModuleDefs.Enabled = MS.HasLoader;

            checkBoxHasStack.Checked = MS.HasStack;
            textBoxPLCIPAddr.Text = MS.PLCAddress;

            checkBoxModule1Present.Checked = MS.StackMod1Installed;
            checkBoxModule2Present.Checked = MS.StackMod2Installed;
            checkBoxModule3Present.Checked = MS.StackMod3Installed;

            comboBoxModule1Type.Text = ((NAbleEnums.StackModuleType)MS.StackMod1Type).ToString();
            comboBoxModule2Type.Text = ((NAbleEnums.StackModuleType)MS.StackMod2Type).ToString();
            comboBoxModule3Type.Text = ((NAbleEnums.StackModuleType)MS.StackMod3Type).ToString();

            textBoxModule1Name.Text = MS.StackMod1Name;
            textBoxModule2Name.Text = MS.StackMod2Name;
            textBoxModule3Name.Text = MS.StackMod3Name;

            if (MC.SyringePumpDetected)
            {
                tabControlPump.SelectedTab = tabPageSyringe;
                buttonSyringAServoCntrl.Enabled = true;
            }
            else
            {
                tabControlPump.SelectedTab = tabPagePOH;
                buttonPOHAServoCntrl.Enabled = true;
            }

            if (MC.SyringePumpBDetected)
            {
                tabControlPumpB.SelectedTab = tabPageSyringeB;
                buttonSyringeBServoCntrl.Enabled = true;
            }
            else
            {
                tabControlPumpB.SelectedTab = tabPagePOHB;
                buttonPOHBServoCntrl.Enabled = true;
            }

            labelFStatus.Text = MS.HasLoader ? "Status: Ready" : "Status: Offline";
            buttonFServoCntrl.Enabled = MS.HasLoader;
            buttonApplyFTuning.Enabled = MS.HasLoader;

            //Options
            checkBoxHideMixing.Checked = MS.HidePumpMixing;
            checkBoxDisableHeadPurge.Checked = MS.DisableHeadPurge;
            checkBoxDualPumpInstalled.Checked = MS.DualPumpInstalled;
            checkBoxHideHeadPurgeOnRunScr.Checked = MS.HideHeadPurgeOnRun;
            checkBoxHideRecircOnFCScr.Checked = MS.HideRecirculateOnFluidControl;
            checkBoxHideHeadPurgeOnFluidConScr.Checked = MS.HideHeadPurgeOnFluidControl;
            checkBoxUsingCognex.Checked = MS.CognexCommunicationsUsed;
            checkBoxRaspberryPiRotary.Checked=MS.HasRaspberryPiRotary;
            checkBoxXHasDoorInterlocks.Checked = MS.HasDoorInterlocks;
            checkBoxKeyenceLasersInstalled.Checked = MS.KeyenceLaserInstalled;
            checkBoxUsingLaser.Checked = MS.KeyenceLaserInstalled && MS.UsingKeyenceLaser;
            checkBoxAdvantechServerInstalled.Checked = MS.AdvantechServerInstalled;
            checkBoxHasResvSensors.Checked = MS.HasReservoirSensors;
            checkBoxHasLiftAndCenter.Checked = MS.HasLiftAndCenter;

            buttonCalibrationUpperHeight.Text = MS.LaserCalibrationUpperHeight.ToString();

            _loadingMachineSettings = false;
        }

        public void UpdateStatus()
        {
            if (LaserMgr != null)
            {
                LaserMgr.EnableAutoConnect = !Visible;
            }

            if (!Visible)
            {
                _bVisibleOneShot = true;
                return;
            }
            else if (_bVisibleOneShot)
            {
                _bVisibleOneShot = false;
                panelEHDI.Visible = MC.AxisCount > 4;
            }


            ShowOrHideTabs();

            buttonSetupSave.Visible = true;
            buttonSetupSave.Enabled = true;

            if (!MS.DualPumpInstalled)
            {
                if (!_oneShot)
                {
                    tabPageSetupPump.Text = MC.SyringePumpDetected ? "Syringe" : "POH";
                    tabControlSetup.TabPages.Remove(tabPageSetupPumpB);
                    _oneShot = true;
                }

                if (!MC.POHPumpDetected)
                {
                    tabControlPump.TabPages.Remove(tabPageSetupPOH);

                    if (!MS.DualPumpInstalled)
                    {
                        tabControlPumpB.TabPages.Remove(tabPagePOHB);
                    }
                }
                else
                {
                    tabControlPump.TabPages.Remove(tabPageSyringe);

                    if (!MS.DualPumpInstalled)
                    {
                        tabControlPumpB.TabPages.Remove(tabPageSyringeB);
                    }
                }
            }
            else
            {
                if (!_oneShot)
                {
                    tabPageSetupPumpB.Show();
                    tabPageSetupPump.Text = MC.SyringePumpDetected ? "Syringe-A" : "POH-A";
                    tabPageSetupPumpB.Text = MC.SyringePumpBDetected ? "Syringe-B" : "POH-B";
                    _oneShot = true;
                }

                if (!MC.POHPumpDetected)
                {
                    tabControlSetup.TabPages.Remove(tabPageSetupPOH);
                    if (!MS.DualPumpInstalled)
                    {
                        tabControlSetup.TabPages.Remove(tabPagePOHB);
                    }
                }
            }

            bool bConnected = (MC != null && MC.Connected) || _frmMain.IsDemoMode;

            buttonLock.Enabled = !_frmMain.UserIsAdmin;

            foreach (Button tmpButton in tabPageSetupOutputs1.Controls.OfType<Button>())
            {
                tmpButton.Enabled = bConnected;
            }

            groupBoxCDAVacSettings.Visible = Settings.Default.HasAnalogInputs;
            groupBoxAnalogs.Visible = Settings.Default.HasAnalogInputs;
            panelDigitalPressureSwitchs.Visible = !Settings.Default.HasAnalogInputs;

            groupBoxSetupXServo.Enabled = bConnected;
            groupBoxSetupLeftServo.Enabled = bConnected;
            groupBoxSetupRightServo.Enabled = bConnected;
            groupBoxSetupLoaderServo.Enabled = bConnected && MS.HasLoader;
            tabControlPump.Enabled = bConnected;
            tabControlPumpB.Enabled = bConnected;
            groupBoxRotaryValve.Enabled = bConnected && MC.SyringePumpDetected;
            groupBoxRotaryValveB.Enabled = bConnected && MC.SyringePumpBDetected;
            //buttonResetRotaryValve.Visible = bConnected && smartRotary && MC.ValvePosition == 7;
            panelDiePressure.Visible = Settings.Default.HasAnalogInputs;
            //labelValveStepPosition.Visible = MS.HasRaspberryPiRotary;
            //buttonRotaryValveSpeed.Visible = smartRotary;
            //labelRotaryValveSpeedTitle.Visible = smartRotary;

            groupBoxNewRotaryControl.Visible = MC.SyringePumpDetected && MS.HasRaspberryPiRotary;
            groupBoxRotaryValve.Visible = MC.SyringePumpDetected && !MS.HasRaspberryPiRotary;

            groupBoxNewRotaryControlB.Visible = MC.SyringePumpDetected && MS.HasRaspberryPiRotary;
            groupBoxRotaryValveB.Visible = MC.SyringePumpDetected && !MS.HasRaspberryPiRotary;

            groupBoxModuleDefs.Enabled = checkBoxHasStack.Checked;

            //==========================================================================================
            if (!Visible)
                return;

            pictureBoxThread0.BackColor = MC.Threads[0] ? Color.Lime : Color.Red;
            pictureBoxThread1.BackColor = MC.Threads[1] ? Color.Lime : Color.Red;
            pictureBoxThread2.BackColor = MC.Threads[2] ? Color.Lime : Color.Red;
            pictureBoxThread3.BackColor = MC.Threads[3] ? Color.Lime : Color.Red;
            pictureBoxThread4.BackColor = MC.Threads[4] ? Color.Lime : Color.Red;
            pictureBoxThread5.BackColor = MC.Threads[5] ? Color.Lime : Color.Red;
            pictureBoxThread6.BackColor = MC.Threads[6] ? Color.Lime : Color.Red;
            pictureBoxThread7.BackColor = MC.Threads[7] ? Color.Lime : Color.Red;

            labelLifetimeCountValue.Text = _frmMain.Storage.LifetimeCount.ToString("#,##0");

            labelStatus_dMem45.Text = $"0x{((int)MC.Memory[0]).ToString("X8")}";
            labelStatus_TC.Text = $"{MC.TC}";
            labelTCDesc.Text = GetTCDescription(MC.TC);
            pictureBoxELOLatched.BackColor = MC.ELOLatched ? Color.Lime : Color.Red;
            pictureBoxSlowDataPoll.BackColor = MC.NeedSlowPoll ? Color.Lime : Color.Red;
            labelDataUpdateRate.Text = string.Format("{0} ms", MC.NeedSlowPoll ? MS.DataUpdateRate * 2 : MS.DataUpdateRate); ;

            pictureBoxInputsDI1.BackColor = (MC.Inputs[1] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI2.BackColor = (MC.Inputs[2] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI3.BackColor = (MC.Inputs[3] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI4.BackColor = (MC.Inputs[4] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI5.BackColor = (MC.Inputs[5] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI6.BackColor = (MC.Inputs[6] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI7.BackColor = (MC.Inputs[7] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI8.BackColor = (MC.Inputs[8] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI9.BackColor = (MC.Inputs[9] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI10.BackColor = (MC.Inputs[10] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI11.BackColor = (MC.Inputs[11] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI12.BackColor = (MC.Inputs[12] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI13.BackColor = (MC.Inputs[13] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI14.BackColor = (MC.Inputs[14] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI15.BackColor = (MC.Inputs[15] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI16.BackColor = (MC.Inputs[16] == 0.0) ? Color.Lime : Color.Red;

            //Extended IO
            pictureBoxInputsDI41.BackColor = (MC.Inputs[41] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI42.BackColor = (MC.Inputs[42] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI43.BackColor = (MC.Inputs[43] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI44.BackColor = (MC.Inputs[44] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI45.BackColor = (MC.Inputs[45] == 0.0) ? Color.Lime : Color.Red;
            pictureBoxInputsDI46.BackColor = (MC.Inputs[46] == 0.0) ? Color.Lime : Color.Red;

            panelReservoirSensors.Visible = MS.HasReservoirSensors;
            if (MS.HasReservoirSensors)
            {
                pictureBoxInputsDI47.BackColor = (MC.Inputs[47] == 0.0) ? Color.Lime : Color.Red;
                pictureBoxInputsDI48.BackColor = (MC.Inputs[48] == 0.0) ? Color.Lime : Color.Red;
            }

            labelAnalog1Val.Text = MC.Analogs[1].ToString("0.000");
            labelAnalog2Val.Text = MC.Analogs[2].ToString("0.000");
            labelAnalog3Val.Text = MC.Analogs[3].ToString("0.000");
            labelAnalog4Val.Text = MC.Analogs[4].ToString("0.000");
            labelAnalog5Val.Text = MC.Analogs[5].ToString("0.000");
            labelAnalog6Val.Text = MC.Analogs[6].ToString("0.000");
            labelAnalog7Val.Text = MC.Analogs[7].ToString("0.000");
            labelAnalog8Val.Text = MC.Analogs[8].ToString("0.000");


            if (textBoxCfgOperatorAccessCode.Text == textBoxCfgEditorAccessCode.Text || textBoxCfgOperatorAccessCode.Text == textBoxCfgAdminAccessCode.Text)
                textBoxCfgOperatorAccessCode.BackColor = Color.Yellow;
            else
                textBoxCfgOperatorAccessCode.BackColor = ColorControl;

            if (textBoxCfgEditorAccessCode.Text == textBoxCfgOperatorAccessCode.Text || textBoxCfgEditorAccessCode.Text == textBoxCfgAdminAccessCode.Text)
                textBoxCfgEditorAccessCode.BackColor = Color.Yellow;
            else
                textBoxCfgEditorAccessCode.BackColor = ColorControl;

            if (textBoxCfgAdminAccessCode.Text == textBoxCfgOperatorAccessCode.Text || textBoxCfgAdminAccessCode.Text == textBoxCfgEditorAccessCode.Text)
                textBoxCfgAdminAccessCode.BackColor = Color.Yellow;
            else
                textBoxCfgAdminAccessCode.BackColor = ColorControl;

            pictureBoxRotaryValHLS.BackColor = pictureBoxInputsDI1.BackColor;
            pictureBoxRotaryValBHLS.BackColor = pictureBoxInputsDI9.BackColor;

            pictureBoxLoaderSubstrate.BackColor = (MS.HasLoader && MC.Inputs[6] == 0.0) ? Color.Lime : Color.Red;


            buttonOutputsDO1.BackColor = (MC.Outputs[1] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO2.BackColor = (MC.Outputs[2] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO3.BackColor = (MC.Outputs[3] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO4.BackColor = (MC.Outputs[4] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO5.BackColor = (MC.Outputs[5] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO6.BackColor = (MC.Outputs[6] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO7.BackColor = (MC.Outputs[7] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsDO8.BackColor = (MC.Outputs[8] == 1.0) ? Color.Lime : Color.Red;

            buttonOutputsIO17.BackColor = (MC.Outputs[17] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO18.BackColor = (MC.Outputs[18] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO19.BackColor = (MC.Outputs[19] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO20.BackColor = (MC.Outputs[20] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO21.BackColor = (MC.Outputs[21] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO22.BackColor = (MC.Outputs[22] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO23.BackColor = (MC.Outputs[23] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO24.BackColor = (MC.Outputs[24] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO25.BackColor = (MC.Outputs[25] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO26.BackColor = (MC.Outputs[26] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO27.BackColor = (MC.Outputs[27] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO28.BackColor = (MC.Outputs[28] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO29.BackColor = (MC.Outputs[29] == 1.0) ? Color.Lime : Color.Red;

            buttonOutputsIO30.BackColor = (MC.Outputs[30] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO32.BackColor = (MC.Outputs[32] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO33.BackColor = (MC.Outputs[33] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO34.BackColor = (MC.Outputs[34] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO35.BackColor = (MC.Outputs[35] == 1.0) ? Color.Lime : Color.Red;
            buttonOutputsIO36.BackColor = (MC.Outputs[36] == 1.0) ? Color.Lime : Color.Red;

            pictureBoxXFLS.BackColor = (MC.XFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxXRLS.BackColor = (MC.XRLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxZLeftFLS.BackColor = (MC.ZLFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxZLeftRLS.BackColor = (MC.ZLRLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxZRightFLS.BackColor = (MC.ZRFLS == 0.0) ? Color.Lime : Color.Red;
            pictureBoxZRightRLS.BackColor = (MC.ZRRLS == 0.0) ? Color.Lime : Color.Red;
            //TODO
            //pictureBoxFFLS.BackColor = (MC.FFLS == 0.0) ? Color.Lime : Color.Red;
            //pictureBoxFRLS.BackColor = (MC.FRLS == 0.0) ? Color.Lime : Color.Red;

            pictureBoxZRightBrake.BackColor = MC.ZRightBrakeReleased ? Color.Lime : Color.Red;
            labelZRightBrake.Text = MC.ZRightBrakeReleased ? "Brake Released" : "Brake Engaged";

            pictureBoxZLeftBrake.BackColor = MC.ZLeftBrakeReleased ? Color.Lime : Color.Red;
            labelZLeftBrake.Text = MC.ZLeftBrakeReleased ? "Brake Released" : "Brake Engaged";


            labelXMainEncValue.Text = MC.X_TP.ToString("0");
            labelHallsAValue.Text = MC.HallsA.ToString("0");
            labelZLeftMainEncValue.Text = MC.LZ_TP.ToString("0");
            labelZLeftDualEncValue.Text = MC.LZ_TD.ToString();
            labelZRightMainEncValue.Text = MC.RZ_TP.ToString("0");
            labelZRightDualEncValue.Text = MC.RZ_TD.ToString("0");
            labelFMainEncValue.Text = MC.TPH.ToString("0");
            labelHallsFValue.Text = MC.HallsH.ToString("0");

            if (!MS.HasRaspberryPiRotary)
            {
                lableRotaryValveMainEncValue.Text = MC.PumpB_TD.ToString();
                //_dAngle = (_frmMain.PLC.TDE / (2000 / 360));
                _angle = (MC.PumpB_TD * 0.18);
                lableRotaryValveAngleValue.Text = _angle.ToString("0.0");
                lableRotaryStepperPosValue.Text = ((int)(MC.PumpB_TP)).ToString();
                switch (MC.ValveHomingStatus)
                {
                    case 0: _tempStr = "Status: Not Ready"; break;
                    case 1: _tempStr = "Status: Homing"; break;
                    case 2: _tempStr = "Status: Ready"; break;
                    default: _tempStr = "Status: Unknown"; break;
                }
                labelRotaryValveStatus.Text = _tempStr;
            }

            labelSmartVlvAStatus.Text = MC.ValveStatusName;
            buttonVlvVent.Enabled = MC.ValveACmdReady;
            buttonVlvOff.Enabled = MC.ValveACmdReady;
            buttonVlvRecharge.Enabled = MC.ValveACmdReady;
            buttonVlvDispense.Enabled = MC.ValveACmdReady;
            buttonRehomeVlvA.Enabled = MC.ValveACmdReady;

            lableRotaryValveBMainEncValue.Text = MC.TDG.ToString();
            //_dAngle = (_frmMain.PLC.TDE / (2000 / 360));
            _angle = (MC.TDG * 0.18);
            lableRotaryValveBAngleValue.Text = _angle.ToString("0.0");
            lableRotaryStepperBPosValue.Text = ((int)(MC.TPG)).ToString();

            switch (MC.ValveBHomingStatus)
            {
                case 0: _tempStr = "Status: Not Ready"; break;
                case 1: _tempStr = "Status: Homing"; break;
                case 2: _tempStr = "Status: Ready"; break;
                default: _tempStr = "Status: Unknown"; break;
            }

            labelRotaryValveBStatus.Text = _tempStr;
            labelCurValBPos.Text = "Valve Pos: " + MC.ValveBStatusName;
            labelSmartVlvBStatus.Text = MC.ValveBStatusName;
            buttonVlvVentB.Enabled = MC.ValveBCmdReady;
            buttonVlvOffB.Enabled = MC.ValveBCmdReady;
            buttonVlvRechargeB.Enabled = MC.ValveBCmdReady;
            buttonVlvDispenseB.Enabled = MC.ValveBCmdReady;
            buttonRehomeVlvB.Enabled = MC.ValveBCmdReady;

            labellGT2posValue.Text = !MS.UsingKeyenceLaser ? _frmMain.Storage.LeftGT2pos.ToString("0.000") : "-----";
            labelrGT2posValue.Text = !MS.UsingKeyenceLaser ? _frmMain.Storage.RightGT2pos.ToString("0.000") : "-----";

            buttonAirKnifeHeaterComID.BackColor = int.Parse(buttonAirKnifeHeaterComID.Text) == _frmMain.MS.AirKnifeHeaterCOMID ? Color.White : Color.Yellow;
            buttonGasHeaterComID.BackColor = int.Parse(buttonGasHeaterComID.Text) == _frmMain.MS.GasHeaterCOMID ? Color.White : Color.Yellow;
            buttonGasHeaterReaderComID.BackColor = int.Parse(buttonGasHeaterReaderComID.Text) == _frmMain.MS.GasHeaterReaderCOMID ? Color.White : Color.Yellow;
            buttonAirKnifeHeaterMaxTemperature.BackColor = double.Parse(buttonAirKnifeHeaterMaxTemperature.Text) == _frmMain.MS.AirKnifeHeaterMaxTemperature ? Color.White : Color.Yellow;

            buttonXServoCntrl.Text = MC.X_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonXServoCntrl.BackColor = MC.X_MotorOff ? ColorDarkControl : Color.Lime;
            buttonZRServoCntrl.Text = MC.RZ_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonZRServoCntrl.BackColor = MC.RZ_MotorOff ? ColorDarkControl : Color.Lime;
            buttonZLServoCntrl.Text = MC.LZ_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonZLServoCntrl.BackColor = MC.LZ_MotorOff ? ColorDarkControl : Color.Lime;
            buttonSyringAServoCntrl.Text = MC.PumpA_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonSyringAServoCntrl.BackColor = MC.PumpA_MotorOff ? ColorDarkControl : Color.Lime;
            buttonPOHAServoCntrl.Text = MC.PumpA_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonPOHAServoCntrl.BackColor = MC.PumpA_MotorOff ? ColorDarkControl : Color.Lime;
            buttonRotaryServoCntrl.Text = MC.PumpB_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonRotaryServoCntrl.BackColor = MC.PumpB_MotorOff ? ColorDarkControl : Color.Lime;
            buttonSyringeBServoCntrl.Text = MC.PumpB_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonSyringeBServoCntrl.BackColor = MC.PumpB_MotorOff ? ColorDarkControl : Color.Lime;
            buttonPOHBServoCntrl.Text = MC.PumpB_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonPOHBServoCntrl.BackColor = MC.PumpB_MotorOff ? ColorDarkControl : Color.Lime;
            buttonRotaryBServoCntrl.Text = MC.PumpB_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonRotaryBServoCntrl.BackColor = MC.PumpB_MotorOff ? ColorDarkControl : Color.Lime;

            buttonFServoCntrl.Text = MC.Loader_MotorOff ? "Servo Is Off" : "Servo Is On";
            buttonFServoCntrl.BackColor = MC.Loader_MotorOff ? ColorDarkControl : Color.Lime;

            if (MC.SyringePumpDetected)
            {
                labelPumpMainEncValue.Text = MC.PumpA_TP.ToString("0");
                pictureBoxPumpFLS.BackColor = (MC.PumpFLS == 0.0) ? Color.Lime : Color.Red;
                pictureBoxPumpRLS.BackColor = (MC.PumpRLS == 0.0) ? Color.Lime : Color.Red;
            }
            else if (MC.POHPumpDetected)
            {
                labelPOHPumpMainEncValue.Text = MC.PumpA_TP.ToString("0");
                pictureBoxPOHPumpFLS.BackColor = (MC.PumpFLS == 0.0) ? Color.Lime : Color.Red;
                pictureBoxPOHPumpRLS.BackColor = (MC.PumpRLS == 0.0) ? Color.Lime : Color.Red;
            }
            else
            {
                labelPOHPumpMainEncValue.Text = "0";
                pictureBoxPOHPumpFLS.BackColor = Color.Red;
                pictureBoxPOHPumpRLS.BackColor = Color.Red;
            }

            if (MC.SyringePumpBDetected)
            {
                labelSyringeBMainEncValue.Text = MC.PumpB_TP.ToString("0");
                pictureBoxSyringeBFLS.BackColor = (MC.PumpBFLS == 0.0) ? Color.Lime : Color.Red;
                pictureBoxSyringeBRLS.BackColor = (MC.PumpBRLS == 0.0) ? Color.Lime : Color.Red;
            }
            else if (MC.POHPumpBDetected)
            {
                labelPOHBMainEncValue.Text = MC.PumpB_TP.ToString("0");
                pictureBoxPOHBFLS.BackColor = (MC.PumpBFLS == 0.0) ? Color.Lime : Color.Red;
                pictureBoxPOHBRLS.BackColor = (MC.PumpBRLS == 0.0) ? Color.Lime : Color.Red;
            }
            else
            {
                labelPOHBMainEncValue.Text = "0";
                pictureBoxPOHBFLS.BackColor = Color.Red;
                pictureBoxPOHBRLS.BackColor = Color.Red;
            }

            labelDieLoadLZPos.Text = MS.ZLDieLoadLoc.ToString("0.000");
            labelDieLoadRZPos.Text = MS.ZRDieLoadLoc.ToString("0.000");

            if (checkBoxChuckHeater.Checked)
            {
                labelChuckTempValue.Text = Heaters.Temp(MS.ChuckCOMID);
                labelChuckTempValue.ForeColor = Heaters.TempColor(MS.ChuckCOMID);
                labelChuckSetPointValue.Text = $"{Heaters.SetPoint(MS.ChuckCOMID):0.0}";
            }

            if (checkBoxDieHeater.Checked)
            {
                labelDieTempValue.Text = Heaters.Temp(MS.DieCOMID);
                labelDieTempValue.ForeColor = Heaters.TempColor(MS.DieCOMID);
                labelDieSetPointValue.Text = $"{Heaters.SetPoint(MS.DieCOMID):0.0}";
            }

            if (checkBoxResvHeater.Checked)
            {
                labelResvHeaterTempValue.Text = Heaters.Temp(MS.ResvCOMID);
                labelResvHeaterTempValue.ForeColor = Heaters.TempColor(MS.ResvCOMID);
                labelResvHeaterSetPointValue.Text = $"{Heaters.SetPoint(MS.ResvCOMID):0.0}";
            }

            labelLZPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrLZ / MC.mmConv):0.000}";
            labelLZMaxPosError.Text = $"Max Err: {Math.Abs(MC.LZMaxPosErr):0.000}";
            labelRZPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrRZ / MC.mmConv):0.000}";
            labelRZMaxPosError.Text = $"Max Err: {Math.Abs(MC.RZMaxPosErr):0.000}";
            labelXPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrX / MC.mmConv):0.000}";
            labelXMaxPosError.Text = $"Max Err: {Math.Abs(MC.XMaxPosErr):0.000}";
            labelSyringePosError.Text = $"Pos Err: {Math.Abs(MC.PosErrPumpA):0}";
            labelSyringeMaxPosError.Text = $"Max Err: {Math.Abs(MC.PumpMaxPosErr):0.000}";
            labelPOHPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrPumpA):0}";
            labelPOHMaxPosError.Text = $"Max Err: {Math.Abs(MC.PumpMaxPosErr):0.000}";
            labelSyringeBPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrPumpB):0}";
            labelSyringeBMaxPosError.Text = $"Max Err: {Math.Abs(MC.PumpBMaxPosErr):0.000}";
            labelPOHBPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrPumpB):0}";
            labelPOHBMaxPosError.Text = $"Max Err: {Math.Abs(MC.PumpBMaxPosErr):0.000}";
            labelFPosError.Text = $"Pos Err: {Math.Abs(MC.PosErrLoader / MC.mmConvTransfer):0.000}";
            labelFMaxPosError.Text = $"Max Err: {Math.Abs(MC.LoaderMaxPosErr):0.000}";

            //Locations
            //labelSetupMaxLoaderPos.Visible = MS.HasLoader; - TODO
            buttonSetupMaxLoaderPos.Visible = MS.HasLoader;
            //labelSetupMaxLoaderPosUnits.Visible = MS.HasLoader; - TODO

            //Delays
            //groupBoxAlignerDelays.Visible = MS.AlignersEnabled; - TODO
            //groupBoxLPDelays.Visible = MS.LiftPinsEnabled; = TODO

            //Keyence Laser Stuff
            bool swOK = LaserMgr != null;

            if (MS.UsingKeyenceLaser && swOK)
            {
                pictureBoxLaserSWOK.BackColor = swOK ? Color.Lime : Color.Red;
                pbLaserConnected.BackColor = LaserMgr.Connected ? Color.Lime : Color.Red;
                btnConnect.BackColor = LaserMgr.Connected ? Color.Lime : Color.FromKnownColor(KnownColor.ControlDark);
                btnConnect.Text = LaserMgr.Connected ? "Disconnect" : "Connect";

                pbError.BackColor = LaserMgr.Error ? Color.Red : Color.FromKnownColor(KnownColor.ControlDark);

                labelErrorCode.Text = $"{LaserMgr.ErrorCode}";

                if (LaserMgr.GetLeftAndRightLaserData(out var left, out var right))
                {
                    groupBoxHead1Status.Visible = LaserMgr.Connected && left != null;

                    if (left != null)
                    {
                        labelHead1.Text = left.ValueAsString;
                        pictureBoxGO.BackColor = left.Go ? Color.Lime : Color.Red;
                        pictureBoxHI.BackColor = left.Hi ? Color.Lime : Color.Red;
                        pictureBoxLO.BackColor = left.Lo ? Color.Lime : Color.Red;
                        pictureBoxAlarm.BackColor = left.Alarm ? Color.Lime : Color.Red;
                        pictureBoxWaiting.BackColor = left.Waiting ? Color.Lime : Color.Red;
                        pictureBoxInvalid.BackColor = left.Invalid ? Color.Lime : Color.Red;
                    }

                    groupBoxHead2Status.Visible = LaserMgr.Connected && right != null && MS.NumberOfLasers > 1;

                    if (right != null)
                    {
                        labelHead2.Text = right.ValueAsString;
                        pictureBoxGO2.BackColor = right.Go ? Color.Lime : Color.Red;
                        pictureBoxHI2.BackColor = right.Hi ? Color.Lime : Color.Red;
                        pictureBoxLO2.BackColor = right.Lo ? Color.Lime : Color.Red;
                        pictureBoxAlarm2.BackColor = right.Alarm ? Color.Lime : Color.Red;
                        pictureBoxWaiting2.BackColor = right.Waiting ? Color.Lime : Color.Red;
                        pictureBoxInvalid2.BackColor = right.Invalid ? Color.Lime : Color.Red;
                    }

                    comboBoxPrgmNum.Visible = true;
                    comboBoxPrgmNum.Enabled = !LaserMgr.WaitingForProgramNumber;

                    if (!comboBoxPrgmNum.DroppedDown)
                    {
                        comboBoxPrgmNum.SelectedIndex = LaserMgr.ProgramNumber;
                    }

                    buttonCalibrationUpperHeight.BackColor = double.Parse(buttonCalibrationUpperHeight.Text) == MS.LaserCalibrationUpperHeight 
                        ? Color.FromKnownColor(KnownColor.ControlDark) : Color.Yellow;
                }
            }
            else
            {
                comboBoxPrgmNum.Visible = false;
            }

            checkBoxHideMixing.Enabled = MS.DualPumpInstalled || checkBoxDualPumpInstalled.Checked;

            groupBoxDiePressure.Visible = MS.HasDiePressureTransducer;

            if (MS.HasDiePressureTransducer)
            {
                switch (_pressDisplayType)
                {
                    case 0:
                    {
                        groupBoxDiePressure.Text = "Die Pressure Avg.";
                        labelDiePressure.Text = $"{MC.DiePressurePSISmoothed.ToString("0.00")} PSI";
                    }
                    break;
                    case 1:
                    {
                        groupBoxDiePressure.Text = "Die Pressure Avg.";
                        labelDiePressure.Text = $"{MC.DiePressureVDCSmoothed.ToString("0.000")} VDC";
                    }
                    break;
                    case 2:
                    {
                        groupBoxDiePressure.Text = "Die Pressure";
                        labelDiePressure.Text = $"{MC.DiePressurePSI.ToString("0.00")} PSI";
                    }
                    break;
                    case 3:
                    {
                        groupBoxDiePressure.Text = "Die Pressure";
                        labelDiePressure.Text = $"{MC.Analogs[8].ToString("0.000")} VDC";
                    }
                    break;
                }
            }
        }

        private void buttonSetupConnect_Click(object sender, EventArgs e)
        {
            bool isValidIP = IPAddress.TryParse(labelSetupIP.Text, out IPAddress addrTemp);

            if (isValidIP)
            {
                _frmMain.StartConnecting();
            }
            else
            {
                nRadMessageBox.Show(this, "Improper format.  Please check the specified address.", "Connect Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDigitalOutputs_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            int nNum = int.Parse(((Control)sender).Tag.ToString());
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed (toggle) Output Button: DO{nNum.ToString("00,2")}", "Action");
            MC.ToggleDigitalOut(nNum);
        }

        private void buttonIOOutputs_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            int nNum = int.Parse(((Control)sender).Tag.ToString());
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed (toggle) Output Button: IO{nNum.ToString("00,2")}", "Action");
            MC.ToggleIO(nNum);
        }

        private void checkBoxChuckHeater_CheckedChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            groupBoxSetupOmegaChuck.Enabled = checkBoxChuckHeater.Checked;
        }
        private void buttonChuckComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Chuck Temperature Controller ID", this, buttonChuckComID, "0", 0, 7, "", false);
        }

        private void checkBoxDieHeater_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSetupOmegaDie.Enabled = checkBoxDieHeater.Checked;
        }
        private void buttonDieComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die Temperature Controller ID", this, buttonDieComID, "0", 0, 7, "", false);
        }

        private void checkBoxResvHeater_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxSetupOmegaResv.Enabled = checkBoxResvHeater.Checked;
        }
        private void buttonResvFluidComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Fluid Reservoir-A Temperature Controller ID", this, buttonResvFluidComID, "0", 0, 7, "", false);
        }
        private void buttonResvHeaterComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Fluid Reservoir-A Heater Controller ID", this, buttonResvHeaterComID, "0", 0, 7, "", false);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to close?  Any unsaved changes will be discarded.", "Exit Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                PopuplateControlsFromMachineSettings();
                _frmMain.ShowPrevForm();
            }
        }

        private void buttonSetupSave_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            // see if the 485 com ports match
            if ((checkBoxChuckHeater.Checked && checkBoxDieHeater.Checked && comboBoxChuckComPort.Text != comboBoxDieComPort.Text) ||
                (checkBoxDieHeater.Checked && checkBoxResvHeater.Checked && comboBoxDieComPort.Text != comboBoxResvComPort.Text) ||
                (checkBoxResvHeater.Checked && checkBoxChuckHeater.Checked && comboBoxResvComPort.Text != comboBoxChuckComPort.Text))
            {
                nRadMessageBox.Show(this, "The COM ports for the heater controls must match.", "Settings Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (buttonZROffset.Text != "0.000" && buttonZLOffset.Text != "0.000")
            {
                nRadMessageBox.Show(this, "Only one Z Offset may be non-zero.  Please adjust Z Offsets", "Settings Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes != nRadMessageBox.Show(this, "Ready to save setup information?", "Save Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                return;
            }

            //Reset Dirty Location Buttons
            buttonXVisionPos.BackColor = Color.FromKnownColor(KnownColor.Control);
            buttonZVisionPos.BackColor = Color.FromKnownColor(KnownColor.Control);

            MS.EnableLooperMode = checkBoxEnableLooper.Checked;
            MC.IsDemo = checkBoxDemoMode.Checked;

            //Locations
            MS.VisionXPos = double.Parse(buttonXVisionPos.Text);
            MS.VisionZPos = double.Parse(buttonZVisionPos.Text);


            MS.OperatorPW = textBoxCfgOperatorAccessCode.Text;
            MS.EditorPW = textBoxCfgEditorAccessCode.Text;
            MS.AdminPW = textBoxCfgAdminAccessCode.Text;

            // X Axis control
            MS.X_KP = int.Parse(buttonSetupXKP.Text);
            MS.X_KI = double.Parse(buttonSetupXKI.Text);
            MS.X_KD = int.Parse(buttonSetupXKD.Text);
            MS.X_FV = double.Parse(buttonSetupXFV.Text);
            MS.X_FA = double.Parse(buttonSetupXFA.Text);
            MS.X_PL = double.Parse(buttonSetupXPL.Text);

            MS.ZR_KP = int.Parse(buttonSetupZRKP.Text);
            MS.ZR_KI = double.Parse(buttonSetupZRKI.Text);
            MS.ZR_KD = int.Parse(buttonSetupZRKD.Text);
            MS.ZR_FV = double.Parse(buttonSetupZRFV.Text);
            MS.ZR_FA = double.Parse(buttonSetupZRFA.Text);
            MS.ZR_PL = double.Parse(buttonSetupZRPL.Text);

            MS.ZL_KP = int.Parse(buttonSetupZLKP.Text);
            MS.ZL_KI = double.Parse(buttonSetupZLKI.Text);
            MS.ZL_KD = int.Parse(buttonSetupZLKD.Text);
            MS.ZL_FV = double.Parse(buttonSetupZLFV.Text);
            MS.ZL_FA = double.Parse(buttonSetupZLFA.Text);
            MS.ZL_PL = double.Parse(buttonSetupZLPL.Text);

            MS.SyringeA_KP = int.Parse(buttonSetupSyringePumpKP.Text);
            MS.SyringeA_KI = double.Parse(buttonSetupSyringePumpKI.Text);
            MS.SyringeA_KD = int.Parse(buttonSetupSyringePumpKD.Text);
            MS.SyringeA_FV = double.Parse(buttonSetupSyringePumpFV.Text);
            MS.SyringeA_FA = double.Parse(buttonSetupSyringePumpFA.Text);
            MS.SyringeA_PL = double.Parse(buttonSetupSyringePumpPL.Text);

            MS.POHA_KP = int.Parse(buttonSetupPOHPumpKP.Text);
            MS.POHA_KI = double.Parse(buttonSetupPOHPumpKI.Text);
            MS.POHA_KD = int.Parse(buttonSetupPOHPumpKD.Text);
            MS.POHA_FV = double.Parse(buttonSetupPOHPumpFV.Text);
            MS.POHA_FA = double.Parse(buttonSetupPOHPumpFA.Text);
            MS.POHA_PL = double.Parse(buttonSetupPOHPumpPL.Text);

            MS.SyringeB_KP = int.Parse(buttonSetupSyringeBKP.Text);
            MS.SyringeB_KI = double.Parse(buttonSetupSyringeBKI.Text);
            MS.SyringeB_KD = int.Parse(buttonSetupSyringeBKD.Text);
            MS.SyringeB_FV = double.Parse(buttonSetupSyringeBFV.Text);
            MS.SyringeB_FA = double.Parse(buttonSetupSyringeBFA.Text);
            MS.SyringeB_PL = double.Parse(buttonSetupSyringeBPL.Text);

            MS.POHB_KP = int.Parse(buttonSetupPOHBKP.Text);
            MS.POHB_KI = double.Parse(buttonSetupPOHBKI.Text);
            MS.POHB_KD = int.Parse(buttonSetupPOHBKD.Text);
            MS.POHB_FV = double.Parse(buttonSetupPOHBFV.Text);
            MS.POHB_FA = double.Parse(buttonSetupPOHBFA.Text);
            MS.POHB_PL = double.Parse(buttonSetupPOHBPL.Text);

            MS.Loader_KP = int.Parse(buttonSetupFKP.Text);
            MS.Loader_KI = double.Parse(buttonSetupFKI.Text);
            MS.Loader_KD = int.Parse(buttonSetupFKD.Text);
            MS.Loader_FV = double.Parse(buttonSetupFFV.Text);
            MS.Loader_FA = double.Parse(buttonSetupFFA.Text);
            MS.Loader_PL = double.Parse(buttonSetupFPL.Text);


            MS.ValveBOffset = double.Parse(buttonRotaryValveBOffset.Text);

            MS.ChuckTempControlEnabled = checkBoxChuckHeater.Checked;
            MS.HasPumpFlowMeters = checkBoxHasFlowMEter.Checked;
            MS.DieTempControlEnabled = checkBoxDieHeater.Checked;
            MS.ReservoirTempControlEnabled = checkBoxResvHeater.Checked;
            MS.ReservoirLimitControlEnabled = checkBoxResvLimitCtl.Checked;
            MS.DisableHeadPurge = checkBoxDisableHeadPurge.Checked;
            MS.KeyenceCOMPort = comboBoxKeyenceComPort.Text;
            MS.DieCOMPort = comboBoxDieComPort.Text;
            MS.DieCOMID = int.Parse(buttonDieComID.Text);
            MS.ChuckCOMPort = comboBoxChuckComPort.Text;
            MS.ChuckCOMID = int.Parse(buttonChuckComID.Text);
            MS.ResvCOMPort = comboBoxResvComPort.Text;
            MS.ResvCOMID = int.Parse(buttonResvFluidComID.Text);
            MS.ResvHeaterCOMPort = comboBoxResvComPort.Text;
            MS.ResvHeaterCOMID = int.Parse(buttonResvHeaterComID.Text);
            MS.AirKnifeHeaterCOMID = int.Parse(buttonAirKnifeHeaterComID.Text);
            MS.GasHeaterCOMID = int.Parse(buttonGasHeaterComID.Text);
            MS.GasHeaterReaderCOMID = int.Parse(buttonGasHeaterReaderComID.Text);
            MS.AirKnifeHeaterMaxTemperature = double.Parse(buttonAirKnifeHeaterMaxTemperature.Text);
            MS.IRLampInstalled = checkBoxIRInstalled.Checked;
            MS.IRCOMPort = comboBoxIRCOM.Text;
            MS.IRSlaveAddress = int.Parse(buttonIRSlaveAddr.Text);

            MS.MaxXTravel = double.Parse(buttonSetupMaxXLoc.Text);
            MS.MaxZTravel = double.Parse(buttonSetupMaxZLoc.Text);
            MS.MaxLoaderTravel = double.Parse(buttonSetupMaxLoaderPos.Text);

            MS.XMaintLoc = double.Parse(buttonSetupMaintXLoc.Text);
            MS.ZMaintLoc = double.Parse(buttonSetupMaintZLoc.Text);

            MS.XDieLoadLoc = double.Parse(buttonSetupDieLoadXPos.Text);

            MS.CrossBarWidth = double.Parse(buttonCrossbarWidth.Text);

            MS.GT2SettleDelay = int.Parse(buttonGT2SettleDelay.Text);
            MS.AirPuffDelay = int.Parse(buttonAirPuffDelay.Text);
            MS.RotaryValveMoveDelay = int.Parse(buttonRotaryValveMoveDelay.Text);
            MS.AlignersDownDelay = int.Parse(buttonAlignersDownDelay.Text);
            MS.AlignersUpDelay = int.Parse(buttonAlignersUpDelay.Text);
            MS.LiftPinsDownDelay = int.Parse(buttonLiftPinsDownDelay.Text);
            MS.LiftPinsUpDelay = int.Parse(buttonLiftPinsUpDelay.Text);

            MS.MinAirPressure = double.Parse(buttonMinAirPressure.Text);
            MS.MinVacuum = double.Parse(buttonMinVac.Text);

            MS.POHDiam = double.Parse(buttonPOHPistonSize.Text);
            MS.POHScrewPitch = double.Parse(buttonPOHScrewPitch.Text);

            MS.POHMotorGearTeeth = int.Parse(buttonPOHMotorGearCount.Text);
            MS.POHPistonGearTeeth = int.Parse(buttonPOHPistonGearCount.Text);

            MS.ZROffset = double.Parse(buttonZROffset.Text);
            MS.ZLOffset = double.Parse(buttonZLOffset.Text);

            MS.PrimingPlateStart = double.Parse(buttonPrimingPlateStart.Text);
            MS.PrimingPlateEnd = double.Parse(buttonPrimingPlateEnd.Text);
            MS.ChuckStart = double.Parse(buttonChuckStart.Text);
            MS.ChuckEnd = double.Parse(buttonChuckEnd.Text);

            MS.MeasureZonePrimingStart = double.Parse(buttonMeasureStartPriming.Text);
            MS.MeasureZonePrimingEnd = double.Parse(buttonMeasureEndPriming.Text);
            MS.MeasureZoneChuckStart = double.Parse(buttonMeasureStartChuck.Text);
            MS.MeasureZoneChuckEnd = double.Parse(buttonMeasureEndChuck.Text);

            MS.HasLoader = checkBoxHasLoader.Checked;
            MS.SafeCarriageLoc = double.Parse(buttonCarriageSafeLoc.Text);
            MS.SafeLoaderLoc = double.Parse(buttonLoaderSafeLoc.Text);
            MS.HasStack = checkBoxHasStack.Checked;
            MS.PLCAddress = textBoxPLCIPAddr.Text;
            MS.StackMod1Installed = checkBoxModule1Present.Checked;
            MS.StackMod2Installed = checkBoxModule2Present.Checked;
            MS.StackMod3Installed = checkBoxModule3Present.Checked;

            MS.StackMod1Type = (int)(NAbleEnums.StackModuleType)Enum.Parse(typeof(NAbleEnums.StackModuleType), comboBoxModule1Type.Text, true);
            MS.StackMod2Type = (int)(NAbleEnums.StackModuleType)Enum.Parse(typeof(NAbleEnums.StackModuleType), comboBoxModule2Type.Text, true);
            MS.StackMod3Type = (int)(NAbleEnums.StackModuleType)Enum.Parse(typeof(NAbleEnums.StackModuleType), comboBoxModule3Type.Text, true);

            MS.StackMod1Name = textBoxModule1Name.Text;
            MS.StackMod2Name = textBoxModule2Name.Text;
            MS.StackMod3Name = textBoxModule3Name.Text;

            MS.AlignersEnabled = checkBoxSetupAligners.Checked;
            MS.LiftPinsEnabled = checkBoxSetupLiftPins.Checked;
            MS.DualZoneLiftPinsEnabled = checkBoxSetupDualZoneLiftPins.Checked;
            MS.LiftPinsDuringPrep2Load = checkBoxLiftPinsPrep2Load.Checked;
            MS.AutoLDULD = checkBoxHasAutoLDULD.Checked;
            MS.HasPrimingArea = checkBoxHasPrimingArea.Checked;

            MS.HasPrimingPlate = checkBoxSetupHasPrimingPlate.Checked;
            MS.HasDiePressureTransducer = checkBoxDiePressureTransducer.Checked;
            MS.DiePressureInputVoltageAdjust = double.Parse(buttonDiePressVAdjustment.Text);
            MS.UsesSelectiveAirVacZones = checkBoxSelectiveZones.Checked;
            MS.NumChuckAirVacZones = int.Parse(buttonChuckNumZones.Text);
            MS.AirKnifeInstalled = checkBoxAirKnife.Checked;
            MS.AirKnifeHeaterInstalled = checkBoxAirKnifeHeaterInstalled.Checked;
            MS.IRLampInstalled = checkBoxIRInstalled.Checked;

            MS.HideHeadPurgeOnRun = checkBoxHideHeadPurgeOnRunScr.Checked;
            MS.HideHeadPurgeOnFluidControl = checkBoxHideHeadPurgeOnFluidConScr.Checked;
            MS.HideRecirculateOnFluidControl = checkBoxHideRecircOnFCScr.Checked;
            MS.HidePumpMixing = checkBoxHideMixing.Checked;
            MS.AllowPumpMixing = !checkBoxHideMixing.Checked;
            MS.DualPumpInstalled = checkBoxDualPumpInstalled.Checked;
            MC.RunCommand($"smrtvlv={(checkBoxRaspberryPiRotary.Checked ? 1 : 0)}");

            //Keyence GT2 Settings
            MS.KeyenceCOMPort = comboBoxKeyenceComPort.Text;

            //Keyence Laser
            MS.KeyenceLaserInstalled = checkBoxKeyenceLasersInstalled.Checked;
            MS.UsingKeyenceLaser = checkBoxUsingLaser.Checked && checkBoxKeyenceLasersInstalled.Checked;
            MS.LaserCalibrationUpperHeight = double.Parse(buttonCalibrationUpperHeight.Text);

            //***************************************
            //if (MS.KeyenceLaserInstalled)
            //{
            //    MS.KeyenceLaserIP = ipAddressInputLasers.Value;
            //    int nTemp = 0;
            //    int.TryParse(labelCurProgram.Text, out nTemp);
            //    MS.KeyenceLaserProgramNumber = nTemp;

            //    if (LaserMgr != null && MC.Connected && MS.UsingKeyenceLaser && LaserMgr.Connected)
            //    {
            //        MC.SetMemoryVal(15, 1);
            //        MS.NumberOfLasers = int.Parse(comboBoxNumOfLasers.Text);
            //        MS.KeyenceLaserProgramNumber = int.Parse(comboBoxPrgmNum.Text);
            //    }
            //}

            //Dual-Pump Option
            MC.RunCommand($"gMem[19]={(MS.DualPumpInstalled ? 1.0 : 0.0)}");
            if (!MS.DualPumpInstalled)
                MC.RunCommand("params[110]=0");

            // AdvantechServer
            MS.AdvantechServerInstalled = checkBoxAdvantechServerInstalled.Checked;

            //Door Interlocks
            MS.HasDoorInterlocks = checkBoxXHasDoorInterlocks.Checked;

            //Options
            MS.CognexCommunicationsUsed = checkBoxUsingCognex.Checked;
            MS.HasRaspberryPiRotary = checkBoxRaspberryPiRotary.Checked;
            MS.HasReservoirSensors = checkBoxHasResvSensors.Checked;
            MS.HasLiftAndCenter = checkBoxHasLiftAndCenter.Checked;

            if (!MS.Save())
            {
                nRadMessageBox.Show(this, "Settings could not be saved: \r\n" + MS.LastError, "Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (dualPumpRecentlyChanged) //Tell Galil About Dual-Pump Installed If Recent Change
                    MC.RunCommand($"devices[7]=" + (MS.DualPumpInstalled ? "1" : "0"));
                MC.DownloadDelays();
                MC.DownloadTransferSettings();
                MC.RunCommand($"setting[0]={MS.MinAirPressure}");
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Settings saved successfully\r\rDo you wish to exit Setup?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    _frmMain.LoadSubForm(_frmMain.frmConfig);
                }
            }
        }

        #region X Tuning

        private void buttonXServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (MC.X_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable X Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.TurnXOn();
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable X Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.TurnXOff();
                }
            }
        }

        private void buttonSetupXKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Proportional Constant", this, buttonSetupXKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupXKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Integrator", this, buttonSetupXKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupXKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Derivative Constant", this, buttonSetupXKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupXFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Acceleration Feedforward", this, buttonSetupXFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupXFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Velocity Feedforward", this, buttonSetupXFV, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupXPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("X Axis Pole", this, buttonSetupXPL, "0.##", 0, 1600, "", true);
        }

        private void buttonApplyXTuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new X Axis tuning parameters?", 
                "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupXKP.Text);
                double KI = double.Parse(buttonSetupXKI.Text);
                int KD = int.Parse(buttonSetupXKD.Text);
                double FV = double.Parse(buttonSetupXFV.Text);
                double FA = double.Parse(buttonSetupXFA.Text);
                double PL = double.Parse(buttonSetupXPL.Text);
                MC.ApplyTuning(MC.X_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        #endregion X Tuning

        public void LoadVisionTeachingPage(Form retForm)
        {
            tabControlSetup.SelectedIndex = 10;
            returnForm = retForm;
            buttonReturn.Visible = true;
        }

        private void buttonKeyenceComTest_Click(object sender, EventArgs e)
        {
            KeyenceState eStateRight = KeyenceState.Unknown, eStateLeft = KeyenceState.Unknown;
            Double dValRight = 0, dValLeft = 0;

            if (MC.TestKeyence(comboBoxKeyenceComPort.Text, ref eStateRight, ref dValRight, ref eStateLeft, ref dValLeft, 1))
            {
                labelKeyenceRightSensorValue.Text = $"{dValRight:0.0000}";
                labelKeyenceLeftSensorValue.Text = $"{dValLeft:0.0000}";
            }
            else
            {
                nRadMessageBox.Show(this, "Could not read GT2 Sensor data:\r\n" + MC.LastError, "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSetupSyringePumpKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Axis Proportional Constant", this, buttonSetupSyringePumpKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupSyringePumpKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Axis Integrator", this, buttonSetupSyringePumpKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupSyringePumpKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Axis Derivative Constant", this, buttonSetupSyringePumpKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupSyringePumpFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Axis Acceleration Feedforward", this, buttonSetupSyringePumpFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupSyringePumpFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Axis Velocity Feedforward", this, buttonSetupSyringePumpFV, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupSyringePumpPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-A Z Axis Pole Filter", this, buttonSetupSyringePumpPL, "0.##", 0, 1600, "", true);
        }

        private void buttonApplySyringeATuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new Syringe Pump-A tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupSyringePumpKP.Text);
                double KI = double.Parse(buttonSetupSyringePumpKI.Text);
                int KD = int.Parse(buttonSetupSyringePumpKD.Text);
                double FV = double.Parse(buttonSetupSyringePumpFV.Text);
                double FA = double.Parse(buttonSetupSyringePumpFA.Text);
                double PL = double.Parse(buttonSetupSyringePumpPL.Text);
                MC.ApplyTuning(MC.PumpA_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonSetupPOHPumpKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Axis Proportional Constant", this, buttonSetupPOHPumpKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupPOHPumpKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Axis Integrator", this, buttonSetupPOHPumpKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupPOHPumpKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Axis Derivative Constant", this, buttonSetupPOHPumpKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupPOHPumpFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Axis Acceleration Feedforward", this, buttonSetupPOHPumpFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupPOHPumpFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Axis Velocity Feedforward", this, buttonSetupPOHPumpFV, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupPOHPumpPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-A Pump Z Axis Pole Filter", this, buttonSetupPOHPumpPL, "0.##", 0, 1600, "", true);
        }

        private void buttonApplyPOHATuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new POH-A tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupPOHPumpKP.Text);
                double KI = double.Parse(buttonSetupPOHPumpKI.Text);
                int KD = int.Parse(buttonSetupPOHPumpKD.Text);
                double FV = double.Parse(buttonSetupPOHPumpFV.Text);
                double FA = double.Parse(buttonSetupPOHPumpFA.Text);
                double PL = double.Parse(buttonSetupPOHPumpPL.Text);
                MC.ApplyTuning(MC.PumpA_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonSyringAServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (MC.PumpA_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Pump-A Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SH" + MC.PumpA_axis);
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Pump-A Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MO" + MC.PumpA_axis);
                }
            }
        }

        private void buttonSetupZLKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Proportional Constant", this, buttonSetupZLKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupZLKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Integrator", this, buttonSetupZLKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupZLKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Derivative Constant", this, buttonSetupZLKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupZLFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Acceleration Feedforward", this, buttonSetupZLFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupZLFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Velocity Feedforward", this, buttonSetupZLFV, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupZLPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Axis Pole", this, buttonSetupZLPL, "0.##", 0, 1600, "", true);
        }

        private void buttonZLServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LastClick = DateTime.Now;
            if (MC.LZ_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Z Left Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SH" + MC.LZ_axis);
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Z Left Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MO" + MC.LZ_axis);
                }
            }
        }

        private void buttonApplyZLeftTuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new Left Z tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupZLKP.Text);
                double KI = double.Parse(buttonSetupZLKI.Text);
                int KD = int.Parse(buttonSetupZLKD.Text);
                double FV = double.Parse(buttonSetupZLFV.Text);
                double FA = double.Parse(buttonSetupZLFA.Text);
                double PL = double.Parse(buttonSetupZLPL.Text);
                MC.ApplyTuning(MC.LZ_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonZLOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Left Z Offset Adjustment", this, buttonZLOffset, "0.000", 0, 10);
        }

        private void buttonSetupZRKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Proportional Constant", this, buttonSetupZRKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupZRKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Integrator", this, buttonSetupZRKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupZRKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Derivative Constant", this, buttonSetupZRKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupZRFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Acceleration Feedforward", this, buttonSetupZRFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupZRFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Velocity Feedforward", this, buttonSetupZRFV, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupZRPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Axis Pole", this, buttonSetupZRPL, "0.##", 0, 1600, "", true);
        }

        private void buttonZRServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (MC.RZ_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Z Right Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SH" + MC.RZ_axis);
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Z Right Axis Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MO" + MC.RZ_axis);
                }
            }
        }

        private void buttonApplyZRightTuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new Right Z tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupZRKP.Text);
                double KI = double.Parse(buttonSetupZRKI.Text);
                int KD = int.Parse(buttonSetupZRKD.Text);
                double FV = double.Parse(buttonSetupZRFV.Text);
                double FA = double.Parse(buttonSetupZRFA.Text);
                double PL = double.Parse(buttonSetupZRPL.Text);
                MC.ApplyTuning(MC.RZ_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonZROffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Right Z Offset Adjustment", this, buttonZROffset, "0.000", 0, 10);
        }

        private void buttonRotaryServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LastClick = DateTime.Now;
            if (MC.PumpB_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Rotary Valve-A Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SHE");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Rotary Valve-A Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MOE");
                }
            }
        }

        private void buttonSetupMaintXLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Maintenance X Location", this, buttonSetupMaintXLoc, "0.0", 0, 300, "", true);
        }

        private void buttonSetupMaintZLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Maintenance Z Location", this, buttonSetupMaintZLoc, "0.0", 0, 60, "", true);
        }

        private void buttonSetupMaxXLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Maximum X Location", this, buttonSetupMaxXLoc, "0.0", 10, 1000, "", true);
        }

        private void buttonSetupMaxZLoc_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Maximum Z Location", this, buttonSetupMaxZLoc, "0.0", 0, 100, "", true);
        }

        private void buttonSetupMaxLoaderPos_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Maximum Loader Location", this, buttonSetupMaxLoaderPos, "0.0", 10, 1100, "", true);
        }

        private void buttonCrossbarWidth_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Crossbar Pivot to Pivot Width", this, buttonCrossbarWidth, "0.000", 700, 750, "", true);
        }

        private void buttonLiftPinsUpDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Lift Pins Up Delay (ms)", this, buttonLiftPinsUpDelay, "0", 0, 10000, "", false);
        }

        private void buttonLiftPinsDownDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Lift Pins Down Delay (ms)", this, buttonLiftPinsDownDelay, "0", 0, 10000, "", false);
        }

        private void buttonAlignersUpDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Aligners Up Delay (ms)", this, buttonAlignersUpDelay, "0", 0, 10000, "", false);
        }

        private void buttonAlignersDownDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Aligners Down Delay (ms)", this, buttonAlignersDownDelay, "0", 0, 10000, "", false);
        }

        private void buttonGT2SettleDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("GT2 Settling Delay (ms)", this, buttonGT2SettleDelay, "0", 0, 10000, "", false);
        }

        private void buttonAirPuffDelay_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Air Puff Delay (ms)", this, buttonAirPuffDelay, "0", 0, 10000, "", false);
        }

        private void buttonRotaryValveMoveDelay_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Rotary Valve Move Delay (ms)", this, buttonRotaryValveMoveDelay, "0", 0, 10000, "", false);
        }

        private void buttonMinAirPressure_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Minimum Allowed Air Pressure", this, buttonMinAirPressure, "0", 55, 120, "", false);
        }

        private void buttonMinVac_Click(object sender, EventArgs e)
        {
            double minValue = 1;
            double maxValue = 29;
            minValue = minValue * MC.VacConv;
            maxValue = maxValue * MC.VacConv;
            _frmMain.GotoNumScreen("Minimum Allowed Vacuum detection for coating", this, buttonMinVac, "0.0", minValue, maxValue);
        }

        private void buttonPOHScrewPitch_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Lead Screw Pitch", this, buttonPOHScrewPitch, "0.000", .1, 50);//OLD-5
        }

        private void buttonPOHPistonSize_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH Piston Diameter", this, buttonPOHPistonSize, "0.000", 1, 500);//OLD-100
        }

        private void buttonPOHMotorGearCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH Motor Gear Tooth Count", this, buttonPOHMotorGearCount, "0", 1, 5000, "", false);//OLD-1000
        }

        private void buttonPOHPistonGearCount_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH Piston Gear Tooth Count", this, buttonPOHPistonGearCount, "0", 1, 5000, "", false);//OLD-1000
        }

        private void comboBoxDieComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void comboBoxChuckComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void comboBoxResvComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void comboBoxResvHeaterComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
        }

        private void buttonMeasureStartPriming_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measure Zone - Start Of Priming", this, buttonMeasureStartPriming, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonMeasureEndPriming_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measure Zone - End Of Priming", this, buttonMeasureEndPriming, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonMeasureStartChuck_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measure Zone - Start Of Chuck", this, buttonMeasureStartChuck, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonMeasureEndChuck_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Measure Zone - End Of Chuck", this, buttonMeasureEndChuck, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonPrimingPlateStart_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priminging Zone - Start Of Priming", this, buttonPrimingPlateStart, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonPrimingPlateEnd_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Priming Zone - End Of Priming", this, buttonPrimingPlateEnd, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonChuckStart_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Zone - Start Of Chuck", this, buttonChuckStart, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonChuckEnd_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Coating Zone - End Of Chuck", this, buttonChuckEnd, "0.0", 0, MS.MaxXTravel);
        }

        private void buttonSetupCancel_Click(object sender, EventArgs e)
        {
            _frmMain.ShowPrevForm();
        }

        private void buttonAccessCode_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Setup Access Code", this, textBoxAccessCode, "#", 0, 999999999, "", false, true);
        }

        private void textBoxAccessCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonSetupEnter_Click(this, new EventArgs());
            }
        }

        private void buttonSetupEnter_Click(object sender, EventArgs e)
        {
            if (textBoxAccessCode.Text == "75238" || textBoxAccessCode.Text == MS.AdminPW)
            {
                textBoxAccessCode.Text = "";
                ShowLockout = false;
                panelSetupHide.Visible = false;
            }
            else
            {
                nRadMessageBox.Show(this, "Invalid Access Code", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonLock_Click(object sender, EventArgs e)
        {
            ShowLockout = true;
            nRadMessageBox.Show(this, "Setup will again require access code.\rSettings have not been saved.", "Exit Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _frmMain.ShowPrevForm();
        }

        private void FormSetup_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                panelSetupHide.Visible = ShowLockout;
                panelSetupHide.Dock = ShowLockout ? DockStyle.Fill : DockStyle.None;
                labelSetupIPTitle.Focus();
                // alt approach would be 
                //   this.VerticalScroll.Value = this.VerticalScrool.Minimum; 
                //   this.PerformLayout();
            }
        }

        private void buttonClearTC_Click(object sender, EventArgs e)
        {
            string sRC = "";
            _log.log(LogType.TRACE, Category.INFO, $"User Cleared Error Code(s)", "ACTION");
            string sTemp = $"TC ({MC.TC}) ";
            sTemp += $"[0] : 0x{((int)MC.Memory[0]).ToString("X8")} ";
            _log.log(LogType.TRACE, Category.INFO, sTemp);
            sRC = MC.RunCommand("TC1;gMem[0]=0");
            labelClearCodeResponce.Text = sRC;

        }

        private void textBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                buttonSend_Click(this, new EventArgs());
                e.Handled = true;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (checkBoxProcessInternally.Checked)
            {
                string output = "";
                int idx = 0;
                string[] varReq = textBoxCommand.Text.Split(new char[] {','});
                if(varReq.Length>1)
                    idx = int.Parse(varReq[1]);
                switch(varReq[0])
                {
                    case "gMem":
                        output = $"gMem[{idx}] = {MC.Memory[idx]}";
                        break;
                    case "rioIn":
                        output = $"rioIn[{idx}] = {(MC.RIO_Inputs[idx] ? "True" : "False")}";
                        break;
                    case "rioOut":
                        output = $"rioOut[{idx}] = {(MC.RIO_Outputs[idx] ? "True" : "False")}";
                        break;
                    case "Mode":
                        output = $"Mode = {(MC.AutoMode ? "AUTO" : "MANUAL")}";
                        break;
                }
                textBoxCommandOutput.AppendText($":{textBoxCommand.Text.TrimEnd()}\r\n");
                textBoxCommandOutput.AppendText($"{output}\r\n");
            }
            else
            {
                string sRC = "";
                _log.log(LogType.TRACE, Category.INFO, $"User DIRECT Command To contorller: '{textBoxCommand.Text}'", "WARNING");
                sRC = MC.RunCommand(textBoxCommand.Text);
                textBoxCommandOutput.AppendText($":{textBoxCommand.Text.TrimEnd()}\r\n");
                textBoxCommandOutput.AppendText($"RC: {sRC}\r\n");
                textBoxCommand.Clear();
                textBoxCommand.Text = "";
            }
        }

        private void buttonClearCommandOutput_Click(object sender, EventArgs e)
        {
            textBoxCommandOutput.Clear();
        }

        private void checkBoxVacuumDisable_CheckedChanged(object sender, EventArgs e)
        {
            MC.VacuumDisabled = checkBoxVacuumDisable.Checked;
        }

        private void buttonVlvVent_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);
            string name = index == 0 ? "A" : "B";

            _log.log(LogType.ACTIVITY, Category.ACTION, $"User Pressed Pump {name} Rotary Valve: Vent");
            MC.MoveValveToVent(index);
        }

        private void buttonVlvRecharge_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);
            string name = index == 0 ? "A" : "B";

            _log.log(LogType.ACTIVITY, Category.ACTION, $"User Pressed Pump {name} Rotary Valve: Recharge");
            MC.MoveValveToRecharge(index);
        }

        private void buttonVlvOff_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);
            string name = index == 0 ? "A" : "B";

            _log.log(LogType.ACTIVITY, Category.ACTION, $"User Pressed Pump {name} Rotary Valve: Off");
            MC.MoveValveToOff(index);
        }

        private void buttonVlvDispense_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);
            string name = index == 0 ? "A" : "B";

            _log.log(LogType.ACTIVITY, Category.ACTION, $"User Pressed Pump {name} Rotary Valve: Dispense");
            MC.MoveValveToDispense(index);
        }

        private void buttonRehomeVlvA_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);

            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Ready to Re-home Valve-A?", "Move Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                _log.log(LogType.ACTIVITY, Category.ACTION, "User Pressed Rotary Valve: Re-Home Valve-A");
                MC.HomeSmartValve(index);
            }
        }

        private void buttonChuckNumZones_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Number of Chuck Vac/Air Zones", this, buttonChuckNumZones, "#", 1, 3, "", false);
        }

        private void pictureBoxELOLatched_Click(object sender, EventArgs e)
        {
            if (MC.ELOLatched)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, $"Ready to clear Latched ELO?", "Recovery Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    MC.RunCommand($"XQ #clrelo,7");
                }
            }
        }

        private void Thread_Click(object sender, EventArgs e)
        {
            PictureBox pbThread = (PictureBox)sender;
            int nThreadID = int.Parse(pbThread.Tag.ToString());
            bool bThreadActive = pbThread.BackColor != Color.Red;
            if (bThreadActive && nThreadID >= 1 && nThreadID <= 7)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, $"Are you sure you want to stop thread '{nThreadID}'?", "Thread Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
                {
                    MC.RunCommand($"HX{nThreadID}");
                }
            }
        }

        private void buttonSetupDieLoadZPos_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Set this Z positions as Die Load/Unload positions?", "Thread Operation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                MS.ZRDieLoadLoc = MC.ZRPos;
                MS.ZLDieLoadLoc = MC.ZLPos;
            }
        }

        private void buttonSetupDieLoadXPos_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die Load/Unload X Location", this, buttonSetupDieLoadXPos, "0.000", -10, 300, "", true);
        }

        private void buttonDataRecord_Click(object sender, EventArgs e)
        {
            if (MC.DataRecordRunning)
            {
                _frmMain.OverrideDataRecordTimeout = true;
                MC.StopDataRecord();
                textBoxCommandOutput.AppendText($"Data Record Stopped!\r\n");
                _log.log(LogType.TRACE, Category.INFO, $"Data Record Stopped by Setup Button!\r\n");
            }
            else
            {
                MC.StartDataRecord();
                System.Threading.Thread.Sleep(50);
                _frmMain.OverrideDataRecordTimeout = false;
                textBoxCommandOutput.AppendText($"Data Record Started!\r\n");
                _log.log(LogType.TRACE, Category.INFO, $"Data Record Started by Setup Button!\r\n");
            }
        }

        private void pictureBoxSlowDataPoll_Click(object sender, EventArgs e)
        {
            MC.NeedSlowPoll = !MC.NeedSlowPoll;
        }

        private void buttonFServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (!MS.HasLoader)
            {
                nRadMessageBox.Show(this, "Loader has been disabled in settings?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MC.Loader_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Loader Conveyor Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SH"+MC.Loader_axis);
                    MC.RunCommand("OE" + MC.Loader_axis + "=1");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Loader Conveyor Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("OE"+MC.Loader_axis+"=0");
                    MC.RunCommand("MO"+MC.Loader_axis);
                }
            }
        }

        private void buttonSetupFKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Loader Axis Proportional Constant", this, buttonSetupFKP, "0.###", 0, 1023.875, "", true);
        }

        private void buttonSetupFKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Loader Axis Integrator", this, buttonSetupFKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupFKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Loader Axis Derivative Constant", this, buttonSetupFKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupFFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Loader Axis Acceleration Feedforward", this, buttonSetupFFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupFFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Loader Axis Velocity Feedforward", this, buttonSetupFFV, "0", 0, 8191, "", false);
        }

        private void buttonSetupFPL_Click(object sender, EventArgs e)
        {
            double dMax = (1000000 / 4) * (1 / MC.TM);
            _frmMain.GotoNumScreen("Loader Axis Pole", this, buttonSetupFPL, "0", 0, 1600, "", false);
        }

        private void buttonApplyFTuning_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (!MS.HasLoader)
            {
                nRadMessageBox.Show(this, "Loader has been disabled in settings?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new Loader Axis tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupFKP.Text);
                double KI = double.Parse(buttonSetupFKI.Text);
                int KD = int.Parse(buttonSetupFKD.Text);
                int FV = int.Parse(buttonSetupFFV.Text);
                double FA = double.Parse(buttonSetupFFA.Text);
                int PL = int.Parse(buttonSetupFPL.Text);
                MC.ApplyTuning(NAbleEnums.AxisName(Axis.Loader), KP, KI, KD, FV, FA, PL);
            }
        }

        private void labelStatus_TC_Click(object sender, EventArgs e)
        {
            string lineData = MC.TC == 0 ? "" : MC.GetCommandText(MC.CommandErrorLineNum);
            string errorMsg = MC.GetErrorMessage(MC.TC)+"\r"+ lineData;
            nRadMessageBox.Show(this, errorMsg, "Current Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonCfgEditOperatorAccessCode_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Operator Access Code", this, textBoxCfgOperatorAccessCode, "0", 0, 9999999999, "S", false);
        }

        private void buttonCfgEditEditorAccessCode_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Operator and Editor Access Code", this, textBoxCfgEditorAccessCode, "0", 0, 9999999999, "S", false);
        }

        private void buttonCfgEditAdminAccessCode_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Administrator Access Code", this, textBoxCfgAdminAccessCode, "0", 0, 9999999999, "S", false);
        }

        private void buttonDiePressVOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Die Pressure Voltage Input Adjustment (-1:1V)", this, buttonDiePressVAdjustment, "0.000", -1, 1, "", true);

        }

        private void labelDiePressure_Click(object sender, EventArgs e)
        {
            _pressDisplayType++;
            if (_pressDisplayType == 4) _pressDisplayType = 0;
        }

        private void buttonKeyboardMod1Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Module 1 Name", this, textBoxModule1Name, 128);
        }

        private void buttonKeyboardMod2Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Module 2 Name", this, textBoxModule2Name, 128);
        }

        private void buttonKeyboardMod3Name_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("Module 3 Name", this, textBoxModule3Name, 128);
        }

        private void buttonKeyboardPLCIPAddr_Click(object sender, EventArgs e)
        {
            _frmMain.GotoTextScreen("PLC IP Address", this, textBoxPLCIPAddr, 128);
        }

        private void checkBoxDemoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDemoMode.Checked != MC.IsDemo)
            {
                checkBoxDemoMode.BackColor = Color.Yellow;
            }
            else
            {
                checkBoxDemoMode.BackColor = Color.FromKnownColor(KnownColor.Transparent);
            }
        }

        private void buttonPumpBServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            if (MC.PumpB_MotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Pump-B Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SH" + MC.PumpB_axis);
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Pump-B Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MO"+MC.PumpB_axis);
                }
            }
        }

        private void buttonSetupSyringePumpBKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Axis Proportional Constant", this, buttonSetupSyringeBKP, "0", 0, 1023, "", false);
        }

        private void buttonSetupSyringePumpBKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Axis Integrator", this, buttonSetupSyringeBKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupSyringePumpBKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Axis Derivative Constant", this, buttonSetupSyringeBKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupSyringePumpBFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Axis Acceleration Feedforward", this, buttonSetupSyringeBFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupSyringePumpBFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Axis Velocity Feedforward", this, buttonSetupSyringeBFV, "0", 0, 8191, "", false);
        }

        private void buttonSetupSyringePumpBPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Syringe Pump-B Z Axis Pole Filter", this, buttonSetupSyringeBPL, "0", 0, 1600, "", false);
        }

        private void buttonSetupPOHPumpBKP_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Axis Proportional Constant", this, buttonSetupPOHBKP, "0.###", 0, 1023.875, "", true);
        }

        private void buttonSetupPOHPumpBKI_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Axis Integrator", this, buttonSetupPOHBKI, "0.###", 0, 256, "", true);
        }

        private void buttonSetupPOHPumpBKD_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Axis Derivative Constant", this, buttonSetupPOHBKD, "0.###", 0, 4095.875, "", true);
        }

        private void buttonSetupPOHPumpBFA_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Axis Acceleration Feedforward", this, buttonSetupPOHBFA, "0.##", 0, 8191, "", true);
        }

        private void buttonSetupPOHPumpBFV_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Axis Velocity Feedforward", this, buttonSetupPOHBFV, "0", 0, 8191, "", false);
        }

        private void buttonSetupPOHPumpBPL_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("POH-B Pump Z Axis Pole Filter", this, buttonSetupPOHBPL, "0", 0, 1600, "", false);
        }

        private void buttonRotaryBServoCntrl_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LastClick = DateTime.Now;
            if (MC.GMotorOff)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Enable Rotary Valve-B Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("SHG");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Disable Rotary Valve-B Motor?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.RunCommand("MOG");
                }
            }
        }

        private void buttonRotaryValveBOffset_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Rotary Valve-B Offset", this, buttonRotaryValveBOffset, "0", -30, 30, "", false);
        }

        private void buttonVlvBVent_Click(object sender, EventArgs e)
        {
            MC.MoveValveToVent(1);
        }

        private void buttonVlvBRecharge_Click(object sender, EventArgs e)
        {
            MC.MoveValveToRecharge(1);
        }

        private void buttonVlvBOff_Click(object sender, EventArgs e)
        {
            MC.MoveValveToOff(1);
        }

        private void buttonVlvBDispense_Click(object sender, EventArgs e)
        {
            MC.MoveValveToDispense(1);
        }

        private void buttonApplySyringeBTuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new Syringe Pump-B tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupSyringeBKP.Text);
                double KI = double.Parse(buttonSetupSyringeBKI.Text);
                int KD = int.Parse(buttonSetupSyringeBKD.Text);
                int FV = int.Parse(buttonSetupSyringeBFV.Text);
                double FA = double.Parse(buttonSetupSyringeBFA.Text);
                int PL = int.Parse(buttonSetupSyringeBPL.Text);
                MC.ApplyTuning(MC.PumpB_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonApplyPOHBTuning_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you ready to test the new POH-B tuning parameters?", "Apply tuning", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                int KP = int.Parse(buttonSetupPOHBKP.Text);
                double KI = double.Parse(buttonSetupPOHBKI.Text);
                int KD = int.Parse(buttonSetupPOHBKD.Text);
                int FV = int.Parse(buttonSetupPOHBFV.Text);
                double FA = double.Parse(buttonSetupPOHBFA.Text);
                int PL = int.Parse(buttonSetupPOHBPL.Text);
                MC.ApplyTuning(MC.PumpB_axis, KP, KI, KD, FV, FA, PL);
            }
        }

        private void buttonResvBFluidComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Fluid Reservoir-B Temperature Controller ID", this, buttonResvBFluidComID, "0", 0, 7, "", false);
        }

        private void buttonResvBHeaterComID_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Fluid Reservoir-B Heater Controller ID", this, buttonResvBHeaterComID, "0", 0, 7, "", false);
        }

        private void tabControlSetup_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage.Name=="tabPageLoader")
                e.Cancel = !MS.HasLoader;
            else if (e.TabPage.Name == "tabPageStack")
                e.Cancel = !MS.HasStack;
        }

        private void checkBoxResvLimitCtl_CheckedChanged(object sender, EventArgs e)
        {
            panelResvALimitControl.Enabled = checkBoxResvLimitCtl.Checked;
            panelResvBLimitControl.Enabled = checkBoxResvLimitCtl.Checked;
        }

        private void comboBoxVacUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loadingMachineSettings)
                return;
            string curVacUnits = MS.VacuumUnits;
            string selectedVacUnits = comboBoxVacUnits.Items[comboBoxVacUnits.SelectedIndex].ToString();
            double convertedMinVacuum = MS.MinVacuum;
            if (selectedVacUnits == curVacUnits)
                return;
            switch (selectedVacUnits)
            {
                case "inHg":
                    switch (curVacUnits)
                    {
                        case "mBar":
                            convertedMinVacuum = convertedMinVacuum * 0.029529983071445;
                            break;
                        case "Torr":
                            convertedMinVacuum = convertedMinVacuum * 0.03937;
                            break;
                        case "kPa":
                            convertedMinVacuum = convertedMinVacuum * 0.295301;
                            break;
                    }
                    break;
                case "mBar":
                    switch (curVacUnits)
                    {
                        case "inHg":
                            convertedMinVacuum = convertedMinVacuum * 33.86389;
                            break;
                        case "Torr":
                            convertedMinVacuum = convertedMinVacuum * 1.33322368;
                            break;
                        case "kPa":
                            convertedMinVacuum = convertedMinVacuum * 10;
                            break;
                    }
                    break;
                case "Torr":
                    switch (curVacUnits)
                    {
                        case "inHg":
                            convertedMinVacuum = convertedMinVacuum * 25.400006316309;
                            break;
                        case "mBar":
                            convertedMinVacuum = convertedMinVacuum * 0.75006169;
                            break;
                        case "kPa":
                            convertedMinVacuum = convertedMinVacuum * 7.50061683;
                            break;
                    }
                    break;
                case "kPa":
                    switch (curVacUnits)
                    {
                        case "inHg":
                            convertedMinVacuum = convertedMinVacuum * 3.3863886666667;
                            break;
                        case "mBar":
                            convertedMinVacuum = convertedMinVacuum * 0.1;
                            break;
                        case "Torr":
                            convertedMinVacuum = convertedMinVacuum * 0.13332237;
                            break;
                    }
                    break;
                default:
                    break;
            }
            MS.VacuumUnits = selectedVacUnits;
            MS.MinVacuum = convertedMinVacuum;
            MS.Save();
            switch (MS.VacuumUnits)
            {
                case ("inHg"):
                    MC.VacConv = 1.00;
                    break;
                case ("mBar"):
                    MC.VacConv = 33.86389;
                    break;
                case ("kPa"):
                    MC.VacConv = 3.3863886666667;
                    break;
                case ("Torr"):
                    MC.VacConv = 25.400006316309;
                    break;
            }
            buttonMinVac.Text = MS.MinVacuum.ToString("#0.000");
            _frmMain.VacUnits = selectedVacUnits;
        }

        private void CheckBoxHideHeadPurgeOnRunScr_CheckedChanged(object sender, EventArgs e)
        {
            MS.HideHeadPurgeOnRun = checkBoxHideHeadPurgeOnRunScr.Checked;
        }

        private void CheckBoxHideHeadPurgeOnFluidConScr_CheckedChanged(object sender, EventArgs e)
        {
            MS.HideHeadPurgeOnFluidControl = checkBoxHideHeadPurgeOnFluidConScr.Checked;
        }

        private void CheckBoxHideRecircOnFCScr_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (LaserMgr is null)
            {
                MC.ConnectKeyenceLaser();
            }

            if (ipAddressInputLasers.Value != "" && LaserMgr != null)
            {
                if (!LaserMgr.Connected)
                {
                    LaserMgr.Connect(ipAddressInputLasers.Value);
                }
                else
                {
                    LaserMgr.Disconnect();
                }
            }
        }

        private void buttonXVisionPos_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Vision X Location", this, buttonXVisionPos, "0.0", 0, MS.MaxXTravel, "", true);
        }

        private void buttonZVisionPos_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Vision Z Location", this, buttonZVisionPos, "0.0", 0, MS.MaxZTravel, "", true);
        }

        private void buttonTeachVisionX_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            MS.VisionXPos = MC.XPos;
            buttonXVisionPos.Text = MC.XPos.ToString("#0.000");
            buttonXVisionPos.BackColor = Color.Yellow;
        }

        private void buttonTeachVisionZ_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            MS.VisionZPos = MC.ZRPos;
            buttonZVisionPos.Text = MC.ZRPos.ToString("#0.000");
            buttonZVisionPos.BackColor = Color.Yellow;
        }

        public Form returnForm = null;
        private void buttonReturn_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            buttonReturn.Visible = false;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Button Return To Previous Screen. - FormSetup", (_frmMain.UserIsAdmin ? "USER" : "ADMIN"));
            _frmMain.LoadSubForm(returnForm);
        }

        private void checkBoxUsingLaser_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUsingLaser.Checked)
            {
                groupBoxKeyenceLaser.Visible = true;
                groupBoxSetupKeyence.Visible = false;
            }
            else
            {
                groupBoxKeyenceLaser.Visible = false;
                groupBoxSetupKeyence.Visible = true;
            }
        }

        private void buttonGetBankInfoGT2_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Button GetGT2BankInfo. - FormSetup", (_frmMain.UserIsAdmin ? "USER" : "ADMIN"));

            double PresetVal = MC.ReadPresetValue(0);
            _log.log(LogType.TRACE, Category.INFO, $"GetGT2BankInfo Returned Preset Value: {PresetVal}", (_frmMain.UserIsAdmin ? "USER" : "ADMIN"));
            labelPresetVal.Text = PresetVal.ToString("#0.0000");
        }

        private void buttonSetBankPreset_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Button SetGT2BankPreset. - FormSetup", (_frmMain.UserIsAdmin ? "USER" : "ADMIN"));

            if (MC.PresetAll(MS.ShimSize, 0))
            {
                _log.log(LogType.TRACE, Category.INFO, $"SetGT2BankPreset({MS.ShimSize}, Bank 0) Was Successful.", (_frmMain.UserIsAdmin ? "USER" : "ADMIN"));
            }
        }

        private void buttonResetLifetimeCt_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(_frmMain, "Reset LifeTime Glass Total Count?", "Continue With Reset?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                MC.Storage.LifetimeCount = 0;
                MC.Storage.Save();
                _log.log(LogType.TRACE, Category.INFO, "Lifetime Glass Count Has Been Reset To: 0.");
            }
        }

        private bool dualPumpRecentlyChanged { get; set; } = false;
        private void checkBoxDualPumpInstalled_CheckedChanged(object sender, EventArgs e)
        {
            if (MS.DualPumpInstalled != checkBoxDualPumpInstalled.Checked)
                dualPumpRecentlyChanged = true;
            else
                dualPumpRecentlyChanged = false;
        }

        private void checkBoxSelectiveZones_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxDisableHeadPurge_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDisableHeadPurge.Checked)
            {
                checkBoxHideHeadPurgeOnFluidConScr.Checked = true;
                checkBoxHideHeadPurgeOnRunScr.Checked = true;
            }
            else
            {
                checkBoxHideHeadPurgeOnFluidConScr.Checked = false;
                checkBoxHideHeadPurgeOnRunScr.Checked = false;
            }
        }

        private void checkBoxEnableLooper_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxEnableLooper.Checked != MS.EnableLooperMode)
                checkBoxEnableLooper.BackColor = Color.Yellow;
            else
                checkBoxEnableLooper.BackColor = Color.FromKnownColor(KnownColor.Transparent);
        }

        private void buttonResetRotaryValve_Click(object sender, EventArgs e)
        {
            //TO-DO
        }

        private void labelInputsDI7_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxSetupLiftPins_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSetupLiftPins.Checked)
            {
                checkBoxHasLiftAndCenter.Checked = false;
            }
        }

        private void buttonDieComTest_Click(object sender, EventArgs e)
        {

        }

        private string GetTCDescription(int code)
        {
            return _tcCodes[code];
        }

        private void LoadTCCodes()
        {
            _tcCodes = new Dictionary<int, string>();
            try
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(@"Data\Galil TC Codes.csv")))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (values.Length == 3)
                        {
                            int tc_code = int.Parse(values[0]);
                            string desc = values[1];
                            _tcCodes.Add(tc_code, desc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Error Loading Galil TC Codes!" + ex.ToString(), "ERROR");
            }
        }

        private void ShowOrHideTabs()
        {
            tabControlFactorySettings.ShowOrHideTab(_irTab, MS.IRLampInstalled);
            tabControlFactorySettings.ShowOrHideTab(_akTab, MS.AirKnifeInstalled);
            tabControlFactorySettings.ShowOrHideTab(_loaderTab, MS.HasLoader);
            tabControlFactorySettings.ShowOrHideTab(_stackTab, MS.HasStack);
            tabControlFactorySettings.ShowOrHideTab(tabPageAdvantechServer, MS.AdvantechServerInstalled);
        }

        private void checkBoxKeyenceLasersInstalled_CheckedChanged(object sender, EventArgs e)
        {
            if (!_loadingMachineSettings)
            {
                CheckBox box = (CheckBox)sender;
                checkBoxUsingLaser.Visible = box.Checked;
                if (!box.Checked)
                {
                    checkBoxUsingLaser.Checked = false;
                }
            }
        }

        private void comboBoxPrgmNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPrgmNum.SelectedIndex < 0 || _updating || LaserMgr is null || LaserMgr.WaitingForProgramNumber)
            {
                return;
            }

            if (!int.TryParse(comboBoxPrgmNum.SelectedItem.ToString(), out var programNumber))
            {
                _log.log(LogType.TRACE, Category.DEBUG, $"Program Number change failed: {comboBoxPrgmNum.SelectedItem}");
                return;
            }

            if (programNumber != LaserMgr.ProgramNumber)
            {
                bool setNum = LaserMgr.SetProgramNumber(programNumber);
                comboBoxPrgmNum.Enabled = !setNum;
            }
        }

        private void buttonLaserCalibrationUpperHeight_Click(object sender, EventArgs e)
        {
            _frmMain.GotoNumScreen("Laser Calibration Upper Height (µm)", this, buttonCalibrationUpperHeight, "#0", MS.ShimSize, 1000);
        }

        private void buttonAirKnifeHeaterComID_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Air Knife Heater Controller ID", this, buttonAirKnifeHeaterComID, "0", 0, 10, "", false);
        }

        private void buttonAirKnifeHeaterMaxTemperature_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Air Knife Heater Max. Temperature (°C): ", this, buttonAirKnifeHeaterMaxTemperature, "##0.#", 0.0, 320.0, "", true);
            Trace.WriteLine($"User Changed Air Knife Max. Temperature: {buttonAirKnifeHeaterMaxTemperature.Text}%", "INFO");
        }

        private void buttonGasHeaterComID_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Gas Heater Controller ID", this, buttonGasHeaterComID, "0", 0, 10, "", false);
        }

        private void buttonGasHeaterReaderComID_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen("Gas Heater Reader Controller ID", this, buttonGasHeaterReaderComID, "0", 0, 10, "", false);
        }

        #endregion

        private int GetPumpIndex(object sender)
        {
            if (sender is Button btn)
            {
                if (btn.Tag is string tag)
                {
                    if (int.TryParse(tag, out int index))
                    {
                        return index;
                    }
                }
            }

            throw new ArgumentException($"GetPumpIndex called with invalid param");
        }

        private void buttonRehomeVlvB_Click(object sender, EventArgs e)
        {
            var index = GetPumpIndex(sender);

            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Ready to Re-home Valve-B?", "Move Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation))
            {
                _log.log(LogType.ACTIVITY, Category.ACTION, "User Pressed Rotary Valve-B: Re-Home Valve");
                MC.HomeSmartValve(index);
            }
        }

        private void checkBoxProcessLogs_CheckedChanged(object sender, EventArgs e)
        {
            MS.ProcessLogsEnabled = checkBoxProcessLogs.Checked;
        }

        private void checkBoxHasLiftAndCenter_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxHasLiftAndCenter.Checked)
            {
                checkBoxSetupLiftPins.Checked = false;
                checkBoxSetupAligners.Checked = false;
            }
        }

        private void checkBoxSetupAligners_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxSetupAligners.Checked)
            {
                checkBoxHasLiftAndCenter.Checked = false;
            }
        }
    }
}

