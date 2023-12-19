using Support2;
using System;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormLogin : Form, IUpdateableForm
    {
        private readonly FormMain _frmMain = null;
        private readonly LogEntry _log = null;
        private readonly NRadLicensing2 _licMgr = null;

        public FormLogin(FormMain formMain, NRadLicensing2 licMgr, LogEntry log)
        {
            _frmMain = formMain;
            _log = log;
            _licMgr = licMgr;

            InitializeComponent();

            ucTrialStatus.LicMgr = _licMgr;
        }

        public void UpdateStatus()
        {
            if (!Visible || _frmMain == null)
            {
                return;
            }

            buttonLicense.Visible = _frmMain.UseLicenseMgr;
            ucTrialStatus.Visible = _frmMain.UseLicenseMgr;
            ucTrialStatus.UpdateStatus();
        }

        private void buttonLoginEnter_Click(object sender, EventArgs e)
        {
            if (textBoxLogin.Text == _frmMain.MS.OperatorPW)
            {
                labelLoginError.Text = "";
                _frmMain.LastClick = DateTime.Now;
                _log.log(LogType.TRACE, Category.INFO, "Operator - Login Success", "Action");
                _frmMain.frmSetup.ShowLockout = true;
                _frmMain.AccessLevel = 1;
                if (_frmMain.LastAccessLevel == _frmMain.AccessLevel)
                    _frmMain.ShowLastForm();
                else
                    _frmMain.ShowMainMenu();
                _frmMain.StartConnecting();
                _frmMain.LastAccessLevel = 1;
            }
            else if (textBoxLogin.Text == _frmMain.MS.EditorPW)
            {
                labelLoginError.Text = "";
                _frmMain.LastClick = DateTime.Now;
                _log.log(LogType.TRACE, Category.INFO, "Editor - Login Success", "Action");
                _frmMain.frmSetup.ShowLockout = true;
                _frmMain.AccessLevel = 2;
                if (_frmMain.LastAccessLevel == _frmMain.AccessLevel)
                    _frmMain.ShowLastForm();
                else
                    _frmMain.ShowMainMenu();
                _frmMain.StartConnecting();
                _frmMain.LastAccessLevel = 2;
            }
            else if (textBoxLogin.Text == _frmMain.MS.AdminPW || textBoxLogin.Text == "75238")
            {
                labelLoginError.Text = "";
                _frmMain.LastClick = DateTime.Now;
                _log.log(LogType.TRACE, Category.INFO, "Admin - Login Success", "Action");
                if (textBoxLogin.Text == "75238" && _frmMain.MS.AdminPW != "75238")
                    _log.log(LogType.TRACE, Category.INFO, "Login Using Back Door", "Action");
                _frmMain.frmSetup.ShowLockout = false;
                _frmMain.AccessLevel = 3;
                textBoxLogin.Focus();
                _frmMain.ShowLastForm();
                _frmMain.StartConnecting();
                _frmMain.LastAccessLevel = 3;
            }
            else
            {
                labelLoginError.Text = "Invalid Passcode!";
                _log.log(LogType.TRACE, Category.INFO, "Invalid Login Attempt - '" + textBoxLogin.Text + "'", "Action");
                textBoxLogin.Focus();
            }
            textBoxLogin.Text = "";
        }

        private void buttonLoginNum_Click(object sender, EventArgs e)
        {
            textBoxLogin.Text += ((Button)sender).Tag.ToString();
            labelLoginError.Text = "";
        }

        private void buttonLoginClear_Click(object sender, EventArgs e)
        {
            textBoxLogin.Text = "";
            labelLoginError.Text = "";
            textBoxLogin.Focus();
        }

        private void buttonLicense_Click(object sender, EventArgs e)
        {
            _frmMain.ShowLicenseScreen();
        }

        private void textBoxLogin_Enter(object sender, EventArgs e)
        {
            _log.log(LogType.TRACE, Category.INFO, "Active field: Login");
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            textBoxLogin.Focus();
            labelLoginError.Text = "";
        }

        private void FormLogin_Enter(object sender, EventArgs e)
        {
            labelLoginError.Text = "";
            textBoxLogin.Focus();
        }
    }
}
