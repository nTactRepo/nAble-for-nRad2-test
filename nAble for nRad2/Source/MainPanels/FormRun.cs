using CommonLibrary.Utils;
using nAble.Data;
using nAble.DataComm.AdvantechSerialServer;
using nAble.DataComm.KeyenceLasers;
using nAble.Managers.ProcessLogCollectors;
using nAble.Utils;
using nTact.DataComm;
using nTact.PLC;
using nTact.Recipes;
using System.ComponentModel;

namespace nAble
{
    public partial class FormRun : Form, IUpdateableForm, ITracksRecipeList, ITracksRecipeChanges
    {
        #region Events

        internal delegate void DelegateOnRecipeFinished(object sender, RecipeEventArgs rea);

        #endregion

        #region Constants

        private const string PrimingText = "Enable Priming Vac";
        private const string ReleasePrimingVac = "Release Priming Vac";
        private const string PrimingVacDisabled = "Priming\rVac\r(Disabled)";
        private const string ChuckText = "Enable Chuck Vac";
        private const string ReleaseChuckVac = "Release Chuck Vac";

        private static readonly Color ControlColor = Color.FromName("Control");

        private static readonly string ProcessLogDirectory = Path.Combine(Application.StartupPath, @"Data\ProcessLogs");

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private readonly IRecipeManager _recipeMgr = null;
        
        private RunIsOKChecker _runChecker = null;

        private GalilWrapper2 MC => _frmMain.MC;
        private MachineSettingsII MS => _frmMain.MS;
        private PLCWrapper PLC => _frmMain.PLC;
        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;
        private AdvantechServer AdvServer => _frmMain.AdvServer;

        private Recipe _currentRecipe = null;
        private Recipe _defaultRecipe = null;
        private System.Windows.Forms.Timer _timerStartCoating = null;
        private BackgroundWorker _recipeWorker = new BackgroundWorker();
        private ProcessLogCollector _processLogs = null;
        private BackgroundWorker _bgwLDULD = new BackgroundWorker();
        private BackgroundWorker _bgwRobotLoad = new BackgroundWorker();
        private BackgroundWorker _bgwRobotUnload = new BackgroundWorker();
        private const int _sleepTime = 50;
        
        private bool _stackOpsShowing = false;
        private bool _visionOpsShowing = false;

        private DateTime _startTime;
        private bool _waitingToUnloadAndXfer = false;
        private bool _newRecipeList = false;
        private bool _updateCurrentRecipe = false;
        private bool _loading = false;
        private bool _unloading = false;
        private bool _unloading2 = false;
        private bool _smoothed = true;
        private int _numCycles = 0;
        private bool _runningRecipe = false;
        private bool _aborting = false;
        private bool _loopMode = false;
        private bool _loadReqLatch = false;
        private bool _unloadReqLatch = false;
        private bool _abortLDULDReqLatch = false;
        private bool _substrateLoaded = true;

        #endregion

        #region Functions

        #region Constructors

        public FormRun(FormMain formMain, LogEntry log, IRecipeManager recipeManager)
        {
            InitializeComponent();
            labelAutoMode.BringToFront();

            _frmMain = formMain;
            _log = log;
            _recipeMgr = recipeManager;

            InitializeBackgroundWorker();
            InitializeRunBlockers();

            _processLogs = new ProcessLogCollector(ProcessLogDirectory, _log, () => MS.ProcessLogsEnabled, new DateTimeSource());
        }

        #endregion

