using nAble.Data;
using nAble.DataComm;
using nAble.DataComm.AdvantechSerialServer;
using nAble.DataComm.AdvantechSerialServer.AdvantechComms;
using nAble.DataComm.KeyenceLasers;
using nAble.Model;
using nTact;
using nTact.DataComm;
using nTact.PLC;
using nTact.Recipes;
using Support2;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Reflection;

namespace nAble
{
    public partial class FormMain : Form
    {
        #region Constants

        private static readonly List<AnalogInput> _advantechServerInputs = new List<AnalogInput>
        {
            new AnalogInput(0, "Die Pressure", "mbar", -12.4, 250),
            new AnalogInput(1, "Pump A Flow", "ml/m", -10, 10),
            new AnalogInput(2, "Pump B Flow", "ml/m", -10, 10),
        };

        #endregion

        #region Properties

        public NRadLicensing2 LicMgr { get; } = null;
        public MachineSettingsII MS { get; private set; } = null;
        public MachineStorage Storage { get; private set; } = null;
        public string LicenseFilesDirectory { get; } = "";
        public string LogDirectory { get; } = "";

        public GalilWrapper2 MC { get; private set; } = null;
        public ILaserManager KeyenceLaser => MC?.LaserMgr;
        public clsCognex CognexVision { get; private set; } = null;

        public Omega485Controller Omega485Controller { get; private set; } = null;
        public AdvantechServer AdvServer { get; private set; } = null;
        public PLCWrapper PLC { get; private set; } = null;

        public double KeyenceLeftVal { get; private set; } = 0.0;
        public double KeyenceRightVal { get; private set; } = 0.0;

        public bool RecipeSelected { get; private set; } = false;
        public bool DebugMode =>
#if DEMOMODE
                true;
#else
                false;
#endif


        public bool UseLicenseMgr =>
#if DISABLE_LICENSE_MGR
                false;
#else
                true;
#endif


        public bool IsDemoMode => MC.IsDemo;
        public int LastAccessLevel { get; set; } = 0;
        public int AccessLevel { get; set; } = 0;
        public bool UserCanOperate => AccessLevel >= 1;
        public bool UserCanEditRecipe => AccessLevel >= 2;
        public bool UserIsAdmin => AccessLevel >= 3;
        public bool TempKeyEntered { get; set; } = false;
        public DateTime LastClick { get; set; }
        public string StatusMessage => labelMainStatus.Text;
        public bool PLCIsConnected => PLC.IsConnected;
        public bool SWIsLocked { get; set; } = false;
        public bool Recirculating { get; private set; } = false;


        public string VacUnits { get; set; } = "inHg";

        public bool OverrideDataRecordTimeout { get; set; } = false;

        #region Form Properties

        private List<Form> _allForms = new List<Form>(30);
        private List<Form> _navForms = new List<Form>(10);

        internal FormChuck frmChuck = null;
        internal FormConfig frmConfig = null;
        internal FormLicensing frmLicensing = null;
        internal FormLockOut frmLockOut = null;
        internal FormFluidFlow frmFluidFlow = null;
        internal FormFluidMain frmFluidMain = null;
        internal FormFluidTemp frmFluidTemp = null;
        internal FormFluidTempAdv frmFluidTempAdv = null;
        internal FormJog frmJog = null;
        internal FormJogX frmJogX = null;
        internal FormJogZ frmJogZ = null;
        internal FormLogin frmLogin = null;
        internal FormMainMenu frmMainMenu = null;
        internal FormMaintenance frmMaintenance = null;
        internal FormNumInput frmNumInput = null;
        internal FormTextInput frmTextInput = null;
        internal FormTimeInput frmTimeInput = null;
        internal FormRecipe frmRecipe = null;
        internal FormRecipeEditor frmRecipeEditor = null;
        internal FormSegmentedRecipeEditor frmSegmentedRecipeEditor = null;
        internal FormRecipeAdd frmRecipeAdd = null;
        internal FormLogListing frmLogListing = null;
        internal FormLogViewer frmLogViewer = null;
        internal FormActivityLogViewer frmActivityLogViewer = null;
        internal FormAlarms frmAlarms = null;
        internal FormProcessStack frmProcessStack = null;
        internal FormProcessStackMaint frmProcessStackMaint = null;
        internal FormLoaderOperation frmLoaderOperation = null;
        internal FormPositionTeaching frmPositionTeaching = null;
        internal FormProcessStackSettings frmProcessStackSettings = null;
        internal FormJogTransfer frmJogTransfer = null;
        internal FormStackRecipes frmStackRecipes = null;
        internal FormVisionView frmVisionView = null;
        internal FormVacTrendCalendar frmVacTrendCalendar = null;
        internal FormVacuumTrend frmVacTrend = null;
        internal FormRun frmRun = null;
        internal FormSetup frmSetup = null;

        //TODO: Make not public!
        public Form _prevForm = null;
        private BackgroundWorker _bgwReadKeyence;
        private bool _readingKeyence;

        #endregion

        #endregion

        #region Data Members

        private readonly IRecipeManager _recipeMgr = null;
        private readonly LogEntry _log = null;
        private readonly DatabaseWrapper _db;

        private bool _formClosing = false;
        private bool _connecting = false;
        private bool _loggedIn = false;
        private bool _confirmLeave = false;
        private string _confirmLeaveMsg = "";
        private string _confirmLeaveTitle = "";
        private string _lastMessage, _lastStackErrorMessage;
        private bool _closePending = false;
        private bool _showStackErrors = false;

        private List<Control> _navControls = new List<Control>(10);

        private KeyenceState _eState0 = KeyenceState.Unknown;
        private KeyenceState _eState1 = KeyenceState.Unknown;

        #region Timers

        private System.Windows.Forms.Timer _timerScreenUpdate = null;
        private System.Windows.Forms.Timer _timerDataUpdate = null;
        private System.Windows.Forms.Timer _timerConnection = null;
        private System.Windows.Forms.Timer _timerGetHeight = null;
        private System.Windows.Forms.Timer _timerRecirc = null;
        private System.Windows.Forms.Timer _timerShowStackErrors = null;
        private System.Windows.Forms.Timer _timerRestartMessaging = null;
        private System.Windows.Forms.Timer _timerLicense = null;

        #endregion

        #endregion

        #region Functions

        #region Constructors

