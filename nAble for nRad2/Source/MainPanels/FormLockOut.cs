using Support2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormLockOut : Form, IUpdateableForm
    {
        public LicenseFailCode LockOutCode { get; set; } = LicenseFailCode.Uninitialized;

        private FormMain _frmMain = null;

        public FormLockOut(FormMain formMain)
        {
            _frmMain = formMain;
            InitializeComponent();
        }

        public void UpdateStatus()
        {
            labelLockoutCode.Text = $"Code: {(int)LockOutCode}";
            labelLockoutCode.Visible = true;

            buttonLicense.Visible = _frmMain.UseLicenseMgr;

            if (LockOutCode == LicenseFailCode.Valid)
            {
                _frmMain.ShowLoginForm();
                Close();
            }
        }

        private void buttonLicense_Click(object sender, EventArgs e)
        {
            _frmMain.ShowLicenseScreen();
        }
    }
}