        #region Public Functions
        public bool RecipeSelected => _currentRecipe != null;
        public void UpdateStatus()
        {
            if(MS.AutoLDULD && MC.AutoMode && !_runningRecipe)
            {
                // Handle Auto Unload Request
                if(MC.UnloadRequest && !_unloadReqLatch && !MC.RobotUnloading && MC.SubstratePresent)
                {
                    if(StartRobotUnload())
                        _unloadReqLatch = true;
                }
                if(!MC.UnloadRequest && _unloadReqLatch)
                {
                    _unloadReqLatch = false;
                }

                // Handle Auto Load Request
                if (MC.LoadRequest && !_loadReqLatch && !MC.RobotLoading && !MC.SubstratePresent)
                {

                    if (StartRobotLoad())
                        _loadReqLatch = true;
                }
                if (!MC.LoadRequest && _loadReqLatch)
                {
                    _loadReqLatch = false;
                }
            }

            if (!Visible)
            {
                return;
            }

            if (_newRecipeList && !_runningRecipe)
            {
                RefreshRecipeList();
                _newRecipeList = false;
            }

            if (_updateCurrentRecipe && !_runningRecipe)
            {
                RefreshCurrentRecipe();
                _updateCurrentRecipe = false;
            }

            bool bConnected = MC.IsDemo || (MC != null && MC.Connected && MC.IsHomed);
            bool bMoving = !MC.IsDemo && (MC.Moving || !MC.MainAirOK);

            bool bReadyToLoad = MC.AlignersUpRequest && !MC.Zone1VacOK && !MC.Zone2VacOK && !MC.Zone3VacOK &&
                                !MC.Zone1VacEngaged && !MC.Zone2VacEngaged && !MC.Zone3VacEngaged;

            bool bLoaded = !MC.AlignersUpRequest && MC.LiftPinsAreDown && MC.Zone1VacOK && MC.Zone2VacOK && MC.Zone3VacOK &&
                            MC.Zone1VacEngaged && MC.Zone2VacEngaged && MC.Zone3VacEngaged;

            bool bUnloaded = (MS.LiftPinsEnabled && MC.LiftPinsUpRequest);

            if (MS.HasStack)
            {
                PLC.UpdateRecipeRunning(_runningRecipe);
            }

            if (_waitingToUnloadAndXfer)
            {
                WaitingOnUnloadAndXfer();
            }

            //Hide Options
            buttonRunHeadPurge.Visible = !MS.HideHeadPurgeOnRun;

            buttonEdit.Enabled = _currentRecipe != null;

            labelLeftCamStatus.Visible = MS.CognexCommunicationsUsed;
            labelRightCamStatus.Visible = MS.CognexCommunicationsUsed;
            labelLeftCamJobSts.Visible = MS.CognexCommunicationsUsed;
            labelRightCamJobSts.Visible = MS.CognexCommunicationsUsed;

            buttonCognexView.Visible = MS.CognexCommunicationsUsed;
            buttonGotoVision.Visible = MS.CognexCommunicationsUsed;
            buttonVisionOperations.Visible = MS.CognexCommunicationsUsed;

            bool needsLoadButtons = MS.HasLoader || MS.AlignersEnabled || MS.HasLiftAndCenter;
            bool alignersPinsMoving = (!MS.AlignersEnabled || (MS.AlignersEnabled && MC.AlignersUpRequest)) &&
                    (!MS.LiftPinsEnabled || (MS.LiftPinsEnabled && MC.LiftPinsUpRequest));

            buttonRunPrepareLoad.Visible = needsLoadButtons;
            buttonRunLoad.Visible = needsLoadButtons;
            buttonRunUnload.Visible = needsLoadButtons;

            buttonRunTransfer.Visible = MS.HasLoader;
            buttonProcessStackOperations.Visible = MS.HasLoader;
            buttonStackOperations.Visible = MS.HasLoader;

            labelAlignersState.Visible = MS.AlignersEnabled && !MS.HasLoader;
            buttonRunAligners.Visible = MS.AlignersEnabled && !MS.HasLoader;
            labelAutoMode.Visible = MS.AutoLDULD && MC.AutoMode;

            buttonRunPrepareLoad.Enabled = bConnected && !bMoving && !bLoaded && !_unloading;
            buttonRunPrepareLoad.BackColor = !_loading ? ControlColor : (alignersPinsMoving ? Color.Lime : Color.Yellow);
            buttonRunLoad.Enabled = bConnected && !bMoving;
            buttonRunLoad.BackColor =  MC.ChuckLiftPinsStatus==LiftPinStatus.Loading ? Color.Yellow : bLoaded ? Color.Lime : ControlColor;

            labelAlignersState.Visible = MS.AlignersEnabled && !MS.HasLoader;
            buttonRunAligners.Visible = MS.AlignersEnabled && !MS.HasLoader;

            checkBoxXRunLoop.Visible = MS.EnableLooperMode;
            groupBoxLoopCount.Visible = checkBoxXRunLoop.Visible;
            labelRunLoopCycles.Visible = checkBoxXRunLoop.Visible;
            labelRunLoopCycles.Text = _numCycles.ToString("#,##0");
            buttonRunTransfer.Enabled = bConnected && !bMoving && (!MS.HasStack || PLC.LoaderHomed);

            if (_unloading && !_unloading2 && alignersPinsMoving)
            {
                _unloading2 = true;  // ready for step 2
            }
            else if (_unloading2 && (!MS.AlignersEnabled || (MS.AlignersEnabled && !MC.AlignersUpRequest)))
            {
                _unloading = false;
            }

            if (MS.UsingKeyenceLaser)
            {
                gbKeyenceLasers.Text = "Keyence Lasers";
                if (LaserMgr?.GetLeftAndRightLaserData(out var left, out var right) ?? false)
                {
                    labelLeft.Text = left.ValueAsString;
                    labelRight.Text = right.ValueAsString;
                }
                else
                {
                    labelLeft.Text = "---------";
                    labelRight.Text = "---------";
                }
            }
            else
            {
                gbKeyenceLasers.Text = "GT2s";
                labelLeft.Text = MC.LeftGT2Pos != 0 ? $"{MC.LeftGT2Pos:#0.000}" : "---------";
                labelRight.Text = MC.RightGT2Pos != 0 ? $"{MC.RightGT2Pos:#0.000}" : "---------";
            }

            buttonRunUnload.Enabled = bConnected && !bMoving;
            buttonRunUnload.Text = bUnloaded ? "Reset\rLift" : "Unload";
            buttonRunUnload.BackColor = MC.ChuckLiftPinsStatus==LiftPinStatus.Unloading ? Color.Yellow : _unloading ? Color.Yellow : ControlColor;

            buttonRunHeadPurge.Enabled = bConnected && !bMoving;
            buttonRunLipWipe.Enabled = bConnected && !bMoving;

            buttonLockScreen.Enabled = !_runningRecipe;
            labelAlignersState.Text = MC.AlignersUpRequest ? "Aligners Are Up" : "Aligners Are Down";
            labelAlignersState.BackColor = MC.AlignersUpRequest ? Color.Yellow : SystemColors.ControlDark;
            buttonRunAligners.Enabled = (bMoving && MC.AlignersUpRequest) || !bMoving;
            buttonRunAligners.Text = MC.AlignersUpRequest ? "Lower Aligners" : "Raise Aligners";

            buttonRunPrimingVacuum.Enabled = bConnected && (!bMoving || MC.OkToRelaseVacDuringRecipe) && MS.HasPrimingPlate;
            buttonRunPrimingVacuum.BackColor = MC.PrimingVacOK && MS.HasPrimingPlate ? Color.Lime : ControlColor;
            buttonRunPrimingVacuum.Text = !MS.HasPrimingPlate ? PrimingVacDisabled : MC.PrimingVacEngaged ? ReleasePrimingVac : PrimingText;
            buttonRunChuckVacuum.Enabled = bConnected && (!bMoving || MC.OkToRelaseVacDuringRecipe);
            buttonRunChuckVacuum.BackColor = !MC.ChuckVacEngaged ? ControlColor : MC.ChuckVacOK ? Color.Lime : Color.Red;
            buttonRunChuckVacuum.Text = MC.ChuckVacEngaged ? ReleaseChuckVac : ChuckText;
            labelSelectiveZones.Visible = MS.UsesSelectiveAirVacZones;
            labelSelectiveZones.Text = MS.UsesSelectiveAirVacZones ? $"Zones: {MS.GetActiveSelectiveZonesText()}" : "Not Using";

            //_currentRecipe = listBoxAdvRecipes != null && listBoxAdvRecipes.SelectedItem != null && listBoxAdvRecipes.SelectedItem.ToString() != "" && _frmMain.Recipes != null ? (Recipe)_frmMain.Recipes[listBoxAdvRecipes.SelectedItem.ToString()] : null;

            if (MS.CognexCommunicationsUsed && _frmMain.CognexVision.AcquireResults())
            {
                //Retreive Job Status Pass/Fail
                string leftSerial = _frmMain.CognexVision.LeftCamera.Sensor.SerialNumber;
                bool resultSetLeftPassed = _frmMain.CognexVision.RightCamera.Results.Cells["E24"].ToString() == "1";
                bool resultSetRightPassed = _frmMain.CognexVision != null && _frmMain.CognexVision.RightCamera.Results.Cells["E24"].ToString() == "1";

                labelLeftCamJobSts.BackColor = resultSetLeftPassed ? Color.Lime : Color.Red;
                labelRightCamJobSts.BackColor = resultSetRightPassed ? Color.Lime : Color.Red;
                labelLeftCamJobSts.Text = resultSetLeftPassed ? "PASS" : "FAIL";
                labelRightCamJobSts.Text = resultSetRightPassed ? "PASS" : "FAIL";
            }

            expandablePanelRecipes.TitleText = listBoxAdvRecipes.SelectedIndex != -1 && listBoxAdvRecipes.SelectedItem != null ? listBoxAdvRecipes.SelectedItem.ToString() : "No Recipe Selected...";

            buttonRunMainAir.Enabled = !MC.Moving;
            buttonRunMainAir.Text = MC.MainAirValveOpen ? "Disable Main Air" : "Enable Main Air";
            buttonRunMainAir.BackColor = MC.MainAirValveOpen ? ControlColor : Color.Red;

            groupBoxDispenseRate.Visible = MS.AdvantechServerInstalled;

            if (MS.AdvantechServerInstalled)
            {
                var inputA = AdvServer.GetAnalogInputByName("Pump A Flow");
                labelPumpAFlow.Text = $"A:{inputA?.Value ?? 0:#0.##} {inputA.Unit}";

                labelPumpBFlow.Visible = MS.DualPumpInstalled;

                if (labelPumpBFlow.Visible)
                {
                    var inputB = AdvServer.GetAnalogInputByName("Pump B Flow");
                    labelPumpBFlow.Text = $"B:{inputB?.Value ?? 0:#0.##} {inputB.Unit}";
                }
            }
            labelDispenseRate.Text = MC.CurrentDispenseRate.ToString("#0.00") + " µl/s";
            labelXPos.Text = MC.XPos.ToString("#0.000") + " mm";

            groupBoxDiePressure.Visible = MS.HasDiePressureTransducer || MS.AdvantechServerInstalled;

            if (MS.AdvantechServerInstalled)
            {
                var inputPressure = AdvServer.GetAnalogInputByName("Die Pressure");
                groupBoxDiePressure.Text = "Die Press";
                labelDiePressure.Text = $"{inputPressure?.Value ?? 0:#0.##}";
                labelDiePressureUnits.Text = $"{inputPressure.Unit}";
            }
            else
            {
                if (_smoothed)
                {
                    groupBoxDiePressure.Text = "Die Pressure Avg.";
                    labelDiePressure.Text = $"{MC.DiePressurePSISmoothed:0.##} PSI";
                }
                else
                {
                    groupBoxDiePressure.Text = "Die Pressure";
                    labelDiePressure.Text = $"{MC.DiePressurePSI:0.##} PSI";
                }
            }

            labelAirKnifeOn.Visible = MS.AirKnifeInstalled;
            buttonAirKnifeWarmUpMode.Visible = MS.AirKnifeHeaterInstalled;
            buttonAirKnifeWarmUpMode.Enabled = !MC.RunningRecipe;
            buttonAirKnifeWarmUpMode.BackColor = MC.AirKknifeWarmupModeEnabled ? Color.Lime : ControlColor;
            buttonAirKnifeWarmUpMode.Text = $"{(MC.AirKknifeWarmupModeEnabled ? "Stop" : "Start")} Air Knife Warm-Up";
            if (MS.AirKnifeInstalled)
            {
                labelAirKnifeOn.BackColor = _frmMain.MC.AirKknifeWarmupModeEnabled ? Color.Orange : _frmMain.MC.AirKnifeOn ? Color.Lime : Color.Gray;
                labelAirKnifeOn.Text = _frmMain.MC.AirKknifeWarmupModeEnabled ? "Air Knife Warming Up" : _frmMain.MC.AirKnifeOn ? "Air Knife On" : "Air Knife Off";
            }

            if (MS.IRLampInstalled)
            {
                labelIRPowerLevel.Text = $"{MC.IRTransmitter.CurrentPowerLevel:##0.0} %";

                buttonIRLampOff.Visible = _frmMain.MC.IRTransmitter != null;
                buttonIRLampOff.Enabled = _frmMain.MC.IRTransmitter != null && MC.IRTransmitter.CurrentPowerLevel > 0 && !MC.RunningRecipe;
            }
            else
            {
                buttonIRLampOff.Visible = MS.IRLampInstalled;
                groupBoxIRPowerLevel.Visible = MS.IRLampInstalled;
            }

            buttonVisionOperations.BackColor = _visionOpsShowing ? Color.Yellow : ControlColor;
            buttonVisionOperations.Text = $"{(_visionOpsShowing ? "Hide" : "Show")} Vision Operations";
            buttonStackOperations.BackColor = _stackOpsShowing ? Color.Yellow : ControlColor;
            buttonStackOperations.Text = $"{(_stackOpsShowing ? "Hide" : "Show")} Stack Operations";

			CheckOKToRun();
        }

