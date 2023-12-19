using nAble.DataComm.KeyenceLasers;
using nAble_for_nRad2.Properties;
using nTact.AppTools;
using nTact.Recipes;
using Support2;
using Support2.Forms;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormMainMenu : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private readonly NRadLicensing2 _licMgr = null;
        private readonly IRecipeManager _recipeMgr = null;
        private UCTrialStatus _ucTrialStatus = null;
        private bool _visibleOneShot = false;

        private Recipe _recipeTemplate = null;

        private ILaserManager LaserMgr => _frmMain.KeyenceLaser;

        public FormMainMenu(FormMain formMain, NRadLicensing2 licMgr, LogEntry log, IRecipeManager recipeManager)
        {
            _frmMain = formMain;
            _log = log;
            _licMgr = licMgr;


            InitializeComponent();

            buttonClearLDULDFailure.BringToFront();
            buttonManualMode.SendToBack();
            buttonAutoMode.SendToBack();

            SetupLicenseStatus();

            _recipeMgr = recipeManager;
        }

        private void SetupLicenseStatus()
        {
            _ucTrialStatus = new UCTrialStatus
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
                FirstWarningDays = 30,
                LicMgr = _licMgr,
                Location = new Point(448, 12),
                Name = "ucTrialStatus",
                SeriousWarningDays = 7,
                Size = new Size(203, 114),
                TabIndex = 23
            };

            Controls.Add(_ucTrialStatus);
        }

        private void buttonMaint_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmMaintenance);
        }

        private void buttonProcessStack_Click(object sender, EventArgs e)
        {
            if (_frmMain.PLC.IsConnected || _frmMain.MS.HasLoader)
            {
                _frmMain.LoadProcessStackForm(); //.LoadSubForm(_frmMain.frmMaintenance);
            }
            else
            {
                nRadMessageBox.Show(this, "Stack PLC not connected! Please try again later.", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonRecipe_Click(object sender, EventArgs e)
        {
            string rtVersion = _recipeMgr.DefaultRecipe.TemplateVersion;
            rtVersion = rtVersion == null ? "MISSING" : rtVersion;
            if (rtVersion!= Settings.Default.TemplateVersion)
            {
                nRadMessageBox.Show(this, $"Default recipe template version is {rtVersion} and should be {Settings.Default.TemplateVersion}. Default template must be updated first.", "Recipe Template Verification", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (_frmMain.UserCanEditRecipe)
            {
                _frmMain.LoadSubForm(_frmMain.frmRecipe);
            }
            else
            {
                nRadMessageBox.Show(this, "Current User does not have Edit Recipe Permissions", "Authentication", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonJogging_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmJog);
        }

        private void buttonChuck_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmChuck);
        }

        private void buttonFluid_Click(object sender, EventArgs e)
        {
            if (_frmMain.MS.DieTempControlEnabled || _frmMain.MS.ReservoirTempControlEnabled)
            {
                _frmMain.LoadSubForm(_frmMain.frmFluidMain);
            }
            else
            {
                _frmMain.LoadSubForm(_frmMain.frmFluidFlow);
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            _frmMain.LoadSubForm(_frmMain.frmRun);
        }

        private void FormMainMenu_Load(object sender, EventArgs e)
        {
            UpdateSystemInfo();
            buttonProcessStack.Visible = _frmMain.MS.HasLoader;
        }

        public void UpdateStatus()
        {
            if (!_frmMain.IsVisible(this))
            {
                _visibleOneShot = true;
                return;
            }
            else if(_visibleOneShot)
            {
                _visibleOneShot = false;
                buttonAutoMode.Visible = _frmMain.MS.AutoLDULD;
                buttonManualMode.Visible = _frmMain.MS.AutoLDULD;
            }

            if(_frmMain.MS.AutoLDULD)
            {
                buttonAutoMode.BackColor = _frmMain.MC.AutoMode ? Color.Lime : Color.White;
                buttonManualMode.BackColor = !_frmMain.MC.AutoMode ? Color.Lime : Color.White;
                buttonClearLDULDFailure.Visible = (_frmMain.MC.RobotLoadFailure || _frmMain.MC.RobotUnloadFailure) && !_frmMain.MC.RobotLoading && !_frmMain.MC.RobotUnloading;
            }

            _ucTrialStatus.Visible = _frmMain.UseLicenseMgr;
            _ucTrialStatus.UpdateStatus();

#if DEBUG || DESKTOP
            buttonJogging.Enabled = true;
            buttonChuck.Enabled = true;
            buttonFluid.Enabled = true;
            buttonRun.Enabled = true;
#else
            buttonJogging.Enabled = _frmMain.MC.IsDemo || _frmMain.MC.IsHomed;
            buttonChuck.Enabled = _frmMain.MC.IsDemo || _frmMain.MC.Connected || _frmMain.MS.ChuckTempControlEnabled;
            buttonFluid.Enabled = _frmMain.MC.IsDemo || _frmMain.MC.IsHomed || _frmMain.MS.ReservoirTempControlEnabled || _frmMain.MS.DieTempControlEnabled;
            buttonRun.Enabled = _frmMain.MC.IsDemo || _frmMain.MC.IsHomed;
#endif

#if DEBUG
            UpdateSystemInfo();
#else
            if (_frmMain.MC.Connected)
            {
                //_bConnected = _frmMain.MC.Connected;
                UpdateSystemInfo();
            }
#endif
        }

        // Create an HTML page to display system info
        private void UpdateSystemInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Color colorControl = Color.FromName("Control");
            StringBuilder sb = new StringBuilder(1000);
            sb.AppendLine("<HTML>");
            sb.AppendLine("<head><STYLE TYPE=\"text/css\"><!--TD{font-family: tahoma; font-size: 10pt;}---></STYLE></head>");
            sb.AppendLine("<body bgcolor=#F2F2F2><font face=\"tahoma\">");
            sb.AppendLine($"{ApplicationDetails.ProductTitle} Ver. <b>{ApplicationDetails.ProductVersion}</b><br>");

            FileVersionInfo fv = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            sb.AppendLine($"Assembly File Ver. {fv.FileVersion}<br>");
            sb.AppendLine($"{ApplicationDetails.CopyRightsDetail}<br>");
            sb.AppendLine($"HMI Machine Name: <b>{Environment.MachineName}</b><br>");
            sb.AppendLine($"Serial Number: <b>{_frmMain.LicMgr.SerialNumber}</b><br>");

            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());
            string sAddrs = "";

            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    sAddrs += sAddrs.Length > 0 ? $", {addr}" : $"{addr}";
                }
            }

            sb.AppendLine($"HMI IP Address(es): <b>{sAddrs}</b><br>");
            sb.AppendLine($"User Access Level: <b>{_frmMain.UserAccessDesc}</b><br>");

            string rtVersion = _recipeMgr.DefaultRecipe.TemplateVersion;
            sb.AppendLine($"Recipe Version: <b>{rtVersion ?? "MISSING"}</b><br>");
            sb.AppendLine("<hr>");

            string sTemp = _frmMain.MC.LibraryVersion;

            // GalilClass1.dll
            if (sTemp != "")
            {
                string libVer = sTemp.Split(new char[] { '\n', '\r' })[0].Substring(16); // GalilClass1.dll 1.5.0.0
                sb.AppendLine($"Controller Lib Ver. : {libVer}<br>");
            }

            sb.AppendLine($"nRad2 IP Address: {_frmMain.MS.IPAddress}<br>");

            if (_frmMain.MC.Connected)
            {
                sb.AppendLine("nRad2 is <b>CONNECTED</b>.<br>");

                if (_frmMain.MS.UsingKeyenceLaser)
                {
                    sb.AppendLine($"Keyence Lasers are <b>{((LaserMgr?.Connected ?? false) ? "CONNECTED" : "NOT CONNECTED")}</b>.<br><br>");
                }
                
                string info = _frmMain.MC.ConnectionInfo;
                string[] temps = info.Split(new char[] { ',', '\n', '\r' });
                sb.AppendLine("<font size='6'><table cellspacing='1'>");
                sb.AppendLine($"<tr><td>Controller ID.</td><td>: {temps[0]}</td></tr>");
                sb.AppendLine($"<tr><td>Firmware</td><td>: {temps[1]}</td></tr>");
                sb.AppendLine($"<tr><td>Serial No.</td><td>: {temps[2]}</td></tr>");
                sb.AppendLine($"<tr><td>Code Ver.</td><td>: {_frmMain.MC.CodeVersion}</td></tr>");
                sb.AppendLine($"<tr><td>Coating Ver.</td><td>: {_frmMain.MC.CoatVersion}</td></tr>");
                sb.AppendLine("</tr></table></font>");
            }
            else
            {
                sb.AppendLine("nRad is <b><font color='red'>NOT CONNECTED</font></b>.<br><br>");
            }

            sb.AppendLine("</body></HTML>");
            webBrowserInfo.DocumentText = sb.ToString();
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            UpdateSystemInfo();
        }

        private void FormMainMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                UpdateSystemInfo();
            }
        }

        private void buttonTester_Click(object sender, EventArgs e)
        {

        }

        private void buttonAutoMode_Click(object sender, EventArgs e)
        {
            if (_frmMain.frmRun.RecipeSelected)
            {
                _log.log(LogType.TRACE, Category.INFO, "Auto Mode Requested", "INFO");
                if (_frmMain.MC.SubstratePresent)
                {
                    _frmMain.MC.ResetReadyToLoad();
                    _frmMain.MC.SetReadyToUnload();
                }
                else if(!_frmMain.MC.LiftIsUp)
                {
                    nRadMessageBox.Show(this, $"Auto Mode Not Allowed. Lift Pins Must Be Up. Please Press Prepare To Load First!", "Auto Mode Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    _frmMain.MC.SetReadyToLoad();
                    _frmMain.MC.ResetReadyToUnload();
                }

                _frmMain.MC.AutoMode = true;
            }
            else
                nRadMessageBox.Show(this, $"Auto Mode Not Allowed. Recipe Must Be Selected!", "Auto Mode Request", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void buttonManualMode_Click(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "Manual Mode Requested", "INFO");
            _frmMain.MC.ResetReadyToLoad();
            _frmMain.MC.ResetReadyToUnload();
            _frmMain.MC.AutoMode = false;
        }

        private void buttonClearLDULDFailure_Click(object sender, EventArgs e)
        {
            _frmMain.MC.ClearLDULDFailure();
        }
    }
}
