using CommonLibrary.Source.Utils;
using CommonLibrary.Source.Utils.Interfaces;
using Support2;
using Support2.RegistryClasses;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace nAble
{
    public partial class FormLicensing : Form, IUpdateableForm
    {
        #region Constants

        private const string DefaultStartDirectory = @"Data\License Files";

        private static readonly Color DefaultColor = Color.FromKnownColor(KnownColor.Window);

        #endregion

        #region Properties

        public string StartDirectory { get; set; } = DefaultStartDirectory;

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly NRadLicensing2 _licMgr = null;
        private readonly LogEntry _log = null;

        private LicenseFile _licenseFile = null;
        private DateTime _creationDate;
        private DateTime _expiryDate;

        private FeatureMask _featureMask = new FeatureMask();
        private ProductKey _productKey;

        #endregion

        #region Functions

        #region Constructors

        public FormLicensing(FormMain frmMain, NRadLicensing2 licMgr, LogEntry log)
        {
            InitializeComponent();

            _frmMain = frmMain;
            _licMgr = licMgr;
            _log = log;

            _productKey = new ProductKey();
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            if (!Visible)
            {
                return;
            }

            SuspendLayout();

            bool keyValid = maskedTextBoxProductKey.MaskCompleted;
            maskedTextBoxProductKey.BackColor = keyValid ? DefaultColor : Color.Yellow;

            try
            {
                if (keyValid)
                {
                    _productKey.ValueAsString = maskedTextBoxProductKey.Text;
                }
            }
            catch (Exception)
            {
            }

            bool codeValid = false;

            if (long.TryParse(textBoxProductCode.Text, out long tempFeatures))
            {
                _featureMask.Value = tempFeatures;
                codeValid = true;
            }

            textBoxProductCode.BackColor = codeValid ? DefaultColor : Color.Yellow;

            _creationDate = NRadLicensing2.MakeDateOnly(dateTimePickerCreation.Value);

            bool expireValid = DateTime.Now.AddDays(-1) < dateTimePickerExpiry.Value;
            labelAlreadyExpired.Visible = !expireValid;

            if (expireValid)
            {
                _expiryDate = _featureMask.Activated ? NRadLicensing2.MakeDateOnly(DateTime.MaxValue.AddYears(-1)) : NRadLicensing2.MakeDateOnly(dateTimePickerExpiry.Value);
            }

            labelNoExpiration.Visible = _featureMask.Activated;
            dateTimePickerExpiry.Visible = !_featureMask.Activated;

            buttonActivate.Enabled = keyValid && codeValid && expireValid;

            ResumeLayout();
        }

        #endregion

        #region Private Functions

        #endregion

        #region Control Event Handlers

        private void FormLicensing_Load(object sender, EventArgs e)
        {
            if (_licMgr is null)
            {
                _log.log(LogType.TRACE, Category.ERROR, "FormLicensing was given a null LicenseManager and is inoperative.");
                labelTitle.Text = "License Manager Missing - Please Contact nTact Support";
                panelEntryControls.Enabled = false;
            }
            else
            {
                panelEntryControls.Enabled = true;
            }
        }

        private void buttonActivate_Click(object sender, EventArgs e)
        {
            if (_licMgr.UpdateLicenseData(_creationDate, _expiryDate, _featureMask.Value, _productKey))
            {
                _log.log(LogType.ACTIVITY, Category.ACTION, "New license information was written and validated.");
                nRadMessageBox.Show(this, "New license data was written successfully and validated.", "License Data Written", MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonBack.PerformClick();
            }
            else
            {
                _log.log(LogType.TRACE, Category.ERROR, "New license information failed to write or failed to validate.");
                nRadMessageBox.Show(this, "The license data was not correctly written or failed validation.  Please try again.", "License Data Failed To Write", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonImportFromXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog()
            {
                Filter = "License File (*.xml)|*.xml",
                CheckFileExists = false,
                Multiselect = false,
                CheckPathExists = true,
                AddExtension = true,
                ValidateNames = true,
                Title = "Select License File",
                InitialDirectory = StartDirectory
            };

            if (DialogResult.OK == fileDlg.ShowDialog(this))
            {
                try
                {
                    _log.log(LogType.ACTIVITY, Category.ACTION, $"User selected a new license file: {fileDlg.FileName}");
                    _licenseFile = LoaderSaver<LicenseFile>.Load(fileDlg.FileName);

                    if (_licenseFile != null)
                    {
                        long featureMask = _licenseFile.Features;
                        var written = _licMgr.UpdateLicenseData(_licenseFile.CreationDate, _licenseFile.ExpirationDate, featureMask, _licenseFile.ProductKey);
                        var msg = written ? $"New license was written." : "New license did not write correctly -- please either try again, or try a new file";
                        _log.log(LogType.TRACE, Category.INFO, msg);
                        nRadMessageBox.Show(this, msg, "License Write", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (written)
                        {
                            buttonBack.PerformClick();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _log.log(LogType.TRACE, Category.ERROR, $"There was a problem loading the new license info: {ex.Message}");
                    nRadMessageBox.Show(this, $"There was a problem reading the license file {fileDlg.FileName}.  Please try another file.",
                        "Error Loading License File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            LicenseFailCode lockOutCode = _licMgr.CheckLicenseValidity();

            if (lockOutCode == LicenseFailCode.Valid)
            {
                _frmMain.SWIsLocked = false;
                _frmMain.ShowLoginForm();
            }
            else
            {
                _frmMain.ShowLockOutScreen(lockOutCode);
            }
        }

        private void maskedTextBoxProductKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            string str = new string(e.KeyChar, 1).ToUpper();

            if (str.Length > 0)
            {
                e.KeyChar = str[0];
            }
        }

        private void labelTitle_Click(object sender, EventArgs e)
        {
            if (!ModifierKeys.HasFlag(Keys.Control))
            {
                return;
            }

            string msg = $"Registry\n\n{_licMgr.DebugString}";
            MessageBox.Show(msg);
        }

        #endregion

        #endregion
    }
}