        #endregion

        #region Control Event Handlers

        private void FormRun_Load(object sender, EventArgs e)
        {
            _timerStartCoating = new System.Windows.Forms.Timer();
            _timerStartCoating.Interval = 250;
            _timerStartCoating.Tick += new EventHandler(timerStartCoating_Tick);
            _timerStartCoating.Enabled = false;

            if (MC != null)
            {
                MC.OnRecipeFinished += new GalilWrapper2.OnRecipeFinishedEventHandler(PLC_OnRecipeFinished);
            }

            _numCycles = (int)_frmMain.Storage.CycleCount;
            checkBoxVacuumDisable.Visible = Settings.Default.ShowBypassVacuum;
            checkBoxVacuumDisable.Checked = MC.VacuumDisabled;
            buttonRunTransfer.Text = !MS.HasStack ? "Transfer\rto\rUnload\rPosition" : "Transfer\rTo\rStack";
#if DEBUG
            buttonRunRun.Enabled = true;
#endif

            if (listBoxAdvRecipes.Items.Count == 0)
            {
                List<string> recipes = _recipeMgr.RecipeList;
                recipes.Remove("Defaults");
                listBoxAdvRecipes.Items.AddRange(recipes.ToArray());
            }

            ShowVisionOps(false);
            ShowStackOps(false);
            expandablePanelRecipes.Expanded = false;
        }

