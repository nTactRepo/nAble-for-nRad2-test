using nAble.Model.Recipes;
using nTact.DataComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using nAble.DataComm;

namespace nAble
{
    public partial class FormRecipeEditAdvanced : Form, IUpdateableForm
    {
        #region Constants

        private const int SCurveMin = 1;
        private const int SCurveMax = 100;

        #endregion

        #region Properties 

        #endregion

        #region Data Members

        private readonly FormMain _frmMain = null;
        private readonly Form _returnForm = null;

        private RecipeAdvancedEditParams _advParams = null;
        private MotionProfile _motionProfile = null;

        private string _accelMsg => $"{_advParams.Title} Acceleration";
        private string _decelMsg => $"{_advParams.Title} Deceleration";
        private string _SCurveMsg => $"{_advParams.Title} S-Curve";

        #endregion

        #region Functions

        #region Constructors

        public FormRecipeEditAdvanced(FormMain frmMain, Form returnForm, RecipeAdvancedEditParams advParams, MotionProfile profile)
        {
            InitializeComponent();

            _frmMain = frmMain ?? throw new ArgumentNullException(nameof(frmMain));
            _returnForm = returnForm ?? throw new ArgumentNullException(nameof(returnForm));
            _advParams = advParams ?? throw new ArgumentNullException(nameof(advParams));
            _motionProfile = profile ?? throw new ArgumentNullException(nameof(profile));
            InitializeFieldsFromParams();
        }

        #endregion

        #region Public Functions

        public void UpdateStatus()
        {
            AreValuesInRange();
        }

        #endregion

        #region Private Functions

        private void InitializeFieldsFromParams()
        {
            labelTitle.Text = $"{_advParams.Title} Advanced Settings";
            buttonAccel.Text = $"{_advParams.Accel:0.000}";
            buttonDecel.Text = $"{_advParams.Decel:0.000}";
            buttonSCurve.Text = $"{_advParams.SCurve:#}";
            labelAccelUnits.Text = $"{_advParams.Units}";
            labelDecelUnits.Text = $"{_advParams.Units}";
        }

        private void ExitForm()
        {
            _frmMain.LoadSubForm(_returnForm);
        }

        private bool AreValuesInRange()
        {
            double accel = double.Parse(buttonAccel.Text);
            double decel = double.Parse(buttonDecel.Text);
            int sCurve = int.Parse(buttonSCurve.Text);

            bool accInRange = accel >= _motionProfile.MinAcc && accel <= _motionProfile.MaxAcc;
            bool decInRange = decel >= _motionProfile.MinAcc && decel <= _motionProfile.MaxAcc;
            bool sCurveInRange = sCurve >= 1 && sCurve <= 100;

            buttonAccel.BackColor = accInRange ? (_advParams.Accel == accel ? SystemColors.ButtonFace : Color.Yellow) : Color.Red;
            buttonDecel.BackColor = decInRange ? (_advParams.Decel == decel ? SystemColors.ButtonFace : Color.Yellow) : Color.Red;
            buttonSCurve.BackColor = sCurveInRange ? (_advParams.SCurve == sCurve ? SystemColors.ButtonFace : Color.Yellow) : Color.Red;

            return accInRange && decInRange && sCurveInRange;
        }

        #endregion

        #region Control Event Handlers

        private void FormRecipeEditAdvanced_Load(object sender, EventArgs e)
        {
            labelTitle.Text = _advParams.Title;
            labelAccelUnits.Text = _advParams.Units;
            labelDecelUnits.Text = _advParams.Units;

            AreValuesInRange();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;

            if (AreValuesInRange())
            {
                double accel = double.Parse(buttonAccel.Text);
                double decel = double.Parse(buttonDecel.Text);
                int sCurve = int.Parse(buttonSCurve.Text);

                _advParams.Accel = accel;
                _advParams.Decel = decel;
                _advParams.SCurve = sCurve;

                ExitForm();
            }
            else
            {
                nRadMessageBox.Show(this, "One or more values are outside of the allowable range", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            ExitForm();
        }

        private void buttonAccel_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen(_accelMsg, this, buttonAccel, "0.000", _motionProfile.MinAcc, _motionProfile.MaxAcc, "", true);
        }

        private void buttonDecel_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen(_decelMsg, this, buttonDecel, "0.000", _motionProfile.MinAcc, _motionProfile.MaxAcc, "", true);
        }

        private void buttonSCurve_Click(object sender, EventArgs e)
        {
            _frmMain.LastClick = DateTime.Now;
            _frmMain.GotoNumScreen(_SCurveMsg, this, buttonSCurve, "#", SCurveMin, SCurveMax, "", false);
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
        }

        #endregion

        #endregion
    }
}