        public FormMain()
        {
            InitializeComponent();

            try
            {
                LogDirectory = Path.Combine(Application.StartupPath, "Data", "Logs");
                Directory.CreateDirectory(LogDirectory);

                _log = new LogEntry(7, LogDirectory, "Action", "Action Logger", _db);
                _log.log(LogType.TRACE, Category.INFO, "  =====================  nAble Application Started   ===================");
                nRadMessageBox.Logger = _log;

                LicenseFilesDirectory = Path.Combine(Application.StartupPath, "Data", "License Files");
                Directory.CreateDirectory(LicenseFilesDirectory);

                LicMgr = new NRadLicensing2();

                FileVersionInfo fileVerInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                _log.log(LogType.TRACE, Category.INFO, $"                             Ver : {fileVerInfo.ProductVersion}");
                _log.log(LogType.TRACE, Category.INFO, $"    Project: {LicMgr.ProjectNo}             Serial No. {LicMgr.SerialNumber}");

                LicMgr.Logger.NewLog += msg => _log.log(LogType.TRACE, Category.ANY, msg);

                var defaultRecipePath = Path.Combine(Application.StartupPath, @"Data\Recipes");

                if (!Directory.Exists(defaultRecipePath))
                {
                    Directory.CreateDirectory(defaultRecipePath);
                }

                _db = new DatabaseWrapper(Settings.Default.DataSource, Settings.Default.Database);
                MachineSettingsII.LogEntry = _log;
                MachineStorage.LogEntry = _log;

                _recipeMgr = new RecipeManager(LicMgr, _log, UseLicenseMgr, defaultRecipePath);
                PLC = new PLCWrapper(_log);
            }
            catch (Exception ex)
            {
                nRadMessageBox.Show(this, $"Error caught during initialization ({ex.Message}).  Please make sure nAble was run in Administrator mode.", "Security Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        #endregion Constructors

        #region Initialization

        private void LoadMainScreens()
        {
            try
            {
                if (MS.CognexCommunicationsUsed)
                {
                    frmVisionView = new FormVisionView(this, _log);
                    _allForms.Add(frmVisionView);
                }

                frmChuck = new FormChuck(this, _log);
                _allForms.Add(frmChuck);
                frmConfig = new FormConfig(this, _log);
                _allForms.Add(frmConfig);
                frmFluidFlow = new FormFluidFlow(this, _log);
                _allForms.Add(frmFluidFlow);
                frmFluidMain = new FormFluidMain(this);
                _allForms.Add(frmFluidMain);
                frmFluidTemp = new FormFluidTemp(this);
                _allForms.Add(frmFluidTemp);
                frmFluidTempAdv = new FormFluidTempAdv(this);
                _allForms.Add(frmFluidTempAdv);
                frmJog = new FormJog(this);
                _allForms.Add(frmJog);
                frmJogX = new FormJogX(this, _log);
                _allForms.Add(frmJogX);
                frmJogZ = new FormJogZ(this, _log);
                _allForms.Add(frmJogZ);
                frmLogin = new FormLogin(this, LicMgr, _log);
                _allForms.Add(frmLogin);
                frmMainMenu = new FormMainMenu(this, LicMgr, _log, _recipeMgr);
                _allForms.Add(frmMainMenu);
                frmMaintenance = new FormMaintenance(this, _log);
                _allForms.Add(frmMaintenance);
                frmNumInput = new FormNumInput(this, _log);
                _allForms.Add(frmNumInput);
                frmTextInput = new FormTextInput(this);
                _allForms.Add(frmTextInput);
                frmTimeInput = new FormTimeInput(this);
                _allForms.Add(frmTimeInput);
                frmRecipe = new FormRecipe(this, LicMgr, _recipeMgr);
                _allForms.Add(frmRecipe);
                frmRecipeAdd = new FormRecipeAdd(this, LicMgr, _recipeMgr);
                _allForms.Add(frmRecipeAdd);
                frmRecipeEditor = new FormRecipeEditor(this, _log, _recipeMgr);
                _allForms.Add(frmRecipeEditor);
                frmSegmentedRecipeEditor = new FormSegmentedRecipeEditor(this, _log, _recipeMgr);
                _allForms.Add(frmSegmentedRecipeEditor);
                frmRun = new FormRun(this, _log, _recipeMgr);
                _allForms.Add(frmRun);
                frmSetup = new FormSetup(this, _log);
                _allForms.Add(frmSetup);
                frmLogListing = new FormLogListing(this);
                _allForms.Add(frmLogListing);

                if (Settings.Default.UseLogDB)
                {
                    frmActivityLogViewer = new FormActivityLogViewer(this, _db, _log);
                    _allForms.Add(frmActivityLogViewer);
                }
                else
                {
                    frmLogViewer = new FormLogViewer(this);
                    _allForms.Add(frmLogViewer);
                }

                frmAlarms = new FormAlarms(this);
                _allForms.Add(frmAlarms);
                frmLicensing = new FormLicensing(this, LicMgr, _log) { StartDirectory = LicenseFilesDirectory };
                _allForms.Add(frmLicensing);
                frmLockOut = new FormLockOut(this);
                _allForms.Add(frmLockOut);

                if (MS.HasLoader)
                {
                    frmProcessStack = new FormProcessStack(this, _log);
                    _allForms.Add(frmProcessStack);
                    frmProcessStackMaint = new FormProcessStackMaint(this, _log);
                    _allForms.Add(frmProcessStackMaint);
                    frmLoaderOperation = new FormLoaderOperation(this, _log);
                    _allForms.Add(frmLoaderOperation);
                    frmPositionTeaching = new FormPositionTeaching(this);
                    _allForms.Add(frmPositionTeaching);
                    frmProcessStackSettings = new FormProcessStackSettings(this, _log);
                    _allForms.Add(frmProcessStackSettings);
                    frmJogTransfer = new FormJogTransfer(this, _log);
                    _allForms.Add(frmJogTransfer);
                    frmStackRecipes = new FormStackRecipes(this, _log);
                    _allForms.Add(frmStackRecipes);

                    if (MS.StackIncludesAdvancedVac)
                    {
                        frmVacTrendCalendar = new FormVacTrendCalendar(this, _log);
                        _allForms.Add(frmVacTrendCalendar);
                        frmVacTrend = new FormVacuumTrend(this, _log);
                        _allForms.Add(frmVacTrend);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ALARM, $"Caught Exception loading main screens:  {ex.Message}");
                MessageBox.Show("Error loading main screens!  Please send a logfile to nTact.");
                Close();
            }
        }

        private void SetupSerialServer()
        {
            if (!MS.AdvantechServerInstalled)
            {
                _log.log(LogType.TRACE, Category.INFO, $"Serial Server not configured, so will not be set up.");
                return;
            }

            try
            {
                var comms = CommsFactory.CreateServerComms(CommsTypes.Adam6017, _log);

                AdvServer = new AdvantechServer(comms, _log, _advantechServerInputs)
                {
                    IP = MS.AdvantechServerIPAddr,
                    Port = MS.AdvantechServerPort
                };

                AdvServer.Start();

                _log.log(LogType.TRACE, Category.INFO, $"Advantech Server has been conigured and started");
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Could not start up the Advantech Server: {ex}");
            }
        }

        private void VerifyLicensing()
        {
            if (UseLicenseMgr)
            {
                // This function will start the timer up
                timerLicense_Tick(null, EventArgs.Empty);
            }
            else
            {
                ShowLoginForm();
            }
        }

        private void SetupTimers()
        {
            _timerScreenUpdate = new System.Windows.Forms.Timer();
            _timerScreenUpdate.Interval = 250;
            _timerScreenUpdate.Tick += new EventHandler(timerScreenUpdate_Tick);
            _timerScreenUpdate.Enabled = false;

            _timerDataUpdate = new System.Windows.Forms.Timer();
            _timerDataUpdate.Interval = MS.DataUpdateRate;// 200;
            _timerDataUpdate.Tick += new EventHandler(timerDataUpdate_Tick);
            _timerScreenUpdate.Enabled = false;
            _timerDataUpdate.Enabled = false; // enabled below

            _timerConnection = new System.Windows.Forms.Timer();
            _timerConnection.Interval = 250;
            _timerConnection.Tick += new EventHandler(timerConnection_Tick);
            _timerConnection.Enabled = false; // enabled below

            _timerGetHeight = new System.Windows.Forms.Timer();
            _timerGetHeight.Interval = 250;
            _timerGetHeight.Tick += new EventHandler(timerGetHeight_Tick);
            _timerGetHeight.Enabled = false;

            _timerRecirc = new System.Windows.Forms.Timer();
            _timerRecirc.Interval = 60000;
            _timerRecirc.Tick += new EventHandler(timerRecirc_Tick);
            _timerRecirc.Enabled = false;

            _timerShowStackErrors = new System.Windows.Forms.Timer();
            _timerShowStackErrors.Interval = 1500;
            _timerShowStackErrors.Tick += _timerShowStackErrors_Tick;
            _timerShowStackErrors.Enabled = true;

            _timerLicense = new System.Windows.Forms.Timer();
            _timerLicense.Interval = 60000;
            _timerLicense.Tick += new EventHandler(timerLicense_Tick);
            _timerLicense.Enabled = false;

            _timerRestartMessaging = new System.Windows.Forms.Timer();
            _timerRestartMessaging.Interval = 250;
            _timerRestartMessaging.Tick += new EventHandler(timerRestartMessaging_Tick);
            _timerRestartMessaging.Enabled = false;

            if (MS.HasStack && MS.StackIncludesAdvancedVac)
            {
                frmVacTrend.StartTimer(250);
            }
        }

        private void SetupGalilWrapper()
        {
            try
            {
                MC = new GalilWrapper2(_log);
                MC.SetMachineSettings(MS, Storage);

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
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "Error While Initializing GalilWrapper: " + ex.ToString(), "ERROR");
            }
        }

        private void SetupDisplay()
        {
            Screen screenTarget = Screen.PrimaryScreen;
            Screen[] allScreens;
            allScreens = Screen.AllScreens;

            MaximizeBox = false;
            MinimizeBox = false;
            Text = String.Format("{0} - v{1}", Application.ProductName, Application.ProductVersion);

            // setup the screen.  we WANT a 1032 by 640.. but will live with something else....
            if (screenTarget.WorkingArea.Height != 600)
            {
                foreach (Screen curScreen in allScreens)
                {
                    if (curScreen.WorkingArea.Height == 600 && curScreen.WorkingArea.Width == 1024)
                    {
                        screenTarget = curScreen;
                    }
                }
            }

            if (screenTarget.WorkingArea.Height > 600)
            {
                Size = new Size(1040, 640);
            }
            else
            {
                ControlBox = false;
                FormBorderStyle = FormBorderStyle.None;
                SizeGripStyle = SizeGripStyle.Hide;
                StartPosition = FormStartPosition.Manual;
                Location = new Point(screenTarget.Bounds.Left, screenTarget.Bounds.Top);
                WindowState = FormWindowState.Maximized;
                TopLevel = true;
            }
        }

        private void SetupCognexCameras()
        {
            if (MS.CognexCommunicationsUsed)
            {
                _log.log(LogType.TRACE, Category.INFO, "Setting Up Cognex Cameras...", "INFO");
                try
                {
                    CognexVision = new clsCognex(_log, MS.LeftCameraIPAddr, MS.RightCameraIPAddr, MS.VisionUserName, MS.VisionPassword);
                    _log.log(LogType.TRACE, Category.INFO, "Cognex Setup Complete!", "INFO");
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.INFO, "Exception Setting Up Cognex Cameras: " + ex.Message, "ERROR");
                }
            }
        }

        #endregion

        #region Event Handlers

        private void FormMain_Load(object sender, EventArgs e)
        {
            try
            {
                SetupDisplay();
                LoadMachineSettings();
                VacUnits = MS.VacuumUnits;
                SetupGalilWrapper();
                MC.SetupIRLamp();
                MC.ConnectKeyenceLaser();
            }
            catch (Exception ex)
            {
                string msg = $"An Exception occurred while setting up required code. nAble will now be closed.  ({ex.Message})";
                _log.log(LogType.TRACE, Category.ALARM, msg);
                MessageBox.Show(this, msg, "Fatal Error - Exiting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Close();
            }

            LoadMainScreens();

            var recipeListFollowers = _allForms.Where(form => form is ITracksRecipeList).Select(form => (ITracksRecipeList)form).ToList();
            var recipeChangeFollowers = _allForms.Where(form => form is ITracksRecipeChanges).Select(form => (ITracksRecipeChanges)form).ToList();
            _recipeMgr.Initialize(recipeListFollowers, recipeChangeFollowers);

            if (MS.HasStack)
            {
                PLC.PLCAddress = MS.PLCAddress;
                PLC.Connect();
            }

            SetupCognexCameras();
            SetupTimers();
            SetupSerialServer();
            VerifyLicensing();

            try
            {
                if (MS.AnyHeaterEnabled)
                {
                    Omega485Controller = new Omega485Controller(_log, MS);
                    Omega485Controller.Setup();
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.ERROR, $"Could not set up Omega485 controller: {ex.Message}");
            }

            _log.log(LogType.TRACE, Category.INFO, $"Setting Dual-Pump Installed = {(MS.DualPumpInstalled ? "True" : "False")}");
            MC.RunCommand($"params[110]={(MS.DualPumpInstalled ? 2 : 0)}");
            MC.SetMemoryVal(19, MS.DualPumpInstalled ? 1.0 : 0.0);

            _timerScreenUpdate.Enabled = true;
            _timerDataUpdate.Enabled = true;

#if DEBUG
            MC.Connected = true;
            MS = MachineSettingsII.Load(Path.Combine(Directory.GetCurrentDirectory(), "MachineSettings.xml"));
#endif
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _formClosing = true;

            UpdateStatusMessage("Application Shutting down.  Please Wait...", Color.Blue, Color.Yellow);

            if (_timerConnection != null)
            {
                _timerConnection.Enabled = false;
            }

            if (_timerDataUpdate != null)
            {
                _timerDataUpdate.Enabled = false;
            }

            Application.DoEvents();

            if (_timerGetHeight is not null)
            {
                _timerGetHeight.Enabled = false;
            }

            KeyenceLaser?.Disconnect(closing: true);
            MC?.IRTransmitter?.Disconnect();
            Omega485Controller?.Shutdown();

            if (MC != null && MC.Connected)
            {
                MC.RetractGT2s();
                MC.Disconnect();	// close Galil
            }

            if (MS != null && MS.HasStack)
            {
                PLC?.Disconnect();
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseLogs();
        }

        private void CloseLogs()
        {
            _log?.log(LogType.TRACE, Category.INFO, "=======   Application Ended.  Main Form Closed.  =======", "INFO");
            Thread.Sleep(10);
            _log?.shutdown();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel thisLink = (LinkLabel)sender;
            LoadSubForm((Form)thisLink.Tag);
        }

        private void linkLabelExit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DialogResult.Yes == nRadMessageBox.Show(this, "Are you sure you wish to close the software?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                _closePending = true;
                _log.log(LogType.TRACE, Category.INFO, "User Closed Program", "Action");
                _formClosing = true;
                DisconnectKeyenceLasers();
                Close();
            }
        }

        private void DisconnectKeyenceLasers()
        {
            if (KeyenceLaser != null && MS.UsingKeyenceLaser && KeyenceLaser.Connected)
            {
                KeyenceLaser.Disconnect(closing: true);
                _log.log(LogType.TRACE, Category.INFO, "Disconnected From Keyence Laser.", "INFO");
            }
        }

        private void linkLabelMinimize_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void labelMainStatus_Click(object sender, EventArgs e)
        {
            if (MC.BuzzerOn)
            {
                MC.ToggleDigitalOut(1);
            }
            if (MC.Memory[0] != 0 || (MS.HasStack && PLC.StackAlarmsExist))
            {
                ShowAlarmsScreen();
            }
        }

        #endregion

        #region Timers

        void timerConnection_Tick(object sender, EventArgs e)
        {
            bool connected = false;
            _timerConnection.Stop();

            try
            {
                if (MC != null)
                {
                    if (!MC.Connected)
                    {
                        UseWaitCursor = true;
                        _log.log(LogType.TRACE, Category.INFO, "Galil Connection OFFLINE: - Attempting connection");

                        try
                        {
                            if (!DebugMode)
                            {
                                connected = MC.Connect(MS.IPAddress);
                            }

                            if (DebugMode || connected)
                            {
                                labelMainStatus.Width = panelMain.Width;// - labelnRadStats.Width;
                                                                        //labelnRadStats.Visible = true;
                                UpdateStatus();

                                Cursor = Cursors.AppStarting;
                                _log.log(LogType.TRACE, Category.INFO, "Waiting for First Data Record.", "Info");

                                if (!DebugMode && connected)
                                {
                                    // Wait for first data record  (we need corrected inputs)
                                    for (int i = 0; i < 40 && !MC.FirstRecordReceived; i++)
                                    {
                                        Thread.Sleep(250);
                                    }

                                    _log.log(LogType.TRACE, Category.INFO, "Waiting for Controller to flag 'started'", "Info");
                                    
                                    for (int i = 0; i < 40 && !MC.Started; i++)
                                    {
                                        Thread.Sleep(250);
                                    }
                                }

                                Cursor = Cursors.Default;

                                if (MC.ELOWasTripped)
                                {
                                    _log.log(LogType.TRACE, Category.INFO, string.Format("Galil connection to: {0}.  System Reports Emergency Stop Activated.", MC.Address), "ERROR");
                                    nRadMessageBox.Show(this, "Emergency Stop was activated.  Please dis-engage and reset the PLC", "Connection Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                                else if (!MC.MainAirOK && MC.MainAirValveOpen)
                                {
                                    _log.log(LogType.TRACE, Category.INFO, string.Format("Galil connection to: {0}.  System Reports No Air Pressure", MC.Address), "ERROR");
                                    nRadMessageBox.Show(this, "Main Air pressure is too low to allow operation.\r\nRestore air supply before continuing", "Connection Warning", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                }
                                else if (!MC.IsHomed && MC.MainAirOK)
                                {
                                    if (MC.SafetyGuardsActive)
                                    {
                                        nRadMessageBox.Show(this, "Initialization Required!\r\nSafety guards prevent the system from being initialized at this time.  Use Maintenance screen to initialize system.", "Connection Complete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (DialogResult.Yes == nRadMessageBox.Show(this, "Initialization Required!\r\nDo you wish to set up the nRad at this time?", "Connection Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                    {
                                        if (!MC.AirPressureFaultDuringInit)
                                        {
                                            CallInitAll();
                                        }
                                        else
                                        {
                                            nRadMessageBox.Show(this, "Cannot Initialize Motors While Main Air Pressure Is LOW!\nPlease Check Main Air Pressure.", "Main Air Too Low!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    _log.log(LogType.TRACE, Category.INFO, String.Format("Galil connection to: {0}.  System Reports OK and Homed.", MC.Address), "INFO");
                                }
                            }
                            else
                            {
                                nRadMessageBox.Show(this, "Failed to connect to nRad Controller\r\nPlease use Status screen to connect", "Connection Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.log(LogType.TRACE, Category.INFO, "ERROR during connect: " + ex.ToString(), "ERROR");
                        }

                        _connecting = false;
                    }
                    else
                    {
                        _connecting = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.log(LogType.TRACE, Category.INFO, "ERROR during connect: " + ex.ToString());
                _connecting = false;
            }

            UseWaitCursor = false;
        }

        private void _timerShowStackErrors_Tick(object sender, EventArgs e)
        {
            _showStackErrors = !_showStackErrors;
        }

        void timerScreenUpdate_Tick(object sender, EventArgs e)
        {
            _timerScreenUpdate.Stop();

            DateTime testTime = LastClick;

            if (!frmLogin.Visible && MS.TimeOut > 0)
            {
                if (DateTime.Now > testTime.AddMinutes(MS.TimeOut))
                {
                    ShowLoginForm();
                }
            }

            UpdateStatus();

            _timerScreenUpdate.Start();
        }

        void timerDataUpdate_Tick(object sender, EventArgs e)
        {
            _timerDataUpdate.Enabled = false;
            _timerDataUpdate.Interval = MC.NeedSlowPoll ? MS.DataUpdateRate * 2 : MS.DataUpdateRate;

            if (!MC.IsResetting)
            {
                MC.DataUpdate();
            }

            if (MS.ChuckTempControlEnabled)
            {
                MC.ChuckTemp = Omega485Controller.Temp(MS.ChuckCOMID);
            }

            if (MS.DieTempControlEnabled)
            {
                MC.DieTemp = Omega485Controller.Temp(MS.DieCOMID);
            }

            if (MS.ReservoirTempControlEnabled)
            {
                MC.ResvTemp = Omega485Controller.Temp(MS.ResvCOMID);
            }

            _timerDataUpdate.Enabled = true;
        }

        void timerGetHeight_Tick(object sender, EventArgs e)
        {
            _timerGetHeight.Stop();

            if (!MS.UsingKeyenceLaser)
            {
                if (MC.KeyenceOn && !_readingKeyence)
                {
                    _readingKeyence = true;
                    _bgwReadKeyence = new BackgroundWorker();
                    _bgwReadKeyence.DoWork += new DoWorkEventHandler(bgwReadKeyence_DoWork);
                    _bgwReadKeyence.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwReadKeyence_RunWorkerCompleted);
                    _bgwReadKeyence.RunWorkerAsync();
                }
                else
                {
                    KeyenceLeftVal = -99;
                    KeyenceRightVal = -99;
                }
            }
            else if (KeyenceLaser != null)
            {
                bool valid = KeyenceLaser.GetLeftAndRightLaserValues(out var left, out var right);
                KeyenceLeftVal = valid ? left : -99;
                KeyenceRightVal = valid ? right : -99;
            }

            _timerGetHeight.Start();
        }

        void timerRecirc_Tick(object sender, EventArgs e)
        {
            _timerRecirc.Stop();

            if (MC != null && MC.Connected && !MC.ErrorOccurred)
            {
                _log.log(LogType.TRACE, Category.INFO, "Running Recirculation Operation");
                MC.RunRecirc();
            }

            _timerRecirc.Start();
        }

        void timerLicense_Tick(object sender, EventArgs e)
        {
            if (frmLockOut.Visible || frmLicensing.Visible || _closePending || !UseLicenseMgr)
            {
                return;
            }

            _timerLicense.Enabled = false;

            var code = LicMgr.CheckLicenseValidity();

            if (code != LicenseFailCode.Valid)
            {
                ShowLockOutScreen(code);
            }

            _timerLicense.Enabled = true;
        }

        void timerRestartMessaging_Tick(object sender, EventArgs e)
        {
            if (OverrideDataRecordTimeout)
            {
                return;
            }

            _timerRestartMessaging.Enabled = false;

            TimeSpan tsElapsedTime = DateTime.Now - MC.LastRecordReceived;

            if (tsElapsedTime.TotalMilliseconds > 2000)
            {
                MC.StartRecords();
            }
            else
            {
                _timerRestartMessaging.Enabled = true;
            }
        }

        #endregion Timers

        #region Keyence

        internal void StartGetHeight()
        {
            if (!MS.UsingKeyenceLaser)
            {
                MC.OpenKeyence();
            }

            _timerGetHeight.Enabled = true;
        }

        internal void StopGetHeight()
        {
            _timerGetHeight.Enabled = false;

            if (!MS.UsingKeyenceLaser)
            {
                MC.CloseKeyence();
            }
        }

        void bgwReadKeyence_DoWork(object sender, DoWorkEventArgs e)
        {
            double left = 0, right = 0;
            MC.ReadKeyence(ref right, ref _eState0, ref left, ref _eState1, 0);
            KeyenceLeftVal = left;
            KeyenceRightVal = right;
        }

        void bgwReadKeyence_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _readingKeyence = false;
        }

        #endregion

        #region Settings, Values & Status

        private void LoadMachineSettings()
        {
            if (MS != null)
                MS = null;
            if (File.Exists("MachineSettings.xml"))
            {
                try
                {
                    MS = MachineSettingsII.Load("MachineSettings.xml");
                }
                catch (Exception ex)
                {
                    nRadMessageBox.Show(this, "Error Loading Machine settings file\r\n" + ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (ex.InnerException != null)
                    {
                        nRadMessageBox.Show(this, "Failure Reason:\r\n" + ex.InnerException.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MS = new MachineSettingsII();
                MS.Save();
            }

            if (Storage != null)
                Storage = null;
            if (File.Exists("MachineStorage.xml"))
            {
                try
                {
                    Storage = MachineStorage.Load("MachineStorage.xml");
                }
                catch (Exception ex)
                {
                    nRadMessageBox.Show(this, "Error Loading Machine storage file\r\n" + ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Storage = new MachineStorage();
                Storage.Save();
            }


        }

        public string UserAccessDesc
        {
            get
            {
                string sRetVal;
                switch (AccessLevel)
                {
                    default: sRetVal = "None"; break;
                    case 1: sRetVal = "Operator"; break;
                    case 2: sRetVal = "Operator & Editor"; break;
                    case 3: sRetVal = "Administrator"; break;
                }
                return sRetVal;
            }
        }

        #endregion Settings, Values & Status

        #region Update Status

        public void UpdateStatus()
        {
            if (_formClosing)
            {
                UpdateStatusMessage("Application Shutting down.  Please Wait...", Color.Blue, Color.Yellow);
                Application.DoEvents();
                return;
            }
            else if (SWIsLocked)
            {
                UpdateStatusMessage("nAble SW Is Locked!", Color.Red, Color.Black);
            }
            else if (!_loggedIn)
            {
                UpdateStatusMessage("Login Required!", Color.Fuchsia, Color.Black);
            }
            else if (_showStackErrors && MS.HasStack && PLC.StackAlarmsExist)
            {
                UpdateStatusMessage($"{PLC.StackAlarmCount} Process Stack Alarms Exist", Color.Red, Color.White);
            }
            else if (MC.IsResetting)
            {
                UpdateStatusMessage("Motion Controller Resetting, Please Wait...", Color.Blue, Color.Yellow);
            }
            else if (MC.Connected && !MC.IsHoming && MC.AirPressureFaultDuringInit)
            {
                UpdateStatusMessage("Main Air Pressure Too Low!!", Color.Red, Color.White);
            }
#if !DEBUG
            else if (!MC.Connected && !(_connecting || MC.IsConnecting))
            {
                UpdateStatusMessage("Motion Controller Not Connected!!!", Color.Red, Color.White);
            }
#endif
            else if (MC.Connected && MC.ELOPressed)
            {
                UpdateStatusMessage("Coater EMO Engaged! Release to continue!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ELOWasTripped)
            {
                UpdateStatusMessage("EMO Activated! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.SafetyGuardsActive)
            {
                UpdateStatusMessage("Safety Guards Active! Clear to Continue!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.AMPErrorOccurred)
            {
                UpdateStatusMessage("AMP Error Occured! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred && MC.XAxisPosError)
            {
                UpdateStatusMessage("X-Axis Position Error! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred && MC.RZAxisPosError)
            {
                UpdateStatusMessage("Right Z-Axis Position Error! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred && MC.LZAxisPosError)
            {
                UpdateStatusMessage("Left Z-Axis Position Error! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred && MC.PumpAxisPosError)
            {
                UpdateStatusMessage("Pump-A Position Error! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred && MC.PumpBAxisPosError)
            {
                UpdateStatusMessage("Pump-B Position Error! Reset Required!!!", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.LowAirOccurred)
            {
                UpdateStatusMessage("Main Air Too Low... Restore Air to continue!!!.", Color.Red, Color.White);
            }
            else if (MC.Connected && MC.ErrorOccurred)
            {
                if (MC.CommutationFailure)
                {
                    switch (MC.CommutatingAxis)
                    {
                        case 1:
                        {
                            UpdateStatusMessage("X Axis Commutation Failure! Reset Required!!!", Color.Red, Color.White);
                        }
                        break;
                        case 2:
                        {
                            UpdateStatusMessage("Right Z Commutation Failure! Reset Required!!!", Color.Red, Color.White);
                        }
                        break;
                        case 3:
                        {
                            UpdateStatusMessage("Left Z Commutation Failure! Reset Required!!!", Color.Red, Color.White);
                        }
                        break;
                        case 4:
                        {
                            UpdateStatusMessage("Pump-A Commutation Failure! Reset Required!!!", Color.Red, Color.White);
                        }
                        break;
                        case 5:
                        {
                            UpdateStatusMessage("Pump-B Commutation Failure! Reset Required!!!", Color.Red, Color.White);
                        }
                        break;
                        default:
                            break;
                    }
                }
                else
                {
                    UpdateStatusMessage("System Error! Reset Required!!!", Color.Red, Color.White);
                }
            }
            else if (MC.Connected && !MC.IsSetup && !_connecting)
            {
                UpdateStatusMessage("Motion Controller Did not Return Startup Data!!!", Color.Red, Color.White);
            }
            else if ((_connecting || MC.IsConnecting) && !MC.Connected)
            {
                UpdateStatusMessage("Connecting... Please wait.", Color.Yellow, Color.Black);
            }
            else if (MC.Connected && !MC.IsHomed)
            {
                if (MC.IsHoming && !MC.AirPressureFaultDuringInit)
                {
                    if (MC.XHomingStatus == 1)
                    {
                        UpdateStatusMessage("Homing X Axis... Please Wait", Color.Yellow, Color.Black);
                    }
                    else if(MC.ZRHomingStatus == 1 && MC.ZLHomingStatus ==1)
                    {
                        UpdateStatusMessage("Homing Both Z Axis... Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.ZRHomingStatus == 1)
                    {
                        UpdateStatusMessage("Homing Z Right Axis... Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.ZLHomingStatus == 1)
                    {
                        UpdateStatusMessage("Homing Z Left Axis... Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.ZLHomingStatus == 5)
                    {
                        UpdateStatusMessage("Lifting Z Axis to safety... Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.PumpHoming)
                    {
                        if (MS.DualPumpInstalled)
                        {
                            if (MC.SyringePumpDetected)
                                UpdateStatusMessage("Homing Syringe-A...   Please Wait", Color.Yellow, Color.Black);
                            else
                                UpdateStatusMessage("Homing POH-A...   Please Wait", Color.Yellow, Color.Black);
                        }
                        else
                        {
                            if (MC.SyringePumpDetected)
                                UpdateStatusMessage("Homing Syringe Pump...   Please Wait", Color.Yellow, Color.Black);
                            else
                                UpdateStatusMessage("Homing POH Pump...   Please Wait", Color.Yellow, Color.Black);
                        }
                    }
                    else if (MC.ValveHoming)
                    {
                        if (MS.DualPumpInstalled)
                            UpdateStatusMessage("Homing Valve-A...  Please Wait", Color.Yellow, Color.Black);
                        else
                            UpdateStatusMessage("Homing Valve...  Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MS.DualPumpInstalled && MC.PumpBHoming)
                    {
                        if(MC.SyringePumpBDetected)
                            UpdateStatusMessage("Homing Syringe-B...   Please Wait", Color.Yellow, Color.Black);
                        else
                            UpdateStatusMessage("Homing POH-B...   Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MS.DualPumpInstalled && MC.ValveBHoming)
                    {
                        UpdateStatusMessage("Homing Valve-B...  Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.LoaderHomingStatus == 1)
                    {
                        UpdateStatusMessage("Homing Loader...  Please Wait", Color.Yellow, Color.Black);
                    }
                    else if (MC.HomingStatus == 4)
                    {
                        int nAxis = (int)MC.Memory[12];
                        switch (nAxis)
                        {
                            case 1: UpdateStatusMessage("Commutating X-Axis...    Please Wait", Color.Yellow, Color.Black); break;
                            case 2: UpdateStatusMessage("Commutating Right Z-Axis...  Please Wait", Color.Yellow, Color.Black); break;
                            case 3: UpdateStatusMessage("Commutating Left X-Axis...   Please Wait", Color.Yellow, Color.Black); break;
                            case 4: UpdateStatusMessage("Commutating Pump-A... Please Wait", Color.Yellow, Color.Black); break;
                            case 5: if (MS.DualPumpInstalled) UpdateStatusMessage("Commutating Pump-B... Please Wait", Color.Yellow, Color.Black); break;
                            case 0:
                            default:
                            {
                                if (MC.XHomingStatus == 4)
                                {
                                    UpdateStatusMessage("Initializing X Axis... Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MC.ZRHomingStatus == 4)
                                {
                                    UpdateStatusMessage("Initializing Z Right Axis... Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MC.ZLHomingStatus == 4)
                                {
                                    UpdateStatusMessage("Initializing Z Left Axis... Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MC.PumpCommutating)
                                {
                                    UpdateStatusMessage("Initializing Pump-A...   Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MC.ValveCommutating)
                                {
                                    UpdateStatusMessage("Initializing Valve-A...  Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MS.DualPumpInstalled && MC.PumpBCommutating)
                                {
                                    UpdateStatusMessage("Initializing Pump-B...   Please Wait", Color.Yellow, Color.Black);
                                }
                                else if (MS.DualPumpInstalled && MC.ValveBCommutating)
                                {
                                    UpdateStatusMessage("Initializing Valve-B...  Please Wait", Color.Yellow, Color.Black);
                                }
                                else
                                {
                                    UpdateStatusMessage("Initializing motors...   Please Wait", Color.Yellow, Color.Black);
                                }
                            }
                            break;
                        }
                    }
                    else
                        UpdateStatusMessage("Initializing...  Please Wait", Color.Yellow, Color.Black);
                }
                else
                {
                    if (MC.AirPressureFaultDuringInit)
                    {
                        UpdateStatusMessage("Air Pressure Fault During Initialization!", Color.Red, Color.White);
                        if (DialogResult.Yes == nRadMessageBox.Show(this, "Would You Like To Reset The Motion Controller?", "Reset Motion Controller?", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            _log.log(LogType.TRACE, Category.INFO, "Resetting Motion Controller Due To Main Air Pressure Fault.");
                            ResetMC();
                        }
                    }
                    else
                    {
                        UpdateStatusMessage("Initialization Required.", Color.Fuchsia, Color.Black);
                    }
                }
            }
#if !DEBUG
            else if (!MC.MainAirValveOpen)
            {
                UpdateStatusMessage("Main Air Disabled. Enable to continue.", Color.Yellow, Color.Black);
            }
            else if (!MC.AllMotorsOn)
            {
                UpdateStatusMessage("Warning! One or More Motors are currently OFF!", Color.Orange, Color.Black);
            }
            else if (MS.UsingKeyenceLaser && KeyenceLaser != null && !KeyenceLaser.Connected)
            {
                UpdateStatusMessage("Keyence Lasers Not Connected! Motion Limited!", Color.Orange, Color.Black);
            }
            else if ((MS.LiftPinsEnabled && !MC.LiftPinsAreDown) || (MS.AlignersEnabled && !MC.AlignersAreDown))
            {
                UpdateStatusMessage("Aligners or Lift Pins are currently up. Motion Limited!", Color.Orange, Color.Black);
            }
#endif
            else
            {
                if (MC.SafetyGuardsActive)
                    UpdateStatusMessage("Safety Guards Activated!  Motion Limited.", Color.Red, Color.Yellow);
                else
                {
                    if (frmRun.checkBoxXRunLoop.Checked)
                        UpdateStatusMessage("Looper Mode Active", Color.Blue, Color.Yellow);
                    else if (MC.RunningRecipe)
                        UpdateStatusMessage("Running Recipe...", Color.Yellow, Color.Black);
                    else if (MC.XJogging)
                        UpdateStatusMessage("X Jog In Process", Color.Yellow, Color.Black);
                    else if (MC.ZJogging)
                        UpdateStatusMessage("Z Jog In Process", Color.Yellow, Color.Black);
                    else if (MC.PrimeRunning)
                        UpdateStatusMessage("Pump Prime/Purge in Process", Color.Yellow, Color.Black);
                    else if (MC.RunningGoto)
                        UpdateStatusMessage("Running 'Goto' Operation.", Color.Yellow, Color.Black);
                    else if (MC.IsFindingPumpLimits)
                        UpdateStatusMessage("Finding Pump Limits.", Color.Yellow, Color.Black);
                    else if (MS.HasLoader && MC.LoaderMoving)
                        UpdateStatusMessage("Loader Move in Process", Color.Yellow, Color.Black);
                    else if (MS.HasReservoirSensors && MC.ReservoirEmpty)
                        UpdateStatusMessage("Reservoir Is Empty", Color.Magenta, Color.White);
                    else if (MC.RobotLoading)
                        UpdateStatusMessage("Robot Load In Process", Color.Blue, Color.White);
                    else if (MC.RobotLoadFailure)
                        UpdateStatusMessage("Robot Load Failure", Color.Orange, Color.Black);
                    else if (MC.RobotUnloading)
                        UpdateStatusMessage("Robot Unload In Process", Color.Blue, Color.White);
                    else if (MC.RobotUnloadFailure)
                        UpdateStatusMessage("Robot Unload Failure", Color.Orange, Color.Black);
                    else
                        UpdateStatusMessage("System Ready.", Color.Lime, Color.Black);
                }
            }

            if (MC.Connected && MC.RestartMessaging)
            {
                _timerRestartMessaging.Enabled = true;
                MC.RestartMessaging = false;
            }

            foreach (IUpdateableForm curForm in _allForms)
            {
                curForm?.UpdateStatus();
            }

            if (MS.UsingKeyenceLaser)
            {
                KeyenceLaser?.UpdateStatus();
            }

            if (MS.IRLampInstalled)
            {
                MC.IRTransmitter?.UpdateStatus();
            }

            RecipeSelected = frmRun.RecipeSelected;
            linkLabelMinimize.Visible = frmMaintenance.Visible || frmConfig.Visible || frmSetup.Visible;
            //if (MC.Connected)
            //    labelnRadStats.Text = string.Format("{6}ZL: {1,7:0.000} ZR: {2,7:0.000}\r\nVol: {3,7:0.000}  X: {0,7:0.000}\r\nVlv: {4,-8}    {5,3} psi", MC.XPos, MC.ZLPos, MC.ZRPos, MC.CurrentmLVolumeUsed, MC.ValvePositionName, MC.MainAirPressure.ToString("0"), MC.NeedSlowPoll ? "*" : " ");
            //else
            //    labelnRadStats.Text = string.Format(" ZL: ------- ZR: -------\r\nVol: -------  X: -------\r\nVlv: --------    --- psi", MC.XPos, MC.ZLPos, MC.ZRPos, MC.CurrentmLVolumeUsed, MC.ValvePositionName, MC.MainAirPressure.ToString("0"), MC.NeedSlowPoll ? "*" : " ");
        }

        #endregion

        #region Sub Form Handling & Loads/Unloads

        internal void SetConfirmLeave(string sMsg, string sTitle)
        {
            _confirmLeaveMsg = sMsg;
            _confirmLeaveTitle = sTitle;
            _confirmLeave = true;
        }

        internal void CancelConfirmLeave()
        {
            _confirmLeave = false;
        }

        public void LoadProcessStackForm()
        {
            if (MS.HasLoader)
            {
                LoadSubForm(frmProcessStack);
            }
        }

        internal bool IsVisible(Form form)
        {
            return this.panelMain.Controls.Contains(form);
        }
        internal void LoadSubForm(Form newForm, bool confirmExit = true)
        {
            if (newForm is null)
            {
                return;
            }

            LastClick = DateTime.Now;

            if (_confirmLeave && confirmExit)
            {
                if (DialogResult.No == nRadMessageBox.Show(this, _confirmLeaveMsg, _confirmLeaveTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    return;
                }
            }

            _confirmLeave = false;

            var matchingIndex = _navForms.IndexOf(newForm);

            // if we found it, remove all after it
            if (matchingIndex != -1)
            {
                _navForms = _navForms.Take(matchingIndex + 1).ToList();
            }

            ShowScreen(newForm);

            // If we did not find it before, add it now
            if (matchingIndex == -1)
            {
                _navForms.Add(newForm);
            }

            SetMenuBar(true);
        }

        internal void ShowLoginForm()
        {
            if (SWIsLocked)
            {
                return;
            }

            ShowScreen(frmLogin);
            _loggedIn = false;
        }

        internal void ShowLockOutScreen(LicenseFailCode code)
        {
            if (!UseLicenseMgr)
            {
                return;
            }

            SWIsLocked = true;
            ShowScreen(frmLockOut);
        }

        internal void ShowLicenseScreen()
        {
            if (!UseLicenseMgr)
            {
                return;
            }

            ShowScreen(frmLicensing);
        }

        private void ShowAlarmsScreen()
        {
            ShowScreen(frmAlarms);
        }

        public void GotoNumScreen(string sTitle, Form tabSourceForm, Control controlReturnFocus, string sNumFormat, double dMin, double dMax, string sAltText = "", bool bEnablePeriod = true, bool bIsPassword = false)
        {
            LastClick = DateTime.Now;
            CancelConfirmLeave();
            SetMenuBar(false);
            frmNumInput.SetupPage(sTitle, tabSourceForm, controlReturnFocus, sNumFormat, dMin, dMax, sAltText, bEnablePeriod, bIsPassword);
            ShowScreen(frmNumInput);
        }

        public void GotoTextScreen(string sTitle, Form tabSourceForm, Control controlReturnFocus, int maxCharacters)
        {
            LastClick = DateTime.Now;
            CancelConfirmLeave();
            SetMenuBar(false);
            frmTextInput.SetupPage(sTitle, tabSourceForm, controlReturnFocus, maxCharacters);
            ShowScreen(frmTextInput);
        }

        public void GotoTimeScreen(string sTitle, Form tabSourceForm, Control controlReturnFocus)
        {
            LastClick = DateTime.Now;
            CancelConfirmLeave();
            SetMenuBar(false);
            frmTimeInput.SetupPage(sTitle, tabSourceForm, controlReturnFocus);
            ShowScreen(frmTimeInput);
        }

        private void ShowScreen(Form form)
        {
            // remove the old ones
            _navControls.Clear();

            form.TopLevel = false;

            panelMain.Controls.Clear();
            panelMain.Controls.Add(form);

            form.Dock = DockStyle.Fill;
            form.Show();
            form.Focus();
            form.Refresh();
        }

        private void UpdateStatusMessage(string message, Color colorBackground, Color colorForeground)
        {
            bool showTrace = false;

            // determine if we want to log it...
            if (message.Contains("Process Stack Alarms"))
            {
                if (_lastStackErrorMessage != message)
                {
                    _lastStackErrorMessage = message;
                    showTrace = true;
                }
            }
            else if (_lastMessage != message)
            {
                _lastMessage = message;
                showTrace = true;
            }

            labelMainStatus.Text = message;
            labelMainStatus.BackColor = colorBackground;
            labelMainStatus.ForeColor = colorForeground;

            if (showTrace)
            {
                _log?.log(LogType.TRACE, Category.INFO, $"New Status Message ({labelMainStatus.ForeColor.Name} on {labelMainStatus.BackColor.Name}): {message}");
            }
        }

        internal void ShowPrevForm()
        {
            // First, remove the current form
            var newForm = _navForms.Count > 0 ? _navForms.Last() : null;

            if (newForm is null)
            {
                return;
            }

            _navForms.Remove(newForm);

            // Now, go to the last remaining, or MainMenu if nothing left.
            newForm = _navForms.Count > 0 ? _navForms.Last() : frmMainMenu;

            if (newForm is null)
            {
                return;
            }

            LoadSubForm(newForm);
        }

        internal void ShowMainMenu()
        {
            _navForms.Clear();
            LoadSubForm(frmMainMenu);
            frmLogin.Visible = false;
        }

        internal void ShowLastForm(bool confirmExit = true)
        {
            var newForm = _navForms.Count > 0 ? _navForms.Last() : frmMainMenu;
            LoadSubForm(newForm, confirmExit);
            frmLogin.Visible = false;
        }

        internal void SetMenuBar(bool enableLinks)
        {
            Point nextPoint = new Point(6, 6);
            bool first = true;
            var last = _navForms.Count == 0 ? null : _navForms.Last();

            _navControls.Clear();
            panelMenu.Controls.Clear();

            // now display the labels/links for the forms... 
            foreach (var form in _navForms)
            {
                var labelText = first ? form.Text : $"- {form.Text}";
                first = false;

                if (form != last && enableLinks) // LinkLabel
                {
                    var newLinkLabel = new LinkLabel
                    {
                        Height = 36,
                        AutoSize = true,
                        Font = linkLabelExit.Font,
                        Text = labelText,
                        Tag = form,
                        Location = nextPoint,
                        BackColor = Color.White
                    };

                    newLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel_LinkClicked);

                    _navControls.Add(newLinkLabel);
                    panelMenu.Controls.Add(newLinkLabel);
                    nextPoint.X += newLinkLabel.Width;
                }
                else // normal label
                {
                    var newLabel = new Label
                    {
                        Height = 36,
                        AutoSize = true,
                        Font = linkLabelExit.Font,
                        Text = labelText,
                        Location = nextPoint,
                        BackColor = Color.White
                    };

                    _navControls.Add(newLabel);
                    panelMenu.Controls.Add(newLabel);
                    nextPoint.X += newLabel.Width;
                }
            }
        }

        #endregion Sub Form Handling  Load/Unloads

        #region Motion Controller Functions

        public void StartConnecting()
        {
            _loggedIn = true;

            if (MC == null || !MC.Connected)
            {
                _log.log(LogType.TRACE, Category.INFO, "StartConnecting() - Starting Connection Timer");
                _timerConnection.Enabled = true;
                _connecting = true;
            }
        }

        internal void ResetMC(bool bSoft = true)
        {
            UpdateStatusMessage("Resetting PLC.  Please Wait...", Color.Yellow, Color.Black);
            if (bSoft)
            {
                MC.SoftReset();
            }
            else
            {
                Application.DoEvents();
                Thread.Sleep(100);
                MC.Reset();
                StartConnecting();

                MC.SetMemoryVal(15, MS.UsingKeyenceLaser ? 1.0 : 0.0);
                MC.SetMemoryVal(19, MS.DualPumpInstalled ? 1.0 : 0.0);
                MC.RunCommand($"params[110]={(MS.DualPumpInstalled ? 2 : 0)}");
            }
        }

        internal void CallInitAll()
        {
            MC.CallInitAll();
        }

        #endregion Motion Controller Functions

        #region Misc FormMain Stuff

        public void ShowTrendingChart(string sChartName = "")
        {
            if (frmVacTrend != null)
            {
                frmVacTrend.AddChart(sChartName);
                LoadSubForm(frmVacTrend);
            }
        }

        internal void Shutdown()
        {
            ManagementBaseObject mboShutdown = null;
            ManagementClass mcWin32 = new ManagementClass("Win32_OperatingSystem");
            mcWin32.Get();

            UpdateStatusMessage("System Shutting Down.  Please wait...", Color.Blue, Color.Yellow);

            // You can't shutdown without security privileges
            mcWin32.Scope.Options.EnablePrivileges = true;
            ManagementBaseObject mboShutdownParams = mcWin32.GetMethodParameters("Win32Shutdown");

            // Flag 1 to shutdown.
            // Flag 2 to reboot.
            // Flag 5 to force shutdown  (ignore pending apps)
            mboShutdownParams["Flags"] = "1";
            mboShutdownParams["Reserved"] = "0";
            foreach (ManagementObject manObj in mcWin32.GetInstances())
            {
                mboShutdown = manObj.InvokeMethod("Win32Shutdown", mboShutdownParams, null);
            }
        }

        internal void StartRecirc()
        {
            Storage.Save();
            frmConfig.buttonRecircTime.Text = Storage.RecirculationInterval.ToString();
            frmConfig.buttonRecircCount.Text = Storage.RecirculationCount.ToString();
            frmFluidFlow.buttonRecircTime.Text = Storage.RecirculationInterval.ToString();
            frmFluidFlow.buttonRecircCount.Text = Storage.RecirculationCount.ToString();

            _timerRecirc.Interval = Storage.RecirculationInterval * 60 * 1000;
            _timerRecirc.Enabled = true;
            Recirculating = true;
        }

        internal void StopRecirc()
        {
            _timerRecirc.Enabled = false;
            Recirculating = false;
        }

        #endregion Misc FormMain Stuff

        #endregion
    }
}