        void timerStartCoating_Tick(object sender, EventArgs e)
        {
            _timerStartCoating.Enabled = false;
            if (_loopMode)
            {
                System.Threading.Thread.Sleep(2000);
                if (!MC.ErrorOccurred)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Loop Mode: Starting Next Coating", "INFO");
                    buttonRunRun_Click(this, new EventArgs());
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "Loop Mode: Error occurred, disabling looper!", "WARNING");
                    _loopMode = false;
                }
            }
        }

        void PLC_OnRecipeFinished(object sender, RecipeEventArgs eventArgs)
        {
            if (InvokeRequired)
            {
                DelegateOnRecipeFinished del = new DelegateOnRecipeFinished(PLC_OnRecipeFinished);
                Invoke(del, new object[] { sender, eventArgs });
            }
            else
            {
                if (MC.Connected)
                {
                    if (MC.ErrorOccurred)
                    {
                        _log.log(LogType.TRACE, Category.INFO, "PLC_OnRecipeFinished: Error Detected During Run!" + eventArgs.Message, "WARNING");
                        _loopMode = false;
                        nRadMessageBox.Show(this, "Recipe Run Failed. Error detected by Motion Cotroller!\r\nCheck Error Codes for additional information!", "Recipe Failed With Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        checkBoxXRunLoop.Checked = false;
                    }
                    else if (eventArgs.ReturnCode != 0)
                    {
                        _loopMode = false;
                        _log.log(LogType.TRACE, Category.INFO, "PLC_OnRecipeFinished: " + eventArgs.Message);
                        switch (eventArgs.ReturnCode)
                        {
                            case -999:
                            {
                                if (DialogResult.Yes == nRadMessageBox.Show(this, "Recipe Abort Complete.  Return to Maintenance Position and Recharge Pump?", "Recipe Abort Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                                {
                                    _log.log(LogType.TRACE, Category.INFO, "User confirmed return to maintenance area", "ACTION");
                                    _frmMain.LastClick = DateTime.Now;
                                    MC.RunGotoMaintenance();
                                    MC.RunRecharge();
                                }
                            }
                            break;
                            default:
                            {
                                nRadMessageBox.Show(this, "Recipe Run Failed: " + eventArgs.Message, "Recipe Complete With Errors", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                checkBoxXRunLoop.Checked = false;
                            }
                            break;
                        }

                        //Release Vacuum On Completion
                        if (_currentRecipe.ReleaseVacuumOnCompletion)
                        {
                            if (MC.ReleaseVacuum(Zones.AllChuck))
                                _log.log(LogType.TRACE, Category.INFO, "Vacuume Zones Release Upon Recipe Completion.");
                            else
                                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Vacuume Zones Failed To Release Upon Recipe Completion!");
                        }
                    }
                    else
                    {
                        if (checkBoxXRunLoop.Checked)
                        {
                            _timerStartCoating.Enabled = true;
                        }

                        //Release Vacuum On Completion
                        if (_currentRecipe.ReleaseVacuumOnCompletion)
                        {
                            if (MC.ReleaseVacuum(Zones.AllChuck))
                                _log.log(LogType.TRACE, Category.INFO, "Vacuume Zones Release Upon Recipe Completion.");
                            else
                                _log.log(LogType.TRACE, Category.ERROR, "ERROR: Vacuume Zones Failed To Release Upon Recipe Completion!");
                        }
                    }
                }
                else
                {
                    checkBoxXRunLoop.Checked = false;
                }
                _log.log(LogType.TRACE, Category.INFO, $"******************** RECIPE RUN COMPLETE!", "", SubCategory.RECIPE_RUN);
            }

            //Reset Devices Array After Recipe Completion
            MC.DownloadDevicesUsed(_defaultRecipe, _currentRecipe, true);
            if (MC.AutoMode)
                MC.SetReadyToUnload();
        }

        public void HandleRecipeListChanged()
        {
            _newRecipeList = true;
        }

        public void HandleRecipeChange(string recipeName)
        {
            if (_currentRecipe?.Name == recipeName)
            {
                _updateCurrentRecipe = true;
            }
        }

        private void buttonRunPrepareLoad_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Prepare To Load Button", "Action");

            if (_loading)
            {
                if (DialogResult.No == nRadMessageBox.Show(this, "System is ready to load.  Run sequence again?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined re-run Prepare To Load", "Action");
                }
            }

            if ((MC.Zone1VacEngaged && MC.Zone1VacOK) || (MC.Zone2VacEngaged && MC.Zone2VacOK) || (MC.Zone1VacEngaged && MC.Zone2VacOK))
            {
                nRadMessageBox.Show(this, "One or more zones show vacuum.  Please correct before Preparing to Load!", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MS.LiftPinsEnabled)
            {
                if (!MC.LiftPinsAreDown && !MS.LiftPinsDuringPrep2Load)
                {
                    if (DialogResult.Yes == nRadMessageBox.Show(this, "Lift Pins are Up. Lower before Prepare to Load?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        _log.log(LogType.TRACE, Category.INFO, "User Confirmed Lower Lift Pins", "Action");
                        MC.LowerLiftPins();
                    }
                    else
                    {
                        _log.log(LogType.TRACE, Category.INFO, "User Declined Lower Lift Pins", "Action");
                    }
                }
            }
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Prepare to Load a Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Prepare To Load", "Action");
                if (MS.HasLiftAndCenter)
                    PrepareToLoad();
                else
                    MC.PrepareToLoadSusbtrate();
                _loading = true;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Prepare To Load", "Action");
            }
        }

        private void buttonRunLoad_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Load Button", "Action");
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Load Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Load operation", "Action");
                _loading = false;
                if (MS.HasLiftAndCenter)
                    LoadSubstrate();
                else
                    MC.LoadSubstrate();
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Load operation", "Action");
            }
        }

        private void buttonRunUnload_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, $"User Pressed Run-Unload ('{buttonRunUnload.Text}') Button", "Action");

            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Unload Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed UnLoad operation", "Action");
                _loading = false;
                _unloading = true;
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined UnLoad operation", "Action");
                return;
            }

            if (MS.LiftPinsEnabled)
            {
                if (!MC.LiftPinsUpRequest)
                {
                    MC.UnLoadSubstrate();
                }
                else
                {
                    MC.LowerLiftPins();
                    _log.log(LogType.TRACE, Category.INFO, "Lowered Lift Pins", "Info");
                }
            }
            else if (MS.HasLiftAndCenter)
            {
                UnloadSubstrate();
            }
            else // no lift pins
            {
                MC.UnLoadSubstrate();
            }
        }

        private void buttonRunPrimingVacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Priming Vacuum Button", "Action");
            if (MC.PrimingVacEngaged)
            {
                _log.log(LogType.TRACE, Category.INFO, "Action is Release Priming Vacuum", "Info");
                MC.ReleaseVacuum(Zones.Priming);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "Action is Engage Priming Vacuum", "Info");
                MC.EngageVacuum(Zones.Priming);
            }
        }

        private void buttonRunChuckVacuum_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Chuck Vac Button", "Action");
            if (MC.ChuckVacEngaged)
            {
                _log.log(LogType.TRACE, Category.INFO, "Action is Release Chuck Vacuum", "Info");
                MC.ReleaseVacuum(Zones.AllChuck);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "Action is Engage Chuck Vacuum", "Info");
                MC.EngageVacuum(Zones.AllChuck);
            }
            _loading = false;
        }

        private void buttonRunHeadPurge_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Head Purge Button", "Action");
            _frmMain.LastClick = DateTime.Now;

            double volume = _frmMain.Storage.HeadPurgingVolume; // ul
            double speed = _frmMain.Storage.HeadPurgeRate;  // ul/s
            double rechargeRate = _frmMain.Storage.HeadPurgeRechargeRate;  // ul/s
            double ulConv = MC.uLConv;

            var prompt = $"Ready to begin Head Purge of {volume:#.###} µl at {speed:#.###} µl/s?";

            if (MC.ELOPressed || MC.ELOWasTripped)
            {
                nRadMessageBox.Show(this, "Cannot proceed due to EMO Status!.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (MC.PrimeRunning)
            {
                nRadMessageBox.Show(this, "Another prime operation is already running.  Please allow the previous operation to complete.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (Math.Abs(MC.XPos - MS.XMaintLoc) > 2)
            {
                _log.log(LogType.TRACE, Category.INFO, "Detected die lips not over trough", "WARNING");
                prompt = "Head does not appear to be over Maintenance positon.  Begin Head Purge anyway?";
            }

            if (MS.DisableHeadPurge)
            {
                nRadMessageBox.Show(this, "Head Purge Is Disabled. \nPlease Visit The Setup/Options Page To Enable This Feature.", "Head Purge", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (DialogResult.Yes == nRadMessageBox.Show(this, prompt, "Confirm Purge", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed Start Head Purge", "Action");
                _log.log(LogType.TRACE, Category.INFO, "===========================");
                _log.log(LogType.TRACE, Category.INFO, "  Beginning Head Purge");
                _log.log(LogType.TRACE, Category.INFO, $"      Volume: {volume:#.###}");
                _log.log(LogType.TRACE, Category.INFO, $"    Dispense: {speed:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"    Recharge: {rechargeRate:#.###} µl/s");
                _log.log(LogType.TRACE, Category.INFO, $"      ulConv: {ulConv:#.###} µl/cnts");

                MC.StartPurge(PumpOp.Head, volume, speed, rechargeRate, ulConv, 4);
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Start Head Purge", "Action");
            }
        }

        private void buttonRunLipWipe_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Move To Wipe Position  Button", "Action");
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Move Shuttle to Home Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Wipe Lip action", "Action");
                _frmMain.LastClick = DateTime.Now;
                MC.RunGotoHome();
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Wipe Lip action", "Action");
            }
        }

        private void buttonRunMainAir_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Main Air Button", "Action");
            if (MC.MainAirValveOpen)
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you sure you wish to disable main air?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Confirmed Disable Main Air action", "Action");
                    MC.SetAirState(false);
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined Disable Main Air action", "Action");
                }
            }
            else
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to enable main air?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Confirmed Enable Main Air Action", "Action");
                    MC.SetAirState(true);
                }
                else
                {
                    _log.log(LogType.TRACE, Category.INFO, "User Declined UnLoad operation", "Action");
                }
            }
        }

        private void buttonRunRun_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            double typeVal = MS.UsingKeyenceLaser ? 1 : 0;
            string tag = MS.UsingKeyenceLaser ? "" : "NOT ";
            _log.log(LogType.TRACE, Category.WARN, $"Informing Galil That Keyence Lasers Will {tag}Be Used... (gMem[15]={typeVal:0})", 
                "", SubCategory.RECIPE_RUN);
            MC.SetMemoryVal(15, typeVal);

            if (_runningRecipe)
            {
                _log.log(LogType.TRACE, Category.INFO, "User pressed Run-Abort Recipe button.", "", SubCategory.ACTION);
                _log.log(LogType.TRACE, Category.INFO, "********** Aborting Recipe...", "", SubCategory.RECIPE_RUN);
                _aborting = true;
                MC.AbortRecipe();
            }
            else
            {
                _aborting = false;
                _defaultRecipe = _recipeMgr.DefaultRecipe;

                LogSelectedDualPumpMode();

                if (MC.AutoMode || _loopMode || DialogResult.Yes == nRadMessageBox.Show(this, string.Format("Run Recipe '{0}'?", _currentRecipe.Name), "Confirm Run", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _log.log(LogType.TRACE, Category.INFO, "User confirmed start of recipe.", "", SubCategory.ACTION);
                    StartRunRecipe();
                }
            }
        }

        private void comboBoxRunRecipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (listBoxAdvRecipes.SelectedItem != null)
            {
                string recipeName = listBoxAdvRecipes.SelectedItem.ToString();
                Recipe newRecipe = _recipeMgr.FetchRecipe(recipeName);

                _currentRecipe = newRecipe;
                expandablePanelRecipes.Expanded = false;
            }
            else
            {
                _currentRecipe = null;
            }
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (!_frmMain.UserCanEditRecipe)
            {
                nRadMessageBox.Show(this, "Current User does not have Edit Recipe Permissions", "Authentication", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (_currentRecipe == null)
            {
                buttonEdit.Enabled = false;
                return;
            }

            if (_currentRecipe.IsSegmented)
            {
                if (_frmMain.frmSegmentedRecipeEditor.LoadRecipe(_currentRecipe.Name))
                {
                    _frmMain.LoadSubForm(_frmMain.frmSegmentedRecipeEditor);
                }
            }
            else
            {
                if (_frmMain.frmRecipeEditor.LoadRecipe(_currentRecipe.Name))
                {
                    _frmMain.LoadSubForm(_frmMain.frmRecipeEditor);
                }
            }
        }

        private void buttonLockScreen_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User confirmed logout on run screen.", "ACTION");
                _frmMain.ShowLoginForm();
            }
        }

        private void buttonRunTransfer_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, string.Format("User Pressed Run-Transfer to Stack Button. Lift is requested as '{0}'", MC.LiftPinsUpRequest ? "Up" : "Down"), "Action");

            if (MC.TransferSubstratePresent)
            {
                nRadMessageBox.Show(this, "There Is Already Glass On The Transfer Conveyor! Unload Glass First!", "Confirmation Required", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            if (MS.HasStack)
            {
                if (PLC.StackSubstrateCount < 2)
                {
                    if (PLC.IsLoadArmInSafePos)
                    {
                        if (PLC.IsLoadArmDocked)
                        {
                            if (!PLC.IsEndEffectorOccupied)
                            {
                                if (DialogResult.Yes == nRadMessageBox.Show(this, "End Effector Is Docked With Load Arm. Park End Effector? ", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    PLC.ParkEndEffector();
                                    _startTime = DateTime.Now;
                                    _waitingToUnloadAndXfer = true;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                nRadMessageBox.Show(this, "There Is Glass On End Effector And It Is Docked With Load Arm. Unload Glass First!", "Confirmation Required", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                return;
                            }
                        }
                        else
                        {
                            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Unload and Transfer Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                MC.UnLoadTransferSubstrate();
                            }
                        }
                    }
                    else
                    {
                        if (PLC.IsLoadArmDocked)
                        {
                            if (!PLC.IsEndEffectorOccupied)
                            {
                                if (DialogResult.Yes == nRadMessageBox.Show(this, "End Effector Is Docked With Load Arm. Park End Effector? ", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    PLC.ParkEndEffector();
                                    _startTime = DateTime.Now;
                                    _waitingToUnloadAndXfer = true;
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                nRadMessageBox.Show(this, "There Is Glass On End Effector And It Is Docked With Load Arm. Unload Glass First!", "Confirmation Required", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                return;
                            }
                        }
                        else
                        {
                            if (DialogResult.Yes == nRadMessageBox.Show(this, "Load Arm Not In A Safe Position to Unload and Transfer Substrate! Move To Clear Position?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                PLC.GotoClearPosEndEffector();
                                _startTime = DateTime.Now;
                                _waitingToUnloadAndXfer = true;
                            }
                            else
                                return;
                        }
                    }
                }
                else
                {

                    nRadMessageBox.Show(this, string.Format("There Are Already {0} Substrates In The Process Stack!", PLC.StackSubstrateCount), "Process Stack Full", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else // no stack
            {
                if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready to Unload and Transfer Substrate?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    MC.UnLoadTransferSubstrate();
                }
            }
        }

        private void buttonProcessStackOperations_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Goto Process Stack Op Button", "Action");
            _frmMain.LoadSubForm(_frmMain.frmProcessStack);
        }

        private void labelDiePressure_Click(object sender, EventArgs e)
        {
            _smoothed = !_smoothed;
        }

        private void buttonGotoVision_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, "User Pressed Run-Move To Vision Position  Button", "Action");
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Ready To Move Shuttle To Vision Position?", "Move Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _log.log(LogType.TRACE, Category.INFO, "User Confirmed Vision Position action", "Action");
                _frmMain.LastClick = DateTime.Now;
                //MC.RunGotoVisionPosition();
            }
            else
            {
                _log.log(LogType.TRACE, Category.INFO, "User Declined Vision Position action", "Action");
            }
        }

        private void buttonCognexView_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.LoadSubForm(_frmMain.frmVisionView);
        }

        private void buttonRunAlignersDown_Click(object sender, EventArgs e)
        {
            bool state = MC.AlignersUpRequest;
            _frmMain.LastClick = DateTime.Now;
            _log.log(LogType.TRACE, Category.INFO, $"Run Screen: User Pressed {(state ? "Lower" : "Raise")} Aligners Button", "Action");
            if (DialogResult.Yes == nRadMessageBox.Show(this, $"Ready to {(state ? "Lower" : "Raise")} Aligners?", "Confirmation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                if (state)
                    MC.LowerAligners();
                else
                    MC.RaiseAligners();
                _loading = false;
            }
        }

        private void labelRunLoopCycles_Click(object sender, EventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(_frmMain, "Reset Cycle Count?", "Cycle Count Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _numCycles = 0;
                _frmMain.Storage.CycleCount = 0;
                _frmMain.Storage.Save();
            }
        }

        private void checkBoxVacuumDisable_CheckedChanged(object sender, EventArgs e)
        {
            MC.VacuumDisabled = checkBoxVacuumDisable.Checked;
        }

        private void buttonAirKnifeWarmUpMode_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            MC.SetGasKnifeWarmUpMode(!MC.AirKknifeWarmupModeEnabled);
            _log.log(LogType.ACTIVITY, Category.INFO, "User Requested To Toggle Air Knife Warm-Up Mode.");
        }

        private void buttonIRLampOff_Click(object sender, EventArgs e)
        {
            if (!_frmMain.MS.IRLampInstalled || _frmMain.MC.IRTransmitter == null)
            {
                return;
            }

            _frmMain.LastClick = DateTime.Now;
            _frmMain.MC.IRTransmitter.SetPowerLevel(_frmMain.MS.IRIdlePower);
            _log.log(LogType.ACTIVITY, Category.INFO, "User Requested To Set IR Power Level To Idle.");
        }

        private void buttonStackOperations_Click(object sender, EventArgs e)
        {
            if (_visionOpsShowing)
            {
                ShowVisionOps(false);
            }

            ShowStackOps(!_stackOpsShowing);
        }

        private void buttonVisionOperations_Click(object sender, EventArgs e)
        {
            if (_stackOpsShowing)
            {
                ShowStackOps(false);
            }

            ShowVisionOps(!_visionOpsShowing);
        }

        #endregion

        #region Background Worker

        private void LoadSubstrate()
        {
            if (_bgwLDULD.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "LoadSubstrate() Failed To Start: Backround Worker Busy", "ERROR");
                return;
            }
            _bgwLDULD = null;
            _bgwLDULD = new BackgroundWorker();
            _bgwLDULD.DoWork += new DoWorkEventHandler(DoLoadSubstrate);
            _bgwLDULD.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwgLoadSubstrateCompleted);
            _bgwLDULD.RunWorkerAsync();
        }
        private void UnloadSubstrate()
        {
            if(_bgwLDULD.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "UnloadSubstrate() Failed To Start: Backround Worker Busy", "ERROR");
                return;
            }
            _bgwLDULD = null;
            _bgwLDULD = new BackgroundWorker();
            _bgwLDULD.DoWork += new DoWorkEventHandler(DoUnloadSubstrate);
            _bgwLDULD.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwgUnloadSubstrateCompleted);
            _bgwLDULD.RunWorkerAsync();
        }
        private void PrepareToLoad()
        {
            if (_bgwLDULD.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "PrepareToLoad() Failed To Start: Backround Worker Busy", "ERROR");
                return;
            }
            _bgwLDULD = null;
            _bgwLDULD = new BackgroundWorker();
            _bgwLDULD.DoWork += new DoWorkEventHandler(DoPrepareToLoad);
            _bgwLDULD.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bwgPrepareToLoadCompleted);
            _bgwLDULD.RunWorkerAsync();
        }

        private void DoLoadSubstrate(object sender, DoWorkEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "bgwDoLoadSubstrate() Started", "INFO");
            MC.LoadSubstrate();
            if (MC.AutoMode)
            {
                bool bContinue = true;
                int timeout = 0;
                int maxtimeout = (int)(2 / (_sleepTime/1000.0));
                while(MC.AutoMode && !(MC.ChuckLiftPinsStatus==LiftPinStatus.Loading) && timeout<maxtimeout)
                {
                    System.Threading.Thread.Sleep(_sleepTime);
                    timeout++;
                }
                if(timeout>=maxtimeout)
                {
                    MC.ResetReadyToLoad();
                    MC.AutoMode = false;
                    bContinue = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Auto Mode Load Substrate Timeout Waiting For Lift Pins State = Loading.", "ERROR");
                }
                timeout = 0;
                maxtimeout = (int)(10 / (_sleepTime/1000.0));
                while (bContinue && MC.AutoMode && !(MC.ChuckLiftPinsStatus == LiftPinStatus.Idle) && timeout<maxtimeout)
                {
                    System.Threading.Thread.Sleep(_sleepTime);
                    timeout++;
                }
                if (timeout >= maxtimeout)
                {
                    MC.ResetReadyToLoad();
                    MC.AutoMode = false;
                    bContinue = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Auto Mode Load Substrate Timeout Waiting For Lift Pins State = Idle.", "ERROR");
                }
                if(bContinue && MC.LiftPinsAreDown)
                {
                    MC.ResetReadyToLoad();
                    _log.log(LogType.TRACE, Category.INFO, "Substrate Loaded. Starting Coating", "INFO");
                    buttonRunRun_Click(this, new EventArgs());
                }
            }
        }
        private void DoUnloadSubstrate(object sender, DoWorkEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "bgwDoUnloadSubstrate() Started", "INFO");
            MC.UnLoadSubstrate();
        }
        private void DoPrepareToLoad(object sender, DoWorkEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "bgwPrepareToLoad() Started", "INFO");
            MC.PrepareToLoadSusbtrate();
        }
        private void bwgLoadSubstrateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MC.RobotLoading = false;
            _log.log(LogType.TRACE, Category.INFO, "bgwLoadSubstrate() Completed", "INFO");
        }
        private void bwgUnloadSubstrateCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MC.ResetReadyToUnload();
            MC.SetReadyToLoad();
            MC.RobotUnloading = false;
            _log.log(LogType.TRACE, Category.INFO, "bgwUnloadSubstrate() Completed", "INFO");
        }
        private void bwgPrepareToLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "bgwPrepareToLoad() Completed", "INFO");
        }
        private void InitializeBackgroundWorker()
        {
            _recipeWorker.DoWork += new DoWorkEventHandler(DoRunRecipe);
            _recipeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            _bgwRobotLoad.DoWork += new DoWorkEventHandler(DoRobotLoad);
            _bgwRobotLoad.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RobotLoadCompleted);

            _bgwRobotUnload.DoWork += new DoWorkEventHandler(DoRobotUnload);
            _bgwRobotUnload.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RobotUnloadCompleted);
        }

        private bool StartRobotLoad()
        {
            if(_bgwRobotUnload.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "Can't Start Robot Load While Robot Unload Is Running!", "ERROR");
                return false;
            }
            else if (_bgwRobotLoad.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "Robot Load Already Running!", "ERROR");
                return false;
            }
            else
            {
                _bgwRobotLoad.RunWorkerAsync();
                return true;
            }
        }
        private void DoRobotLoad(object sender, DoWorkEventArgs e)
        {
            bool loaded = false;
            bool aborted = false;
            MC.RobotLoading = true;
            _log.log(LogType.TRACE, Category.INFO, "Robot Load Started", "INFO");
            MC.SetOKToLDULD();
            int timeout = 0;
            int maxtimeout = (int)(30 / (_sleepTime / 1000.0));
            while (!MC.AbortLDULDReq && !MC.LoadComplete && timeout < maxtimeout)
            {
                System.Threading.Thread.Sleep(_sleepTime);
                timeout++;
            }
            if (timeout >= maxtimeout)
            {
                MC.AutoMode = false;
                MC.RobotLoadFailure = true;
                _log.log(LogType.TRACE, Category.ERROR, "Robot Load Timeout", "ERROR");
                MC.ResetOKToLDULD();
            }
            else if (MC.AbortLDULDReq)
            {
                aborted = true;
                _log.log(LogType.TRACE, Category.INFO, "Robot Load Abort Request", "INFO");
            }
            else
            {
                if (!MC.SubstratePresent)
                {
                    MC.RobotLoadFailure = true;
                    MC.AutoMode = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Robot Load Failed. Substrate Not Present", "ERROR");
                }
                else
                    loaded = true;
            }
            MC.ResetOKToLDULD();
            timeout = 0;
            maxtimeout = (int)(5 / (_sleepTime / 1000.0));
            while ((MC.LoadRequest || MC.LoadComplete || MC.AbortLDULDReq) && timeout < maxtimeout)
            {
                System.Threading.Thread.Sleep(_sleepTime);
                timeout++;
            }
            if (timeout >= maxtimeout)
            {
                MC.RobotLoadFailure = true;
                MC.AutoMode = false;
                _log.log(LogType.TRACE, Category.ERROR, "Robot Load Timeout Waiting On Request To Clear", "ERROR");
            }
            else if(loaded)
            {
                MC.ResetReadyToLoad();
                if (!aborted)
                    _substrateLoaded = true;
            }
        }
        private void RobotLoadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "Robot Load Completed", "INFO");
            MC.RobotLoading = false;
            if (_substrateLoaded)
                LoadSubstrate();
        }
        private bool StartRobotUnload()
        {
            if (_bgwRobotLoad.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "Can't Start Robot Unload While Robot Load Is Running!", "ERROR");
                return false;
            }
            else if (_bgwRobotUnload.IsBusy)
            {
                _log.log(LogType.TRACE, Category.ERROR, "Robot Unload Already Running!", "ERROR");
                return false;
            }
            else
            {
                _bgwRobotUnload.RunWorkerAsync();
                return true;
            }
        }
        private void DoRobotUnload(object sender, DoWorkEventArgs e)
        {
            bool bCont = true;
            MC.RobotUnloading = true;
            _log.log(LogType.TRACE, Category.INFO, "Robot Unload Started", "INFO");
            int timeout = 0;
            int maxtimeout = 0;
            if (!MC.LiftIsUp)
            {
                UnloadSubstrate();
                timeout = 0;
                maxtimeout = (int)(10 / (_sleepTime / 1000.0));
                while (!MC.LiftIsUp && timeout<maxtimeout)
                {
                    System.Threading.Thread.Sleep(_sleepTime);
                    timeout++;
                }
                if(timeout>=maxtimeout)
                {
                    bCont = false;
                    MC.AutoMode = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Substrate Unload Timeout", "ERROR");
                }
            }
            if(bCont)
                MC.SetOKToLDULD();
            timeout = 0;
            maxtimeout = (int)(30 / (_sleepTime / 1000.0));
            while (bCont && !MC.AbortLDULDReq && !MC.UnloadComplete && timeout < maxtimeout)
            {
                System.Threading.Thread.Sleep(_sleepTime);
                timeout++;
            }
            if(timeout>=maxtimeout)
            {
                MC.RobotUnloadFailure = true;
                MC.AutoMode = false;
                _log.log(LogType.TRACE, Category.ERROR, "Robot Unload Timeout", "ERROR");
                MC.ResetOKToLDULD();
            }
            else if(MC.AbortLDULDReq)
            {
                _log.log(LogType.TRACE, Category.INFO, "Robot Unload Abort Request", "INFO");
            }
            else
            {
                if(bCont && MC.SubstratePresent)
                {
                    MC.RobotUnloadFailure = true;
                    MC.AutoMode = false;
                    _log.log(LogType.TRACE, Category.ERROR, "Robot Unload Failed. Substrate Still Present", "ERROR");
                }
            }
            MC.ResetOKToLDULD();
            timeout = 0;
            maxtimeout = (int)(5 / (_sleepTime / 1000.0));
            while ((MC.UnloadRequest || MC.UnloadComplete || MC.AbortLDULDReq) && timeout<maxtimeout)
            {
                System.Threading.Thread.Sleep(_sleepTime);
                timeout++;
            }
            if (timeout >= maxtimeout)
            {
                MC.RobotUnloadFailure = true;
                MC.AutoMode = false;
                _log.log(LogType.TRACE, Category.ERROR, "Robot Unload Timeout Waiting On Request To Clear", "ERROR");
            }
            else
            {
                if (bCont)
                {
                    MC.ResetReadyToUnload();
                    MC.SetReadyToLoad();
                }
            }
        }
        private void RobotUnloadCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "Robot Unload Completed", "INFO");
            MC.RobotUnloading = false;
        }
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, string.Format(" RunWorkerCompleted() - Recipe RC: {0}", MC.RecipeRunErrorCode), "INFO");

            if (_aborting)
            {
                _log.log(LogType.TRACE, Category.INFO, "RunWorkerCompleted - Recipe Aborted");
            }
            else
            {
                if (MC.RecipeRunErrorCode != 0)
                {
                    _loopMode = false;
                    _log.log(LogType.TRACE, Category.INFO, "RunWorkerCompleted Setting RunningRecipe = false");
                    _runningRecipe = false;
                }
                else if (checkBoxXRunLoop.Checked)
                {
                    _numCycles++;
                    _loopMode = true;
                    _log.log(LogType.TRACE, Category.INFO, "RunWorkerCompleted Setting RunningRecipe = false");
                    _runningRecipe = true;
                }
                else
                {
                    _numCycles++;
                    _loopMode = false;
                    _log.log(LogType.TRACE, Category.INFO, "RunWorkerCompleted Setting RunningRecipe = false");
                    _runningRecipe = false;
                }
            }

            if (MS.ProcessLogsEnabled)
            {
                _processLogs.RecipeStopped(_aborting, MC.RecipeRunErrorCode);
            }

            _log.log(LogType.TRACE, Category.INFO, "RunWorkerCompleted Setting RunningRecipe = false");
            _aborting = false;
            _runningRecipe = false;
            _frmMain.Storage.CycleCount = _numCycles;
            _frmMain.Storage.LifetimeCount++;
            _frmMain.Storage.Save();
            if (MC.AutoMode && _currentRecipe.UnloadSubstrateOnCompletion)
                UnloadSubstrate();
        }

        private void DoRunRecipe(object sender, DoWorkEventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, " Calling PLC.RunRecipe()", "", SubCategory.RECIPE_RUN);
            _log.log(LogType.TRACE, Category.INFO, " Calling PLC.RunRecipe()", "", SubCategory.RECIPE_RUN);
            MC.RunRecipe(_defaultRecipe, _currentRecipe, _frmMain.CognexVision);
        }

        #endregion

        #region Private Functions

        private void InitializeRunBlockers()
        {
            var runBlockers = new List<RunBlocker>()
            {
                new RunBlocker(() => MC is null, "Galil Not Connected"),
                new RunBlocker(() => !MC.MainAirOK, "Main Air"),
                new RunBlocker(() => !MC.VacuumDisabled && !MC.MainVacOK, "Main Vacuum"),
                new RunBlocker(() => !MS.SelectiveZonesConfigOK, "Selective Zones"),
                new RunBlocker(() => !MC.VacuumDisabled && (!MC.Zone1VacOK || !MC.Zone2VacOK || !MC.Zone3VacOK), "Vacuum Chuck Zones"),
                new RunBlocker(() => !MC.MainAirValveOpen, "Main Air Valve"),
                new RunBlocker(() => !MC.IsHomed, "Not Homed"),
                new RunBlocker(() => _runningRecipe, "Recipe Running"),
                new RunBlocker(() => _currentRecipe is null, "No Recipe Selected"),
                new RunBlocker(() => _recipeMgr.DefaultRecipe is null, "No Default Recipe"),
                new RunBlocker(() => (_currentRecipe?.HasPriming ?? false) && !MC.VacuumDisabled && MS.HasPrimingPlate && !MC.PrimingVacOK, "Priming Vacuum"),
                new RunBlocker(() => !MC.IsPrimingMaxZSet, "Priming Plate Max Z"),
                new RunBlocker(() => MC.ELOWasTripped || MC.ELOPressed, "EMO Status"),
                new RunBlocker(() => MC.SafetyGuardsActive && !MS.BypassSafetyGuards, "System In Motion"),
                new RunBlocker(() => _frmMain.Recirculating, "Fluid Recirculation is Enabled"),
                new RunBlocker(() => MS.AlignersEnabled && MC.AlignersUpRequest, "Aligners"),
                new RunBlocker(() => MS.LiftPinsEnabled && (!MC.LiftPinsAreDown || MC.LiftPinsUpRequest), "Lift Pins Up"),
                new RunBlocker(() => MS.HasLoader && !MC.LoaderIsClear, "Loader Position"),
                new RunBlocker(() => _currentRecipe != null && _currentRecipe.UsingKeyenceLaser && LaserMgr != null && !LaserMgr.Connected, "Lasers"),
                new RunBlocker(() => MS.CognexCommunicationsUsed && !_frmMain.CognexVision.LeftCamIsConnected, "Left Camera"),
                new RunBlocker(() => MS.CognexCommunicationsUsed && !_frmMain.CognexVision.RightCamIsConnected, "Right Camera"),
                new RunBlocker(() => MC.Moving, "Motor Moving"),
                new RunBlocker(() => MS.HasReservoirSensors && MC.ReservoirEmpty, "Reservoir Empty"),
                new RunBlocker(() => CheckPumps(), "Dual Pump Mismatch"),
            };

            _runChecker = new RunIsOKChecker(_log, runBlockers);
        }

        private bool CheckPumps()
        {
            return !MS.DualPumpInstalled && _currentRecipe != null && 
                (_currentRecipe.SelectedPump == "1" || _currentRecipe.SelectedPump == "2");
        }

        private void LogSelectedDualPumpMode()
        {
            if (MS.HidePumpMixing)
            {
                return;
            }

            if (_currentRecipe.SelectedPump == "MIXING")
            {
                _log.log(LogType.TRACE, Category.INFO, "RecipeRun() INFO: RecipeRun() Dual-Pump Mixing Requested.", "", SubCategory.RECIPE_RUN);
            }
            else if (_currentRecipe.SelectedPump == "Pump-A")
            {
                _log.log(LogType.TRACE, Category.INFO, "RecipeRun() INFO: RecipeRun() Pump-A Dispense Requested.", "", SubCategory.RECIPE_RUN);
            }
            else if (_currentRecipe.SelectedPump == "Pump-B")
            {
                _log.log(LogType.TRACE, Category.INFO, "RecipeRun() INFO: RecipeRun() Pump-B Dispense Requested.", "", SubCategory.RECIPE_RUN);
            }
        }

        private void ShowStackOps(bool show)
        {
            _stackOpsShowing = show;

            if (show)
            {
                panelStackOperations.Show();
                panelStackOperations.Visible = true;
                panelStackOperations.BringToFront();
            }
            else
            {
                panelStackOperations.Visible = false;
                panelStackOperations.SendToBack();
            }
        }

        private void ShowVisionOps(bool show)
        {
            _visionOpsShowing = show;

            if (show)
            {
                panelVisionOperations.Show();
                panelVisionOperations.Visible = true;
                panelVisionOperations.BringToFront();
            }
            else
            {
                panelVisionOperations.Visible = false;
                panelVisionOperations.SendToBack();
            }
        }

        private void WaitingOnUnloadAndXfer()
        {
            TimeSpan elapsedTime = DateTime.Now - _startTime;
            if ((elapsedTime.TotalSeconds < 5) && PLC.IsLoadArmInSafePos && !PLC.IsLoadArmDocked)
            {
                _waitingToUnloadAndXfer = false;
                MC.UnLoadTransferSubstrate();
            }
            else if (elapsedTime.TotalSeconds > 5)
            {
                _waitingToUnloadAndXfer = false;
                nRadMessageBox.Show(this, "Timeout Waiting On Load Arm Action!", "Timeout Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void CheckOKToRun()
        {
            if (_runChecker.OKToRun)
            {
                buttonRunRun.Enabled = true;
                buttonRunRun.Text = "Run\r\nRecipe";
                buttonRunRun.BackColor = ControlColor;
                labelRunStatus.BackColor = Color.Lime;
                labelRunStatus.Text = $"Ready to run recipe: {_currentRecipe.Name} - {_currentRecipe.CoatHeight} µm at {_currentRecipe.CoatingVel.ToString("0.000")} mm/s, Pump: {_currentRecipe.SelectedPump} Rate: {_currentRecipe.DispenseRate.ToString("0.000")} µl/s";
            }
            else
            {
                if (!_runningRecipe)
                {
                    buttonRunRun.Enabled = false;
                    buttonRunRun.Text = "Not\r\nReady";
                    labelRunStatus.Text = $"Not Ready! Please check: {_runChecker.StatusString}";
                    labelRunStatus.BackColor = Color.Red;
                }
                else
                {
                    buttonRunRun.Enabled = !_aborting;
                    buttonRunRun.Text = _aborting ? "Aborting\r\nRecipe" : "Abort\r\nRecipe";
                    buttonRunRun.BackColor = _aborting ? Color.Blue : Color.Red;
                    //labelRunStatus.Text = "Running Recipe: " + _currentRecipe.Name;
                    string stateMsg = _aborting ? "Aborting... " : MC.RecipeStateMsg;
                    labelRunStatus.Text = $"Running Recipe: {_currentRecipe.Name} - {_currentRecipe.CoatHeight} µm at {_currentRecipe.CoatingVel.ToString("0.000")} mm/s   Pump Rate: {_currentRecipe.DispenseRate.ToString("0.000")} µl/s" + Environment.NewLine + stateMsg;
                    labelRunStatus.BackColor = Color.Fuchsia;
                }
            }
        }

        private void StartRunRecipe()
        {
            if (!_runningRecipe)
            {
                _log.log(LogType.TRACE, Category.INFO, "Begin running recipe: " + _currentRecipe.Name, "", SubCategory.RECIPE_RUN);
                _runningRecipe = true;
                _log.log(LogType.TRACE, Category.INFO, "Requesting Thread Start", "", SubCategory.RECIPE_RUN);

                if (MS.ProcessLogsEnabled)
                {
                    _processLogs.RecipeStarted(_currentRecipe);
                }

                _recipeWorker.RunWorkerAsync();
            }
        }

        private void RefreshRecipeList()
        {
            string currentRecipeName = (string)listBoxAdvRecipes.SelectedItem ?? "";

            listBoxAdvRecipes.Items.Clear();
            List<string> recipes = _recipeMgr.RecipeList;
            recipes.Remove("Defaults");
            listBoxAdvRecipes.Items.AddRange(recipes.ToArray());

            if (currentRecipeName != "" && listBoxAdvRecipes.Items.Contains(currentRecipeName))
            {
                listBoxAdvRecipes.SelectedItem = currentRecipeName;
                _updateCurrentRecipe = true;
            }
            else
            {
                _currentRecipe = null;
                _updateCurrentRecipe = false;
            }
        }

        private void RefreshCurrentRecipe()
        {
            if (_currentRecipe != null)
            {
                _currentRecipe = _recipeMgr.FetchRecipe(_currentRecipe.Name);
            }
        }

        #endregion

        #endregion
    }
}
